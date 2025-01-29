namespace BO;
public enum CallStatusEnum
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
/// Specifies the fields by which a list of volunteers can be sorted.
/// </summary>
public enum VolunteerSortBy
{
    Id,            
    FullName,     
    Phone,          
    Email,         
    Role,           
    IsActive,      
    MaxDistance,    
    TotalHandledCalls,  
    TotalCanceledCalls, 
    TotalExpiredCalls  
}

/// <summary>
/// Enum for specifying the field by which calls can be sorted.
/// </summary>
public enum CallField
{
    Id,             // Sort by the unique identifier of the call
    Type,           // Sort by the type of the call
    Description,    // Sort by the description of the call
    Address,        // Sort by the address of the call
    Latitude,       // Sort by the latitude coordinate
    Longitude,      // Sort by the longitude coordinate
    OpeningTime,    // Sort by the opening time of the call
    MaxEndTime,     // Sort by the maximum end time
    Status          // Sort by the current status of the call
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
//public enum Role
//{
//    Volunteer,
//    Manager
//}