using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DO;

/// <summary>
/// 
/// </summary>
/// <param name="Id"></param>
/// <param name="Name"></param>
/// <param name="Phone"></param>
/// <param name="Email"></param>
/// <param name="Password"></param>
/// <param name="Adrass"></param>
/// <param name="Latitude"></param>
/// <param name="Longitude"></param>
/// <param name="Job"></param>
/// <param name="IsActive"></param>
/// <param name="MaximumDistance"></param>
/// <param name="DistanceType"></param>
public record Volunteer
(
    int Id,
    string Name,
    string Phone,
    string Email,
    Role Role,
    bool IsActive,
    DistanceType DistanceType,
    double? MaximumDistance = null,
    string? Password = null,
    string? Adress = null,
    double? Latitude = null,
    double? Longitude = null
)
{
public Volunteer(): this(0, "", "", "", Role.Volunteer,false, DistanceType.AerialDistance) { }
}
