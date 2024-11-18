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
    string? Password,
    string? Adrass,
    double? Latitude,
    double? Longitude,
    Enum Job,
    bool IsActive,
    double? MaximumDistance,
    Enum DistanceType
)
