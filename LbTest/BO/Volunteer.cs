using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.BO
{

    /// <summary>
    /// Logical data entity representing a volunteer.
    /// </summary>
    class Volunteer
    {
        int Id { get; init; } // Volunteer ID - Unique and immutable after initialization.

        string FullName { get; } // Full name (first and last name).

        string Phone { get; set; } // Mobile phone number.

        string Email { get; set; } // Email address.

        string? Password { get; set; } // Password (optional, encrypted).

        string? CurrentAddress { get; set; } // Current full address of the volunteer.

        double? Latitude { get; set; } // Latitude - used for distance calculations only.

        double? Longitude { get; set; } // Longitude - used for distance calculations only.

        Role Role { get; set; } // Role (Admin/Volunteer).

        bool IsActive { get; set; } // Indicates if the volunteer is active.

        double? MaxDistance { get; set; } // Maximum distance for handling calls.

        DistanceType DistanceType { get; set; } // Type of distance (Air/Walking/Driving).

        int TotalHandledCalls { get; } // Total number of calls handled (view only).

        int TotalCanceledCalls { get;  } // Total number of calls canceled (view only).

        int TotalExpiredCalls { get;  } // Total number of expired calls (view only).

        BO.CallInProgress? CurrentCall { get; set; } // The call currently being handled by the volunteer (optional).
    }


    /// <summary>
    /// Representation of a call currently in progress handled by a volunteer.
    /// </summary>
    class CallInProgress
    {
        // Properties will be defined according to project specifications for CallInProgress.
    }

}
