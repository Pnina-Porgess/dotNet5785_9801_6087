using System;
using System.Windows;
using BO;

namespace PL.Volunteer
{
    public partial class VolunteerWindow : Window
    {
        private readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        // נשמור את הפונקציה כדי שנוכל להסיר אותה בדיוק
        private readonly Action _volunteerObserver;

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
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    Close();
                    return;
                }
            }

            // יצירת המשקיף באופן חד-פעמי לפר instance
            _volunteerObserver = () =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (CurrentVolunteer == null || CurrentVolunteer.Id == 0) return;

                    var updated = s_bl.Volunteer.GetVolunteerDetails(CurrentVolunteer.Id);
                    SetCurrentValue(CurrentVolunteerProperty, updated);
                });
            };


            // הרשמה למשקיף
            if (CurrentVolunteer != null && CurrentVolunteer.Id != 0)
                s_bl.Volunteer.AddObserver(CurrentVolunteer.Id, _volunteerObserver);

            // הסרה של המשקיף בסגירת החלון
            this.Closed += (s, e) =>
            {
                if (CurrentVolunteer != null && CurrentVolunteer.Id != 0)
                    s_bl.Volunteer.RemoveObserver(CurrentVolunteer.Id, _volunteerObserver);
            };
        }

        // תכונת תלות לאובייקט
        public BO.Volunteer? CurrentVolunteer
        {
            get => (BO.Volunteer?)GetValue(CurrentVolunteerProperty);
            set => SetValue(CurrentVolunteerProperty, value);
        }

        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));

        // תכונת תלות לטקסט הכפתור
        public string ButtonText
        {
            get => (string)GetValue(ButtonTextProperty);
            set => SetValue(ButtonTextProperty, value);
        }

        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register("ButtonText", typeof(string), typeof(VolunteerWindow), new PropertyMetadata("Add"));

        // אירוע כפתור
        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurrentVolunteer == null)
                    throw new Exception("Volunteer object is not initialized.");

                // בדיקות תקינות בסיסיות
                if (string.IsNullOrWhiteSpace(CurrentVolunteer.FullName))
                    throw new Exception("Please enter full name.");

                if (string.IsNullOrWhiteSpace(CurrentVolunteer.Phone))
                    throw new Exception("Please enter phone number.");

                if (!System.Text.RegularExpressions.Regex.IsMatch(CurrentVolunteer.Phone, @"^0\d{9}$"))
                    throw new Exception("Invalid phone number. It must be 10 digits, e.g. 0501234567.");

                if (string.IsNullOrWhiteSpace(CurrentVolunteer.Email))
                    throw new Exception("Please enter email address.");

                if (!System.Text.RegularExpressions.Regex.IsMatch(CurrentVolunteer.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    throw new Exception("Invalid email format.");

                if (string.IsNullOrWhiteSpace(CurrentVolunteer.Password))
                    throw new Exception("Please enter a password.");

                if (CurrentVolunteer.Password.Length < 8 ||
                    !System.Text.RegularExpressions.Regex.IsMatch(CurrentVolunteer.Password, @"[A-Z]") ||
                    !System.Text.RegularExpressions.Regex.IsMatch(CurrentVolunteer.Password, @"[a-z]") ||
                    !System.Text.RegularExpressions.Regex.IsMatch(CurrentVolunteer.Password, @"\d") ||
                    !System.Text.RegularExpressions.Regex.IsMatch(CurrentVolunteer.Password, @"[\W_]"))
                {
                    throw new Exception("Password must be at least 8 characters long and include an uppercase letter, lowercase letter, number, and special character.");
                }

                if (string.IsNullOrWhiteSpace(CurrentVolunteer.CurrentAddress))
                    throw new Exception("Please enter address.");

                if (CurrentVolunteer.Role == null)
                    throw new Exception("Please select a role.");

                if (CurrentVolunteer.DistanceType == null)
                    throw new Exception("Please select distance type.");

                if (CurrentVolunteer.MaxDistance <= 0)
                    throw new Exception("Max distance must be a positive number.");

                // קריאה ל־BL
                if (ButtonText == "Add")
                {
                    s_bl.Volunteer.AddVolunteer(CurrentVolunteer!);
                    MessageBox.Show("Volunteer added successfully!");
                }
                else
                {
                    s_bl.Volunteer.UpdateVolunteerDetails(CurrentVolunteer!.Id, CurrentVolunteer!);
                    MessageBox.Show("Volunteer updated successfully!");
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
