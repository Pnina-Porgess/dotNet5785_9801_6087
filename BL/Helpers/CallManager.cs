using BO;
using DalApi;
using DO;
using Helpers;

internal static class CallManager
    
{
    private static IDal _dal = Factory.Get; //stage 4
    internal static void ValidateInputFormat(BO.Call call)
    {
        if (call == null)
            throw new BO.NotFoundException("Volunteer object cannot be null.");
        if (call.Id < 1000)
            throw new BO.InvalidFormatException("Invalid ID format. ID must be a valid number with a correct checksum.");
        if ((call.Type!=BO.CallType.None)&& (call.Type != BO.CallType.Regular)&& (call.Type != BO.CallType.Emergency)&& (call.Type != BO.CallType.HighPriority))
            throw new BO.InvalidFormatException(@"Invalid CallType format. PCallType must be None\\Regular\\Emergency\\HighPriority.");
        if (call.Description.Length < 2)
            throw new BO.InvalidFormatException("Volunteer name is too short. Name must have at least 2 characters.");
    }
    internal static (double Latitude, double Longitude) logicalChecking(BO.Call call)
    {
        if(call.MaxEndTime<call.OpeningTime)
            throw new BO.InvalidFormatException(".");

        return Tools.GetCoordinatesFromAddress(call.Address);


    }
    internal static DO.Call CreateDoCall(BO.Call newCall)
    {
        return new DO.Call(
                    Id: 0,
                    TypeOfReading: (TypeOfReading)newCall.Type,
                    Description: newCall.Description,
                    Adress: newCall.Address,
                    Latitude: newCall.Latitude,
                    Longitude: newCall.Longitude,
                    TimeOfOpen: newCall.OpeningTime,
                    MaxTimeToFinish: newCall?.MaxEndTime ?? DateTime.Now.AddHours(1)
        );
    }
    internal static CallStatus CalculateCallStatus(int callId)
    {
        try
        {
            //// Get the call from database
            var call = _dal.Call.Read(callId);
            if (call == null)
                throw new ArgumentException($"Call with ID={callId} does not exist.");

            // Get all assignments for this call
            var assignments = _dal.Assignment.ReadAll(a => a.CallId == callId);

            // If there are no assignments at all
            if (!assignments.Any())
            {
                // Check if call has expired
                if (ClockManager.Now > call.MaxTimeToFinish)
                    return CallStatus.Expired;

                // Check if call is at risk (less than 30 minutes to expiration)
                var timeToExpiration = call.MaxTimeToFinish - ClockManager.Now;
                if (timeToExpiration?.TotalMinutes <= 30)
                    return CallStatus.OpenAtRisk;

                return CallStatus.Open;
            }

            // Get the latest active assignment (no EndTime)
            var activeAssignment = assignments.FirstOrDefault(a => a.EndTime == null);
            // If there's no active assignment but there are completed assignments
            if (activeAssignment == null)
            {
                // Check if any assignment was completed successfully
                var successfulAssignment = assignments.Any(a => a.TypeOfEndTime == TypeOfEndTime.treated);
                return successfulAssignment ? CallStatus.Closed : CallStatus.Open;
            }
            // There is an active assignment - check if it's at risk
            var remainingTime = call.MaxTimeToFinish - ClockManager.Now;
            if (remainingTime?.TotalMinutes <= 30)
                return CallStatus.InProgressAtRisk;

            return CallStatus.InProgress;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error calculating call status: {ex.Message}", ex);
        }
    }
    internal static bool WasNeverAssigned(int callId)
    {
        try
        {
            // First verify the call exists
            var call = _dal.Call.Read(callId);
            if (call == null)
            {
                throw new ArgumentException($"Call with ID={callId} does not exist.");
            }

            // Check if there are any assignments for this call
            var assignments = _dal.Assignment.ReadAll(a => a.CallId == callId);

            // The call was never assigned if there are no assignments at all
            return !assignments.Any();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error checking call assignment status: {ex.Message}", ex);
        }
    }
    internal static BO.CallInList CreateCallInList(DO.Call call, IEnumerable<DO.Assignment> assignments, Dictionary<int, string> volunteers)
    {
        var callAssignments = assignments.Where(a => a.CallId == call.Id).OrderByDescending(a => a.EntryTime).ToList();
        var latestAssignment = callAssignments.FirstOrDefault();

        return new BO.CallInList
        {
            CallId = call.Id,
            CallType = (BO.CallType)call.TypeOfReading,
            OpeningTime = call.TimeOfOpen,
            CallStatus = CalculateCallStatus(call.Id),
            AssignmentId = latestAssignment?.Id,
            LastVolunteerName = latestAssignment?.VolunteerId != 0 && volunteers.TryGetValue(latestAssignment.VolunteerId, out var name) ? name : null,
            TotalAssignments = callAssignments.Count,
            RemainingTime = call.MaxTimeToFinish.HasValue
            ? call.MaxTimeToFinish.Value - DateTime.Now
            : null,
            CompletionTime = latestAssignment?.EndTime.HasValue == true ? latestAssignment.EndTime - latestAssignment.EntryTime : null
        };
    }

