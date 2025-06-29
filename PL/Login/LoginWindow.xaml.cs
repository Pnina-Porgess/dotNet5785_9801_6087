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
            try
            {
                ErrorMessage = "";
                OnPropertyChanged(nameof(ErrorMessage));

                if (!int.TryParse(UserId, out int id))
                {
                    ErrorMessage = "Invalid ID number - please enter numbers only.";
                    OnPropertyChanged(nameof(ErrorMessage));
                    MessageBox.Show(ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var role = s_bl.Volunteer.Login(id, Password);

                if (role == BO.Role.Manager)
                {
                    var result = MessageBox.Show(
                        "Manager login - Would you like to enter the main management screen?\n(Yes - Management, No - Volunteer)",
                        "Screen Selection", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        var adminWindow = new MainWindow(id);
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
                ErrorMessage = "An error occurred: " + ex.Message;
                OnPropertyChanged(nameof(ErrorMessage));
                MessageBox.Show(ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // INotifyPropertyChanged for data binding
        public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
}