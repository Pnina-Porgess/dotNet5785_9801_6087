using BlApi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;

namespace PL
{
    /// <summary>
    /// Interaction logic for viewing a volunteer's call history.
    /// </summary>
    public partial class VolunteerHistoryWindow : Window, INotifyPropertyChanged
    {
        private static readonly IBl s_bl = Factory.Get();
        private volatile DispatcherOperation? _observerOperation = null;

        /// <summary>
        /// Initializes the window and loads the volunteer's call history.
        /// </summary>
        /// <param name="volunteerId">ID of the volunteer.</param>
        public VolunteerHistoryWindow(int volunteerId)
        {
            InitializeComponent();
            VolunteerId = volunteerId;
            RefreshClosedCallsObserver();
            s_bl.Volunteer.AddObserver(volunteerId, RefreshClosedCallsObserver);
        }

        /// <summary>
        /// Volunteer ID used for retrieving history.
        /// </summary>
        public int VolunteerId { get; set; }

        private BO.TypeOfReading _selectedCallType = BO.TypeOfReading.None;
        public BO.TypeOfReading SelectedCallType
        {
            get => _selectedCallType;
            set
            {
                _selectedCallType = value;
                OnPropertyChanged();
                RefreshClosedCallsObserver();
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
                RefreshClosedCallsObserver();
            }
        }

        private IEnumerable<BO.ClosedCallInList> _closedCalls = Enumerable.Empty<BO.ClosedCallInList>();
        public IEnumerable<BO.ClosedCallInList> ClosedCalls
        {
            get => _closedCalls;
            set
            {
                _closedCalls = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Retrieves the closed calls for the volunteer, filtered and sorted.
        /// Uses DispatcherOperation to avoid multiple concurrent refreshes.
        /// </summary>
        private void RefreshClosedCallsObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
            {
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    try
                    {
                        var calls = s_bl.Call.GetClosedCallsByVolunteer(
                            volunteerId: VolunteerId,
                            SelectedCallType == BO.TypeOfReading.None ? null : SelectedCallType,
                            sortField: SelectedSortOption
                        );

                        ClosedCalls = calls.ToList();
                    }
                    catch (BO.BlNotFoundException ex)
                    {
                        MessageBox.Show("Volunteer not found: " + ex.Message, "Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    catch (BO.BlDatabaseException ex)
                    {
                        MessageBox.Show("Error accessing database: " + ex.Message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Unexpected error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                });
            }
        }

        /// <summary>
        /// Unsubscribes the observer when window is closed.
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            s_bl.Volunteer.RemoveObserver(VolunteerId, RefreshClosedCallsObserver);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Notifies that a property value has changed.
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
