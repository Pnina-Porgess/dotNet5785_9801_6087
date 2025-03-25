namespace BlImplementation
{
    using BlApi;
    using DalApi;
    using Helpers;

    /// <summary>
    /// Implementation of the IAdmin interface providing administrative functionality.
    /// </summary>
    internal class AdminImplementation : IAdmin
    {
        private readonly IDal _dal = DalApi.Factory.Get;

        /// <summary>
        /// Gets the current system time.
        /// </summary>
        /// <returns>The current date and time.</returns>
        public DateTime GetClock()
        {
            return ClockManager.Now;
        }

        /// <summary>
        /// Advances the system clock by the specified time unit (e.g., minute, hour, day).
        /// </summary>
        /// <param name="unit">The time unit to advance the clock by (e.g., minute, hour, day).</param>
        /// <exception cref="NotImplementedException">Thrown if an invalid time unit is provided.</exception>
        public void ForwardClock(BO.TimeUnit unit)
        {
            DateTime newTime = unit switch
            {
                BO.TimeUnit.Minute => ClockManager.Now.AddMinutes(1),
                BO.TimeUnit.Hour => ClockManager.Now.AddHours(1),
                BO.TimeUnit.Day => ClockManager.Now.AddDays(1),
                BO.TimeUnit.Month => ClockManager.Now.AddMonths(1),
                BO.TimeUnit.Year => ClockManager.Now.AddYears(1),
                _ => throw new NotImplementedException("Invalid time unit")
            };

            ClockManager.UpdateClock(newTime);
        }

        /// <summary>
        /// Retrieves the current risk range setting.
        /// </summary>
        /// <returns>The current risk range as a TimeSpan.</returns>
        public TimeSpan GetRiskRange()
        {
            return _dal.Config.GetRiskRange();
        }

        /// <summary>
        /// Updates the risk range setting.
        /// </summary>
        /// <param name="range">The new risk range to set.</param>
        /// <exception cref="BO.BlConfigException">Thrown if the update fails.</exception>
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

        /// <summary>
        /// Resets the database to its initial state.
        /// </summary>
        /// <exception cref="BO.BlConfigException">Thrown if the reset fails.</exception>
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

        /// <summary>
        /// Initializes the database with test data after resetting it.
        /// </summary>
        /// <exception cref="BO.BlConfigException">Thrown if the initialization fails.</exception>
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
}
