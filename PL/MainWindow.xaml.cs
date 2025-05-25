using PL.Call;
using PL.Volunteer;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PL;

/// <summary>
/// Main window logic
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        this.Loaded += MainWindow_Loaded;
        this.Closed += MainWindow_Closed;
    }

    // Called when the window is loaded
    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            // Get initial values from the BL
            CurrentTime = s_bl.Admin.GetClock();
            RiskRange = s_bl.Admin.GetRiskRange();

            // Subscribe to observers
            s_bl.Admin.AddClockObserver(clockObserver);
            s_bl.Admin.AddConfigObserver(configObserver);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading screen: {ex.Message}", "Error");
        }
    }

    // Called when the window is closed
    private void MainWindow_Closed(object sender, EventArgs e)
    {
        // Remove observers when closing
        s_bl.Admin.RemoveClockObserver(clockObserver);
        s_bl.Admin.RemoveConfigObserver(configObserver);
    }

    // BL reference
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    // Dependency property for current time
    public DateTime CurrentTime
    {
        get { return (DateTime)GetValue(CurrentTimeProperty); }
        set { SetValue(CurrentTimeProperty, value); }
    }

    public static readonly DependencyProperty CurrentTimeProperty =
        DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow));

    // Dependency property for risk range
    public TimeSpan RiskRange
    {
        get { return (TimeSpan)GetValue(RiskRangeProperty); }
        set { SetValue(RiskRangeProperty, value); }
    }

    public static readonly DependencyProperty RiskRangeProperty =
        DependencyProperty.Register("RiskRange", typeof(TimeSpan), typeof(MainWindow));

    // Advance clock by one minute
    private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.ForwardClock(BO.TimeUnit.Minute);
    }

    // Advance clock by one hour
    private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.ForwardClock(BO.TimeUnit.Hour);
    }

    // Advance clock by one day
    private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.ForwardClock(BO.TimeUnit.Day);
    }

    // Advance clock by one month
    private void btnAddOneMonth_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.ForwardClock(BO.TimeUnit.Month);
    }

    // Advance clock by one year
    private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.ForwardClock(BO.TimeUnit.Year);
    }

    // Update risk range in BL
    private void UpdateButton_Click(object sender, RoutedEventArgs e)
    {
        s_bl.Admin.SetRiskRange(RiskRange);
    }

    // Observer for clock changes
    private void clockObserver()
    {
        // Update the CurrentTime property by fetching the clock value from the BL
        CurrentTime = s_bl.Admin.GetClock();
    }

    // Observer for configuration changes
    private void configObserver()
    {
        // Update RiskRange property by fetching it from the BL
        RiskRange = s_bl.Admin.GetRiskRange();
    }
    // Open volunteer list window
    private void OnHandlevolunteerButton_Click(object sender, RoutedEventArgs e)
    {
        new VolunteerListWindow().Show();
    }
    // Open call list window
    private void OnHandleCallsButton_Click(object sender, RoutedEventArgs e)
    {
        new CallListWindow().Show();
    }

    // Event handler for database initialization
    private void InitializeDatabase_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show("Are you sure you want to initialize the database?", "Confirm Initialization", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                // Close all other windows except this one
                foreach (Window window in Application.Current.Windows)
                {
                    if (window != this)
                        window.Close();
                }

                // Initialize the database
                s_bl.Admin.InitializeDB();
                MessageBox.Show("Database initialized successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while initializing the database: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }
    }

    // Event handler for database reset
    private void ResetDatabase_Click(object sender, RoutedEventArgs e)
    {
        var result = MessageBox.Show("Are you sure you want to reset the database?", "Confirm Reset", MessageBoxButton.YesNo, MessageBoxImage.Warning);

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;

                // Close all other windows except this one
                foreach (Window window in Application.Current.Windows)
                {
                    if (window != this)
                        window.Close();
                }

                // Reset the database
                s_bl.Admin.ResetDB();
                MessageBox.Show("Database reset successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while resetting the database: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }
    }
}
