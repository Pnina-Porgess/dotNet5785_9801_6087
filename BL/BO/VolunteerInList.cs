using DO;

namespace BO;

/// <summary>
/// Represents a volunteer displayed in a list of volunteers, used in management screens.
/// </summary>
public class VolunteerInList
{
    public int Id { get; init; } // Volunteer ID. Taken from DO.Volunteer.

    public string FullName { get; set; } // Full name (first and last). Taken from DO.Volunteer.

    public bool IsActive { get; set; } // Indicates whether the volunteer is active. Taken from DO.Volunteer.

    public int TotalHandledCalls { get; set; } // Total number of calls handled by the volunteer. Calculated from DO.Assignment where the end status is "Handled."

    public int TotalCancelledCalls { get; set; } // Total number of calls cancelled by the volunteer. Calculated from DO.Assignment where the end status is "Cancelled."

    public int TotalExpiredCalls { get; set; } // Total number of calls expired while under the volunteer's responsibility. Calculated from DO.Assignment where the end status is "Expired."

    public int? CurrentCallId { get; set; } // The ID of the call currently being handled by the volunteer, if any. Found in DO.Assignment where actual end time is null.

    public TypeOfReading CurrentCallType { get; set; } // Type of the call currently being handled by the volunteer. If no call is being handled, the value is None.

}



