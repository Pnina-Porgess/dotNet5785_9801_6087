using BlApi;
using BO;
using DalApi;
using DO;
using Helpers;
using System.Collections.Generic;

namespace BlImplementation
{
    internal class CallImplementation : BlApi.ICall
    {
        private readonly DalApi.IDal _dal = DalApi.Factory.Get;
        public void AddCall(BO.Call newCall)
        {
            AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
            CallManager.ValidateInputFormat(newCall);
            DO.Call doCall;

            lock (AdminManager.BlMutex)
            {
                // לא מחושב קואורדינטות כאן
                doCall = CallManager.CreateDoCall(newCall);
                _dal.Call.Create(doCall);
                _ = CallManager.SendEmailToNearbyVolunteersAsync(newCall);
            }

            CallManager.Observers.NotifyListUpdated();  //stage 5 

            // חישוב קואורדינטות אסינכרוני
            _ = CallManager.UpdateCallCoordinatesAsync(doCall);
        }

        public void UpdateCall(BO.Call call)
        {
            try
            {
                AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
                CallManager.ValidateInputFormat(call);

                DO.Call doCall;

                lock (AdminManager.BlMutex)
                {
                    // לא מחשבים קואורדינטות פה
                    doCall = CallManager.CreateDoCall(call);
                    _dal.Call.Update(doCall);
                }

                CallManager.Observers.NotifyItemUpdated(doCall.Id);  //stage 5
                CallManager.Observers.NotifyListUpdated();  //stage 5

                // חישוב קואורדינטות אסינכרוני
                _ = CallManager.UpdateCallCoordinatesAsync(doCall);
            }
            catch (DO.DalAlreadyExistsException ex)
            {
                throw new BO.BlNotFoundException($"There is no call with ID={call.Id} number.", ex);
            }
            catch (Exception ex)
            {
                throw new BO.BlDatabaseException("An unexpected error occurred while update call.", ex);
            }
        }
      
        public void DeleteCall(int callId)
        {
            try
            {
                AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
                if (CallManager.CalculateCallStatus(callId) != BO.CallStatus.Open || CallManager.WasNeverAssigned(callId))
                {
                    // If the call is not open or has assignments, throw an exception
                    throw new BO.BlInvalidInputException("Cannot delete a call that is not in 'Open' status or has been assigned.");
                }
                // Attempt to delete the call
                lock (AdminManager.BlMutex)
                    _dal.Call.Delete(callId);
                CallManager.Observers.NotifyListUpdated();  //stage 5 
            }
            catch (DO.DalAlreadyExistsException ex)
            {
                throw new BO.BlNotFoundException($"There is no call with ID={callId} number.", ex);
            }
            catch (Exception ex)
            {
                throw new BO.BlDatabaseException("An unexpected error occurred while update call.", ex);
            }
        }

        public BO.Call GetCallDetails(int callId)
        {
            IEnumerable<DO.Assignment> assignments;
            DO.Call? dalCall;
            Dictionary<int, String> volunteers;

            lock (AdminManager.BlMutex)

            {    // Get call from DAL
                dalCall = _dal.Call.Read(callId) ??
               throw new BO.BlNotFoundException($"Call with ID={callId} was not found in the system.");

                // Get all assignments for this call
                assignments = _dal.Assignment.ReadAll(a => a.CallId == callId).ToList();

                // Load all relevant volunteers in a single query
                var volunteerIds = assignments
                    .Where(a => a.VolunteerId != 0)
                    .Select(a => a.VolunteerId)
                    .Distinct()
                    .ToList();

                volunteers = _dal.Volunteer.ReadAll(v => volunteerIds.Contains(v.Id))
                   .ToDictionary(v => v.Id, v => v.Name);
            }

                // Convert assignments to BO.CallAssignInList
                var assignmentsList = assignments.Select(assignment => new BO.CallAssignInList
                {
                    VolunteerId = assignment.VolunteerId != 0 ? assignment.VolunteerId : null,
                    VolunteerName = volunteers.GetValueOrDefault(assignment.VolunteerId),
                    AssignmentStartTime = assignment.EntryTime,
                    AssignmentEndTime = assignment.EndTime,
                    CompletionType = assignment.EndTime.HasValue ?
                        (BO.TypeOfEndTime)assignment.TypeOfEndTime! : null
                }).ToList();
            

            // Create and return the BO.Call object
            return new BO.Call
            {
                Id = dalCall.Id,
                Type = (BO.TypeOfReading)dalCall.TypeOfReading,
                Description = dalCall.Description,
                Address = dalCall.Adress,
                Latitude = dalCall.Latitude,
                Longitude = dalCall.Longitude,
                OpeningTime = dalCall.TimeOfOpen,
                MaxEndTime = dalCall.MaxTimeToFinish,
                Assignments = assignmentsList,
                Status= CallManager.CalculateCallStatus(dalCall.Id) // Calculate the status of the call
            };
        }

