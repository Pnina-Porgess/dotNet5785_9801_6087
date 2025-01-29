using BO;
using BlApi;
using DO;
using Helpers;
using DalApi;

namespace BlImplementation
{
    public class CallImplementation : ICall
    {
        private readonly DalApi.IDal _dal = DalApi.Factory.Get;
        public void AddCall(BO.Call newCall)
        {
            CallManager.ValidateInputFormat(newCall);
            var (latitude, longitude) = CallManager.logicalChecking(newCall);
           if (latitude != null && longitude != null)
            {
                newCall.Latitude = latitude;
                newCall.Longitude = longitude;
            }
            var call = CallManager.CreateDoCall(newCall);
            _dal.Call.Create(call);
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


        public void DeleteCall(int callId)
        {
            try
            {
                // Retrieve the call by its ID
                var call = _dal.Call.Read(callId);
                if (call == null)
                {
                    // Throw an exception if the call does not exist
                    throw new ArgumentException($"Call with ID={callId} does not exist.");
                }
                CalculateStatus(DO.Assignment currentAssignment,call, int timeMarginMinutes)
                // Check if the call can be deleted
                if (call.Status != CallStatus.Open || call.HasAssignments)
                {
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


        public int[] GetCallCountsByStatus()
        {
            throw new NotImplementedException();
        }

        public BO.Call GetCallDetails(int callId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CallInList> GetCalls(CallField? filterField, object? filterValue, CallField? sortField)
        {
            throw new NotImplementedException();
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

        // Helper method to calculate the distance between the volunteer and the call (simplified example)
        private double CalculateDistance(double callLatitude, double callLongitude, int volunteerId)
        {
            // Retrieve volunteer's current location from the data layer
            var volunteer = _dal.Volunteer.Read(volunteerId);
            if (volunteer == null)
            {
                throw new ArgumentException($"Volunteer with ID={volunteerId} does not exist.");
            }

            // Example: You can use a distance calculation formula like Haversine formula
            double volunteerLatitude = volunteer.Latitude;
            double volunteerLongitude = volunteer.Longitude;

            // For simplicity, let's assume a placeholder distance calculation here
            double distance = Math.Sqrt(Math.Pow(callLatitude - volunteerLatitude, 2) + Math.Pow(callLongitude - volunteerLongitude, 2));

            return distance; // Return distance in some unit (e.g., kilometers or miles)
        }


        public IEnumerable<ClosedCallInList> GetClosedCallsByVolunteer(int volunteerId, CallStatus? filterStatus, CallField? sortField)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<OpenCallInList> GetOpenCallsForVolunteer(int volunteerId, CallStatus? filterStatus, CallField? sortField)
        {
            throw new NotImplementedException();
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


        public void UpdateCall(BO.Call call)
        {
            var existingCall = _dal.Call.Read(call.Id);
            if (existingCall == null)
                throw new ArgumentException($"Call with ID={call.Id} does not exist.");

            var updatedCall = new DO.Call
            (
                Id: call.Id,
                TypeOfReading: (TypeOfReading)call.Type, // המרת סוג הקריאה
                Description: call.Description,
                Adress: call.Address,
                Latitude: call.Latitude,
                Longitude: call.Longitude,
                TimeOfOpen: call.OpeningTime,
                MaxTimeToFinish:call.MaxEndTime
            );

            // עדכון הקריאה במאגר
            _dal.Call.Update(updatedCall);
        }

    }
}
