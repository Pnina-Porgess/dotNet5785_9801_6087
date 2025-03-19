

using BO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text.Json;
namespace Helpers;
    internal static class Tools
    {
 private static readonly DalApi.IDal _dal = DalApi.Factory.Get; //stage 4
 

    public static double CalculateDistance(object latitude1, object longitude1, double latitude2, double longitude2)
    {
        // המרת פרמטרים מסוג object ל-double
        if (!double.TryParse(latitude1?.ToString(), out double lat1) ||
            !double.TryParse(longitude1?.ToString(), out double lon1))
        {
            throw new ArgumentException("Invalid latitude or longitude values.");
        }

        const double EarthRadiusKm = 6371; // רדיוס כדור הארץ בקילומטרים

        // המרת מעלות לרדיאנים
        double lat1Rad = DegreesToRadians(lat1);
        double lon1Rad = DegreesToRadians(lon1);
        double lat2Rad = DegreesToRadians(latitude2);
        double lon2Rad = DegreesToRadians(longitude2);

        // חישוב ההפרשים
        double deltaLat = lat2Rad - lat1Rad;
        double deltaLon = lon2Rad - lon1Rad;

        // נוסחת Haversine
        double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                   Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                   Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        // חישוב המרחק
        double distance = EarthRadiusKm * c;
        return distance;
    }

    public static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }
    public static (double, double) GetCoordinatesFromAddress(string address)
    {
        string apiKey = "PK.83B935C225DF7E2F9B1ee90A6B46AD86";
        using var client = new HttpClient();
        string url = $"https://us1.locationiq.com/v1/search.php?key={apiKey}&q={Uri.EscapeDataString(address)}&format=json";

        var response = client.GetAsync(url).GetAwaiter().GetResult();
        if (!response.IsSuccessStatusCode)
            throw new Exception("Invalid address or API error.");

        var json = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        using var doc = JsonDocument.Parse(json);

        if (doc.RootElement.ValueKind != JsonValueKind.Array || doc.RootElement.GetArrayLength() == 0)
            throw new Exception("Address not found.");

        var root = doc.RootElement[0];

        // המרת הערכים ל-double
        double latitude = double.Parse(root.GetProperty("lat").GetString());
        double longitude = double.Parse(root.GetProperty("lon").GetString());

        return (latitude, longitude);
    }
}
   





