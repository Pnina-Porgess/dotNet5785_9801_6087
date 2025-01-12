
using DO;
using System;

namespace BO
{
    /// <summary>
    /// Represents a logical data entity for a volunteer.
    /// </summary>
    public class Volunteer    {
        public int Id { get; init; } /// Volunteer ID - Unique and immutable after initialization.

        public string FullName { get; set; }/// Full name of the volunteer (first and last name).
        public string Phone { get; set; }///Mobile phone number of the volunteer.
        public string Email { get; set; } /// Email address of the volunteer.
        public string? Password { get; set; }/// Password (optional, encrypted).
        public string? CurrentAddress { get; set; }  /// Current full address of the volunteer.
        public double? Latitude { get; set; } /// Latitude - used for distance calculations only.
        public double? Longitude { get; set; }/// Longitude - used for distance calculations only.
        public Role Role { get; set; } /// Role of the volunteer (Admin/Volunteer).
        public bool IsActive { get; set; }   /// Indicates whether the volunteer is active
        public double? MaxDistance { get; set; } /// Maximum distance for the volunteer to accept calls.
        public DistanceType DistanceType { get; set; } /// Type of distance calculation (Air/Walking/Driving).
        public int TotalHandledCalls { get; init; }  /// Total number of calls the volunteer has handled (read-only).
        public int TotalCanceledCalls { get; init; }    /// Total number of calls the volunteer has canceled (read-only).
        public int TotalExpiredCalls { get; init; } /// Total number of expired calls the volunteer attempted to handle (read-only).
        public CallInProgress? CurrentCall { get; set; }/// The call currently being handled by the volunteer (optional).
    }


}
