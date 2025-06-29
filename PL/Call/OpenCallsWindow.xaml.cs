using BlApi;
using BO;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PL.Call
{
    public partial class OpenCallsWindow : Window, INotifyPropertyChanged
    {
        private readonly IBl bl = Factory.Get();
        public BO.Volunteer CurrentVolunteer { get; }

        public ObservableCollection<OpenCallInList> OpenCalls { get; set; } = new();

        private BO.OpenCallField _sortField = BO.OpenCallField.Id;
        public BO.OpenCallField SortField
        {
            get => _sortField;
            set
            {
                _sortField = value;
                OnPropertyChanged(nameof(SortField));
                LoadOpenCalls();
            }
        }

        private BO.TypeOfReading _filterStatus = BO.TypeOfReading.None;
        public BO.TypeOfReading FilterStatus
        {
            get => _filterStatus;
            set
            {
                _filterStatus = value;
                OnPropertyChanged(nameof(FilterStatus));
                LoadOpenCalls();
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
            CurrentAddress = volunteer.CurrentAddress;
            DataContext = this;
            LoadOpenCalls();
        }

        private void LoadOpenCalls()
        {
            try
            {
                var calls = bl.Call.GetOpenCallsForVolunteer(
                    CurrentVolunteer.Id,
                    FilterStatus == BO.TypeOfReading.None ? null : FilterStatus,
                    SortField);

                OpenCalls.Clear();
                foreach (var call in calls)
                    OpenCalls.Add(call);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"שגיאה בטעינת קריאות: {ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show("הקריאה נבחרה לטיפול!", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"שגיאה בבחירת קריאה: {ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void FilterOrSort_Changed(object sender, SelectionChangedEventArgs e)
        {
            LoadOpenCalls();
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
                MessageBox.Show("הכתובת עודכנה בהצלחה.", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadOpenCalls(); // לרענן מרחקים
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה בעדכון כתובת: " + ex.Message, "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