    internal static IEnumerable<BO.CallInList> FilterCall(IEnumerable<BO.CallInList> calls, CallField filterField, object filterValue)
    {

        return filterField switch
        {
            CallField.Id => calls.Where(call => call.CallId.ToString() == filterValue.ToString()),
            CallField.Type => calls.Where(call => call.CallType == (BO.CallType)filterValue),
            CallField.Status => calls.Where(call => call.CallStatus == (BO.CallStatus)filterValue),
            CallField.OpeningTime => calls.Where(call => call.OpeningTime.Date == ((DateTime)filterValue).Date),
            CallField.AssignmentId => calls.Where(call => call.AssignmentId.ToString() == filterValue.ToString()),
            _ => throw new BO.InvalidOperationException($"Filtering by {filterField} is not supported")
        };
    }


    internal static IEnumerable<T> SortCallsGeneric<T>(IEnumerable<T> calls, CallField? sortField) where T : class
    {
        if (!sortField.HasValue)
            return calls.OrderBy(c => GetPropertyValue(c, "Id") ?? GetPropertyValue(c, "CallId"));

        return sortField switch
        {
            CallField.Id => calls.OrderBy(c => GetPropertyValue(c, "Id") ?? GetPropertyValue(c, "CallId")),
            CallField.Type => calls.OrderBy(c => GetPropertyValue(c, "CallType") ?? GetPropertyValue(c, "Type")),
            CallField.Status => calls.OrderBy(c => GetPropertyValue(c, "CallStatus") ?? GetPropertyValue(c, "Status")),
            CallField.OpeningTime => calls.OrderBy(c => GetPropertyValue(c, "OpeningTime") ?? GetPropertyValue(c, "OpenTime")),
            CallField.AssignmentId => calls.OrderBy(c => GetPropertyValue(c, "AssignmentId")),
            _ => throw new BO.InvalidOperationException($"Sorting by {sortField} is not supported")
        };
    }

    private static object GetPropertyValue(object obj, string propertyName)
    {
        return obj?.GetType().GetProperty(propertyName)?.GetValue(obj);
    }

    /// <summary>
    /// Validates whether the assignment can be completed.
    /// </summary>
    internal static void ValidateAssignmentForCompletion(DO.Assignment assignment, int volunteerId)
    {
        if (assignment == null)
        {
            throw new ArgumentException($"Assignment with ID={assignmentId} does not exist.");
        }
        if (assignment.VolunteerId != volunteerId)
        {
            throw new BO.UnauthorizedActionException("The volunteer does not have permission to complete this treatment.");
        }

        if (assignment.EndTime != null)
        {
            throw new BO.InvalidOperationException("This treatment has already been completed or cancelled.");
        }
    }
    public static IEnumerable<BO.ClosedCallInList> CreateClosedCallList(IEnumerable<DO.Call> calls, IEnumerable<DO.Assignment> assignments)
    {
        return calls.Select(call =>
        {
            var assignment = assignments.First(a => a.CallId == call.Id);
            return new BO.ClosedCallInList
            {
                Id = call.Id,
                CallType = (BO.TypeOfReading)call.TypeOfReading,
                OpenTime = call.TimeOfOpen,
                FullAddress = call.Adress,
                ActualEndTime = assignment.EndTime,
                AssignmentEntryTime = assignment.EntryTime,
                EndType = (BO.TypeOfEndTime)assignment.TypeOfEndTime
            };
        });
    

    internal static IEnumerable<T> SortCalls<T>(IEnumerable<T> calls, BO.CallField sortField) 
    {
        return sortField switch
        {
            BO.CallField.Status => calls.OrderBy(c => ((dynamic)c).Status),
            BO.CallField.Latitude => calls.OrderBy(c => ((dynamic)c).Latitude),
            BO.CallField.Status => calls.OrderBy(c => ((dynamic)c).Status),
            BO.CallField.Distance => calls.OrderBy(c => ((dynamic)c).Distance),
            _ => calls.OrderBy(c => ((dynamic)c).CallNumber)
        };
    }
}
