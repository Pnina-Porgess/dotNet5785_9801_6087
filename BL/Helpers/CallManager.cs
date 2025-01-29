using DalApi;
using DO;
namespace Helpers;

internal static class CallManager
    
{
    private static IDal _dal = Factory.Get; //stage 4
    // Method to get call status based on the system clock and database
    internal static CallStatus GetCallStatus(int callId)
    {
        var call = _dal.Call.Read(callId);
        if (call == null)
            throw new ArgumentException($"Call with ID={callId} does not exist.");

        if (DateTime.Now > call.MaxTimeToFinish)
            return CallStatus.Expired;

        return CallStatus.Open; // Adjust logic as needed
    }

    // Method to calculate aerial distance between two addresses
    internal static double CalculateDistance(string address1, string address2)
    {
        var (lat1, lon1) = GetCoordinates(address1);
        var (lat2, lon2) = GetCoordinates(address2);

        return HaversineDistance(lat1, lon1, lat2, lon2);
    }

    // Haversine formula to calculate distance between two lat/lon points
    private static double HaversineDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double R = 6371; // Earth's radius in km
        double dLat = ToRadians(lat2 - lat1);
        double dLon = ToRadians(lon2 - lon1);

        double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                   Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                   Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return R * c;
    }

    private static double ToRadians(double degrees) => degrees * Math.PI / 180;

    // Method to update expired calls
    internal static void UpdateExpiredCalls()
    {
        var calls = _dal.Call.GetAll()
            .Where(call => DateTime.Now > call.MaxTimeToFinish && call.Status == CallStatus.Open)
            .ToList();

        foreach (var call in calls)
        {
            var assignments = _dal.Assignment.GetByCallId(call.Id);

            if (assignments.Count == 0)
            {
                var newAssignment = new DO.Assignment
                {
                    CallId = call.Id,
                    VolunteerId = 0,
                    AssignmentEndTime = DateTime.Now,
                    CompletionType = CompletionType.Expired
                };
                _dal.Assignment.Create(newAssignment);
            }
            else
            {
                foreach (var assignment in assignments.Where(a => a.AssignmentEndTime == null))
                {
                    assignment.AssignmentEndTime = DateTime.Now;
                    assignment.CompletionType = CompletionType.Expired;
                    _dal.Assignment.Update(assignment);
                }
            }
        }
    }
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
    internal static (double? Latitude, double? Longitude) logicalChecking(BO.Call call)
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
}
