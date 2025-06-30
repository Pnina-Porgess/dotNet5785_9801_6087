using BlApi;
using PL;
using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Threading;

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

        // DispatcherOperation לפי אפשרות 2
        private volatile DispatcherOperation? _observerOperation = null;

        public VolunteerSelfWindow(int volunteerId)
        {
            InitializeComponent();
            Volunteer = _bl.Volunteer.GetVolunteerDetails(volunteerId);
            DataContext = this;

            // הרשמה ל־Observer
            _bl.Volunteer.AddObserver(volunteerId, RefreshVolunteerObserver);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _bl.Volunteer.RemoveObserver(Volunteer.Id, RefreshVolunteerObserver);
        }

        private void RefreshVolunteer()
        {
            Volunteer = _bl.Volunteer.GetVolunteerDetails(Volunteer.Id);
        }

        // ✅ פונקציית Observer נפרדת עם DispatcherOperation
        private void RefreshVolunteerObserver()
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
            {
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    RefreshVolunteer();
                });
            }
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

                if (Volunteer.MaxDistance <= 0)
                    throw new Exception("Please enter a valid maximum distance (must be a positive number).");

                if (!Regex.IsMatch(Volunteer.Phone, @"^0\d{9}$"))
                    throw new Exception("Invalid phone number. It must be 10 digits, e.g. 0501234567.");

                if (!Regex.IsMatch(Volunteer.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                    throw new Exception("Invalid email address.");

                if (!string.IsNullOrWhiteSpace(Volunteer.Password))
                {
                    if (Volunteer.Password.Length < 8 ||
                        !Regex.IsMatch(Volunteer.Password, @"[A-Z]") ||
                        !Regex.IsMatch(Volunteer.Password, @"[a-z]") ||
                        !Regex.IsMatch(Volunteer.Password, @"\d") ||
                        !Regex.IsMatch(Volunteer.Password, @"[\W_]"))
                    {
                        throw new Exception("Password must be at least 8 characters long and include an uppercase letter, a lowercase letter, a digit, and a special character.");
                    }
                }

                _bl.Volunteer.UpdateVolunteerDetails(Volunteer.Id, Volunteer);
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
            historyWindow.Show();
        }

        private void btnOpenCalls_Click(object sender, RoutedEventArgs e)
        {
            var openCalls = new PL.Call.OpenCallsWindow(Volunteer);

            // הוספת האזנה לסגירת החלון
            openCalls.Closed += (_, _) => RefreshVolunteer();

            openCalls.Show(); // לא חוסם את החלון הראשי
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

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
