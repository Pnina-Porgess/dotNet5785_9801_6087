using BO;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Threading;

namespace PL.Volunteer
{
    public partial class VolunteerWindow : Window
    {
        private readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        private volatile DispatcherOperation? _observerOperation = null;

        public VolunteerWindow(int id = 0)
        {
            ButtonText = id == 0 ? "Add" : "Update";
            InitializeComponent();

            if (id == 0)
            {
                CurrentVolunteer = new BO.Volunteer();
            }
            else
            {
                try
                {
                    CurrentVolunteer = s_bl.Volunteer.GetVolunteerDetails(id);
                    s_bl.Volunteer.AddObserver(id, RefreshVolunteerObserver);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Close();
                    return;
                }
            }

            this.Closed += (s, e) =>
            {
                if (CurrentVolunteer != null && CurrentVolunteer.Id != 0)
                    s_bl.Volunteer.RemoveObserver(CurrentVolunteer.Id, RefreshVolunteerObserver);
            };
        }

        public BO.Volunteer? CurrentVolunteer
        {
            get => (BO.Volunteer?)GetValue(CurrentVolunteerProperty);
            set => SetValue(CurrentVolunteerProperty, value);
        }

        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));

        public string ButtonText
        {
            get => (string)GetValue(ButtonTextProperty);
            set => SetValue(ButtonTextProperty, value);
        }

        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register("ButtonText", typeof(string), typeof(VolunteerWindow), new PropertyMetadata("Add"));

        private void RefreshVolunteerObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
            {
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    if (CurrentVolunteer == null || CurrentVolunteer.Id == 0) return;
                    var updated = s_bl.Volunteer.GetVolunteerDetails(CurrentVolunteer.Id);
                    SetCurrentValue(CurrentVolunteerProperty, updated);
                });
            }
        }

        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurrentVolunteer == null)
                    throw new Exception("Volunteer object is not initialized.");

                if (string.IsNullOrWhiteSpace(CurrentVolunteer.FullName))
                    throw new Exception("Please enter full name.");

                if (string.IsNullOrWhiteSpace(CurrentVolunteer.Phone))
                    throw new Exception("Please enter phone number.");

                if (!Regex.IsMatch(CurrentVolunteer.Phone, @"^0\d{9}$"))
                    throw new Exception("Invalid phone number. It must be 10 digits, e.g. 0501234567.");

                if (string.IsNullOrWhiteSpace(CurrentVolunteer.Email))
                    throw new Exception("Please enter email address.");

                if (!Regex.IsMatch(CurrentVolunteer.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    throw new Exception("Invalid email format.");

                if (!string.IsNullOrWhiteSpace(CurrentVolunteer.Password))
                {
                    if (CurrentVolunteer.Password.Length < 8 ||
                        !Regex.IsMatch(CurrentVolunteer.Password, @"[A-Z]") ||
                        !Regex.IsMatch(CurrentVolunteer.Password, @"[a-z]") ||
                        !Regex.IsMatch(CurrentVolunteer.Password, @"\d") ||
                        !Regex.IsMatch(CurrentVolunteer.Password, @"[\W_]"))
                    {
                        throw new Exception("Password must be at least 8 characters long and include an uppercase letter, a lowercase letter, a digit, and a special character.");
                    }
                }

                if (string.IsNullOrWhiteSpace(CurrentVolunteer.CurrentAddress))
                    throw new Exception("Please enter address.");

                if (CurrentVolunteer.Role == null)
                    throw new Exception("Please select a role.");

                if (CurrentVolunteer.DistanceType == null)
                    throw new Exception("Please select distance type.");

                if (CurrentVolunteer.MaxDistance <= 0)
                    throw new Exception("Max distance must be a positive number.");

                if (ButtonText == "Add")
                {
                    s_bl.Volunteer.AddVolunteer(CurrentVolunteer!);
                    MessageBox.Show("Volunteer added successfully!");
                }
                else
                {
                    try
                    {
                        s_bl.Volunteer.UpdateVolunteerDetails(CurrentVolunteer!.Id, CurrentVolunteer!);
                        MessageBox.Show("Volunteer updated successfully!");
                    }
                    catch (BO.BLTemporaryNotAvailableException)
                    {
                        MessageBox.Show("הסימולטור פועל כרגע. לא ניתן לעדכן פרטים בזמן סימולציה.");
                    }

                }

                Close();
            }
       
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
