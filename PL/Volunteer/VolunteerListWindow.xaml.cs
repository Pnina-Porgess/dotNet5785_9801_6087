// VolunteerListWindow.xaml.cs
using BlApi;
using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PL.Volunteer
{
    public partial class VolunteerListWindow : Window
    {
        static readonly IBl s_bl = BlApi.Factory.Get();

        public VolunteerListWindow()
        {
            InitializeComponent();
        }

        public IEnumerable<BO.VolunteerInList> VolunteerList
        {
            get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerListProperty); }
            set { SetValue(VolunteerListProperty, value); }
        }

        public static readonly DependencyProperty VolunteerListProperty =
            DependencyProperty.Register("VolunteerList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerListWindow), new PropertyMetadata(null));

        public BO.TypeOfReading TypeOfReading { get; set; } = BO.TypeOfReading.None;
        public BO.TypeOfReading SortByReading { get; set; } = BO.TypeOfReading.None;
        public BO.VolunteerInList? SelectedVolunteer { get; set; }

        private void queryVolunteerList()
        {
            var list = (TypeOfReading == BO.TypeOfReading.None)
                ? s_bl?.Volunteer.GetVolunteersList()
                : s_bl?.Volunteer.GetVolunteersList(null, null, TypeOfReading);

            if (SortByReading != BO.TypeOfReading.None)
            {
                list = list?.OrderBy(v => v.CurrentCallType != null && v.CurrentCallType == SortByReading ? 0 : 1)
                             .ThenBy(v => v.CurrentCallType);
            }

            VolunteerList = list!;
        }

        private void VolunteerListObserver()
            => queryVolunteerList();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Volunteer.AddObserver(VolunteerListObserver);
            queryVolunteerList();
        }

        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Volunteer.RemoveObserver(VolunteerListObserver);

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            queryVolunteerList();
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedVolunteer != null)
            {
                var window = new VolunteerWindow(SelectedVolunteer.Id);
                window.Show();
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new VolunteerWindow();
            window.Show();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is BO.VolunteerInList volunteer)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete volunteer #{volunteer.Id} ({volunteer.FullName})?",
                    "Confirm Deletion",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        s_bl.Volunteer.DeleteVolunteer(volunteer.Id);
                        MessageBox.Show("Volunteer deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (BO.BlNotFoundException ex)
                    {
                        MessageBox.Show($"Cannot delete volunteer: {ex.Message}", "Delete Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}
