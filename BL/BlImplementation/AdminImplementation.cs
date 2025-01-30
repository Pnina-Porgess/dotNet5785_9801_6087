namespace BlImplementation;
using BlApi;
using DalApi;
using Helpers;
using TimeUnit = BO.TimeUnit;
internal class AdminImplementation : IAdmin
{
    private readonly IDal _dal = DalApi.Factory.Get;

    public DateTime GetClock()
    {
        return ClockManager.Now;
    }

    public void ForwardClock(BO.TimeUnit unit)
    {
        DateTime newTime = unit switch
        {
            TimeUnit.Minute => ClockManager.Now.AddMinutes(1),
            TimeUnit.Hour => ClockManager.Now.AddHours(1),
            TimeUnit.Day => ClockManager.Now.AddDays(1),
            TimeUnit.Month => ClockManager.Now.AddMonths(1),
            TimeUnit.Year => ClockManager.Now.AddYears(1),
            _ => throw new NotImplementedException("Invalid time unit")
        };

        ClockManager.UpdateClock(newTime);
    }

    public TimeSpan GetRiskRange()
    {
        return _dal.Config.GetRiskRange();
    }

    public void SetRiskRange(TimeSpan range)
    {
        try
        {
            _dal.Config.SetRiskRange(range);
        }
        catch (DO.DalConfigException ex)
        {
            throw new BO.BlConfigException("Failed to update risk range", ex);
        }
    }

    public void ResetDB()
    {
        try
        {
      
            _dal.ResetDB();
            ClockManager.UpdateClock(ClockManager.Now);
        }
        catch (DO.DalConfigException ex)
        {
            throw new BO.BlConfigException("Failed to reset database", ex);
        }
    }

    public void InitializeDB()
    {
        try
        {
            ResetDB(); // First reset the database
            DalTest.Initialization.Do(); // Initialize with test data
            ClockManager.UpdateClock(ClockManager.Now);
        }
        catch (DO.DalConfigException ex)
        {
            throw new BO.BlConfigException("Failed to initialize database", ex);
        }
    }

}