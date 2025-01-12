using DO;

namespace BO;

/// <summary>
/// Represents a closed call displayed in the volunteer's call history.
/// </summary>
public class ClosedCallInList
{
    public int Id { get; init; } // Unique identifier for the call. Taken from DO.Call.

    public TypeOfReading CallType { get; set; } // Type of the call. Taken from DO.Call.

    public string FullAddress { get; set; } // Full address of the call. Taken from DO.Call.

    public DateTime OpenTime { get; set; } // The time the call was opened. Taken from DO.Call.

    public DateTime AssignmentEntryTime { get; set; } // The time the call was assigned to a volunteer. Taken from DO.Assignment.

    public DateTime? ActualEndTime { get; set; } // The actual time the call was closed. Taken from DO.Assignment.

    public TypeOfEndTime? EndType { get; set; } // Type of the call closure (e.g., handled, cancelled, expired). Taken from DO.Assignment.
}

