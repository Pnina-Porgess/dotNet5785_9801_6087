using BlApi;
using Microsoft.VisualBasic;
using PL.Volunteer;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PL.Call
{
    /// <summary>
    /// Interaction logic for CallListWindow.xaml
    /// </summary>
    public partial class CallListWindow : Window
    {
        static readonly IBl s_bl = BlApi.Factory.Get();
        public BO.CallInList? SelectedCall { get; set; }
        public CallListWindow()
        {
            InitializeComponent();
        }
       public IEnumerable<BO.CallInList> CallList
{
    get { return (IEnumerable<BO.CallInList>)GetValue(CallListProperty); }
    set { SetValue(CallListProperty, value); }
}
public static readonly DependencyProperty CallListProperty =
    DependencyProperty.Register("CallList", typeof(IEnumerable<BO.CallInList>), typeof(CallListWindow), new PropertyMetadata(null));

        public BO.CallField CallType { get; set; } = BO.CallField.AssignmentId;
        private void queryCallList()
           => CallList = (CallType == BO.CallField.None) ?
               s_bl?.Call.GetCalls(null, null, null)! : s_bl?.Call.GetCalls(null, null, CallType)!;

        private void callListObserver()
            => queryCallList();
 
private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Call.AddObserver(callListObserver);
            queryCallList();
        }

        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Call.RemoveObserver(callListObserver);

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            queryCallList();
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
                var result = MessageBox.Show(
                    $"Are you sure you want to delete call #{call.CallId}?",
                    "Confirm Deletion",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        s_bl.Call.DeleteCall(call.CallId);
                        MessageBox.Show("Call deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new CallWindow(); // פתיחה ללא מזהה -> מצב הוספה
            window.Show();
        }

    }
}
