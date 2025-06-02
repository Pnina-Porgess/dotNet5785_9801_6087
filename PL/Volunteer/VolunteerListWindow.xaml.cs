using BlApi;
using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerListWindow.xaml
    /// </summary>
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
        private void queryCallList()
           => VolunteerList = (TypeOfReading == BO.TypeOfReading.None) ?
               s_bl?.Volunteer.GetVolunteersList()! : s_bl?.Volunteer.GetVolunteersList(null, null, TypeOfReading)!;

        private void VolunteerListObserver()
            => queryCallList();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            s_bl.Call.AddObserver(VolunteerListObserver);
            queryCallList();
        }

        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Call.RemoveObserver(VolunteerListObserver);

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            queryCallList();
        }
        public BO.VolunteerInList? SelectedVolunteer { get; set; }
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
            if (sender is Button deleteButton && deleteButton.Tag is BO.VolunteerInList volunteer)
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
                        // אם יש מנגנון Observer כמו שתיארת – הרשימה תתעדכן לבד
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

