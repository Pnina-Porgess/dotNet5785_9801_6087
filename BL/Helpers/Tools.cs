

using BO;

namespace Helpers;
    internal static class Tools
    {

        public static double CalculateDistance(double? lat1, double? lon1, double? lat2, double? lon2)
        {
            if (!lat1.HasValue || !lon1.HasValue || !lat2.HasValue || !lon2.HasValue)
                throw new ArgumentException("Latitude and Longitude must have valid values.");
            const double R = 6371000;
            double lat1Rad = DegreesToRadians(lat1.Value);
            double lon1Rad = DegreesToRadians(lon1.Value);
            double lat2Rad = DegreesToRadians(lat2.Value);
            double lon2Rad = DegreesToRadians(lon2.Value);
            double deltaLat = lat2Rad - lat1Rad;
            double deltaLon = lon2Rad - lon1Rad;
            double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                       Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                       Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double distance = R * c;
            return distance;
         }
        private static double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }

    public static CallStatusEnum CalculateStatus(DO.Assignment currentAssignment, DO.Call callDetails, int timeMarginMinutes)
    {
        if (currentAssignment.EndTime.HasValue)
        {
            // השיחה הושלמה, אין צורך לעדכן את המצב
            return CallStatusEnum.InProgress; // אנחנו מחזירים את המצב InProgress אם השיחה הושלמה
        }
        // חישוב הזמן הנותר לסיום השיחה
        DateTime maxFinishTime = callDetails.MaxTimeToFinish;
        // הזמן הנוכחי
        DateTime currentTime = DateTime.Now;
        // חישוב ההפרש בין הזמן הנוכחי לזמן הסיום המקסימלי של השיחה
        TimeSpan timeDifference = maxFinishTime - currentTime;
        // אם הזמן הנותר לשיחה הוא פחות מ-30 דקות (סיכון), נחזיר AtRisk
        if (timeDifference.TotalMinutes <= timeMarginMinutes)
        {
            return CallStatusEnum.AtRisk;
        }

        // אם הזמן נותר יותר מ-30 דקות, אז השיחה עדיין בתהליך
        return CallStatusEnum.InProgress;
    }
}
    }


