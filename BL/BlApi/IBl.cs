namespace BlApi
{
    /// <summary>
    /// Provides access to the main system interfaces for Volunteers, Calls, and Admin operations.
    /// </summary>
    public interface IBl   //stage 5 הרחבת ממשק
    {
        /// <summary>
        /// Gets the volunteer operations interface.
        /// </summary>
        IVolunteer Volunteer { get; }

        /// <summary>
        /// Gets the call operations interface.
        /// </summary>
        ICall Call { get; }

        /// <summary>
        /// Gets the admin operations interface.
        /// </summary>
        IAdmin Admin { get; }
    }
}
