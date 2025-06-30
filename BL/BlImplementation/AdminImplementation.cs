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
            lock (AdminManager.BlMutex) //stage 7
            {
                return AdminManager.Now;
            }
        }

        /// <summary>
        /// Advances the system clock by the specified time unit (e.g., minute, hour, day).
        /// </summary>
        /// <param name="unit">The time unit to advance the clock by (e.g., minute, hour, day).</param>
        /// <exception cref="NotImplementedException">Thrown if an invalid time unit is provided.</exception>
        public void ForwardClock(BO.TimeUnit unit)
        {
            AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
            lock (AdminManager.BlMutex) //stage 7
            {
                DateTime newTime = unit switch
                {
                    BO.TimeUnit.Minute => AdminManager.Now.AddMinutes(1),
                    BO.TimeUnit.Hour => AdminManager.Now.AddHours(1),
                    BO.TimeUnit.Day => AdminManager.Now.AddDays(1),
                    BO.TimeUnit.Month => AdminManager.Now.AddMonths(1),
                    BO.TimeUnit.Year => AdminManager.Now.AddYears(1),
                    _ => throw new NotImplementedException("Invalid time unit")
                };

                AdminManager.UpdateClock(newTime);
            }
        }

        /// <summary>
        /// Retrieves the current risk range setting.
        /// </summary>
        /// <returns>The current risk range as a TimeSpan.</returns>
        public TimeSpan GetRiskRange()
        {
            lock (AdminManager.BlMutex) //stage 7
            {
                return AdminManager.RiskRange;
            }
        }

        /// <summary>
        /// Updates the risk range setting.
        /// </summary>
        /// <param name="range">The new risk range to set.</param>
        /// <exception cref="BO.BlConfigException">Thrown if the update fails.</exception>
        public void SetRiskRange(TimeSpan RiskRange)
        {
            AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
            lock (AdminManager.BlMutex) //stage 7
            {
                AdminManager.RiskRange = RiskRange;
            }
        }

        /// <summary>
        /// Initializes the database with test data after resetting it.
        /// </summary>
        /// <exception cref="BO.BlConfigException">Thrown if the initialization fails.</exception>
        public void InitializeDB()
        {
            AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
            lock (AdminManager.BlMutex) //stage 7
            {
                AdminManager.InitializeDB();
                CallManager.Observers.NotifyListUpdated();
            }
        }

        /// <summary>
        /// Resets the database to its initial state.
        /// </summary>
        /// <exception cref="BO.BlConfigException">Thrown if the reset fails.</exception>
        public void ResetDB()
        {
            AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
            lock (AdminManager.BlMutex) //stage 7
            {
                AdminManager.ResetDB();
                CallManager.Observers.NotifyListUpdated();
            }
        }

        #region Stage 5
        public void AddClockObserver(Action clockObserver) =>
            AdminManager.ClockUpdatedObservers += clockObserver;
        public void RemoveClockObserver(Action clockObserver) =>
            AdminManager.ClockUpdatedObservers -= clockObserver;
        public void AddConfigObserver(Action configObserver) =>
            AdminManager.ConfigUpdatedObservers += configObserver;
        public void RemoveConfigObserver(Action configObserver) =>
            AdminManager.ConfigUpdatedObservers -= configObserver;
        #endregion Stage 5

        public void StartSimulator(int interval)  //stage 7
        {
            AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
            lock (AdminManager.BlMutex) //stage 7
            {
                AdminManager.Start(interval); //stage 7
            }
        }

        public void StopSimulator()
        {
            lock (AdminManager.BlMutex) //stage 7
            {
                AdminManager.Stop(); //stage 7
            }
        }
    }
}
