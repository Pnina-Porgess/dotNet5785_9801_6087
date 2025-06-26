namespace BlApi
{
    /// <summary>
    /// Provides methods for managing volunteers in the system.
    /// </summary>
    public interface IVolunteer : IObservable //stage 5 הרחבת ממשק
    {
        /// <summary>
        /// Logs in a volunteer using their username and password.
        /// </summary>
        /// <param name="username">The volunteer's username.</param>
        /// <param name="password">The volunteer's password.</param>
        /// <returns>The role assigned to the volunteer upon successful login.</returns>
        BO.Role Login(int id, string password);

        /// <summary>
        /// Gets a list of volunteers, optionally filtered by their activity status and sorted by a specific field.
        /// </summary>
        /// <param name="isActive">Optional filter for volunteer activity status.</param>
        /// <param name="sortBy">Optional sorting parameter for the volunteer list.</param>
        /// <returns>An enumerable list of volunteers.</returns>
        IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive = null, BO.VolunteerSortBy? sortBy = null, BO.TypeOfReading? filterField = null);

        /// <summary>
        /// Gets detailed information about a specific volunteer.
        /// </summary>
        /// <param name="volunteerId">The ID of the volunteer.</param>
        /// <returns>Detailed information about the volunteer.</returns>
        BO.Volunteer GetVolunteerDetails(int volunteerId);

        /// <summary>
        /// Updates the details of a specific volunteer.
        /// </summary>
        /// <param name="id">The ID of the volunteer to update.</param>
        /// <param name="volunteer">The volunteer object containing the updated information.</param>
        void UpdateVolunteerDetails(int id, BO.Volunteer volunteer);

        /// <summary>
        /// Deletes a specific volunteer from the system.
        /// </summary>
        /// <param name="id">The ID of the volunteer to delete.</param>
        void DeleteVolunteer(int id);

        /// <summary>
        /// Adds a new volunteer to the system.
        /// </summary>
        /// <param name="volunteer">The volunteer object to add.</param>
        void AddVolunteer(BO.Volunteer volunteer);
    }
}
