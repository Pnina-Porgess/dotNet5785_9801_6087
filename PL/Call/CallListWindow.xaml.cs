using BlApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PL.Call
{
    public partial class CallListWindow : Window
    {
        static readonly IBl s_bl = Factory.Get();

        public BO.CallInList? SelectedCall { get; set; }
        public int VolunteerId { get; set; }

        public CallListWindow(int volunteerId)
        {
            VolunteerId = volunteerId;
            InitializeComponent();
        }

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

        private static void OnFilterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is CallListWindow window)
                window.queryCallList();
        }

        private void queryCallList()
        {
            BO.CallField? filterField = SelectedField;
            BO.CallField? sortField = SelectedField;

            IEnumerable<BO.CallInList> list = s_bl?.Call.GetCalls(filterField, SelectedStatus, sortField)!;

            if (SelectedStatus != null)
                list = list.Where(c => c.CallStatus == SelectedStatus);

            CallList = list;
        }

        private void callListObserver() => queryCallList();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Call.AddObserver(callListObserver);
            queryCallList();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Call.RemoveObserver(callListObserver);
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedCall != null)
            {
                var window = new CallWindow(SelectedCall.CallId);
                window.Show();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is BO.CallInList call)
            {
                var result = MessageBox.Show($"Are you sure you want to delete call #{call.CallId}?",
                    "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        s_bl.Call.DeleteCall(call.CallId);
                        MessageBox.Show("Call deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                        queryCallList();
                    }
                    catch (BO.BlNotFoundException ex)
                    {
                        MessageBox.Show($"Cannot delete call: {ex.Message}", "Delete Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void UnassignButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is BO.CallInList call)
            {
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

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new CallWindow();
            window.Show();
        }
    }
}
