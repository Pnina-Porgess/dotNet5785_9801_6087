using BlApi;
using BO;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System.ComponentModel; // נדרש עבור INotifyPropertyChanged
using System.Linq;
using System.Runtime.CompilerServices; // נדרש עבור [CallerMemberName]
using System.Windows;
//using static BO.Enums;

namespace PL
{
    public partial class VolunteerHistoryWindow : Window
    {
        private static readonly IBl s_bl = Factory.Get();

        public VolunteerHistoryWindow(int volunteerId)
        {
            InitializeComponent();
            VolunteerId = volunteerId;
            DataContext = this;
            QueryClosedCalls();
        }

        public int VolunteerId { get; set; }
        public IEnumerable<CallType> CallTypeCollection => Enum.GetValues(typeof(CallType)).Cast<CallType>();
        public IEnumerable<string> SortOptions => new[] { "Finish Time", "Type", "ID" };

        private BO.TypeOfReading _selectedCallType = BO.TypeOfReading.None;
        public BO.TypeOfReading SelectedCallType
        {
            get => _selectedCallType;
            set
            {
                _selectedCallType = value;
                OnPropertyChanged(nameof(ClosedCalls));
            QueryClosedCalls(); // רענון הרשימה בעת שינוי
            }
        }

        private string _selectedSortOption = "Finish Time";
        public string SelectedSortOption
        {
            get => _selectedSortOption;
            set
            {
                _selectedSortOption = value;
                OnPropertyChanged(nameof(ClosedCalls));
              QueryClosedCalls(); // רענון הרשימה בעת שינוי
            }
        }

        private IEnumerable<ClosedCallInList> _closedCalls;
        public IEnumerable<ClosedCallInList> ClosedCalls
        {
            get => _closedCalls;
            set
            {
                _closedCalls = value;
                OnPropertyChanged(nameof(ClosedCalls));
            }
        }

        private void QueryClosedCalls()
        {
            var calls = s_bl.Call.GetClosedCallsByVolunteer(
                volunteerId: VolunteerId,
               SelectedCallType ==  BO.TypeOfReading.None? null : SelectedCallType,
                sortField: SelectedSortOption == "Finish Time" ? BO.ClosedCallField.EndType :
                           SelectedSortOption == "Type" ? BO.ClosedCallField.CallType :
                           BO.ClosedCallField.Id
            );

            ClosedCalls = calls.ToList();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        //private void OnPropertyChanged(string propertyName)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}