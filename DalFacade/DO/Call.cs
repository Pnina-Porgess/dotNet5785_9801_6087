using System;

namespace DO;

public record Call
(
   int Id,
   Enum TypeOfReading,
   string? Description,
   string? Adress,
   double Longitude,
   double Latitude,
   DateTime TimeOfOpen,
   DateTime MaxTimeToFinish
    );



