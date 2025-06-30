using DalApi;
using Helpers;

internal static class CallManager
{
    private static IDal _dal = Factory.Get; //stage 4
    internal static ObserverManager Observers = new();

    /// <summary>
    /// Validates the input format of the call object.
    /// </summary>
    /// <param name="call">The call object to validate.</param>
    /// <exception cref="BO.BlNotFoundException">Thrown if the call is null.</exception>
    /// <exception cref="BO.BlInvalidInputException">Thrown if the call type is invalid or other properties are incorrect.</exception>
    internal static void ValidateInputFormat(BO.Call call)
    {
        if (call == null)
            throw new BO.BlNotFoundException("Volunteer object cannot be null.");

        if ((call.Type != BO.TypeOfReading.None) &&
            (call.Type != BO.TypeOfReading.EngineFailure) &&
            (call.Type != BO.TypeOfReading.DeadBattery) &&
            (call.Type != BO.TypeOfReading.FlatTire))
            throw new BO.BlInvalidInputException(@"Invalid CallType format. PCallType must be None\\Regular\\Emergency\\HighPriority.");
    }

    /// <summary>
    /// Performs logical checks on the call’s timings and returns the coordinates from the address.
    /// </summary>
    /// <param name="call">The call object to check.</param>
    /// <returns>A tuple containing latitude and longitude of the call address.</returns>
    /// <exception cref="BO.BlInvalidInputException">Thrown if the max end time is earlier than the opening time.</exception>
    internal static (double Latitude, double Longitude) logicalChecking(BO.Call call)
    {
        if (call.MaxEndTime < call.OpeningTime)
            throw new BO.BlInvalidInputException("Max end time cannot be earlier than opening time.");

        return Tools.GetCoordinatesFromAddress(call.Address);
    }

    /// <summary>
    /// Creates a DO call object from the BO call object.
    /// </summary>
    /// <param name="newCall">The BO call object to convert.</param>
    /// <returns>A DO call object.</returns>
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
    /// Calculates the current status of a call based on its assignments and other parameters.
    /// </summary>
    /// <param name="callId">The ID of the call to check.</param>
    /// <returns>The calculated call status.</returns>
    /// <exception cref="ArgumentException">Thrown if the call does not exist or has no assignments.</exception>
    internal static BO.CallStatus CalculateCallStatus(int callId)

