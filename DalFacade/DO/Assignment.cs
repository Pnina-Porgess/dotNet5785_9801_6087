namespace DO;
/// <summary>
/// 
/// </summary>
/// <param name="Id"></param>
/// <param name="CallId"></param>
/// <param name="VolunteerId"></param>
/// <param name="EntryTime"></param>
/// <param name="EndTime"></param>
/// <param name="TypeOfEndTime"></param>

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
public Assignment() : this(0, 0 , 0 , TypeOfEndTime.treated, DateTime.Now) { }
}
