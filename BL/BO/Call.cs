﻿
using System;

namespace BO
{
    /// <summary>
    /// Logical data entity representing a call in the system.
    /// </summary>
    public class Call
    {

        public int Id { get; init; }// Unique identifier for the call.
        public TypeOfReading Type { get; set; }// Type of the call (enumeration).
        public string? Description { get; set; }// Textual description of the call.
        public string Address { get; set; }// Full address of the call.
        public double Latitude { get; set; }// Latitude coordinate of the address.
        public double Longitude { get; set; }// Longitude coordinate of the address.
        public DateTime OpeningTime { get; init; }// The time when the call was opened.
        public DateTime? MaxEndTime { get; set; }// Maximum allowed time for completing the call.
        public CallStatus Status { get; set; }// Current status of the call (enumeration).
        public List<CallAssignInList>? Assignments { get; set; }// List of assignments related to the call.

        public override string ToString()
        {
            return $"Call ID: {Id}\n" +
                   $"Type: {Type}\n" +
                   $"Description: {Description ?? "No description"}\n" +
                   $"Address: {Address}\n" +
                   $"Coordinates: ({Latitude}, {Longitude})\n" +
                   $"Opening Time: {OpeningTime}\n" +
                   $"Max End Time: {(MaxEndTime.HasValue ? MaxEndTime.Value.ToString() : "N/A")}\n" +
                   $"Status: {Status}\n" +
                   $"Assignments: {(Assignments != null && Assignments.Count > 0 ? string.Join(", ", Assignments) : "No assignments")}";
        }

    }
}
