

namespace BO
{
    public class OpenCallInList
    {
       
        public int Id { get; } // Call ID, unique for each call.

        public TypeOfReading Type { get; } // Enum representing the type of the call.

       
        public string Description { get; } // A textual description of the call.

       
        public string FullAddress { get; } // The full address where the call is located.

        
        public DateTime OpenTime { get; } // The time when the call was opened.

        
        public DateTime? MaxEndTime { get; } // The deadline for completing the call (nullable).

        public double DistanceFromVolunteer { get; } // Computed distance from the volunteer's location.

    }

}
