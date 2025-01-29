using BO;
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

    // Method to get coordinates from an address
    internal static (double Latitude, double Longitude) GetCoordinates(string address)
    {
        using (var client = new HttpClient())
        {
            string url = $"https://geocode.maps.co/search?q={Uri.EscapeDataString(address)}&format=json";
            var response = client.GetStringAsync(url).Result;
            var data = JsonConvert.DeserializeObject<List<GeoResponse>>(response);

            if (data == null || data.Count == 0)
                throw new ArgumentException("Invalid address");

            return (data[0].Lat, data[0].Lon);
        }
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
}

