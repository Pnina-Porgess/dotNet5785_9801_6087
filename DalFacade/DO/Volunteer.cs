using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DO;

/// <summary>
/// Creates a new volunteer
/// </summary>
/// <param name="Id">Personal unique ID of the Volunteer (as in national id card)</param>
/// <param name="Name">Private Name of the Volunteer</param>
/// <param name="Phone">Phone number of the Volunteer</param>
/// <param name="Email">Email number of the Volunteer</param>
/// <param name="Password">Password of the Volunteer</param>
/// <param name="Adress">A complete and true address in the correct format of the volunteer.</param>
///  <param name="Latitude">Geographic latitude of the volunteer (nullable)</param>
///  <param name="Longitude">Geographic longitude of the volunteer (nullable)</param>
/// <param name="Role">Manager or volunteer</param>
/// <param name="IsActive">Is the volunteer active or inactive?</param>
/// <param name="MaximumDistance">The maximum distance to receive a assigment.</param>
/// <param name="DistanceType">Distance type: Air distance, walking distance, driving distance</param>
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
