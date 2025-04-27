using BO;

namespace BlApi
{
    /// <summary>
    /// Provides methods for managing calls in the system.
    /// </summary>
    public interface ICall : IObservable //stage 5 הרחבת ממשק
    {
        /// <summary>
        /// Gets the count of calls grouped by their status.
        /// </summary>
        /// <returns>An array where each element represents the count of calls for a specific status.</returns>
        int[] GetCallCountsByStatus();

        /// <summary>
        /// Gets a list of calls based on a specific filter and sorting criteria.
        /// </summary>
        /// <param name="filterField">The field to filter the calls by (e.g., call status, date).</param>
        /// <param name="filterValue">The value to filter the calls by (e.g., a specific status or date).</param>
        /// <param name="sortField">The field to sort the calls by (e.g., call date, priority).</param>
        /// <returns>An enumerable list of calls that match the filter and sort criteria.</returns>
        IEnumerable<CallInList> GetCalls(CallField? filterField, object? filterValue, CallField? sortField);

        /// <summary>
        /// Gets the details of a specific call by its ID.
        /// </summary>
        /// <param name="callId">The ID of the call to retrieve details for.</param>
        /// <returns>The detailed information of the specified call.</returns>
        Call GetCallDetails(int callId);

        /// <summary>
        /// Updates the details of an existing call.
        /// </summary>
        /// <param name="call">The call object containing the updated information.</param>
        void UpdateCall(Call call);

        /// <summary>
        /// Deletes a specific call by its ID.
        /// </summary>
        /// <param name="callId">The ID of the call to delete.</param>
        void DeleteCall(int callId);

        /// <summary>
        /// Adds a new call to the system.
        /// </summary>
        /// <param name="call">The call object to add.</param>
        void AddCall(Call call);

        /// <summary>
        /// Gets a list of closed calls handled by a specific volunteer.
        /// </summary>
        /// <param name="volunteerId">The ID of the volunteer whose closed calls are to be retrieved.</param>
        /// <param name="filterStatus">Optional filter for the status of the calls (e.g., completed, canceled).</param>
        /// <param name="sortField">Optional sorting field for the closed calls list.</param>
        /// <returns>An enumerable list of closed calls handled by the volunteer.</returns>
        IEnumerable<ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, TypeOfReading? filterStatus, ClosedCallField? sortField);

        /// <summary>
        /// Gets a list of open calls available for a volunteer to handle.
        /// </summary>
        /// <param name="volunteerId">The ID of the volunteer.</param>
        /// <param name="filterStatus">Optional filter for the status of the open calls.</param>
        /// <param name="sortField">Optional sorting field for the open calls list.</param>
        /// <returns>An enumerable list of open calls available for the volunteer to choose from.</returns>
        IEnumerable<OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, CallStatus? filterStatus, OpenCallField? sortField);

        /// <summary>
        /// Marks the treatment for a call as completed.
        /// </summary>
        /// <param name="volunteerId">The ID of the volunteer completing the treatment.</param>
        /// <param name="assignmentId">The ID of the assignment related to the call.</param>
        void CompleteCallTreatment(int volunteerId, int assignmentId);

        /// <summary>
        /// Cancels the treatment for a call.
        /// </summary>
        /// <param name="requesterId">The ID of the person requesting the cancellation.</param>
        /// <param name="assignmentId">The ID of the assignment related to the call.</param>
        void CancelCallTreatment(int requesterId, int assignmentId);

        /// <summary>
        /// Assigns a volunteer to handle a specific call.
        /// </summary>
        /// <param name="volunteerId">The ID of the volunteer to assign to the call.</param>
        /// <param name="callId">The ID of the call to assign to the volunteer.</param>
        void SelectCallForTreatment(int volunteerId, int callId);
    }
}
