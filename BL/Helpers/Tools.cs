

using BO;
using Newtonsoft.Json.Linq;
namespace Helpers;
    internal static class Tools
    {
 private static readonly DalApi.IDal _dal = DalApi.Factory.Get; //stage 4
    private static readonly string apiUrl = "https://geocode.maps.co/search?q={0}&api_key={1}";
    private static readonly string apiKey = "6797d46098d51743505867ysm058e34";

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



    public static (double Latitude, double Longitude) GetCoordinatesFromAddress(string address)
    {
        if (string.IsNullOrWhiteSpace(address))
        {
            throw new InvalidAddressException(address); // חריגה אם הכתובת לא תקינה
        }

        try
        {
            // יצירת ה-URL לפנייה ל-API
            string url = string.Format(apiUrl, Uri.EscapeDataString(address), apiKey);

            using (HttpClient client = new HttpClient())
            {
                // בקשה סינכרונית ל-API
                HttpResponseMessage response = client.GetAsync(url).Result;

                // בדיקה אם הבקשה הצליחה
                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = response.Content.ReadAsStringAsync().Result;

                    // ניתוח התשובה כ-JSON
                    JArray jsonArray = JArray.Parse(jsonResponse);

                    // אם יש תוצאות, מחזירים את הקואורדינטות
                    if (jsonArray.Count > 0)
                    {
                        var firstResult = jsonArray[0];
                        double latitude = (double)firstResult["lat"]!;
                        double longitude = (double)firstResult["lon"]!;
                        return (latitude, longitude);
                    }
                    else
                    {
                        throw new BlGeolocationNotFoundException(address); // חריגה אם לא נמצאה גיאוקולציה
                    }
                }
                else
                {
                    throw new BlApiRequestException($"API request failed with status code: {response.StatusCode}"); // חריגה אם הבקשה נכשלה
                }
            }
        }
        catch (Exception ex)
        {
            // אם קרתה שגיאה כלשהי, זורקים חריגה עם פרטי השגיאה
            throw new BlApiRequestException($"Error occurred while fetching coordinates for the address. {ex.Message}");
        }
    }
}
   





