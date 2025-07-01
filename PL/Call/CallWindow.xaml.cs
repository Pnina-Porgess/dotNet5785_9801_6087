
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;


namespace PL.Call
{
    /// <summary>
    /// Interaction logic for viewing and editing a single call.
    /// </summary>
    public partial class CallWindow : Window, INotifyPropertyChanged
    {
        private readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private volatile DispatcherOperation? _observerOperation = null;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        /// <summary>
        /// Initializes the window for creating or editing a call.
        /// </summary>
        /// <param name="id">The ID of the call to edit. If 0, creates a new call.</param>
        public CallWindow(int id = 0)
        {
            InitializeComponent();
            SetCurrentValue(ButtonTextProperty, id == 0 ? "Add" : "Update");

            if (id == 0)
            {
                CurrentCall = new BO.Call
                {
                    Status = BO.CallStatus.Open,
                    OpeningTime = s_bl.Admin.GetClock()
                };
            }
            else
            {
                try
                {
                    CurrentCall = s_bl.Call.GetCallDetails(id);
                    s_bl.Call.AddObserver(id, RefreshCallObserver);
                }
                catch (BO.BlNotFoundException ex)
                {
                    MessageBox.Show($"Call not found: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
                catch (BO.BlGeneralException ex)
                {
                    MessageBox.Show($"Failed to load call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Unexpected error: {ex.Message}", "Critical", MessageBoxButton.OK, MessageBoxImage.Error);
                    Close();
                }
            }

            this.Closed += (s, e) =>
            {
                if (CurrentCall != null && CurrentCall.Id != 0)
                    s_bl.Call.RemoveObserver(CurrentCall.Id, RefreshCallObserver);
            };
        }

        public BO.Call? CurrentCall
        {
            get => (BO.Call?)GetValue(CurrentCallProperty);
            set
            {
                SetValue(CurrentCallProperty, value);
                OnPropertyChanged(nameof(CanEditDetails));
                OnPropertyChanged(nameof(CanEditMaxEndTime));
            }
        }

        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(CallWindow), new PropertyMetadata(null));

        public string ButtonText
        {
            get => (string)GetValue(ButtonTextProperty);
            set => SetValue(ButtonTextProperty, value);
        }

        public static readonly DependencyProperty ButtonTextProperty =
            DependencyProperty.Register("ButtonText", typeof(string), typeof(CallWindow), new PropertyMetadata("Update"));

        /// <summary>
        /// Indicates if call details are editable.
        /// </summary>
        public bool CanEditDetails =>
            CurrentCall is { Status: BO.CallStatus.Open or BO.CallStatus.OpenAtRisk };

        /// <summary>
        /// Indicates if max end time can be edited.
        /// </summary>
        public bool CanEditMaxEndTime =>
            CurrentCall is { Status: BO.CallStatus.Open or BO.CallStatus.OpenAtRisk or BO.CallStatus.InProgress or BO.CallStatus.InProgressAtRisk };

        /// <summary>
        /// Refreshes the displayed call when notified.
        /// </summary>
        private void RefreshCallObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
            {
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    if (CurrentCall == null) return;
                    int id = CurrentCall.Id;
                    try
                    {
                        CurrentCall = null;
                        CurrentCall = s_bl.Call.GetCallDetails(id);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to refresh call: {ex.Message}", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                });
            }
        }

        /// <summary>
        /// Handles click event for add/update button.
        /// </summary>
        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (CurrentCall?.MaxEndTime == null)
                    throw new BO.BlInvalidInputException("Please select a maximum end time.");

                if (CurrentCall.MaxEndTime <= CurrentCall.OpeningTime)
                    throw new BO.BlInvalidInputException("End time must be after opening time.");

                if (CurrentCall.Status is BO.CallStatus.Closed or BO.CallStatus.Expired)
                    throw new BO.BlInvalidInputException("You cannot edit a closed or expired call.");

                if (CurrentCall.Status is BO.CallStatus.InProgress or BO.CallStatus.InProgressAtRisk)
                {
                    s_bl.Call.UpdateCall(CurrentCall);
                    MessageBox.Show("Max end time updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    Close();
                    return;
                }

                if (string.IsNullOrWhiteSpace(CurrentCall?.Address))
                    throw new BO.BlInvalidInputException("Address is required.");

                if (CurrentCall.Type == null)
                    throw new BO.BlInvalidInputException("Call type is required.");

                if (ButtonText == "Add")
                {
                    s_bl.Call.AddCall(CurrentCall!);
                    MessageBox.Show("Call added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    s_bl.Call.UpdateCall(CurrentCall!);
                    MessageBox.Show("Call updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                Close();
            }
            catch (BO.BlInvalidInputException ex)
            {
                MessageBox.Show($"Input error: {ex.Message}", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (BO.BlNotFoundException ex)
            {
                MessageBox.Show($"Call not found: {ex.Message}", "Not Found", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BO.BlAlreadyExistsException ex)
            {
                MessageBox.Show($"Call already exists: {ex.Message}", "Duplicate", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BO.BLTemporaryNotAvailableException ex)
            {
                MessageBox.Show($"Call is temporarily unavailable: {ex.Message}", "Temporary", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (BO.BlUnauthorizedAccessException ex)
            {
                MessageBox.Show($"Unauthorized: {ex.Message}", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Stop);
            }
            catch (BO.BlGeneralException ex)
            {
                MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unexpected exception: {ex.Message}", "Critical", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
