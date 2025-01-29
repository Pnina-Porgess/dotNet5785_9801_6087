using BO;
using BlApi;
using DO;
using Helpers;
using DalApi;
using System;

namespace BlImplementation
{
    public class CallImplementation : BlApi.ICall
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
                throw new BO.BLDoesNotExist($"There is no call with ID={call.Id} number.", ex);
            }
            catch (Exception ex)
            {
                throw new BO.GeneralDatabaseException("An unexpected error occurred while update call.", ex);
            }
        }

        public void DeleteCall(int callId)
        {
            try
            {
                if (CallManager.CalculateCallStatus(callId) != CallStatus.Open || CallManager.WasNeverAssigned(callId))
                {
                    //?
                    // If the call is not open or has assignments, throw an exception
                    throw new InvalidOperationException("Cannot delete a call that is not in 'Open' status or has been assigned.");
                }
                // Attempt to delete the call
                _dal.Call.Delete(callId);
            }
            catch (Exception ex)
            {
                // Catch and rethrow exceptions with appropriate messages
                throw new InvalidOperationException($"An error occurred while attempting to delete the call: {ex.Message}", ex);
            }
        }

        public BO.Call GetCallDetails(int callId)
        {
            // Get call from DAL
            var dalCall = _dal.Call.Read(callId) ??
                throw new BO.NotFoundException($"Call with ID={callId} was not found in the system.");

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

        public void CancelCallTreatment(int requesterId, int assignmentId)
        {
            try
            {
                var assignment = _dal.Assignment.Read(assignmentId);
                if (assignment == null)
                {
                    throw new ArgumentException($"Assignment with ID={assignmentId} does not exist.");
                }

                if (assignment.VolunteerId != requesterId && _dal.Volunteer.Read(requesterId) is null )
                {
                    throw new InvalidOperationException("Requester does not have permission to cancel this treatment.");
                }

                if (assignment.EndTime != null)
                {
                    throw new InvalidOperationException("Cannot cancel an assignment that has already been completed.");
                }

                var call = _dal.Call.Read(assignment.CallId);
                if (call == null)
                {
                    throw new ArgumentException($"Call with ID={assignment.CallId} does not exist.");
                }

                var updatedCall = call with
                {
                    TypeOfReading = call.TypeOfReading,
                    Description = call.Description,
                    Adress = call.Adress,
                    Longitude = call.Longitude,
                    Latitude = call.Latitude,
                    TimeOfOpen = call.TimeOfOpen,
                    MaxTimeToFinish = call.MaxTimeToFinish
                };

                // עדכון הקריאה בחזרה למצב פתוח
                _dal.Call.Update(updatedCall);

                // עדכון ההקצאה
                var updatedAssignment = assignment with
                {
                    EndTime = DateTime.Now,
                    TypeOfEndTime = assignment.VolunteerId == requesterId
                    ? TypeOfEndTime.CancelingAnAdministrator
                    : TypeOfEndTime.SelfCancellation
                };
                _dal.Assignment.Update(updatedAssignment);

                // עדכון ההקצאה בשכבת הנתונים
                _dal.Assignment.Update(assignment);
            }
            catch (Exception ex)
            {
                // טיפול בחריגות כלליות
                throw new InvalidOperationException($"An unexpected error occurred: {ex.Message}", ex);
            }
        }


        public void CompleteCallTreatment(int volunteerId, int assignmentId)
        {
            try
            {
                // Retrieve the assignment by ID
                var assignment = _dal.Assignment.Read(assignmentId);
                if (assignment == null)
                {
                    throw new ArgumentException($"Assignment with ID={assignmentId} does not exist.");
                }

                // Check if the volunteer is the one assigned to this call
                if (assignment.VolunteerId != volunteerId)
                {
                    throw new InvalidOperationException("The volunteer does not have permission to complete this treatment.");
                }

                // Check if the assignment is still open (i.e., treatment has not been completed or cancelled)
                if (assignment.EndTime != null)
                {
                    throw new InvalidOperationException("This treatment has already been completed or cancelled.");
                }

                // Retrieve the associated call
                var call = _dal.Call.Read(assignment.CallId);
                if (call == null)
                {
                    throw new ArgumentException($"Call with ID={assignment.CallId} does not exist.");
                }

                // Update the assignment to reflect the completion of treatment
                var updatedAssignment = assignment with
                {
                    EndTime = DateTime.Now, // Set the end time to the current time
                    TypeOfEndTime = TypeOfEndTime.treated // Set the end type as "Treated"
                };

                // Update the assignment in the data layer
                _dal.Assignment.Update(updatedAssignment);
            }
            catch (Exception ex)
            {
                // Catch any general exceptions and throw a new exception to the UI layer
                throw new InvalidOperationException($"An unexpected error occurred: {ex.Message}", ex);
            }
        }

        public IEnumerable<OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, CallType? filterType, CallField? sortField)
        {
            try
            {
                // Retrieve all assignments for the given volunteer
                var assignments = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId && a.EndTime == null);

                // Retrieve all open calls (either "open" or "at-risk")
                var openCalls = _dal.Call.ReadAll(c => c.Status == CallStatus.Open || c.Status == CallStatus.OpenRisk);

                // Filter open calls by the specified call type, if provided
                if (filterType.HasValue)
                {
                    openCalls = openCalls.Where(c => c.TypeOfReading == (TypeOfReading)filterType);
                }

                // Create the result list which includes the distance to the volunteer (just an example distance calculation)
                var openCallsWithDistance = openCalls.Select(call =>
                {
                    // Calculate the distance from the volunteer to the call (assuming you have latitude and longitude for both)
                    var distance = CalculateDistance(call.Latitude, call.Longitude, volunteerId); // A method to calculate the distance

                    return new OpenCallInList
                    {
                        CallId = call.Id,
                        Type = (CallType)call.TypeOfReading,
                        Description = call.Description,
                        Address = call.Adress,
                        Latitude = call.Latitude,
                        Longitude = call.Longitude,
                        Distance = distance
                    };
                });

                // Sort the list by the specified field, if provided
                if (sortField.HasValue)
                {
                    switch (sortField.Value)
                    {
                        case CallField.Id:
                            openCallsWithDistance = openCallsWithDistance.OrderBy(call => call.CallId);
                            break;
                        case CallField.Description:
                            openCallsWithDistance = openCallsWithDistance.OrderBy(call => call.Description);
                            break;
                        case CallField.Distance:
                            openCallsWithDistance = openCallsWithDistance.OrderBy(call => call.Distance);
                            break;
                            // Add more sorting options as needed
                    }
                }
                else
                {
                    // Default sorting by call ID
                    openCallsWithDistance = openCallsWithDistance.OrderBy(call => call.CallId);
                }

                return openCallsWithDistance;
            }
            catch (Exception ex)
            {
                // General exception handling
                throw new InvalidOperationException($"An unexpected error occurred: {ex.Message}", ex);
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
                    throw new InvalidOperationException("Call is already being handled or has an open assignment.");
                }

                // 3. בדיקת אם הקריאה לא פג תוקפה
                if (call.MaxTimeToFinish < DateTime.Now)
                {
                    throw new InvalidOperationException("Call has expired and can no longer be assigned.");
                }

                // 4. יצירת הקצאה חדשה
                var assignment = new DO.Assignment
                (
                    Id: 0, // מזהה ההקצאה יתעדכן אוטומטית
                    CallId: callId,
                    VolunteerId: volunteerId,
                    TypeOfEndTime: TypeOfEndTime.treated, // סוג הטיפול נשאר כ-Treated עד לסיום
                    EntryTime: DateTime.Now // זמן כניסת המתנדב לטיפול
                );

                // 5. הוספת ההקצאה לשכבת הנתונים
                _dal.Assignment.Create(assignment);
            }
            catch (Exception ex)
            {
                // טיפול בחריגות כלליות
                throw new InvalidOperationException($"An unexpected error occurred: {ex.Message}", ex);
            }
        }

        public int[] GetCallCountsByStatus()
        {
            throw new NotImplementedException();
        }
        public IEnumerable<ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, CallStatus? filterStatus, CallField? sortField)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, CallStatus? filterStatus, CallField? sortField)
        {
            throw new NotImplementedException();
        }
        public IEnumerable<CallInList> GetCalls(CallField? filterField, object? filterValue, CallField? sortField)
        {
            throw new NotImplementedException();
        }

    }
}
