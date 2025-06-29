namespace BlApi
{
    /// <summary>
    /// Provides administrative operations for managing system configuration.
    /// </summary>
    public interface IAdmin
    {
        /// <summary>
        /// Initializes the database.
        /// </summary>
        void InitializeDB();

        /// <summary>
        /// Resets the database.
        /// </summary>
        void ResetDB();

        /// <summary>
        /// Gets the current risk range.
        /// </summary>
        TimeSpan GetRiskRange();

        /// <summary>
        /// Sets the risk range.
        /// </summary>
        void SetRiskRange(TimeSpan range);

        /// <summary>
        /// Gets the current system clock time.
        /// </summary>
        DateTime GetClock();

        /// <summary>
        /// Advances the system clock by the specified time unit.
        /// </summary>
        void ForwardClock(BO.TimeUnit unit);
        #region Stage 5
        void AddConfigObserver(Action configObserver);
        void RemoveConfigObserver(Action configObserver);
        void AddClockObserver(Action clockObserver);
        void RemoveClockObserver(Action clockObserver);
        #endregion Stage 5
        void StartSimulator(int interval); //stage 7
        void StopSimulator(); //stage 7

    }
}
