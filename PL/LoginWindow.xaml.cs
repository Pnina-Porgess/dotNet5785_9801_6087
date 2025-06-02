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

using System.Windows;
using BO;

using System.Windows;
using BO;

namespace PL.Login
{
    public partial class LoginWindow : Window
    {
        private readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorTextBlock.Text = "";
            if (!int.TryParse(UserIdTextBox.Text, out int id))
            {
                ErrorTextBlock.Text = "Invalid ID";
                return;
            }

            try
            {
                // נניח ששם המשתמש הוא ת.ז
                var volunteer = s_bl.Volunteer.GetVolunteerDetails(id);
                var role = s_bl.Volunteer.Login(volunteer.FullName, PasswordBox.Password);

                if (role == Role.Manager)
                {
                    var result = MessageBox.Show(
                        "כניסה כמנהל - האם להיכנס למסך ניהול ראשי?\n(כן - ניהול, לא - מתנדב)",
                        "בחירת מסך", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        var adminWindow = new MainWindow(); // שנה לחלון הניהול שלך
                        adminWindow.Show();
                    }
                    else if (result == MessageBoxResult.No)
                    {
                        var volunteerWindow = new Volunteer.VolunteerWindow(id);
                        volunteerWindow.Show();
                    }
                }
                else
                {
                    var volunteerWindow = new Volunteer.VolunteerWindow(id);
                    volunteerWindow.Show();
                }
            }
            catch (Exception ex)
            {
                ErrorTextBlock.Text = ex.Message;
            }
        }
    }
}

