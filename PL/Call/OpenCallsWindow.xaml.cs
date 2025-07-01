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
    public partial class OpenCallsWindow : Window, INotifyPropertyChanged
    {
        private readonly IBl bl = Factory.Get();
        public BO.Volunteer CurrentVolunteer { get; }

        public ObservableCollection<OpenCallInList> OpenCalls { get; set; } = new();

        private volatile DispatcherOperation? _observerOperation = null;

        private OpenCallField _sortField = OpenCallField.Id;
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
        public string CurrentAddress
        {
            get => _currentAddress;
            set
            {
                _currentAddress = value;
                OnPropertyChanged(nameof(CurrentAddress));
            }
        }

        public OpenCallsWindow(BO.Volunteer volunteer)
        {
            InitializeComponent();
            CurrentVolunteer = volunteer;
            CurrentAddress = volunteer.CurrentAddress!;

            RefreshOpenCallsObserver(); // initial load
        }

        // === Observer function ===
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
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error loading open calls: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                });
            }
        }

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
            catch (Exception ex)
            {
                MessageBox.Show($"Error selecting call: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void FilterOrSort_Changed(object sender, SelectionChangedEventArgs e)
        {
            RefreshOpenCallsObserver();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedCall != null)
                SelectedDescription = SelectedCall.Description ?? string.Empty;
        }

        private void UpdateAddress_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CurrentVolunteer.CurrentAddress = CurrentAddress;
                bl.Volunteer.UpdateVolunteerDetails(CurrentVolunteer.Id, CurrentVolunteer);
                MessageBox.Show("Address updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                RefreshOpenCallsObserver(); // refresh distances
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
