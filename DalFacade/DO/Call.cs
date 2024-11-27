
namespace DO;
/// <summary>
/// 
/// </summary>
/// <param name="Id">Id that uniquely identifies the call</param>
/// <param name="Description">Description of the reading. Detailed details about the reading.</param>
/// <param name="Adress">A complete and genuine address in the correct format,</param>
/// <param name="Longitude"></param>
/// <param name="Latitude"></param>
/// <param name="TimeOfOpen">Time (date and time) when the call was opened by the administrator.</param>
/// <param name="MaxTimeToFinish">Time (date and time) by which the call should close.</param>

public record Call
(
   int Id,
   TypeOfReading TypeOfReading,
   string? Description,
   string? Adress,
   double Longitude,
   double Latitude,
   DateTime TimeOfOpen,
   DateTime MaxTimeToFinish
)
{
    public Call() : this(0, TypeOfReading.sari, "", "", 0, 0, DateTime.Now, DateTime.Now) { }
}
;





