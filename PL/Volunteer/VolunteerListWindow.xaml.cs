using BlApi;
using BO;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace PL.Volunteer
{
    public partial class VolunteerListWindow : Window
    {
        static readonly IBl s_bl = BlApi.Factory.Get();

        // DispatcherOperation לשימוש ב-Dispatcher על פי אפשרות 2
        private volatile DispatcherOperation? _observerOperation = null;

        public VolunteerListWindow()
        {
            InitializeComponent();
            DeleteVolunteerCommand = new RelayCommand<VolunteerInList>(DeleteVolunteer);
        }

        // תכונת רשימת מתנדבים
        public IEnumerable<VolunteerInList> VolunteerList
        {
            get { return (IEnumerable<VolunteerInList>)GetValue(VolunteerListProperty); }
            set { SetValue(VolunteerListProperty, value); }
        }

        public static readonly DependencyProperty VolunteerListProperty =
            DependencyProperty.Register("VolunteerList", typeof(IEnumerable<VolunteerInList>), typeof(VolunteerListWindow), new PropertyMetadata(null));

        // תכונות עזר לסינון ומיון
        public TypeOfReading TypeOfReading { get; set; } = TypeOfReading.None;
        public VolunteerSortBy SortBy { get; set; } = VolunteerSortBy.id;
        public VolunteerInList? SelectedVolunteer { get; set; }

        public ICommand DeleteVolunteerCommand { get; }

        // שאילתת נתונים מה-BL
        private void queryVolunteerList()
        {
            var list = (TypeOfReading == TypeOfReading.None)
                ? s_bl?.Volunteer.GetVolunteersList(null, SortBy, null)
                : s_bl?.Volunteer.GetVolunteersList(null, SortBy, TypeOfReading);
            VolunteerList = list!;
        }

        // מתודת ה-observer לצורך רענון השקפה, לפי אפשרות 2
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

        // הרשמה ל-observer בעת טעינת החלון
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Volunteer.AddObserver(RefreshVolunteerListObserver);
            queryVolunteerList();
        }

        // הסרה של ה-observer כשסוגרים את החלון
        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Volunteer.RemoveObserver(RefreshVolunteerListObserver);
        }

        // מיון/סינון – עדכון הרשימה
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            queryVolunteerList();
        }

        // לחיצה כפולה – פתיחת חלון מתנדב
        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedVolunteer != null)
            {
                new VolunteerWindow(SelectedVolunteer.Id).Show();
            }
        }

        // כפתור הוספת מתנדב
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            new VolunteerWindow().Show();
        }

        // מחיקת מתנדב
        private void DeleteVolunteer(VolunteerInList volunteer)
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
                catch (BlNotFoundException ex)
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
