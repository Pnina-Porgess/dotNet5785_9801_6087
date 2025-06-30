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

        public CallListWindow(int volunteerId)
        {
            VolunteerId = volunteerId;
            InitializeComponent();

            DeleteCallCommand = new RelayCommand<BO.CallInList>(DeleteCall);
            UnassignCallCommand = new RelayCommand<BO.CallInList>(UnassignCall);
        }

        private static void OnFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CallListWindow window)
                window.queryCallList();
        }

        private void queryCallList()
        {
            BO.CallField? filterField = SelectedField;
            BO.CallField? sortField = SelectedField;

            IEnumerable<BO.CallInList> list = s_bl?.Call.GetCalls(filterField,
                (SelectedStatus == BO.CallStatus.None) ? null : SelectedStatus, sortField)!;

            if (SelectedStatus != null && SelectedStatus != BO.CallStatus.None)
                list = list.Where(c => c.CallStatus == SelectedStatus);

            CallList = list;
        }

        private void RefreshCallListObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
            {
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    queryCallList();
                });
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Call.AddObserver(RefreshCallListObserver);
            queryCallList();
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

        private void DeleteCall(BO.CallInList call)
        {
            if (call == null) return;

            var result = MessageBox.Show($"האם אתה בטוח שברצונך למחוק את הקריאה #{call.CallId}?",
                "אישור מחיקה", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    s_bl.Call.DeleteCall(call.CallId);
                    MessageBox.Show("הקריאה נמחקה בהצלחה.", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);
                    queryCallList();
                }
                catch (BO.BlNotFoundException ex)
                {
                    MessageBox.Show($"לא ניתן למחוק את הקריאה: {ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"שגיאה לא צפויה: {ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void UnassignCall(BO.CallInList call)
        {
            if (call == null) return;

            if (call.CallStatus != BO.CallStatus.InProgress)
            {
                MessageBox.Show("ניתן לבטל הקצאה רק לקריאה שנמצאת בטיפול.", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                s_bl.Call.CancelCallTreatment(VolunteerId, call.AssignmentId!.Value);
                MessageBox.Show("ההקצאה בוטלה בהצלחה.", "בוצע", MessageBoxButton.OK, MessageBoxImage.Information);
                queryCallList();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"שגיאה בביטול הקצאה: {ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