        public IEnumerable<BO.CallInList> GetCalls(BO.CallField? filterField = null, object? filterValue = null, BO.CallField? sortField = null)
        {
            try
            {
                IEnumerable<BO.CallInList> callList;
                lock (AdminManager.BlMutex)
                {
                    var calls = _dal.Call.ReadAll();
                    var assignments = _dal.Assignment.ReadAll();
                    var volunteers = _dal.Volunteer.ReadAll().ToDictionary(v => v.Id, v => v.Name);
                     callList = calls.Select(call => CallManager.CreateCallInList(call, assignments, volunteers));
                }

                // סינון לפי שדה ספציפי וערך
                if (filterField.HasValue && filterValue != null)
                {
                    var property = typeof(BO.CallInList).GetProperty(filterField.ToString()!);
                    if (property != null)
                    {
                        callList = callList.Where(c =>
                        {
                            var val = property.GetValue(c);
                            return val != null && val.Equals(filterValue);
                        });
                    }
                }

                // מיון
                return sortField switch
                {
                    CallField.AssignmentId => callList.OrderBy(c => c.AssignmentId),
                    CallField.CallId => callList.OrderBy(c => c.CallId),
                    CallField.CallType => callList.OrderBy(c => c.CallType),
                    CallField.OpeningTime => callList.OrderBy(c => c.OpeningTime),
                    CallField.RemainingTime => callList.OrderBy(c => c.RemainingTime),
                    CallField.LastVolunteerName => callList.OrderBy(c => c.LastVolunteerName),
                    CallField.CompletionTime => callList.OrderBy(c => c.CompletionTime),
                    CallField.CallStatus => callList.OrderBy(c => c.CallStatus),
                    CallField.TotalAssignments => callList.OrderBy(c => c.TotalAssignments),
                    _ => callList.OrderBy(c => c.CallId)
                };
            }
            catch (Exception ex)
            {
                throw new BO.BlNotFoundException("Error retrieving calls list", ex);
            }
        }

        public int[] GetCallCountsByStatus()
        {
            try
            {
                IEnumerable<DO.Call> calls;
                lock (AdminManager.BlMutex)
                {
                     calls = _dal.Call.ReadAll();
                }

                var statusGroups =
                    from call in calls
                    let currentStatus = CallManager.CalculateCallStatus(call.Id)
                    group call by currentStatus into statusGroup
                    orderby statusGroup.Key
                    select new
                    {
                        Status = statusGroup.Key,
                        Count = statusGroup.Count()
                    };

                // יצירת מערך בגודל מספר הערכים באינום CallStatus
                int statusCount = Enum.GetValues<BO.CallStatus>().Length;
                int[] quantities = new int[statusCount];

                statusGroups.ToList()
                           .ForEach(group => quantities[(int)group.Status] = group.Count);

                return quantities;
            }

            catch (DO.DalDoesNotExistException ex)
            {
                throw new BO.BlNotFoundException("Required data was not found in the database", ex);
            }
            catch (Exception ex)
            {
                throw new BO.BlDatabaseException("An unexpected error occurred while calculating call quantities", ex);
            }
        }

        public void CompleteCallTreatment(int volunteerId, int assignmentId)
        {
            try
            {
                AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
                DO.Assignment assignment;
                lock (AdminManager.BlMutex)
                     assignment = _dal.Assignment.Read(assignmentId)!;

                // Validate authorization and status
                CallManager.ValidateAssignmentForCompletion(assignment!, volunteerId);

                // Update the assignment to reflect the completion of treatment
                var updatedAssignment = assignment! with
                {
                    EndTime = AdminManager.Now, // Set the end time to the current time
                    TypeOfEndTime = DO.TypeOfEndTime.treated // Set the end type as "Treated"
                };
                lock (AdminManager.BlMutex)
                    _dal.Assignment.Update(updatedAssignment);
                CallManager.Observers.NotifyListUpdated();
                CallManager.Observers.NotifyItemUpdated(volunteerId);

            }
            catch (DO.DalAlreadyExistsException ex)
            {
                throw new BO.BlNotFoundException($"There is no assignment with ID={assignmentId} .", ex);
            }
            catch (Exception ex)
            {
                throw new BO.BlDatabaseException("An unexpected error occurred while update call.", ex);
            }
        }

