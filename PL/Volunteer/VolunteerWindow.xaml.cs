using BO;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Threading;

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for adding or updating a volunteer.
    /// </summary>
    public partial class VolunteerWindow : Window
    {
        private readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private volatile DispatcherOperation? _observerOperation = null;

        /// <summary>
        /// Initializes a new instance of the VolunteerWindow.
        /// </summary>
        /// <param name="id">Volunteer ID. 0 for adding new volunteer.</param>
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
                catch (BO.BlNotFoundException ex)
                {
                    MessageBox.Show("Volunteer not found: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    Close();
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Unexpected error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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

        /// <summary>
        /// Refreshes the volunteer from the business layer using observer.
        /// </summary>
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

        /// <summary>
        /// Validates the input and performs add or update operation.
        /// </summary>
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

                // ADD
                if (ButtonText == "Add")
                {
                    s_bl.Volunteer.AddVolunteer(CurrentVolunteer!);
                    MessageBox.Show("Volunteer added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else // UPDATE
                {
                    s_bl.Volunteer.UpdateVolunteerDetails(CurrentVolunteer!.Id, CurrentVolunteer!);
                    MessageBox.Show("Volunteer updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                Close();
            }
            catch (BO.BlInvalidInputException ex)
            {
                MessageBox.Show("Input error: " + ex.Message, "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (BO.BlAlreadyExistsException ex)
            {
                MessageBox.Show("Volunteer already exists: " + ex.Message, "Duplicate", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (BO.BlNotFoundException ex)
            {
                MessageBox.Show("Volunteer not found: " + ex.Message, "Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (BO.BLTemporaryNotAvailableException)
            {
                MessageBox.Show("Simulator is currently running. Cannot perform this operation.", "Simulator Active", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (BO.BlDatabaseException ex)
            {
                MessageBox.Show("Database error: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
