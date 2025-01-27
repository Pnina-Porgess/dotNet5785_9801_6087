using BO;

namespace BlApi;
public interface ICall
{
    // Method to request the count of calls by their status
    int[] GetCallCountsByStatus();

    // Method to request a list of calls
    IEnumerable<CallInList> GetCalls(CallField? filterField, object? filterValue, CallField? sortField);

    // Method to request the details of a specific call
    Call GetCallDetails(int callId);

    // Method to update call details
    void UpdateCall(Call call);

    // Method to delete a call
    void DeleteCall(int callId);

    // Method to add a new call
    void AddCall(Call call);

    // Method to request a list of closed calls by volunteer
    IEnumerable<ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, CallStatus? filterStatus, CallField? sortField);

    // Method to request a list of open calls available for a volunteer to choose
    IEnumerable<OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, CallStatus? filterStatus, CallField? sortField);

    // Method to update "treatment completion" for a call
    void CompleteCallTreatment(int volunteerId, int assignmentId);

    // Method to update "treatment cancellation" for a call
    void CancelCallTreatment(int requesterId, int assignmentId);

    // Method to assign a volunteer to handle a specific call
    void SelectCallForTreatment(int volunteerId, int callId);
}
