using BlApi;
using BO;
using System;
using System.Collections.Generic;
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
            DeleteVolunteerCommand = new RelayCommand<BO.VolunteerInList>(DeleteVolunteer);
        }

        public IEnumerable<BO.VolunteerInList> VolunteerList
        {
            get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerListProperty); }
            set { SetValue(VolunteerListProperty, value); }
        }

        public static readonly DependencyProperty VolunteerListProperty =
            DependencyProperty.Register("VolunteerList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerListWindow), new PropertyMetadata(null));

        public BO.TypeOfReading TypeOfReading { get; set; } = BO.TypeOfReading.None;
        public BO.VolunteerSortBy SortBy { get; set; } = BO.VolunteerSortBy.id;
        public BO.VolunteerInList? SelectedVolunteer { get; set; }

        public ICommand DeleteVolunteerCommand { get; }

        private void queryVolunteerList()
        {
            var list = (TypeOfReading == BO.TypeOfReading.None)
                ? s_bl?.Volunteer.GetVolunteersList(null, SortBy, null)
                : s_bl?.Volunteer.GetVolunteersList(null, SortBy, TypeOfReading);
            VolunteerList = list!;
        }

        private void VolunteerListObserver() => queryVolunteerList();

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
                new VolunteerWindow(SelectedVolunteer.Id).Show();
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            new VolunteerWindow().Show();
        }

        private void DeleteVolunteer(BO.VolunteerInList volunteer)
        {
            if (volunteer == null) return;

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
