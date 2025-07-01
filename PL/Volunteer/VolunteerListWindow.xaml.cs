using BlApi;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for viewing, filtering, sorting, and managing the volunteer list.
    /// </summary>
    public partial class VolunteerListWindow : Window
    {
        static readonly IBl s_bl = BlApi.Factory.Get();

        private volatile DispatcherOperation? _observerOperation = null;

        /// <summary>
        /// Initializes the window and command bindings.
        /// </summary>
        public VolunteerListWindow()
        {
            InitializeComponent();
            DeleteVolunteerCommand = new RelayCommand<BO.VolunteerInList>(DeleteVolunteer);
        }

        /// <summary>
        /// The list of volunteers shown in the UI.
        /// </summary>
        public IEnumerable<BO.VolunteerInList> VolunteerList
        {
            get => (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerListProperty);
            set => SetValue(VolunteerListProperty, value);
        }

        public static readonly DependencyProperty VolunteerListProperty =
            DependencyProperty.Register("VolunteerList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerListWindow), new PropertyMetadata(null));

        /// <summary>
        /// Filtering and sorting options.
        /// </summary>
        public BO.TypeOfReading TypeOfReading { get; set; } = BO.TypeOfReading.None;
        public BO.VolunteerSortBy SortBy { get; set; } = BO.VolunteerSortBy.id;
        public BO.VolunteerInList? SelectedVolunteer { get; set; }

        public ICommand DeleteVolunteerCommand { get; }

        /// <summary>
        /// Queries the volunteer list based on the selected filters.
        /// </summary>
        private void queryVolunteerList()
        {
            var list = (TypeOfReading == BO.TypeOfReading.None)
                ? s_bl?.Volunteer.GetVolunteersList(null, SortBy, null)
                : s_bl?.Volunteer.GetVolunteersList(null, SortBy, TypeOfReading);

            VolunteerList = list!;
        }

        /// <summary>
        /// Refresh observer that updates the volunteer list on changes.
        /// </summary>
        private void RefreshVolunteerListObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
            {
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    queryVolunteerList();
                });
            }
        }

        /// <summary>
        /// Called when the window is loaded – sets observer and populates data.
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Volunteer.AddObserver(RefreshVolunteerListObserver);
            queryVolunteerList();
        }

        /// <summary>
        /// Called when the window is closed – removes observer.
        /// </summary>
        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Volunteer.RemoveObserver(RefreshVolunteerListObserver);
        }

        /// <summary>
        /// Called when filter/sort combo boxes are changed.
        /// </summary>
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            queryVolunteerList();
        }

        /// <summary>
        /// Opens volunteer details window on double click.
        /// </summary>
        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedVolunteer != null)
            {
                new VolunteerWindow(SelectedVolunteer.Id).Show();
            }
        }

        /// <summary>
        /// Opens the add-volunteer window.
        /// </summary>
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            new VolunteerWindow().Show();
        }

        /// <summary>
        /// Attempts to delete the selected volunteer after confirmation.
        /// </summary>
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
                    MessageBox.Show($"Volunteer not found: {ex.Message}", "Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (BO.InvalidOperationException ex)
                {
                    MessageBox.Show($"Cannot delete volunteer: {ex.Message}", "Invalid Operation", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                catch (BO.BlDatabaseException ex)
                {
                    MessageBox.Show($"Database error: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
