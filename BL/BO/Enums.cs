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

