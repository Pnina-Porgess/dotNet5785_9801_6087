using BlImplementation;
using DalApi;
using System.Security.Cryptography;
using System.Text;
namespace Helpers;
internal static class VolunteerManager
{
    private static IDal s_dal = Factory.Get; //stage 4
    internal static ObserverManager Observers = new();

    /// <summary>
    /// Retrieves a list of volunteers and their statistics.
    /// </summary>
    /// <param name="volunteers">A collection of DO.Volunteer objects.</param>
    /// <returns>A list of BO.VolunteerInList objects with volunteer details and statistics.</returns>
    internal static IEnumerable<BO.VolunteerInList> GetVolunteerList(IEnumerable<DO.Volunteer> volunteers)
    {
        try
        {
            var volunteerInList = volunteers.Select(v =>
            {
                List<DO.Assignment> volunteerAssignments;
                DO.Call? currentCall = null;
                int? assignedResponseId;

                lock (AdminManager.BlMutex)
                {
                    volunteerAssignments = s_dal.Assignment.ReadAll(a => a.VolunteerId == v.Id).ToList();
                    assignedResponseId = volunteerAssignments.FirstOrDefault()?.CallId;

                    if (assignedResponseId.HasValue)
                    {
                        currentCall = s_dal.Call.Read(assignedResponseId.Value);
                    }
                }

                return new BO.VolunteerInList
                {
                    Id = v.Id,
                    FullName = v.Name,
                    IsActive = v.IsActive,
                    TotalHandledCalls = volunteerAssignments.Count(a => a.TypeOfEndTime == DO.TypeOfEndTime.treated),
                    TotalCancelledCalls = volunteerAssignments.Count(a => a.TypeOfEndTime == DO.TypeOfEndTime.SelfCancellation),
                    TotalExpiredCalls = volunteerAssignments.Count(a => a.TypeOfEndTime == DO.TypeOfEndTime.CancellationHasExpired),
                    CurrentCallId = assignedResponseId,
                    CurrentCallType = assignedResponseId.HasValue && currentCall != null
                        ? (BO.TypeOfReading)currentCall.TypeOfReading
                        : BO.TypeOfReading.None
                };
            }).ToList();

            return volunteerInList;
        }
        catch (Exception ex)
        {
            throw new BO.BlDatabaseException("An error occurred while retrieving closed calls", ex);
        }
    }

    internal static bool IsPasswordStrong(string password)
    {
        if (password.Length < 8) return false;
        if (!password.Any(char.IsUpper)) return false;
        if (!password.Any(char.IsLower)) return false;
        if (!password.Any(char.IsDigit)) return false;
        if (!password.Any(c => "@#$%^&*".Contains(c))) return false;
        return true;
    }

    internal static void ValidateInputFormat(BO.Volunteer boVolunteer)
    {
        if (boVolunteer == null)
            throw new BO.BlNotFoundException("Volunteer object cannot be null.");

        if (!System.Text.RegularExpressions.Regex.IsMatch(boVolunteer.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            throw new BO.BlInvalidInputException("Invalid email format.");

        if (boVolunteer.Id < 0 || !IsValidId(boVolunteer.Id))
            throw new BO.BlInvalidInputException("Invalid ID format. ID must be a valid number with a correct checksum.");

        if (!System.Text.RegularExpressions.Regex.IsMatch(boVolunteer.Phone, @"^\d{10}$"))
            throw new BO.BlInvalidInputException("Invalid phone number format. Phone number must have 10 digits.");

        if (boVolunteer.FullName.Length < 2)
            throw new BO.BlInvalidInputException("Volunteer name is too short. Name must have at least 2 characters.");
    }

    internal static void ValidatePassword(BO.Volunteer volunteer)
    {
        if (string.IsNullOrEmpty(volunteer.Password) || volunteer.Password.Length < 6 || !VolunteerManager.IsPasswordStrong(volunteer.Password))
            throw new BO.BlInvalidInputException("Password is too weak. It must have at least 6 characters, including uppercase, lowercase, and numbers.");
    }

    internal static bool IsValidId(int id)
    {
        string idString = id.ToString();
        if (idString.Length != 9) return false;
        int sum = 0;

        for (int i = 0; i < 9; i++)
        {
            int digit = int.Parse(idString[i].ToString());
            if (i % 2 == 1) digit *= 2;
            if (digit > 9) digit -= 9;
            sum += digit;
        }

        return sum % 10 == 0;
    }

    internal static string EncryptPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256?.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes!);
    }

    internal static DO.Volunteer CreateDoVolunteer(BO.Volunteer boVolunteer)
    {
        return new DO.Volunteer(
            boVolunteer.Id,
            boVolunteer.FullName,
            boVolunteer.Phone,
            boVolunteer.Email,
            (DO.Role)boVolunteer.Role,
            boVolunteer.IsActive,
            (DO.DistanceType)boVolunteer.DistanceType,
            boVolunteer.MaxDistance,
            boVolunteer.Password!,
            boVolunteer.CurrentAddress,
            boVolunteer.Latitude,
            boVolunteer.Longitude
        );
    }
    internal static async Task<(double? Latitude, double? Longitude)> LogicalCheckingAsync(BO.Volunteer boVolunteer)
    {
        if (!IsValidId(boVolunteer.Id))
            throw new BO.BlLogicalException("The ID is not correct");
        var result = await Tools.GetCoordinatesFromAddressAsync(boVolunteer.CurrentAddress!);
        return (result?.Latitude, result?.Longitude);
    }