        public void CancelCallTreatment(int requesterId, int assignmentId)
        {
            try
            {
                AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
                lock (AdminManager.BlMutex)
                {

                    var assignment = _dal.Assignment.Read(assignmentId);
                    var volunteer = _dal.Volunteer.Read(requesterId);

                    if (assignment!.VolunteerId != requesterId && volunteer is null)
                    {
                        throw new BO.BlInvalidInputException("Requester does not have permission to cancel this treatment.");
                    }

                    //if (!=) // בדיקה אם המשתמש הוא מנהל
                    //    throw new BO.UnauthorizedActionException("The volunteer does not have permission to complete this treatment.");

                    var call = _dal.Call.Read(assignment.CallId);
                    if (call == null)
                        throw new ArgumentException($"Call with ID={assignment.CallId} does not exist.");

                    var updatedCall = call with
                    {
                        // לוודא שאין תקלות בשדות
                        TypeOfReading = call.TypeOfReading,
                        Description = call.Description,
                        Adress = call.Adress,
                        Longitude = call.Longitude,
                        Latitude = call.Latitude,
                        TimeOfOpen = call.TimeOfOpen,
                        MaxTimeToFinish = call.MaxTimeToFinish
                    };

                    _dal.Call.Update(updatedCall);

                    var updatedAssignment = assignment with
                    {
                        EndTime = DateTime.Now,
                        TypeOfEndTime = (assignment.VolunteerId == requesterId)
                        ? DO.TypeOfEndTime.SelfCancellation
                        : DO.TypeOfEndTime.CancelingAnAdministrator
                    };

                    _dal.Assignment.Update(updatedAssignment);
                    if (assignment.VolunteerId != requesterId)
                    {
                        volunteer = _dal.Volunteer.Read(assignment.VolunteerId);
                    }
                    _ = CallManager.SendEmailToVolunteerAsync(volunteer!, assignment);
                }
                CallManager.Observers.NotifyListUpdated();
                CallManager.Observers.NotifyItemUpdated(requesterId);



            }
           
            catch (Exception ex)
            {
                // טיפול בחריגות כלליות
                throw new BO.BlInvalidInputException($"An unexpected error occurred: {ex.Message}", ex);
            }
        }

        public void SelectCallForTreatment(int volunteerId, int callId)
        {
            try
            {
                AdminManager.ThrowOnSimulatorIsRunning();  //stage 7
                lock (AdminManager.BlMutex)
                {
                    // 1. קבלת הקריאה לפי מזהה הקריאה
                    var call = _dal.Call.Read(callId);
                    if (call == null)
                    {
                        throw new ArgumentException($"Call with ID={callId} does not exist.");
                    }

                    // 2. בדיקת אם הקריאה כבר טופלה או אם יש הקצאה פתוחה עליה
                    var existingAssignments = _dal.Assignment.ReadAll(a => a.CallId == callId && a.EndTime == null);
                    if (existingAssignments.Any())
                    {
                        throw new BO.InvalidOperationException("Call is already being handled or has an open assignment.");
                    }


                    // 3. בדיקת אם הקריאה לא פג תוקפה
                    if (call.MaxTimeToFinish < DateTime.Now)
                    {
                        throw new BO.InvalidOperationException("Call has expired and can no longer be assigned.");
                    }

                    // 4. יצירת הקצאה חדשה
                    var assignment = new DO.Assignment
                    (
                        Id: 0, // מזהה ההקצאה יתעדכן אוטומטית
                        CallId: callId,
                        VolunteerId: volunteerId,
                        TypeOfEndTime: null, // סוג הטיפול נשאר כ-Treated עד לסיום
                        EntryTime: DateTime.Now // זמן כניסת המתנדב לטיפול
                    );

                    // 5. הוספת ההקצאה לשכבת הנתונים
                    _dal.Assignment.Create(assignment);
                }
                CallManager.Observers.NotifyListUpdated();
                CallManager.Observers.NotifyItemUpdated(volunteerId);

            }
            catch (Exception ex)
            {
                throw new BO.BlDatabaseException("An unexpected error occurred while update call.", ex);
            }
        }

