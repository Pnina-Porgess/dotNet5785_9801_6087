using System.Windows;
using System.Windows.Controls;

namespace PL.Login
{
    public partial class LoginWindow : Window
    {
        public readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public string UserId { get; set; } = "";
        public string Password { get; set; } = "";
        public string ErrorMessage { get; set; } = "";

        public LoginWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox pb)
                Password = pb.Password;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        
        {
            ErrorMessage = "";
            if (!int.TryParse(UserId, out int id))
            {
                
                ErrorMessage = "Invalid ID";
                OnPropertyChanged(nameof(ErrorMessage));
                return;
            }

            try
            {
                var volunteer = s_bl.Volunteer.GetVolunteerDetails(id);
                var role = s_bl.Volunteer.Login(volunteer.FullName, Password);

                if (role == BO.Role.Manager)
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
                        var volunteerSelfWindow = new Volunteer.VolunteerSelfWindow(id);
                        volunteerSelfWindow.Show();
                    }
                }
                else
                {
                    var volunteerSelfWindow = new Volunteer.VolunteerSelfWindow(id);
                    volunteerSelfWindow.Show();
                }
            }
            catch (System.Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            OnPropertyChanged(nameof(ErrorMessage));
        }

        // INotifyPropertyChanged למען ה-Binding
        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
}
