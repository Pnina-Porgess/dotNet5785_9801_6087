

namespace BO
{
    public class OpenCallInList
    {

        public int Id { get; init; } // Call ID, unique for each call.

        public TypeOfReading Type { get; init; } // Enum representing the type of the call.


        public string Description { get; init; } // A textual description of the call.


        public string FullAddress { get; init; } // The full address where the call is located.


        public DateTime OpenTime { get; init; } // The time when the call was opened.


        public DateTime? MaxEndTime { get; init; } // The deadline for completing the call (nullable).

        public double DistanceFromVolunteer { get; init; } // Computed distance from the volunteer's location.
        public override string ToString()
        {
            return $"Call ID: {Id}\n" +
                   $"Type: {Type}\n" +
                   $"Description: {(!string.IsNullOrEmpty(Description) ? Description : "None")}\n" +
                   $"Address: {FullAddress}\n" +
                   $"Opened At: {OpenTime}\n" +
                   $"MaxEndTime: {(MaxEndTime.HasValue ? MaxEndTime.ToString() : "None")}\n" +
                   $"Distance from Volunteer: {DistanceFromVolunteer} km";
        }
    }

}
