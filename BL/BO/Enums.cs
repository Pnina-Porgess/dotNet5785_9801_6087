namespace BO;
public enum Role
{
    Volunteer,
    Manager
}
/// <summary>
/// 
/// </summary>
public enum DistanceType
{
    AerialDistance,
    walkingDistance,
    drivingDistance
}
/// <summary>
/// 
/// </summary>
public enum TypeOfReading
{
    None,
    FlatTire,
    DeadBattery,
    EngineFailure
   
}
/// <summary>
/// 
/// </summary>

public enum TypeOfEndTime
{
    treated,
    SelfCancellation,
    CancelingAnAdministrator,
    CancellationHasExpired
}
public enum CallStatusInProgress
{
    InProgress, // Currently being handled
    AtRisk      // Close to the maximum allowed time
}
/// <summary>
/// Types of calls.
/// </summary>
//public enum TypeOfReading
//{
//    None,
//    Regular,
//    Emergency,
//    HighPriority
//}
/// <summary>
/// Specifies the fields by which a list of volunteers can be sorted.
/// </summary>
public enum VolunteerSortBy
{id,
    FullName,
    TotalHandledCalls,
    TotalCanceledCalls,
    TotalExpiredCalls
}
/// <summary>
/// Possible statuses of a call.
/// </summary>
public enum CallStatus
{
    Open,
    InProgress,
    Closed,
    Expired,
    OpenAtRisk,
    InProgressAtRisk
}

/// <summary>
/// Enumeration for call completion types.
/// </summary>
public enum CallCompletionType
{
    Completed,
    Canceled,
    Expired
}


/// <summary>
/// Enum for specifying the field by which calls can be sorted.
/// </summary>
/// <summary>
/// Fields for sorting and filtering calls.
/// </summary>
public enum CallField
{
    AssignmentId,        // ID of the assignment
    CallId,              // ID of the call
    CallType,            // Type of the call
    OpeningTime,         // Time the call was opened
    RemainingTime,       // Remaining time to complete the call
    LastVolunteerName,   // Name of the last assigned volunteer
    CompletionTime,      // Time taken to complete the call
    CallStatus,          // Status of the call
    TotalAssignments     // Total number of assignments for the call
}

/// <summary>
/// Enumeration representing the time units for advancing the system clock.
/// </summary>
public enum TimeUnit
{
    Minute,
    Hour,
    Day,
    Month,
    Year
}
/// <summary>
/// Represents the fields available for sorting in an open call.
/// </summary>
public enum OpenCallField
{
    Id,                     // Unique identifier for the call
    Type,                   // Type of the call
    Description,            // A textual description of the call
    FullAddress,            // The full address where the call is located
    OpenTime,               // The time when the call was opened
    MaxEndTime,             // The deadline for completing the call
    DistanceFromVolunteer   // Computed distance from the volunteer's location
}
/// <summary>
/// Represents the fields available for sorting in a closed call.
/// </summary>
public enum ClosedCallField
{
    Id,                     // Unique identifier for the call
    CallType,               // Type of the call
    FullAddress,            // Full address of the call
    OpenTime,               // The time the call was opened
    AssignmentEntryTime,    // The time the call was assigned to a volunteer
    ActualEndTime,          // The actual time the call was closed
    EndType                 // Type of call closure (e.g., handled, cancelled, expired)
}


