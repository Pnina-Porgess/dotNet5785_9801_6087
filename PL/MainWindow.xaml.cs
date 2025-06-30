using PL.Call;
using PL.Volunteer;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace PL
{
    public partial class MainWindow : Window
    {
        public MainWindow() : this(0) // מפעיל את הבנאי הקיים עם volunteerId=0
        {
          

            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed;
        }
        public MainWindow(int volunteerId)
        {
            id = volunteerId;
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            this.Closed += MainWindow_Closed;
        }

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        private volatile DispatcherOperation? _clockObserverOperation = null;
        private volatile DispatcherOperation? _callStatisticsObserverOperation = null;

        public int id { get; set; } // Connected volunteer ID

        // Static fields for tracking open windows
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

        // Call Statistics Properties
        public int OpenCallsCount
        {
            get { return (int)GetValue(OpenCallsCountProperty); }
            set { SetValue(OpenCallsCountProperty, value); }
        }
        public static readonly DependencyProperty OpenCallsCountProperty =
            DependencyProperty.Register("OpenCallsCount", typeof(int), typeof(MainWindow));

        public int InProgressCallsCount
        {
            get { return (int)GetValue(InProgressCallsCountProperty); }
            set { SetValue(InProgressCallsCountProperty, value); }
        }
        public static readonly DependencyProperty InProgressCallsCountProperty =
            DependencyProperty.Register("InProgressCallsCount", typeof(int), typeof(MainWindow));

        public int ClosedCallsCount
        {
            get { return (int)GetValue(ClosedCallsCountProperty); }
            set { SetValue(ClosedCallsCountProperty, value); }
        }
        public static readonly DependencyProperty ClosedCallsCountProperty =
            DependencyProperty.Register("ClosedCallsCount", typeof(int), typeof(MainWindow));

        public int ExpiredCallsCount
        {
            get { return (int)GetValue(ExpiredCallsCountProperty); }
            set { SetValue(ExpiredCallsCountProperty, value); }
        }
        public static readonly DependencyProperty ExpiredCallsCountProperty =
            DependencyProperty.Register("ExpiredCallsCount", typeof(int), typeof(MainWindow));

        public int OpenAtRiskCallsCount
        {
            get { return (int)GetValue(OpenAtRiskCallsCountProperty); }
            set { SetValue(OpenAtRiskCallsCountProperty, value); }
        }
        public static readonly DependencyProperty OpenAtRiskCallsCountProperty =
            DependencyProperty.Register("OpenAtRiskCallsCount", typeof(int), typeof(MainWindow));

        public int InProgressAtRiskCallsCount
        {
            get { return (int)GetValue(InProgressAtRiskCallsCountProperty); }
            set { SetValue(InProgressAtRiskCallsCountProperty, value); }
        }
        public static readonly DependencyProperty InProgressAtRiskCallsCountProperty =
            DependencyProperty.Register("InProgressAtRiskCallsCount", typeof(int), typeof(MainWindow));
        public static readonly DependencyProperty IntervalProperty =
            DependencyProperty.Register("Interval", typeof(int), typeof(MainWindow), new PropertyMetadata(1));

        public int Interval
        {
            get => (int)GetValue(IntervalProperty);
            set => SetValue(IntervalProperty, value);
        }

        public static readonly DependencyProperty IsSimulatorRunningProperty =
            DependencyProperty.Register("IsSimulatorRunning", typeof(bool), typeof(MainWindow));

        public bool IsSimulatorRunning
        {
            get => (bool)GetValue(IsSimulatorRunningProperty);
            set => SetValue(IsSimulatorRunningProperty, value);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                CurrentTime = s_bl.Admin.GetClock();
                RiskRange = s_bl.Admin.GetRiskRange();
                s_bl.Admin.AddClockObserver(clockObserver);
                s_bl.Admin.AddConfigObserver(configObserver);
                s_bl.Call.AddObserver(callStatisticsObserver); // Add observer for call statistics
                LoadCallStatistics(); // Load initial statistics
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading screen: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            s_bl.Admin.RemoveClockObserver(clockObserver);
            s_bl.Admin.RemoveConfigObserver(configObserver);
            s_bl.Call.RemoveObserver(callStatisticsObserver);

            if (IsSimulatorRunning)
            {
                try { s_bl.Admin.StopSimulator(); } catch { /* לא לעצור אם נכשל */ }
            }
        }


        private void LoadCallStatistics()
        {
            try
            {
                var counts = s_bl.Call.GetCallCountsByStatus();
                // Array indices correspond to CallStatus enum values:
                // 0: Open, 1: InProgress, 2: Closed, 3: Expired, 4: OpenAtRisk, 5: InProgressAtRisk
                OpenCallsCount = counts.Length > 0 ? counts[0] : 0;
                InProgressCallsCount = counts.Length > 1 ? counts[1] : 0;
                ClosedCallsCount = counts.Length > 2 ? counts[2] : 0;
                ExpiredCallsCount = counts.Length > 3 ? counts[3] : 0;
                OpenAtRiskCallsCount = counts.Length > 4 ? counts[4] : 0;
                InProgressAtRiskCallsCount = counts.Length > 5 ? counts[5] : 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading call statistics: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void callStatisticsObserver()
        {
            if (_callStatisticsObserverOperation is null || _callStatisticsObserverOperation.Status == DispatcherOperationStatus.Completed)
                _callStatisticsObserverOperation = Dispatcher.BeginInvoke(() =>
                {
                    LoadCallStatistics();
                });
        }

        // Call Statistics Button Handlers
        private void OpenCalls_Click(object sender, RoutedEventArgs e) => OpenCallListWithFilter(BO.CallStatus.Open);
        private void InProgressCalls_Click(object sender, RoutedEventArgs e) => OpenCallListWithFilter(BO.CallStatus.InProgress);
        private void ClosedCalls_Click(object sender, RoutedEventArgs e) => OpenCallListWithFilter(BO.CallStatus.Closed);
        private void ExpiredCalls_Click(object sender, RoutedEventArgs e) => OpenCallListWithFilter(BO.CallStatus.Expired);
        private void OpenAtRiskCalls_Click(object sender, RoutedEventArgs e) => OpenCallListWithFilter(BO.CallStatus.OpenAtRisk);
        private void InProgressAtRiskCalls_Click(object sender, RoutedEventArgs e) => OpenCallListWithFilter(BO.CallStatus.InProgressAtRisk);

        private void OpenCallListWithFilter(BO.CallStatus status)
        {
            try
            {
                if (callWindow == null || !callWindow.IsVisible)
                {
                    callWindow = new CallListWindow(id);
                    callWindow.SelectedStatus = status; // Set the filter
                    callWindow.Closed += (s, args) => callWindow = null;
                    callWindow.Show();
                }
                else
                {
                    if (callWindow.WindowState == WindowState.Minimized)
                        callWindow.WindowState = WindowState.Normal;
                    callWindow.SelectedStatus = status; // Update the filter
                    callWindow.Activate();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening call list: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Admin.ForwardClock(BO.TimeUnit.Minute);
                MessageBox.Show("Time advanced by 1 minute successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error advancing time: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Admin.ForwardClock(BO.TimeUnit.Hour);
                MessageBox.Show("Time advanced by 1 hour successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error advancing time: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Admin.ForwardClock(BO.TimeUnit.Day);
                MessageBox.Show("Time advanced by 1 day successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error advancing time: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAddOneMonth_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Admin.ForwardClock(BO.TimeUnit.Month);
                MessageBox.Show("Time advanced by 1 month successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error advancing time: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Admin.ForwardClock(BO.TimeUnit.Year);
                MessageBox.Show("Time advanced by 1 year successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error advancing time: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Admin.SetRiskRange(RiskRange);
                MessageBox.Show("Risk range updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating risk range: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void clockObserver()
        {
            if (_clockObserverOperation is null || _clockObserverOperation.Status == DispatcherOperationStatus.Completed)
                _clockObserverOperation = Dispatcher.BeginInvoke(() =>
                {
                    CurrentTime = s_bl.Admin.GetClock();
                });
        }


        private void configObserver() =>
            RiskRange = s_bl.Admin.GetRiskRange();

        private void OnHandleCallsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (callWindow == null || !callWindow.IsVisible)
                {
                    callWindow = new CallListWindow(id);
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
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening call management window: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnHandleVolunteersButton_Click(object sender, RoutedEventArgs e)
        {
            try
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
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening volunteer management window: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitializeDatabase_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to initialize the database?", "Initialize Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                foreach (Window window in Application.Current.Windows)
                    if (window != this) window.Close();

                s_bl.Admin.InitializeDB();
                MessageBox.Show("Database initialized successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing database: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void ResetDatabase_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to reset the database?", "Reset Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes)
                return;

            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                foreach (Window window in Application.Current.Windows)
                    if (window != this) window.Close();

                s_bl.Admin.ResetDB();
                MessageBox.Show("Database reset successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error resetting database: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }
        private async void ToggleSimulator_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!IsSimulatorRunning)
                {
                    s_bl.Admin.StartSimulator(Interval);
                    IsSimulatorRunning = true;
                }
                else
                {
                    await Task.Run(() => s_bl.Admin.StopSimulator()); // אם זה חסום אתה יכול לשים סינכרוני
                    IsSimulatorRunning = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error controlling simulator: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}