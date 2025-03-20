
using BlApi;
using Helpers;

namespace BlImplementation
{
    public class CallImplementation : ICall
    {
        private readonly DalApi.IDal _dal = DalApi.Factory.Get;
        public void AddCall(BO.Call newCall)
        {
            CallManager.ValidateInputFormat(newCall);
            var (latitude, longitude) = CallManager.logicalChecking(newCall);
            newCall.Latitude = latitude;
            newCall.Longitude = longitude;
            var call = CallManager.CreateDoCall(newCall);
            _dal.Call.Create(call);
        }

        public void UpdateCall(BO.Call call)
        {
            try
            {
                CallManager.ValidateInputFormat(call);
                var (latitude, longitude) = CallManager.logicalChecking(call);
                call.Latitude = latitude;
                call.Longitude = longitude;
                var updatedCall = CallManager.CreateDoCall(call);
                _dal.Call.Update(updatedCall);
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
                if (CallManager.CalculateCallStatus(callId) != BO.CallStatus.Open || CallManager.WasNeverAssigned(callId))
                {
                    // If the call is not open or has assignments, throw an exception
                    throw new BO.BlInvalidInputException("Cannot delete a call that is not in 'Open' status or has been assigned.");
                }
                // Attempt to delete the call
                _dal.Call.Delete(callId);
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
            // Get call from DAL
            var dalCall = _dal.Call.Read(callId) ??
                throw new BO.BlNotFoundException($"Call with ID={callId} was not found in the system.");

            // Get all assignments for this call
            var assignments = _dal.Assignment.ReadAll(a => a.CallId == callId).ToList();

            // Load all relevant volunteers in a single query
            var volunteerIds = assignments
                .Where(a => a.VolunteerId != 0)
                .Select(a => a.VolunteerId)
                .Distinct()
                .ToList();

            var volunteers = _dal.Volunteer.ReadAll(v => volunteerIds.Contains(v.Id))
                .ToDictionary(v => v.Id, v => v.Name);

            // Convert assignments to BO.CallAssignInList
            var assignmentsList = assignments.Select(assignment => new BO.CallAssignInList
            {
                VolunteerId = assignment.VolunteerId != 0 ? assignment.VolunteerId : null,
                VolunteerName = volunteers.GetValueOrDefault(assignment.VolunteerId),
                AssignmentStartTime = assignment.EntryTime,
                AssignmentEndTime = assignment.EndTime,
                CompletionType = assignment.EndTime.HasValue ?
                    (BO.CallCompletionType)assignment.TypeOfEndTime : null
            }).ToList();

            // Create and return the BO.Call object
            return new BO.Call
            {
                Id = dalCall.Id,
                Type = (BO.CallType)dalCall.TypeOfReading,
                Description = dalCall.Description,
                Address = dalCall.Adress,
                Latitude = dalCall.Latitude,
                Longitude = dalCall.Longitude,
                OpeningTime = dalCall.TimeOfOpen,
                MaxEndTime = dalCall.MaxTimeToFinish,
                Assignments = assignmentsList
            };
        }

        public IEnumerable<BO.CallInList> GetCalls(BO.CallField? filterField = null, object? filterValue = null, BO.CallField? sortField = null)
        {
            try
            {
                var calls = _dal.Call.ReadAll();
                var assignments = _dal.Assignment.ReadAll();
                var volunteers = _dal.Volunteer.ReadAll().ToDictionary(v => v.Id, v => v.Name);
                var callList = calls.Select(call => CallManager.CreateCallInList(call, assignments, volunteers));

                if (filterField.HasValue && filterValue != null)
                {
                    callList = CallManager.FilterCall(callList, filterField.Value, filterValue);
                }

                return CallManager.SortCalls(callList, sortField ?? BO.CallField.CallId).ToList();
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
                var calls = _dal.Call.ReadAll();
                var assignments = _dal.Assignment.ReadAll();

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
                // Retrieve the assignment by ID
                var assignment = _dal.Assignment.Read(assignmentId);

                // Validate authorization and status
                CallManager.ValidateAssignmentForCompletion(assignment!, volunteerId);

                // Update the assignment to reflect the completion of treatment
                var updatedAssignment = assignment! with
                {
                    EndTime = ClockManager.Now, // Set the end time to the current time
                    TypeOfEndTime = DO.TypeOfEndTime.treated // Set the end type as "Treated"
                };

                _dal.Assignment.Update(updatedAssignment);
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
                var assignment = _dal.Assignment.Read(assignmentId);
                var volunteer = _dal.Volunteer.Read(requesterId);

                if (assignment.VolunteerId != requesterId && volunteer is null)
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
                    TypeOfEndTime = assignment.VolunteerId == requesterId
                    ? DO.TypeOfEndTime.CancelingAnAdministrator
                    : DO.TypeOfEndTime.SelfCancellation
                };

                _dal.Assignment.Update(updatedAssignment);
                CallManager.SendEmailToVolunteer(volunteer!, assignment);
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
                    TypeOfEndTime: DO.TypeOfEndTime.treated, // סוג הטיפול נשאר כ-Treated עד לסיום
                    EntryTime: DateTime.Now // זמן כניסת המתנדב לטיפול
                );

                // 5. הוספת ההקצאה לשכבת הנתונים
                _dal.Assignment.Create(assignment);
            }
            catch (Exception ex)
            {
                throw new BO.BlDatabaseException("An unexpected error occurred while update call.", ex);
            }
        }

