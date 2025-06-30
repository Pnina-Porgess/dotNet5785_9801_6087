
using System.Text.Json;
namespace Helpers;
using System.Net;
using System.Net.Mail;

internal static class Tools
{
    private static readonly DalApi.IDal _dal = DalApi.Factory.Get; //stage 4

    /// <summary>
    /// Calculates the distance in kilometers between two geographical points using the Haversine formula.
    /// </summary>
    /// <param name="latitude1">Latitude of the first point (can be an object that is convertible to double).</param>
    /// <param name="longitude1">Longitude of the first point (can be an object that is convertible to double).</param>
    /// <param name="latitude2">Latitude of the second point.</param>
    /// <param name="longitude2">Longitude of the second point.</param>
    /// <returns>The distance between the two points in kilometers.</returns>
    /// <exception cref="ArgumentException">Thrown when the latitude or longitude values are invalid.</exception>
    public static double CalculateDistance(object latitude1, object longitude1, double latitude2, double longitude2)
    {
        if (!double.TryParse(latitude1?.ToString(), out double lat1) ||
            !double.TryParse(longitude1?.ToString(), out double lon1))
        {
            throw new ArgumentException("Invalid latitude or longitude values.");
        }

        const double EarthRadiusKm = 6371; // Radius of Earth in kilometers

        double lat1Rad = DegreesToRadians(lat1);
        double lon1Rad = DegreesToRadians(lon1);
        double lat2Rad = DegreesToRadians(latitude2);
        double lon2Rad = DegreesToRadians(longitude2);

        double deltaLat = lat2Rad - lat1Rad;
        double deltaLon = lon2Rad - lon1Rad;

        double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                   Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                   Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        double distance = EarthRadiusKm * c;
        return distance;
    }

    /// <summary>
    /// Converts degrees to radians.
    /// </summary>
    /// <param name="degrees">Angle in degrees.</param>
    /// <returns>The angle in radians.</returns>
    public static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180;
    }

    /// <summary>
    /// Retrieves the latitude and longitude of a given address using the LocationIQ API.
    /// </summary>
    /// <param name="address">The address to geocode.</param>
    /// <returns>A tuple containing the latitude and longitude of the address.</returns>
    /// <exception cref="Exception">Thrown when the address is not found or the API returns an error.</exception>
    public static async Task<(double Latitude, double Longitude)?> GetCoordinatesFromAddressAsync(string address)
    {
        string apiKey = "PK.83B935C225DF7E2F9B1ee90A6B46AD86";
        using var client = new HttpClient();
        string url = $"https://us1.locationiq.com/v1/search.php?key={apiKey}&q={Uri.EscapeDataString(address)}&format=json";

        try
        {
            var response = await client.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return null; // אפשרות לשדרג: לזרוק שגיאה עם מידע מדויק

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            if (doc.RootElement.ValueKind != JsonValueKind.Array || doc.RootElement.GetArrayLength() == 0)
                return null;

            var root = doc.RootElement[0];

            double latitude = double.Parse(root.GetProperty("lat").GetString()!);
            double longitude = double.Parse(root.GetProperty("lon").GetString()!);

            return (latitude, longitude);
        }
        catch
        {
            return null; 
        }
    }


    /// <summary>
    /// Sends an email using an SMTP server.
    /// </summary>
    /// <param name="toEmail">The recipient's email address.</param>
    /// <param name="subject">The subject of the email.</param>
    /// <param name="body">The body of the email.</param>
    /// <exception cref="Exception">Thrown when the email cannot be sent.</exception>
    public static void SendEmail(string toEmail, string subject, string body)
    {
        var fromAddress = new MailAddress("projectydidim@gmail.com", "Yedidim");
        var toAddress = new MailAddress(toEmail);

        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential("yedidimproject1234@gmail.com", "lucg ughi pfwj fzol"),
            EnableSsl = true,
        };

        using (var message = new MailMessage(fromAddress, toAddress)
        {
            Subject = subject,
            Body = body,
        })
        {
            smtpClient.Send(message);
        }
    }
}
