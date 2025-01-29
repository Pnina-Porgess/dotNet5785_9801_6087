
using System;

namespace BO

{
    public class CallAssignInList
    {
        public int? VolunteerId { get; set; } // ID of the volunteer assigned to the call, nullable if artificial.
        public string? VolunteerName { get; set; } // Name of the volunteer, nullable if artificial.
        public DateTime AssignmentStartTime { get; set; } // Time the assignment started, cannot be null.
        public DateTime? AssignmentEndTime { get; set; } // Actual end time of the assignment, nullable if not ended.
        public CallCompletionType? CompletionType { get; set; } // Type of assignment completion, nullable for open assignments.
    }
}