    internal static async Task UpdateVolunteerCoordinatesAsync(DO.Volunteer doVolunteer)
    {
        if (!string.IsNullOrEmpty(doVolunteer.Address))
        {
            var coordinates = await Tools.GetCoordinatesFromAddressAsync(doVolunteer.Address);
            if (coordinates is not null)
            {
                var (lat, lon) = coordinates.Value;

                doVolunteer = doVolunteer with { Latitude = lat, Longitude = lon };
                lock (AdminManager.BlMutex)
                    s_dal.Volunteer.Update(doVolunteer);

                CallManager.Observers.NotifyListUpdated();
                CallManager.Observers.NotifyItemUpdated(doVolunteer.Id);
            }
        }
    }

    internal static void ValidatePermissions(int requesterId, BO.Volunteer boVolunteer)
    {
        if (!(requesterId == boVolunteer.Id) && !(boVolunteer.Role == BO.Role.Manager))
            throw new BO.BlUnauthorizedAccessException("Only an admin or the volunteer themselves can perform this update.");
    }

    internal static bool CanUpdateFields(DO.Volunteer original, DO.Volunteer boVolunteer)
    {
        if ((BO.Role)original.Role != (BO.Role)boVolunteer.Role)
        {
            if ((BO.Role)boVolunteer.Role != BO.Role.Manager)
                return false;
        }
        return true;
    }

    public static BO.CallStatusInProgress CalculateStatus(DO.Call call)
    {
        TimeSpan timeToEnd = (TimeSpan)(call.MaxTimeToFinish - AdminManager.Now)!;
        if (timeToEnd <= AdminManager.RiskRange)
        {
            return BO.CallStatusInProgress.AtRisk;
        }
        return BO.CallStatusInProgress.InProgress;
    }

    internal static void SimulateVolunteerAssignmentsAndCallHandling()
    {
        Thread.CurrentThread.Name = $"Simulator{Thread.CurrentThread.ManagedThreadId}";

        List<int> updatedVolunteerIds = new();
        List<int> updatedCallIds = new();

        List<DO.Volunteer> activeVolunteers;
        lock (AdminManager.BlMutex)
            activeVolunteers = s_dal.Volunteer.ReadAll(v => v.IsActive).ToList();

        foreach (var volunteer in activeVolunteers)
        {
            DO.Assignment? currentAssignment;

            lock (AdminManager.BlMutex)
            {
                currentAssignment = s_dal.Assignment
                    .ReadAll(a => a.VolunteerId == volunteer.Id && a.EndTime == null)
                    .FirstOrDefault();
            }

            if (currentAssignment == null)
            {
                List<BO.OpenCallInList> openCalls;
                lock (AdminManager.BlMutex)
                    openCalls = new CallImplementation().GetOpenCallsForVolunteer(volunteer.Id).ToList();

                if (!openCalls.Any() || Random.Shared.NextDouble() > 0.2) continue;
                if (openCalls.Any())
                {
                    var selectedCall = openCalls[Random.Shared.Next(openCalls.Count)];
                    try
                    {
                        new CallImplementation().SelectCallForTreatment(volunteer.Id, selectedCall.Id);
                        updatedVolunteerIds.Add(volunteer.Id);
                        updatedCallIds.Add(selectedCall.Id);
                    }
                    catch { continue; }
                }// במקרה של בעיה עם הקריאה
            }
            else
            {
                DO.Call? call;
                lock (AdminManager.BlMutex)
                    call = s_dal.Call.Read(currentAssignment.CallId);

                if (call is null) continue;

                double distance = Tools.CalculateDistance(volunteer.Latitude!, volunteer.Longitude!, call.Latitude, call.Longitude);
                TimeSpan baseTime = TimeSpan.FromMinutes(distance * 2);
                TimeSpan extra = TimeSpan.FromMinutes(Random.Shared.Next(1, 5));
                TimeSpan totalNeeded = baseTime + extra;
                TimeSpan actual = AdminManager.Now - currentAssignment.EntryTime;

                if (actual >= totalNeeded)
                {
                    try
                    {
                        new CallImplementation().CompleteCallTreatment(volunteer.Id, currentAssignment.Id);
                        updatedVolunteerIds.Add(volunteer.Id);
                        updatedCallIds.Add(call.Id);
                    }
                    catch { continue; }
                }
                else if (Random.Shared.NextDouble() < 0.1)
                {
                    try
                    {
                        new CallImplementation().CancelCallTreatment(volunteer.Id, currentAssignment.Id);
                        updatedVolunteerIds.Add(volunteer.Id);
                        updatedCallIds.Add(call.Id);
                    }
                    catch { continue; }
                }
            }
        }

        foreach (var id in updatedVolunteerIds.Distinct())
            VolunteerManager.Observers.NotifyItemUpdated(id);
        foreach (var id in updatedCallIds.Distinct())
            CallManager.Observers.NotifyItemUpdated(id);
    }
}
