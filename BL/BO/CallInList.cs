namespace BO
{
    public class CallInList
    {
        public int? AssignmentId { get; set; } // Unique ID for the assignment, nullable if no assignment exists.
        public int CallId { get; set; } // Unique running ID for the call, cannot be null.
        public TypeOfReading CallType { get; set; } // Type of the call, cannot be null.
        public DateTime OpeningTime { get; set; } // The time the call was opened, cannot be null.
        public TimeSpan? RemainingTime { get; set; } // Remaining time until the call's maximum allowed completion, nullable if no limit exists.
        public string? LastVolunteerName { get; set; } // Name of the last assigned volunteer, nullable if no volunteer was ever assigned.
        public TimeSpan? CompletionTime { get; set; } // Total time to complete the call, nullable if the call is not completed.
        public CallStatus CallStatus { get; set; } // Status of the call (e.g., open, in progress, closed), cannot be null.
        public int TotalAssignments { get; set; } // Total number of assignments for the call, cannot be null.
        public override string ToString()
        {
            return $"Assignment ID: {(AssignmentId.HasValue ? AssignmentId.ToString() : "None")}\n" +
                   $"Call ID: {CallId}\n" +
                   $"Call Type: {CallType}\n" +
                   $"Opened At: {OpeningTime}\n" +
                   $"Remaining Time: {(RemainingTime.HasValue ? RemainingTime.ToString() : "None")}\n" +
                   $"Last Volunteer: {(string.IsNullOrEmpty(LastVolunteerName) ? "None" : LastVolunteerName)}\n" +
                   $"Completion Time: {(CompletionTime.HasValue ? CompletionTime.ToString() : "None")}\n" +
                   $"Call Status: {CallStatus}\n" +
                   $"Total Assignments: {TotalAssignments}";
        }
    }
}