        public IEnumerable<BO.ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId,  BO.TypeOfReading? filterType = null, BO.CallField? sortField = null)
        {
           try
            {
                // Get all assignments for this volunteer
                var assignments = _dal.Assignment.ReadAll(a =>a.VolunteerId == volunteerId &&a.EndTime != null);  // Only closed calls

                // Get all calls associated with these assignments
                var callIds = assignments.Select(a => a.CallId).Distinct();
                var calls = _dal.Call.ReadAll(c => callIds.Contains(c.Id));

                // Create ClosedCallInList objects using CallManager
                var closedCalls = CallManager.CreateClosedCallList(calls, assignments);
                if (filterType.HasValue)
                {
                    closedCalls= closedCalls.Where(c => c.CallType == filterType.Value);
                }

                return CallManager.SortCalls( closedCalls, sortField ?? BO.CallField.CallId);
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
       
        public IEnumerable<BO.OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, BO.CallStatus? filterStatus, BO.CallField? sortField)
        {
            try
            {
                var volunteer = _dal.Volunteer.Read(volunteerId) ?? throw new BO.BlNotFoundException($"Volunteer with ID={volunteerId} does not exist.");
                var openCalls = _dal.Call.ReadAll()
                    .Where(c =>
                    // מחשבים סטטוס של כל קריאה
                    (CallManager.CalculateCallStatus(c.Id) == BO.CallStatus.Open || CallManager.CalculateCallStatus(c.Id) == BO.CallStatus.OpenAtRisk)) // הפשטת הבדיקה
                    .Select(c => new BO.OpenCallInList
                    {
                        Id = volunteerId, // ת.ז של המתנדב
                        Type = (BO.TypeOfReading)c.TypeOfReading, // סוג הקריאה
                        Description = c.Description!, // תיאור מילולי
                        FullAddress = c.Adress, // כתובת הקריאה
                        OpenTime = c.TimeOfOpen, // זמן פתיחת הקריאה
                        MaxEndTime = c.MaxTimeToFinish, // זמן סיום משוער
                        DistanceFromVolunteer = Tools.CalculateDistance(volunteer.Latitude!,volunteer.Longitude!, c.Latitude, c.Longitude)
                    });

                return CallManager.SortCalls(openCalls, sortField ?? BO.CallField.CallId);

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

      
    }
}
