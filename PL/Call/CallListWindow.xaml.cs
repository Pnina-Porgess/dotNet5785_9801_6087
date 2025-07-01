using BlApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for CallListWindow.xaml
    /// </summary>
    public partial class CallListWindow : Window
    {
        static readonly IBl s_bl = Factory.Get();

        public BO.CallInList? SelectedCall { get; set; }
        public int VolunteerId { get; set; }

        public ICommand DeleteCallCommand { get; }
        public ICommand UnassignCallCommand { get; }

        private volatile DispatcherOperation? _observerOperation = null;

        public IEnumerable<BO.CallInList> CallList
        {
            get => (IEnumerable<BO.CallInList>)GetValue(CallListProperty);
            set => SetValue(CallListProperty, value);
        }
        public static readonly DependencyProperty CallListProperty =
            DependencyProperty.Register("CallList", typeof(IEnumerable<BO.CallInList>), typeof(CallListWindow), new PropertyMetadata(null));

        public BO.CallField? SelectedField
        {
            get => (BO.CallField?)GetValue(SelectedFieldProperty);
            set => SetValue(SelectedFieldProperty, value);
        }
        public static readonly DependencyProperty SelectedFieldProperty =
            DependencyProperty.Register("SelectedField", typeof(BO.CallField?), typeof(CallListWindow), new PropertyMetadata(null, OnFilterChanged));

        public BO.CallStatus? SelectedStatus
        {
            get => (BO.CallStatus?)GetValue(SelectedStatusProperty);
            set => SetValue(SelectedStatusProperty, value);
        }
        public static readonly DependencyProperty SelectedStatusProperty =
            DependencyProperty.Register("SelectedStatus", typeof(BO.CallStatus?), typeof(CallListWindow), new PropertyMetadata(null, OnFilterChanged));

        /// <summary>
        /// Initializes the window with a specific volunteer ID.
        /// </summary>
        public CallListWindow(int volunteerId)
        {
            VolunteerId = volunteerId;
            InitializeComponent();

            DeleteCallCommand = new RelayCommand<BO.CallInList>(DeleteCall);
            UnassignCallCommand = new RelayCommand<BO.CallInList>(UnassignCall);
        }

        /// <summary>
        /// Handles filter change and refreshes the call list.
        /// </summary>
        private static void OnFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CallListWindow window)
                window.QueryCallList();
        }

        /// <summary>
        /// Queries and filters the call list from the business layer.
        /// </summary>
        private void QueryCallList()
        {
            try
            {
                BO.CallField? filterField = SelectedField;
                BO.CallField? sortField = SelectedField;

                IEnumerable<BO.CallInList> list = s_bl?.Call.GetCalls(
                    filterField,
                    (SelectedStatus == BO.CallStatus.None) ? null : SelectedStatus,
                    sortField)!;

                if (SelectedStatus != null && SelectedStatus != BO.CallStatus.None)
                    list = list.Where(c => c.CallStatus == SelectedStatus);

                CallList = list;
            }
            catch (BO.BlGeneralException ex)
            {
                MessageBox.Show($"Failed to retrieve calls: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Refreshes the call list asynchronously when notified.
        /// </summary>
        private void RefreshCallListObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
            {
                _observerOperation = Dispatcher.BeginInvoke(() => QueryCallList());
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Call.AddObserver(RefreshCallListObserver);
            QueryCallList();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Call.RemoveObserver(RefreshCallListObserver);
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedCall != null)
            {
                var window = new CallWindow(SelectedCall.CallId);
                window.Show();
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new CallWindow();
            window.Show();
        }

        /// <summary>
        /// Deletes the selected call after confirmation.
        /// </summary>
        private void DeleteCall(BO.CallInList call)
        {
            if (call == null) return;

            var result = MessageBox.Show($"Are you sure you want to delete Call #{call.CallId}?",
                "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    s_bl.Call.DeleteCall(call.CallId);
                    MessageBox.Show("Call deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    QueryCallList();
                }
                catch (BO.BlNotFoundException ex)
                {
                    MessageBox.Show($"Cannot delete call: {ex.Message}", "Not Found", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (BO.BLTemporaryNotAvailableException ex)
                {
                    MessageBox.Show($"Call cannot be deleted right now: {ex.Message}", "Temporary Error", MessageBoxButton.OK, MessageBoxImage.Warning);
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

        /// <summary>
        /// Cancels the assignment of the selected call if it's in progress.
        /// </summary>
        private void UnassignCall(BO.CallInList call)
        {
            if (call == null) return;

            if (call.CallStatus != BO.CallStatus.InProgress)
            {
                MessageBox.Show("Only calls in progress can be unassigned.", "Invalid Action", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                s_bl.Call.CancelCallTreatment(VolunteerId, call.AssignmentId!.Value);
                MessageBox.Show("Call unassignment completed successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                QueryCallList();
            }
            catch (BO.BlNotFoundException ex)
            {
                MessageBox.Show($"Unassignment failed: {ex.Message}", "Not Found", MessageBoxButton.OK, MessageBoxImage.Error);
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
