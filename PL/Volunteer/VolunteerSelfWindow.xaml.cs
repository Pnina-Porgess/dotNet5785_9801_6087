using BlApi;
using PL;
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Threading;

namespace PL.Volunteer
{
    public partial class VolunteerSelfWindow : Window, INotifyPropertyChanged
    {
        private readonly IBl _bl = Factory.Get();

        private BO.Volunteer _volunteer;
        public BO.Volunteer Volunteer
        {
            get => _volunteer;
            set
            {
                _volunteer = value;
                OnPropertyChanged(nameof(Volunteer));
            }
        }

        private volatile DispatcherOperation? _observerOperation = null;

        /// <summary>
        /// Initializes the window with volunteer details and sets up an observer.
        /// </summary>
        /// <param name="volunteerId">ID of the volunteer to load.</param>
        public VolunteerSelfWindow(int volunteerId)
        {
            InitializeComponent();
            Volunteer = _bl.Volunteer.GetVolunteerDetails(volunteerId);
            _bl.Volunteer.AddObserver(volunteerId, RefreshVolunteerObserver);
        }

        /// <summary>
        /// Removes observer when the window is closed.
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _bl.Volunteer.RemoveObserver(Volunteer.Id, RefreshVolunteerObserver);
        }

        /// <summary>
        /// Refreshes the volunteer data from the business layer.
        /// </summary>
        private void RefreshVolunteer()
        {
            Volunteer = _bl.Volunteer.GetVolunteerDetails(Volunteer.Id);
        }

        /// <summary>
        /// Observer function that updates the volunteer data asynchronously.
        /// </summary>
        private void RefreshVolunteerObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
            {
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    RefreshVolunteer();
                });
            }
        }

        /// <summary>
        /// Updates volunteer details after validating the input.
        /// </summary>
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Volunteer == null)
                    throw new Exception("Volunteer details failed to load.");

                if (string.IsNullOrWhiteSpace(Volunteer.FullName))
                    throw new Exception("Please enter full name.");

                if (string.IsNullOrWhiteSpace(Volunteer.Phone))
                    throw new Exception("Please enter phone number.");

                if (string.IsNullOrWhiteSpace(Volunteer.Email))
                    throw new Exception("Please enter email address.");

                if (Volunteer.MaxDistance <= 0)
                    throw new Exception("Please enter a valid maximum distance (must be a positive number).");

                if (!Regex.IsMatch(Volunteer.Phone, @"^0\d{9}$"))
                    throw new Exception("Invalid phone number. It must be 10 digits, e.g. 0501234567.");

                if (!Regex.IsMatch(Volunteer.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    throw new Exception("Invalid email address.");

                if (!string.IsNullOrWhiteSpace(Volunteer.Password))
                {
                    if (Volunteer.Password.Length < 8 ||
                        !Regex.IsMatch(Volunteer.Password, @"[A-Z]") ||
                        !Regex.IsMatch(Volunteer.Password, @"[a-z]") ||
                        !Regex.IsMatch(Volunteer.Password, @"\d") ||
                        !Regex.IsMatch(Volunteer.Password, @"[\W_]"))
                    {
                        throw new Exception("Password must be at least 8 characters long and include an uppercase letter, a lowercase letter, a digit, and a special character.");
                    }
                }

                _bl.Volunteer.UpdateVolunteerDetails(Volunteer.Id, Volunteer);
                MessageBox.Show("Details updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (BO.BlInvalidInputException ex)
            {
                MessageBox.Show("Input error: " + ex.Message, "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (BO.BlUnauthorizedAccessException ex)
            {
                MessageBox.Show("You do not have permission to perform this action.\n" + ex.Message, "Unauthorized", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
            catch (BO.BlNotFoundException ex)
            {
                MessageBox.Show("Volunteer not found: " + ex.Message, "Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (BO.InvalidOperationException ex)
            {
                MessageBox.Show("Invalid operation: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BO.BlDatabaseException ex)
            {
                MessageBox.Show("Database error: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An unexpected error occurred: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Opens the volunteer's call history window.
        /// </summary>
        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            var historyWindow = new VolunteerHistoryWindow(Volunteer.Id);
            historyWindow.Show();
        }

        /// <summary>
        /// Opens the window displaying currently available calls.
        /// </summary>
        private void btnOpenCalls_Click(object sender, RoutedEventArgs e)
        {
            var openCalls = new Call.OpenCallsWindow(Volunteer);
            openCalls.Closed += (_, _) => RefreshVolunteer();
            openCalls.Show();
        }

        /// <summary>
        /// Completes the current call being handled by the volunteer.
        /// </summary>
        private void FinishCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _bl.Call.CompleteCallTreatment(Volunteer.Id, Volunteer.CurrentCall!.Id);
                MessageBox.Show("The call was successfully completed.", "Call Completed", MessageBoxButton.OK, MessageBoxImage.Information);
                RefreshVolunteer();
            }
            catch (BO.BlNotFoundException ex)
            {
                MessageBox.Show("Call not found or already handled.\n" + ex.Message, "Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (BO.InvalidOperationException ex)
            {
                MessageBox.Show("Cannot complete the call: " + ex.Message, "Invalid Operation", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (BO.BlDatabaseException ex)
            {
                MessageBox.Show("Database error while completing call: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Cancels the current call being handled by the volunteer.
        /// </summary>
        private void CancelCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _bl.Call.CancelCallTreatment(Volunteer.Id, Volunteer.CurrentCall!.Id);
                MessageBox.Show("The call was successfully cancelled.", "Call Cancelled", MessageBoxButton.OK, MessageBoxImage.Information);
                RefreshVolunteer();
            }
            catch (BO.BlNotFoundException ex)
            {
                MessageBox.Show("Call not found or already handled.\n" + ex.Message, "Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (BO.InvalidOperationException ex)
            {
                MessageBox.Show("Cannot cancel the call: " + ex.Message, "Invalid Operation", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (BO.BlDatabaseException ex)
            {
                MessageBox.Show("Database error while cancelling call: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="name">The name of the property that changed.</param>
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