        public IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId,  BO.TypeOfReading? filterType = null, BO.ClosedCallField? sortField = null)
        {
           try
            {
                IEnumerable<BO.ClosedCallInList> closedCalls;
                lock (AdminManager.BlMutex)
                {
                    // Get all assignments for this volunteer
                    var assignments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId && a.EndTime != null);  // Only closed calls

                    // Get all calls associated with these assignments
                    var callIds = assignments.Select(a => a.CallId).Distinct();
                    var calls = _dal.Call.ReadAll(c => callIds.Contains(c.Id));

                    // Create ClosedCallInList objects using CallManager
                     closedCalls = CallManager.CreateClosedCallList(calls, assignments);
                }
                if (filterType.HasValue)
                {
                    closedCalls = closedCalls.Where(c => (BO.TypeOfReading)c.CallType == filterType.Value);
                }

                return sortField switch
                {
                    ClosedCallField.CallType => closedCalls.OrderBy(c => c.CallType),
                    ClosedCallField.FullAddress => closedCalls.OrderBy(c => c.FullAddress),
                    ClosedCallField.OpenTime => closedCalls.OrderBy(c => c.OpenTime),
                    ClosedCallField.AssignmentEntryTime => closedCalls.OrderBy(c => c.AssignmentEntryTime),
                    ClosedCallField.ActualEndTime => closedCalls.OrderBy(c => c.ActualEndTime.GetValueOrDefault(DateTime.MinValue)),
                    ClosedCallField.EndType => closedCalls.OrderBy(c => c.EndType),
                    _ =>closedCalls.OrderBy(c => c.Id)
                };
            }
           
            catch (DO.DalDoesNotExistException ex)
            {
                throw new BO.BlNotFoundException($"Could not find data for volunteer {volunteerId}", ex);
            }
            catch (Exception ex)
            {
                throw new BO.BlDatabaseException("An error occurred while retrieving closed calls", ex);
            }
        }
       
        public IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, BO.TypeOfReading? filterType = null, BO.OpenCallField? sortField=null)
        {
            try
            {
                lock (AdminManager.BlMutex)
                {
                    var volunteer = _dal.Volunteer.Read(volunteerId) ?? throw new BO.BlNotFoundException($"Volunteer with ID={volunteerId} does not exist.");
                    var openCalls = _dal.Call.ReadAll()
           .Where(c =>
               CallManager.CalculateCallStatus(c.Id) == BO.CallStatus.Open ||
               CallManager.CalculateCallStatus(c.Id) == BO.CallStatus.OpenAtRisk)
           .Select(c => new BO.OpenCallInList
           {
               Id = c.Id,
               Type = (BO.TypeOfReading)c.TypeOfReading,
               Description = c.Description!,
               FullAddress = c.Adress,
               OpenTime = c.TimeOfOpen,
               MaxEndTime = c.MaxTimeToFinish,
               DistanceFromVolunteer = Tools.CalculateDistance(volunteer.Latitude!, volunteer.Longitude!, c.Latitude, c.Longitude)
           })
           .Where(c => c.DistanceFromVolunteer < volunteer.MaximumDistance);
                    if (filterType.HasValue)
                    {
                        openCalls = openCalls.Where(c => (BO.TypeOfReading)c.Type == filterType.Value);
                    }

                    return sortField switch
                    {
                        OpenCallField.Id => openCalls.OrderBy(c => c.Id),
                        OpenCallField.Type => openCalls.OrderBy(c => c.Type),
                        OpenCallField.Description => openCalls.OrderBy(c => c.Description),
                        OpenCallField.FullAddress => openCalls.OrderBy(c => c.FullAddress),
                        OpenCallField.OpenTime => openCalls.OrderBy(c => c.OpenTime),
                        OpenCallField.MaxEndTime => openCalls.OrderBy(c => c.MaxEndTime),
                        OpenCallField.DistanceFromVolunteer => openCalls.OrderBy(c => c.DistanceFromVolunteer),
                        _ => openCalls.OrderBy(c => c.Id)
                    };
                }
            }


            catch (DO.DalDoesNotExistException ex)
            {
                throw new BO.BlNotFoundException($"Could not find data for volunteer {volunteerId}", ex);
            }
            catch (Exception ex)
            {
                throw new BO.BlDatabaseException("An error occurred while retrieving closed calls", ex);
            }

        }
        #region Stage 5
        public void AddObserver(Action listObserver) =>
        CallManager.Observers.AddListObserver(listObserver); //stage 5
        public void AddObserver(int id, Action observer) =>
    CallManager.Observers.AddObserver(id, observer); //stage 5
        public void RemoveObserver(Action listObserver) =>
    CallManager.Observers.RemoveListObserver(listObserver); //stage 5
        public void RemoveObserver(int id, Action observer) =>
    CallManager.Observers.RemoveObserver(id, observer); //stage 5
        #endregion Stage 5



    }
}
