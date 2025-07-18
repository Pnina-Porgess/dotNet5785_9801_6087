﻿
using Microsoft.VisualBasic;
using System;

namespace BO
{

    /// <summary>
    /// Represents a call currently being handled by a volunteer, for display in relevant screens.
    /// </summary>
    public class CallInProgress
    {
        public int Id { get; init; } // Identifier of the assignment entity. Found by searching in DO.Assignment using the volunteer ID and appropriate call status.

        public int CallId { get; init; } // Sequential identifier of the call. Taken from DO.Assignment.

        public TypeOfReading CallType { get; set; } // The type of the call. Taken from DO.Call.

        public string? Description { get; set; } // A textual description of the call. Taken from DO.Call.

        public string Address { get; set; } // Full address of the call. Taken from DO.Call.

        public DateTime OpenTime { get; set; } // The opening time of the call. Taken from DO.Call.

        public DateTime? MaxEndTime { get; set; } // The maximum time for resolving the call. Taken from DO.Call.

        public DateTime StartTime { get; set; } // The time when the volunteer started handling the call. Taken from DO.Assignment.

        public double DistanceFromVolunteer { get; set; } // The distance of the call from the current location of the volunteer. Calculated in the logic layer.

        public CallStatusInProgress Status { get; set; } // The current status of the call. Possible values: "In Progress" or "At Risk".
        public override string ToString()
        {
            return $"Assignment ID: {Id}\n" +
                   $"Call ID: {CallId}\n" +
                   $"Call Type: {CallType}\n" +
                   $"Description: {(!string.IsNullOrEmpty(Description) ? Description : "None")}\n" +
                   $"Call Address: {Address}\n" +
                   $"Opening Time: {OpenTime}\n" +
                   $"Maximum Resolution Time: {(MaxEndTime.HasValue ? MaxEndTime.ToString() : "None")}\n" +
                   $"Start Handling Time: {StartTime}\n" +
                   $"Distance from Volunteer: {DistanceFromVolunteer} km\n" +
                   $"Current Status: {Status}";
        }

    }

}


