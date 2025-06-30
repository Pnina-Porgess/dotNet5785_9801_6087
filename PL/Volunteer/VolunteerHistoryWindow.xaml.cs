using BlApi;
using BO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;

namespace PL
{
    public partial class VolunteerHistoryWindow : Window, INotifyPropertyChanged
    {
        private static readonly IBl s_bl = Factory.Get();

        private volatile DispatcherOperation? _observerOperation = null;

        public VolunteerHistoryWindow(int volunteerId)
        {
            InitializeComponent();
            VolunteerId = volunteerId;
            DataContext = this;

            // טעינה ראשונית
            RefreshClosedCallsObserver();

            // הרשמה ל־Observer
            s_bl.Volunteer.AddObserver(volunteerId, RefreshClosedCallsObserver);
        }

        public int VolunteerId { get; set; }

        private TypeOfReading _selectedCallType = TypeOfReading.None;
        public TypeOfReading SelectedCallType
        {
            get => _selectedCallType;
            set
            {
                _selectedCallType = value;
                OnPropertyChanged();
                RefreshClosedCallsObserver();
            }
        }

        private ClosedCallField _selectedSortOption = ClosedCallField.EndType;
        public ClosedCallField SelectedSortOption
        {
            get => _selectedSortOption;
            set
            {
                _selectedSortOption = value;
                OnPropertyChanged();
                RefreshClosedCallsObserver();
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

        // === Observer ===
        private void RefreshClosedCallsObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
            {
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    var calls = s_bl.Call.GetClosedCallsByVolunteer(
                        volunteerId: VolunteerId,
                        SelectedCallType == TypeOfReading.None ? null : SelectedCallType,
                        sortField: SelectedSortOption
                    );

                    ClosedCalls = calls.ToList();
                });
            }
        }

        // ניקוי זיכרון - הסרת Observer
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            s_bl.Volunteer.RemoveObserver(VolunteerId, RefreshClosedCallsObserver);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
