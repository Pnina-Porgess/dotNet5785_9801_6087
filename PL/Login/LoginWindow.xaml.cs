using System.Windows;
using System.Windows.Controls;
using BlApi;
using BO;
using System.ComponentModel;

namespace PL.Login
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window, INotifyPropertyChanged
    {
        public readonly IBl s_bl = Factory.Get();

        /// <summary>
        /// User ID entered by the user.
        /// </summary>
        public string UserId { get; set; } = "";

        /// <summary>
        /// Password entered by the user.
        /// </summary>
        public string Password { get; set; } = "";

        /// <summary>
        /// Message to be displayed in case of errors.
        /// </summary>
        public string ErrorMessage { get; set; } = "";

        public LoginWindow()
        {
            InitializeComponent();        }

        /// <summary>
        /// Handles password change in PasswordBox.
        /// </summary>
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox pb)
                Password = pb.Password;
        }

        /// <summary>
        /// Handles login button click: validates input, performs login and navigates based on role.
        /// </summary>
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ErrorMessage = "";
                OnPropertyChanged(nameof(ErrorMessage));

                if (!int.TryParse(UserId, out int id))
                {
                    ShowError("Invalid ID number - please enter digits only.");
                    return;
                }

                Role role = s_bl.Volunteer.Login(id, Password);

                if (role == Role.Manager)
                {
                    var result = MessageBox.Show(
                        "Manager login - Would you like to enter the main management screen?\n(Yes - Management, No - Volunteer)",
                        "Screen Selection", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        new MainWindow(id).Show();
                        
                    }
                    else if (result == MessageBoxResult.No)
                    {
                        new Volunteer.VolunteerSelfWindow(id).Show();
                       
                    }
                }
                else
                {
                    new Volunteer.VolunteerSelfWindow(id).Show();
                }
            }
            catch (BlInvalidInputException ex)
            {
                ShowError($"Invalid input: {ex.Message}");
            }
            catch (BlNotFoundException ex)
            {
                ShowError($"User not found: {ex.Message}");
            }
            catch (BlUnauthorizedAccessException ex)
            {
                ShowError($"Unauthorized: {ex.Message}");
            }
            catch (System.Exception ex)
            {
                ShowError($"An unexpected error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Displays an error message in the UI and message box.
        /// </summary>
        private void ShowError(string message)
        {
            ErrorMessage = message;
            OnPropertyChanged(nameof(ErrorMessage));
            MessageBox.Show(message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        /// <summary>
        /// PropertyChanged event for data binding.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Notifies the UI that a property value has changed.
        /// </summary>
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
