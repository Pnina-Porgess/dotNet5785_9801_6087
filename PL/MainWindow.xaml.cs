using PL.Call;
using PL.Volunteer;
using System;
using System.Windows;
using System.Windows.Input;

namespace PL
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed;
        }

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        // שדות סטטיים למעקב אחר חלונות פתוחים
        private static CallListWindow? callWindow;
        private static VolunteerListWindow? volunteerWindow;

        public DateTime CurrentTime
        {
            get { return (DateTime)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }
        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow));

        public TimeSpan RiskRange
        {
            get { return (TimeSpan)GetValue(RiskRangeProperty); }
            set { SetValue(RiskRangeProperty, value); }
        }
        public static readonly DependencyProperty RiskRangeProperty =
            DependencyProperty.Register("RiskRange", typeof(TimeSpan), typeof(MainWindow));

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                CurrentTime = s_bl.Admin.GetClock();
                RiskRange = s_bl.Admin.GetRiskRange();
                s_bl.Admin.AddClockObserver(clockObserver);
                s_bl.Admin.AddConfigObserver(configObserver);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"שגיאה בטעינת המסך: {ex.Message}", "שגיאה");
            }
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            s_bl.Admin.RemoveClockObserver(clockObserver);
            s_bl.Admin.RemoveConfigObserver(configObserver);
        }

        private void btnAddOneMinute_Click(object sender, RoutedEventArgs e) =>
            s_bl.Admin.ForwardClock(BO.TimeUnit.Minute);
        private void btnAddOneHour_Click(object sender, RoutedEventArgs e) =>
            s_bl.Admin.ForwardClock(BO.TimeUnit.Hour);
        private void btnAddOneDay_Click(object sender, RoutedEventArgs e) =>
            s_bl.Admin.ForwardClock(BO.TimeUnit.Day);
        private void btnAddOneMonth_Click(object sender, RoutedEventArgs e) =>
            s_bl.Admin.ForwardClock(BO.TimeUnit.Month);
        private void btnAddOneYear_Click(object sender, RoutedEventArgs e) =>
            s_bl.Admin.ForwardClock(BO.TimeUnit.Year);

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.SetRiskRange(RiskRange);
        }

        private void clockObserver() =>
            CurrentTime = s_bl.Admin.GetClock();

        private void configObserver() =>
            RiskRange = s_bl.Admin.GetRiskRange();

        private void OnHandleCallsButton_Click(object sender, RoutedEventArgs e)
        {
            if (callWindow == null || !callWindow.IsVisible)
            {
                callWindow = new CallListWindow();
                callWindow.Closed += (s, args) => callWindow = null;
                callWindow.Show();
            }
            else
            {
                if (callWindow.WindowState == WindowState.Minimized)
                    callWindow.WindowState = WindowState.Normal;
                callWindow.Activate();
            }
        }

        private void OnHandleVolunteersButton_Click(object sender, RoutedEventArgs e)
        {
            if (volunteerWindow == null || !volunteerWindow.IsVisible)
            {
                volunteerWindow = new VolunteerListWindow();
                volunteerWindow.Closed += (s, args) => volunteerWindow = null;
                volunteerWindow.Show();
            }
            else
            {
                if (volunteerWindow.WindowState == WindowState.Minimized)
                    volunteerWindow.WindowState = WindowState.Normal;
                volunteerWindow.Activate();
            }
        }

        private void InitializeDatabase_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("האם אתה בטוח שברצונך לאתחל את בסיס הנתונים?", "אישור אתחול", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                foreach (Window window in Application.Current.Windows)
                    if (window != this) window.Close();

                s_bl.Admin.InitializeDB();
                MessageBox.Show("בסיס הנתונים אותחל בהצלחה.", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"אירעה שגיאה באתחול בסיס הנתונים: {ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void ResetDatabase_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("האם אתה בטוח שברצונך לאפס את בסיס הנתונים?", "אישור איפוס", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                foreach (Window window in Application.Current.Windows)
                    if (window != this) window.Close();

                s_bl.Admin.ResetDB();
                MessageBox.Show("בסיס הנתונים אופס בהצלחה.", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"אירעה שגיאה באיפוס בסיס הנתונים: {ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }
    }
}
