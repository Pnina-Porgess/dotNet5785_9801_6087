using System;

namespace DO;

public record Assignment
(
    int Id,
    int CallId,
    int VolunteerId,
    DateTime EntryTime,
    DateTime? EndTime=null,
    Enum? TypeOfTreatment=null,
 

);
