using BlApi;
using PL;
using System.ComponentModel;
using System.Windows;

namespace PL.Volunteer
{
    public partial class VolunteerSelfWindow : Window, INotifyPropertyChanged
    {
        private readonly IBl _bl = Factory.Get();

        private BO.Volunteer _volunteer;
        public BO.Volunteer Volunteer
        {
            get => _volunteer;
            set
            {
                _volunteer = value;
                OnPropertyChanged(nameof(Volunteer));
            }
        }

        public VolunteerSelfWindow(int volunteerId)
        {
            InitializeComponent();
            Volunteer = _bl.Volunteer.GetVolunteerDetails(volunteerId);
            DataContext = this;

            // ✅ הוספת Observer לעדכון אוטומטי
            _bl.Volunteer.AddObserver(volunteerId, RefreshVolunteer);
        }

        // ✅ הסרת Observer בסגירת החלון למניעת דליפות זיכרון
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _bl.Volunteer.RemoveObserver(Volunteer.Id, RefreshVolunteer);
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Volunteer == null)
                    throw new Exception("Volunteer details failed to load.");

                if (string.IsNullOrWhiteSpace(Volunteer.FullName))
                    throw new Exception("Please enter full name.");

                if (string.IsNullOrWhiteSpace(Volunteer.Phone))
                    throw new Exception("Please enter phone number.");

                if (string.IsNullOrWhiteSpace(Volunteer.Email))
                    throw new Exception("Please enter email address.");

                if (string.IsNullOrWhiteSpace(Volunteer.Password))
                    throw new Exception("Please enter a password.");

                if (Volunteer.MaxDistance <= 0)
                    throw new Exception("Please enter a valid maximum distance (must be a positive number).");

                if (!System.Text.RegularExpressions.Regex.IsMatch(Volunteer.Phone, @"^0\d{9}$"))
                    throw new Exception("Invalid phone number. It must be 10 digits, e.g. 0501234567.");

                if (!System.Text.RegularExpressions.Regex.IsMatch(Volunteer.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    throw new Exception("Invalid email address.");

                if (string.IsNullOrWhiteSpace(Volunteer.Password) || Volunteer.Password.Length < 8 ||
                    !System.Text.RegularExpressions.Regex.IsMatch(Volunteer.Password, @"[A-Z]") ||  // upper case
                    !System.Text.RegularExpressions.Regex.IsMatch(Volunteer.Password, @"[a-z]") ||  // lower case
                    !System.Text.RegularExpressions.Regex.IsMatch(Volunteer.Password, @"\d") ||     // digit
                    !System.Text.RegularExpressions.Regex.IsMatch(Volunteer.Password, @"[\W_]"))    // special char
                {
                    throw new Exception("Password must be at least 8 characters long and include an uppercase letter, a lowercase letter, a digit, and a special character.");
                }

                // שליחת העדכון ל־BL
                _bl.Volunteer.UpdateVolunteerDetails(Volunteer.Id,Volunteer);
                MessageBox.Show("הפרטים עודכנו בהצלחה!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        private void btnHistory_Click(object sender, RoutedEventArgs e)
        {
            var historyWindow = new VolunteerHistoryWindow(Volunteer.Id);
            historyWindow.ShowDialog();
        }

        private void btnOpenCalls_Click(object sender, RoutedEventArgs e)
        {
            var openCalls = new PL.Call.OpenCallsWindow(Volunteer);
            openCalls.ShowDialog();
            RefreshVolunteer(); // ריענון לאחר בחירת קריאה
        }

        private void FinishCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _bl.Call.CompleteCallTreatment(Volunteer.Id, Volunteer.CurrentCall.Id);
                MessageBox.Show("הטיפול הסתיים בהצלחה.");
                RefreshVolunteer(); // ריענון מקומי, ו־Observer יעדכן אחרים
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה: " + ex.Message);
            }
        }

        private void CancelCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _bl.Call.CancelCallTreatment(Volunteer.Id, Volunteer.CurrentCall.Id);
                MessageBox.Show("הטיפול בוטל.");
                RefreshVolunteer(); // ריענון מקומי, ו־Observer יעדכן אחרים
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה: " + ex.Message);
            }
        }

        private void RefreshVolunteer()
        {
            Volunteer = _bl.Volunteer.GetVolunteerDetails(Volunteer.Id);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
