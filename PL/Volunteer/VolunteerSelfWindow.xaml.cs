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
                OnPropertyChanged(nameof(HasCurrentCall));
                OnPropertyChanged(nameof(NoCurrentCall));
            }
        }

        public bool HasCurrentCall => Volunteer.CurrentCall != null;
        public bool NoCurrentCall => !HasCurrentCall;

        public VolunteerSelfWindow(int volunteerId)
        {
            InitializeComponent();
            Volunteer = _bl.Volunteer.GetVolunteerDetails(volunteerId);
            DataContext = this;
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _bl.Volunteer.UpdateVolunteerDetails(Volunteer.Id, Volunteer);
                MessageBox.Show("עודכן בהצלחה!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("שגיאה: " + ex.Message);
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
            RefreshVolunteer();
        }

        private void FinishCall_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _bl.Call.CompleteCallTreatment(Volunteer.Id, Volunteer.CurrentCall.Id);
                MessageBox.Show("הטיפול הסתיים בהצלחה.");
                RefreshVolunteer();
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
                RefreshVolunteer();
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
