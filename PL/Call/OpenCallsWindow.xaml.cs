using BlApi;
using BO;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace PL.Call
{
    /// <summary>
    /// Window for viewing and selecting open calls for a volunteer.
    /// </summary>
    public partial class OpenCallsWindow : Window, INotifyPropertyChanged
    {
        private readonly IBl bl = Factory.Get();
        public BO.Volunteer CurrentVolunteer { get; }

        public ObservableCollection<OpenCallInList> OpenCalls { get; set; } = new();

        private volatile DispatcherOperation? _observerOperation = null;

        private OpenCallField _sortField = OpenCallField.Id;

        /// <summary>
        /// Gets or sets the current sort field.
        /// </summary>
        public OpenCallField SortField
        {
            get => _sortField;
            set
            {
                _sortField = value;
                OnPropertyChanged(nameof(SortField));
                RefreshOpenCallsObserver();
            }
        }

        private TypeOfReading _filterStatus = TypeOfReading.None;

        /// <summary>
        /// Gets or sets the current filter status.
        /// </summary>
        public TypeOfReading FilterStatus
        {
            get => _filterStatus;
            set
            {
                _filterStatus = value;
                OnPropertyChanged(nameof(FilterStatus));
                RefreshOpenCallsObserver();
            }
        }

        private OpenCallInList? selectedCall;

        /// <summary>
        /// Gets or sets the currently selected call.
        /// </summary>
        public OpenCallInList? SelectedCall
        {
            get => selectedCall;
            set
            {
                selectedCall = value;
                SelectedDescription = selectedCall?.Description ?? string.Empty;
                OnPropertyChanged(nameof(SelectedCall));
                OnPropertyChanged(nameof(SelectedDescription));
            }
        }

        private string selectedDescription = string.Empty;

        /// <summary>
        /// Gets or sets the description of the selected call.
        /// </summary>
        public string SelectedDescription
        {
            get => selectedDescription;
            set
            {
                selectedDescription = value;
                OnPropertyChanged(nameof(SelectedDescription));
            }
        }

        private string _currentAddress;

        /// <summary>
        /// Gets or sets the current address of the volunteer.
        /// </summary>
        public string CurrentAddress
        {
            get => _currentAddress;
            set
            {
                _currentAddress = value;
                OnPropertyChanged(nameof(CurrentAddress));
            }
        }

        /// <summary>
        /// Initializes a new instance of the OpenCallsWindow for a given volunteer.
        /// </summary>
        public OpenCallsWindow(BO.Volunteer volunteer)
        {
            InitializeComponent();
            CurrentVolunteer = volunteer;
            CurrentAddress = volunteer.CurrentAddress!;

            RefreshOpenCallsObserver(); // initial load
        }

        /// <summary>
        /// Refreshes the list of open calls based on current sort/filter.
        /// </summary>
        private void RefreshOpenCallsObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
            {
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    try
                    {
                        var calls = bl.Call.GetOpenCallsForVolunteer(
                            CurrentVolunteer.Id,
                            FilterStatus == TypeOfReading.None ? null : FilterStatus,
                            SortField);

                        OpenCalls.Clear();
                        foreach (var call in calls)
                            OpenCalls.Add(call);
                    }
                    catch (BlNotFoundException ex)
                    {
                        MessageBox.Show($"Volunteer not found: {ex.Message}", "Not Found", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (BLTemporaryNotAvailableException ex)
                    {
                        MessageBox.Show($"Calls temporarily unavailable: {ex.Message}", "Temporary", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    catch (BlDatabaseException ex)
                    {
                        MessageBox.Show($"Database error: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                });
            }
        }

        /// <summary>
        /// Selects a call for treatment.
        /// </summary>
        private void SelectCall_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedCall == null)
                return;

            try
            {
                bl.Call.SelectCallForTreatment(CurrentVolunteer.Id, SelectedCall.Id);
                OpenCalls.Remove(SelectedCall);
                SelectedCall = null;
                MessageBox.Show("The call was successfully selected for treatment.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (BlAlreadyExistsException ex)
            {
                MessageBox.Show($"This call has already been taken: {ex.Message}", "Already Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (BlInvalidInputException ex)
            {
                MessageBox.Show($"Invalid operation: {ex.Message}", "Invalid", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (BlNotFoundException ex)
            {
                MessageBox.Show($"Call not found: {ex.Message}", "Not Found", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Called when filter or sort selection changes.
        /// </summary>
        private void FilterOrSort_Changed(object sender, SelectionChangedEventArgs e)
        {
            RefreshOpenCallsObserver();
        }

        /// <summary>
        /// Called when the selected item in the DataGrid changes.
        /// </summary>
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedCall != null)
                SelectedDescription = SelectedCall.Description ?? string.Empty;
        }

        /// <summary>
        /// Updates the volunteer's address and refreshes the list.
        /// </summary>
        private void UpdateAddress_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CurrentVolunteer.CurrentAddress = CurrentAddress;
                bl.Volunteer.UpdateVolunteerDetails(CurrentVolunteer.Id, CurrentVolunteer);
                MessageBox.Show("Address updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                RefreshOpenCallsObserver(); // Refresh distances
            }
            catch (BlNotFoundException ex)
            {
                MessageBox.Show($"Volunteer not found: {ex.Message}", "Not Found", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (BlInvalidInputException ex)
            {
                MessageBox.Show($"Invalid input: {ex.Message}", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating address: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
