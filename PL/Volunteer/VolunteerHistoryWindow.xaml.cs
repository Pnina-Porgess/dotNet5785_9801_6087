using BlApi;
using BO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;

namespace PL
{
    public partial class VolunteerHistoryWindow : Window, INotifyPropertyChanged
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

        private BO.TypeOfReading _selectedCallType = BO.TypeOfReading.None;
        public BO.TypeOfReading SelectedCallType
        {
            get => _selectedCallType;
            set
            {
                _selectedCallType = value;
                OnPropertyChanged();
                QueryClosedCalls();
            }
        }

        private BO.ClosedCallField _selectedSortOption = BO.ClosedCallField.EndType;
        public BO.ClosedCallField SelectedSortOption
        {
            get => _selectedSortOption;
            set
            {
                _selectedSortOption = value;
                OnPropertyChanged();
                QueryClosedCalls();
            }
        }

        private IEnumerable<ClosedCallInList> _closedCalls = Enumerable.Empty<ClosedCallInList>();
        public IEnumerable<ClosedCallInList> ClosedCalls
        {
            get => _closedCalls;
            set
            {
                _closedCalls = value;
                OnPropertyChanged();
            }
        }

        private void QueryClosedCalls()
        {
            var calls = s_bl.Call.GetClosedCallsByVolunteer(
                volunteerId: VolunteerId,
                SelectedCallType == BO.TypeOfReading.None ? null : SelectedCallType,
                sortField: SelectedSortOption
            );

            ClosedCalls = calls.ToList();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
