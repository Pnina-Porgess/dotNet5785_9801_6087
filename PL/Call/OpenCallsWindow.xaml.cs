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

        public IEnumerable<TypeOfReading> CallField => Enum.GetValues(typeof(TypeOfReading)).Cast<TypeOfReading>();
        public IEnumerable<OpenCallField> SortOptions => Enum.GetValues(typeof(OpenCallField)).Cast<OpenCallField>();

        public OpenCallField? SortField { get; set; } = OpenCallField.Id;
        public TypeOfReading? FilterStatus { get; set; } = null;

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

        public OpenCallsWindow(BO.Volunteer volunteer)
        {
            InitializeComponent();
            CurrentVolunteer = volunteer;
            DataContext = this;
            LoadOpenCalls();
        }

        private void LoadOpenCalls()
        {
            try
            {
                var calls = bl.Call.GetOpenCallsForVolunteer(CurrentVolunteer.Id,
                                                             FilterStatus,
                                                             SortField ?? OpenCallField.Id);
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
            if (sender is Button btn && btn.DataContext is OpenCallInList call)
            {
                try
                {
                    bl.Call.SelectCallForTreatment(CurrentVolunteer.Id, call.Id);
                    OpenCalls.Remove(call);
                    MessageBox.Show("הקריאה נבחרה לטיפול!", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"שגיאה בבחירת קריאה: {ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void FilterOrSort_Changed(object sender, SelectionChangedEventArgs e)
        {
            LoadOpenCalls();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