    {
        try
        {

            // Get the call from database
            DO.Call call;
            IEnumerable<DO.Assignment> assignments;
            lock (AdminManager.BlMutex)
            {
                 call = _dal.Call.Read(callId)!;
                if (call == null)
                    throw new ArgumentException($"Call with ID={callId} does not exist.");
                if (AdminManager.Now > call.MaxTimeToFinish)
                    return BO.CallStatus.Expired;
                assignments = _dal.Assignment.ReadAll(a => a.CallId == callId);
            }
            if (assignments == null)
            {
                TimeSpan timeToExpiration = (DateTime)call.MaxTimeToFinish! -AdminManager.Now;
                if (timeToExpiration <= AdminManager.RiskRange)
                    return BO.CallStatus.OpenAtRisk;

               return BO.CallStatus.Open;
            }

            var activeAssignment = assignments.FirstOrDefault(a => a.EndTime == null&&a.TypeOfEndTime==null);
            if (activeAssignment == null)
            {
                var successfulAssignment = assignments.Any(a => a.TypeOfEndTime == DO.TypeOfEndTime.treated);
                return successfulAssignment ? BO.CallStatus.Closed : BO.CallStatus.Open;
            }


            var remainingTime = (DateTime)call.MaxTimeToFinish - AdminManager.Now;
            if (remainingTime <= AdminManager.RiskRange)
                return BO.CallStatus.InProgressAtRisk;

            return BO.CallStatus.InProgress;
        }
        catch (Exception ex)
        {
            throw new BO.InvalidOperationException($"Error calculating call status: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Checks whether the call was ever assigned.
    /// </summary>
    /// <param name="callId">The ID of the call to check.</param>
    /// <returns>True if the call was never assigned, false otherwise.</returns>
    /// <exception cref="ArgumentException">Thrown if the call does not exist.</exception>
    internal static bool WasNeverAssigned(int callId)
    {
        try
        {
            IEnumerable<DO.Assignment> assignments;
            lock (AdminManager.BlMutex)
            {
                // First verify the call exists
                var call = _dal.Call.Read(callId);
                if (call == null)
                {
                    throw new ArgumentException($"Call with ID={callId} does not exist.");
                }

                // Check if there are any assignments for this call
                 assignments = _dal.Assignment.ReadAll(a => a.CallId == callId);
            }
                // The call was never assigned if there are no assignments at all
                return assignments.Any();
            
        }
        catch (Exception ex)
        {
            throw new BO.InvalidOperationException($"Error checking call assignment status: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Creates a CallInList object to display information about a call in the list.
    /// </summary>
    /// <param name="call">The DO call object.</param>
    /// <param name="assignments">The assignments related to the call.</param>
    /// <param name="volunteers">A dictionary of volunteer names by their ID.</param>
    /// <returns>A BO.CallInList object.</returns>
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

    /// <summary>
    /// Validates if the assignment can be marked as completed.
    /// </summary>
    /// <param name="assignment">The assignment to validate.</param>
    /// <param name="volunteerId">The ID of the volunteer attempting to complete the assignment.</param>
    /// <exception cref="ArgumentException">Thrown if the assignment does not exist.</exception>
    /// <exception cref="BO.BlInvalidInputException">Thrown if the assignment has already been completed or the volunteer doesn't have permission.</exception>
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
    /// Creates a list of closed calls with related assignments.
    /// </summary>
    /// <param name="calls">A collection of DO calls.</param>
    /// <param name="assignments">A collection of DO assignments.</param>
    /// <returns>A collection of BO.ClosedCallInList objects.</returns>
    public static IEnumerable<BO.ClosedCallInList> CreateClosedCallList(IEnumerable<DO.Call> calls, IEnumerable<DO.Assignment> assignments)
    {
        return calls.SelectMany(call =>
            assignments.Where(a => a.CallId == call.Id)
                       .Select(assignment => new BO.ClosedCallInList
                       {
                           Id = call.Id,
                           CallType = (BO.TypeOfReading)call.TypeOfReading,
                           OpenTime = call.TimeOfOpen,
                           FullAddress = call.Adress,
                           ActualEndTime = assignment.EndTime,
                           AssignmentEntryTime = assignment.EntryTime,
                           EndType = (BO.TypeOfEndTime)assignment.TypeOfEndTime
                       }));
    }

    /// <summary>
    /// Periodically updates the calls based on the current clock and checks for expired assignments.
    /// </summary>
    /// <param name="oldClock">The previous clock time.</param>
    /// <param name="newClock">The new clock time.</param>
    internal static void PeriodicCallsUpdates(DateTime oldClock, DateTime newClock)
    {
        //Thread.CurrentThread.Name = $"Periodic{++s_periodicCounter}"; //stage 7 (optional)
        List<DO.Call> expiredCalls;
        List<DO.Assignment> assignments;
        List<DO.Assignment> assignmentsWithNull;
        lock (AdminManager.BlMutex) //stage 7
            expiredCalls = _dal.Call.ReadAll(c => c.MaxTimeToFinish <newClock).ToList();
        expiredCalls.ForEach(call =>
        {
            lock (AdminManager.BlMutex)
            {//stage 7
                assignments = _dal.Assignment.ReadAll(a => a.CallId == call.Id).ToList();
                if (!assignments.Any())
                {
                    _dal.Assignment.Create(new DO.Assignment(
                    Id:0,
                    CallId: call.Id,
                    VolunteerId: 0,
                    TypeOfEndTime: (DO.TypeOfEndTime)BO.TypeOfEndTime.CancellationHasExpired,
                    EntryTime: AdminManager.Now,
                    EndTime: AdminManager.Now
                ));
                }
            }
            Observers.NotifyItemUpdated(call.Id);


            lock (AdminManager.BlMutex) //stage 7
                assignmentsWithNull =_dal.Assignment.ReadAll(a => a.CallId == call.Id && a.TypeOfEndTime is null).ToList();
            if (assignmentsWithNull.Any())
            {
                lock (AdminManager.BlMutex) //stage 7
                    foreach (var assignment in assignmentsWithNull)
                    {
                        _dal.Assignment.Update(assignment with
                        {
                            EndTime = AdminManager.Now,
                            TypeOfEndTime = (DO.TypeOfEndTime)BO.TypeOfEndTime.CancellationHasExpired
                        });
                    }

                Observers.NotifyItemUpdated(call.Id);
            }

        });

    }
    /// <summary>
    /// Sends an email notification to all volunteers within the specified distance from a new call.
    /// </summary>
    /// <param name="call">The call object that was opened.</param>
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

    /// <summary>
    /// Sends an email notification to the volunteer when their assignment is canceled.
    /// </summary>
    /// <param name="volunteer">The volunteer to notify.</param>
    /// <param name="assignment">The assignment that was canceled.</param>
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