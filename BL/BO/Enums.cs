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
public enum CallType
{
    None,
    Regular,
    Emergency,
    HighPriority
}
/// <summary>
/// Specifies the fields by which a list of volunteers can be sorted.
/// </summary>
public enum VolunteerSortBy
{
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

public enum CallField
{
    CallId,             // Sort by the unique identifier of the call
    Type,           // Sort by the type of the call
    Description,    // Sort by the description of the call
    Address,        // Sort by the address of the call
    OpeningTime,    // Sort by the opening time of the call
    MaxEndTime,     // Sort by the maximum end time
    Status,         // Sort by the current status of the call
    AssignmentId    // Sort by the assignment ID
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

