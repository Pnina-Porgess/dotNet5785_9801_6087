
using BO;
using DalApi;
using DO;
using Helpers;

internal static class CallManager

{
    private static IDal _dal = Factory.Get; //stage 4
    /// <summary>
    /// Validates the format of a given call object.
    /// </summary>
    internal static void ValidateInputFormat(BO.Call call)
    {
        if (call == null)
            throw new BO.BlNotFoundException("Volunteer object cannot be null.");
        //if (call.Id < 1000)
        //    throw new BO.BlInvalidInputException("Invalid ID format. ID must be a valid number with a correct checksum.");
        if ((call.Type != BO.TypeOfReading.None) && (call.Type != BO.TypeOfReading.EngineFailure) && (call.Type != BO.TypeOfReading.DeadBattery) && (call.Type != BO.TypeOfReading.FlatTire))
            throw new BO.BlInvalidInputException(@"Invalid CallType format. PCallType must be None\\Regular\\Emergency\\HighPriority.");
        //if (call.Description?.Length < 2)
        //    throw new BO.BlInvalidInputException("Volunteer name is too short. Name must have at least 2 characters.");
    }
    /// <summary>
    /// Performs logical checks on a call's time and address.
    /// </summary>
    internal static (double Latitude, double Longitude) logicalChecking(BO.Call call)
    {
        if (call.MaxEndTime < call.OpeningTime)
            throw new BO.BlInvalidInputException(".");

        return Tools.GetCoordinatesFromAddress(call.Address);


    }
    /// <summary>
    /// Converts a business object (BO) call to a data object (DO) call.
    /// </summary>
    internal static DO.Call CreateDoCall(BO.Call newCall)
    {
        return new DO.Call(
                    Id: newCall.Id,
                    TypeOfReading: (DO.TypeOfReading)newCall.Type,
                    Description: newCall.Description,
                    Adress: newCall.Address,
                    Latitude: newCall.Latitude,
                    Longitude: newCall.Longitude,
                    TimeOfOpen: newCall.OpeningTime,
                    MaxTimeToFinish: newCall?.MaxEndTime ?? DateTime.Now.AddHours(1)
        );
    }
    /// <summary>
    /// Calculates the status of a call based on its assignments and time constraints.
    /// </summary>
    internal static BO.CallStatus CalculateCallStatus(int callId)
    {
        try
        {
            //// Get the call from database
            var call = _dal.Call.Read(callId);
            if (call == null)
                throw new ArgumentException($"Call with ID={callId} does not exist.");

            // Get all assignments for this call
            var assignments = _dal.Assignment.ReadAll(a => a.CallId == callId);
            if (assignments == null)
                throw new ArgumentException($"Call with ID={callId} does not has assignment.");
            // If there are no assignments at all
            if (!assignments.Any())
            {
                // Check if call has expired
                if (ClockManager.Now > call.MaxTimeToFinish)
                    return BO.CallStatus.Expired;

                // Check if call is at risk (less than 30 minutes to expiration)
                var timeToExpiration = call.MaxTimeToFinish - ClockManager.Now;
                if (timeToExpiration?.TotalMinutes <= 30)
                    return BO.CallStatus.OpenAtRisk;

                return BO.CallStatus.Open;
            }

            // Get the latest active assignment (no EndTime)
            var activeAssignment = assignments.FirstOrDefault(a => a.EndTime == null);
            // If there's no active assignment but there are completed assignments
            if (activeAssignment == null)
            {
                // Check if any assignment was completed successfully
                var successfulAssignment = assignments.Any(a => a.TypeOfEndTime == DO.TypeOfEndTime.treated);
                return successfulAssignment ? BO.CallStatus.Closed : BO.CallStatus.Open;
            }
            // There is an active assignment - check if it's at risk
            var remainingTime = call.MaxTimeToFinish - ClockManager.Now;
            if (remainingTime?.TotalMinutes <= 30)
                return BO.CallStatus.InProgressAtRisk;

            return BO.CallStatus.InProgress;
        }
        catch (Exception ex)
        {
            throw new BO.InvalidOperationException($"Error calculating call status: {ex.Message}", ex);
        }
    }
    /// <summary>
    /// Determines whether a call was never assigned.
    /// </summary>
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
            throw new BO.InvalidOperationException($"Error checking call assignment status: {ex.Message}", ex);
        }
    }
    /// <summary>
    /// Creates a summary of a call including assignment and volunteer details.
    /// </summary>
    internal static BO.CallInList CreateCallInList(DO.Call call, IEnumerable<DO.Assignment> assignments, Dictionary<int, string> volunteers)
    {
        var callAssignments = assignments.Where(a => a.CallId == call.Id).OrderByDescending(a => a.EntryTime).ToList();
        var latestAssignment = callAssignments.FirstOrDefault();

        return new BO.CallInList
        {
            CallId = call.Id,
            CallType = (BO.TypeOfReading)call.TypeOfReading,
            OpeningTime = call.TimeOfOpen,
            CallStatus = CalculateCallStatus(call.Id),
            AssignmentId = latestAssignment?.Id,
            LastVolunteerName = latestAssignment?.VolunteerId != null && volunteers.TryGetValue(latestAssignment!.VolunteerId, out var name) ? name : null,
            TotalAssignments = callAssignments.Count,
            RemainingTime = call.MaxTimeToFinish.HasValue
            ? call.MaxTimeToFinish.Value - DateTime.Now
            : null,
            CompletionTime = latestAssignment?.EndTime.HasValue == true ? latestAssignment.EndTime - latestAssignment.EntryTime : null
        };
    }

    //internal static IEnumerable<BO.CallInList> FilterCall(IEnumerable<BO.CallInList> calls, BO.CallField filterField, object filterValue)
    //{
    //    return filterField switch
    //    {
    //        BO.CallField.CallId => calls.Where(call => call.CallId.ToString() == filterValue.ToString()),
    //        BO.CallField.Type => calls.Where(call => call.CallType == (BO.CallType)filterValue),
    //        BO.CallField.Status => calls.Where(call => call.CallStatus == (BO.CallStatus)filterValue),
    //        BO.CallField.OpeningTime => calls.Where(call => call.OpeningTime.Date == ((DateTime)filterValue).Date),
    //        BO.CallField.AssignmentId => calls.Where(call => call.AssignmentId.ToString() == filterValue.ToString()),
    //        _ => throw new BO.BlInvalidInputException($"Filtering by {filterField} is not supported")
    //    };
    //}

    internal static IEnumerable<BO.CallInList> FilterCall(
    IEnumerable<BO.CallInList> calls,
    BO.CallField filterField,
    object filterValue)
    {
        return filterField switch
        {
            CallField.AssignmentId => calls.Where(call => call.AssignmentId.ToString() == filterValue.ToString()),
            CallField.CallId => calls.Where(call => call.CallId.ToString() == filterValue.ToString()),
            CallField.CallType => calls.Where(call => call.CallType == (BO.TypeOfReading)filterValue),
            CallField.OpeningTime => calls.Where(call => call.OpeningTime.Date == ((DateTime)filterValue).Date),
            CallField.RemainingTime => calls.Where(call => call.RemainingTime == (TimeSpan?)filterValue),
            CallField.LastVolunteerName => calls.Where(call => call.LastVolunteerName == filterValue.ToString()),
            CallField.CompletionTime => calls.Where(call => call.CompletionTime == (TimeSpan?)filterValue),
            CallField.CallStatus => calls.Where(call => call.CallStatus == (BO.CallStatus)filterValue),
            CallField.TotalAssignments => calls.Where(call => call.TotalAssignments == (int)filterValue),
            _ => throw new BO.BlInvalidInputException($"Filtering by {filterField} is not supported")
        };
    }


    //internal static IEnumerable<T> SortCalls<T>(IEnumerable<T> calls, BO.CallField? sortField) where T : class
    //{
    //    if (!sortField.HasValue)
    //        return calls.OrderBy(c => GetPropertyValue(c, "Id") ?? GetPropertyValue(c, "CallId"));

    //    return sortField switch
    //    {
    //        BO.CallField.CallId => calls.OrderBy(c => GetPropertyValue(c, "Id") ?? GetPropertyValue(c, "CallId")),
    //        BO.CallField.Type => calls.OrderBy(c => GetPropertyValue(c, "CallType") ?? GetPropertyValue(c, "Type")),
    //        BO.CallField.Status => calls.OrderBy(c => GetPropertyValue(c, "CallStatus") ?? GetPropertyValue(c, "Status")),
    //        BO.CallField.OpeningTime => calls.OrderBy(c => GetPropertyValue(c, "OpeningTime") ?? GetPropertyValue(c, "OpenTime")),
    //        BO.CallField.AssignmentId => calls.OrderBy(c => GetPropertyValue(c, "AssignmentId")),
    //        BO.CallField.Description => calls.OrderBy(c => GetPropertyValue(c, "Description")),
    //        BO.CallField.Address => calls.OrderBy(c => GetPropertyValue(c, "Address")),
    //        BO.CallField.MaxEndTime => calls.OrderBy(c => GetPropertyValue(c, "MaxEndTime")),
    //        _ => throw new BO.BlInvalidInputException($"Sorting by {sortField} is not supported")
    //    };
    //}

    //internal static IEnumerable<T> SortCallsGeneric<T>(IEnumerable<T> calls, BO.CallField? sortField) where T : class
    //{
    //    if (!sortField.HasValue)
    //        return calls.OrderBy(c => GetPropertyValue(c, "Id") ?? GetPropertyValue(c, "CallId"));

    //    return sortField switch
    //    {
    //        BO.CallField.CallId => calls.OrderBy(c => GetPropertyValue(c, "Id") ?? GetPropertyValue(c, "CallId")),
    //        BO.CallField.Type => calls.OrderBy(c => GetPropertyValue(c, "CallType") ?? GetPropertyValue(c, "Type")),
    //        BO.CallField.Status => calls.OrderBy(c => GetPropertyValue(c, "CallStatus") ?? GetPropertyValue(c, "Status")),
    //        BO.CallField.OpeningTime => calls.OrderBy(c => GetPropertyValue(c, "OpeningTime") ?? GetPropertyValue(c, "OpenTime")),
    //        BO.CallField.AssignmentId => calls.OrderBy(c => GetPropertyValue(c, "AssignmentId")),
    //        _ => throw new BO.BlInvalidInputException($"Sorting by {sortField} is not supported")
    //    };
    //}

    private static object GetPropertyValue(object obj, string propertyName)
    {
        return obj?.GetType().GetProperty(propertyName)?.GetValue(obj)!;
    }

    /// <summary>
    /// Validates whether the assignment can be completed.
    /// </summary>
    internal static void ValidateAssignmentForCompletion(DO.Assignment assignment, int volunteerId)
    {
        if (assignment == null)
        {
            throw new ArgumentException($"Assignment with ID={assignment?.Id} does not exist.");
        }
        if (assignment.VolunteerId != volunteerId)
        {
            throw new Exception("The volunteer does not have permission to complete this treatment.");
        }

        if (assignment.EndTime != null)
        {
            throw new BO.BlInvalidInputException("This treatment has already been completed or cancelled.");
        }
    }
    /// <summary>
    /// Creates a list of closed calls based on the call and assignment data.
           else if (item.MaximumDistance >= Tools.CalculateDistance(item.Latitude!, item.Longitude!, call.Latitude, call.Longitude))
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
    }
    /// <summary>
    /// Performs periodic updates for open calls based on time progression.
    /// </summary>
    public static void PeriodicCallsUpdates(DateTime oldClock, DateTime newClock)
    {
        try
        {
            _dal.Call.ReadAll(c => c.MaxTimeToFinish > ClockManager.Now).ToList().ForEach(call =>
            {
                List<DO.Assignment> allAssignmentsCall = _dal.Assignment.ReadAll(a => a.CallId == call.Id && a.EndTime == null).ToList();

                if (!allAssignmentsCall.Any())
                {
                    DO.Assignment newAssignment = new DO.Assignment(0, call.Id, 0, DO.TypeOfEndTime.CancellationHasExpired, ClockManager.Now);
                    _dal.Assignment.Create(newAssignment);
                }
                else
                {
                    DO.Assignment updatedAssignment = allAssignmentsCall.FirstOrDefault(a => a.EndTime == null);
                    _dal.Assignment.Update(updatedAssignment with { EndTime = ClockManager.Now, TypeOfEndTime = DO.TypeOfEndTime.CancellationHasExpired });
                }
            });
        }

        catch (BO.BlInvalidInputException e)
        {
        }
    }
    /// <summary>
    /// Sends an email notification to nearby volunteers when a call is opened.
    /// </summary>
    internal static void SendEmailWhenCalOpened(BO.Call call)
    {
        var volunteer = _dal.Volunteer.ReadAll();

    //internal static IEnumerable<BO.ClosedCallInList> SortClosedCalls(
    //IEnumerable<BO.ClosedCallInList> calls,
    //BO.ClosedCallField sortField)
    //{
    //    return sortField switch
    //    {
    //        ClosedCallField.Id => calls.OrderBy(c => c.Id),
    //        ClosedCallField.CallType => calls.OrderBy(c => c.CallType),
    //        ClosedCallField.FullAddress => calls.OrderBy(c => c.FullAddress),
    //        ClosedCallField.OpenTime => calls.OrderBy(c => c.OpenTime),
    //        ClosedCallField.AssignmentEntryTime => calls.OrderBy(c => c.AssignmentEntryTime),
    //        ClosedCallField.ActualEndTime => calls.OrderBy(c => c.ActualEndTime),
    //        ClosedCallField.EndType => calls.OrderBy(c => c.EndType),
    //        _ => throw new BO.BlInvalidInputException($"Sorting by {sortField} is not supported")
    //    };
    //}

    //internal static IEnumerable<BO.OpenCallInList> SortOpenCalls(
    //IEnumerable<BO.OpenCallInList> calls,
    //BO.OpenCallField sortField)
    //{
    //    return sortField switch
    //    {
    //        OpenCallField.Id => calls.OrderBy(c => c.Id),
    //        OpenCallField.Type => calls.OrderBy(c => c.Type),
    //        OpenCallField.Description => calls.OrderBy(c => c.Description),
    //        OpenCallField.FullAddress => calls.OrderBy(c => c.FullAddress),
    //        OpenCallField.OpenTime => calls.OrderBy(c => c.OpenTime),
    //        OpenCallField.MaxEndTime => calls.OrderBy(c => c.MaxEndTime),
    //        OpenCallField.DistanceFromVolunteer => calls.OrderBy(c => c.DistanceFromVolunteer),
    //        _ => throw new BO.BlInvalidInputException($"Sorting by {sortField} is not supported")
    //    };
    //}
    internal static IEnumerable<BO.CallInList> SortCalls(
    IEnumerable<BO.CallInList> calls,
    BO.CallField sortField)
    {
        return sortField switch
        {
            CallField.AssignmentId => calls.OrderBy(c => c.AssignmentId),
            CallField.CallId => calls.OrderBy(c => c.CallId),
            CallField.CallType => calls.OrderBy(c => c.CallType),
            CallField.OpeningTime => calls.OrderBy(c => c.OpeningTime),
            CallField.RemainingTime => calls.OrderBy(c => c.RemainingTime),
            CallField.LastVolunteerName => calls.OrderBy(c => c.LastVolunteerName),
            CallField.CompletionTime => calls.OrderBy(c => c.CompletionTime),
            CallField.CallStatus => calls.OrderBy(c => c.CallStatus),
            CallField.TotalAssignments => calls.OrderBy(c => c.TotalAssignments),
            _ => throw new BO.BlInvalidInputException($"Sorting by {sortField} is not supported")
        };
    }


        foreach (var item in volunteer)
        {
            if (item.MaximumDistance == null)
            { break; }
            else if (item.MaximumDistance >= Tools.CalculateDistance(item.Latitude!, item.Longitude!, call.Latitude, call.Longitude))
            {
                string subject = "Openning call";
                string body = $@"
        Hello {item.Name},

       A new call has been opened in your area.
        Call Details:
        - Call ID: {call.Id}
        - Call Type: {call.Type}
        - Call Address: {call.Address}
        - Opening Time: {call.OpeningTime}
        - Description: {call.Description}
        - Entry Time for Treatment: {call.MaxEndTime}
        -call Status:{call.Status}

        If you wish to handle this call, please log into the system.

        Best regards,  
       Call Management System";

                Tools.SendEmail(item.Email, subject, body);
            }

        }
    }
    /// <summary>
    /// Sends an email to a volunteer when their assignment is canceled.
    /// </summary>
    internal static void SendEmailToVolunteer(DO.Volunteer volunteer, DO.Assignment assignment)
    {
        var call = _dal.Call.Read(assignment.CallId)!;

        string subject = "Assignment Canceled";
        string body = $@"
        Hello {volunteer.Name},

        Your assignment for handling call {assignment.Id} has been canceled by the administrator.

        Call Details:
        - Call ID: {assignment.CallId}
        - Call Type: {call.TypeOfReading}
        - Call Address: {call.Adress}
        - Opening Time: {call.TimeOfOpen}
        - Description: {call.Description}
        - Entry Time for Treatment: {assignment.EntryTime}

        Best regards,  
        Call Management System";

        Tools.SendEmail(volunteer.Email, subject, body);
    }
}
