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
            throw new BO.BlNotFoundException("Volunteer object cannot be null.");
        //if (call.Id < 1000)
        //    throw new BO.BlInvalidInputException("Invalid ID format. ID must be a valid number with a correct checksum.");
        if ((call.Type != BO.TypeOfReading.None) && (call.Type != BO.TypeOfReading.EngineFailure) && (call.Type != BO.TypeOfReading.DeadBattery) && (call.Type != BO.TypeOfReading.FlatTire))
            throw new BO.BlInvalidInputException(@"Invalid CallType format. PCallType must be None\\Regular\\Emergency\\HighPriority.");
        if (call.Description?.Length < 2)
           throw new BO.BlInvalidInputException("Volunteer name is too short. Name must have at least 2 characters.");
    }
    internal static (double Latitude, double Longitude) logicalChecking(BO.Call call)
    {
        if (call.MaxEndTime < call.OpeningTime)
            throw new BO.BlInvalidInputException(".");

        return Tools.GetCoordinatesFromAddress(call.Address);


    }
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
            Console.WriteLine($"Error updating periodic calls: {e.Message}"); // Logging error
        }
    }
    internal static void SendEmailWhenCalOpened(BO.Call call)
    {
        var volunteer = _dal.Volunteer.ReadAll();
        foreach (var item in volunteer)
        {
     
         if (item.MaximumDistance >= Tools.CalculateDistance(item.Latitude!, item.Longitude!, call.Latitude, call.Longitude))
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