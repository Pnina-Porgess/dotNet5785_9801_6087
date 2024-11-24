
namespace DO;
/// <summary>
/// 
/// </summary>
/// <param name="Id"></param>
/// <param name="Description"></param>
/// <param name="Adress"></param>
/// <param name="Longitude"></param>
/// <param name="Latitude"></param>
/// <param name="TimeOfOpen"></param>
/// <param name="MaxTimeToFinish"></param>

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
);
public call() : this{ }





