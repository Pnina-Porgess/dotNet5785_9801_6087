namespace DO;
/// <summary>
/// Assignment Entity represents a link between a call and a volunteer
/// </summary>
/// <param name="Id">Unique identifier for the assignment entity</param>
/// <param name="CallId">The ID of the call being handled</param>
/// <param name="VolunteerId">The ID of the volunteer handling the call</param>
/// <param name="StartDateTime">The time the call was assigned to the volunteer</param>
/// <param name="EndDateTime">The time the volunteer finished handling the call (nullable)</param>
/// <param name="CompletionType">Enum indicating how the call was completed (nullable)</param>
/// 
public record Assignment
(
    int Id,
    int CallId,
    int VolunteerId,
    TypeOfEndTime TypeOfEndTime,
    DateTime EntryTime,
    DateTime? EndTime=null
)

{
public Assignment() : this(0,0 , 0 , TypeOfEndTime.treated, DateTime.Now) { }
}
