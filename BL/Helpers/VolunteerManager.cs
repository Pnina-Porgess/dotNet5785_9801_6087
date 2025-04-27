
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
            var volunteerInList = volunteers.Select(static v =>
          {
              var volunteerAssignments = s_dal.Assignment.ReadAll(a => a.VolunteerId == v.Id);
              var assignedResponseId = volunteerAssignments.FirstOrDefault(a => a?.EntryTime == null)?.CallId;
              return new BO.VolunteerInList
              {
                  Id = v.Id,
                  FullName = v.Name,
                  IsActive = v.IsActive,
                  TotalHandledCalls = volunteerAssignments.Count(a => a.TypeOfEndTime == DO.TypeOfEndTime.treated), // חישוב מספר השיחות שנפגעו
                  TotalCancelledCalls = volunteerAssignments.Count(a => a.TypeOfEndTime == DO.TypeOfEndTime.SelfCancellation),  // חישוב מספר השיחות שבוטלו
                  TotalExpiredCalls = volunteerAssignments.Count(a => a.TypeOfEndTime == DO.TypeOfEndTime.CancellationHasExpired),  // חישוב מספר השיחות שזמן ההגשה שלהן פג
                  CurrentCallId = assignedResponseId,
                  CurrentCallType = (BO.TypeOfReading)(assignedResponseId.HasValue
                        ? (BO.TypeOfReading)(s_dal.Call.Read(assignedResponseId.Value)?.TypeOfReading)!
                        : BO.TypeOfReading.None)
              };
          }).ToList();
            return volunteerInList;
        }
      
        catch (Exception ex)
        {
            throw new BO.BlDatabaseException("An error occurred while retrieving closed calls", ex);
        }

     
    }

    /// <summary>
    /// Checks if the password meets the strength requirements.
    /// </summary>
    /// <param name="password">The password to validate.</param>
    /// <returns>True if the password is strong; otherwise, false.</returns>
    internal static bool IsPasswordStrong(string password)
    {
        if (password.Length < 8)
            return false;
        if (!password.Any(char.IsUpper))
            return false;
        if (!password.Any(char.IsLower))
            return false;
        if (!password.Any(char.IsDigit))
            return false;
        if (!password.Any(c => "@#$%^&*".Contains(c)))
            return false;
        return true;
    }

    /// <summary>
    /// Validates the format of a volunteer's input.
    /// </summary>
    /// <param name="boVolunteer">The volunteer object to validate.</param>
    /// <exception cref="BO.BlInvalidInputException">Thrown if any input field is invalid.</exception>
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

        //if (boVolunteer?.Password?.Length < 6 || !VolunteerManager.IsPasswordStrong(boVolunteer?.Password!))
        //    throw new BO.BlInvalidInputException("Password is too weak. It must have at least 6 characters, including uppercase, lowercase, and numbers.");
    }

    /// <summary>
    /// Validates an Israeli ID using a checksum algorithm.
    /// </summary>
    /// <param name="id">The ID to validate.</param>
    /// <returns>True if the ID is valid; otherwise, false.</returns>
    internal static bool IsValidId(int id)
    {
        string idString = id.ToString();
        if (idString.Length != 9)
            return false;
        int sum = 0;

        for (int i = 0; i < 9; i++)
        {
            int digit = int.Parse(idString[i].ToString());

            if (i % 2 == 1)
                digit *= 2;

            if (digit > 9)
                digit -= 9;

            sum += digit;
        }

        return sum % 10 == 0;
    }

    /// <summary>
    /// Encrypts a password using SHA256.
    /// </summary>
    /// <param name="password">The password to encrypt.</param>
    /// <returns>The encrypted password as a Base64 string.</returns>
    internal static string EncryptPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256?.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes!);
    }

    /// <summary>
    /// Creates a DO.Volunteer object from a BO.Volunteer object.
    /// </summary>
    /// <param name="boVolunteer">The business object representing the volunteer.</param>
    /// <returns>A data object representing the volunteer.</returns>
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
            EncryptPassword(boVolunteer.Password!),
            boVolunteer.CurrentAddress,
            boVolunteer.Latitude,
            boVolunteer.Longitude
    

        );
    }

    /// <summary>
    /// Performs logical checks on the volunteer's data.
    /// </summary>
    /// <param name="boVolunteer">The volunteer object to check.</param>
    /// <returns>The volunteer's coordinates.</returns>
    /// <exception cref="BO.BlLogicalException">Thrown if the ID is invalid.</exception>
    internal static (double? Latitude, double? Longitude) LogicalChecking(BO.Volunteer boVolunteer)
    {
        if (!IsValidId(boVolunteer.Id))
            throw new BO.BlLogicalException("The ID is not correct");
        (double? r, double? w) = Tools.GetCoordinatesFromAddress(boVolunteer.CurrentAddress!);
        return (r,w);
    }

    /// <summary>
    /// Validates permissions for performing an action on a volunteer.
    /// </summary>
    /// <param name="requesterId">The ID of the requester.</param>
    /// <param name="boVolunteer">The volunteer object.</param>
    /// <exception cref="BO.BlUnauthorizedAccessException">Thrown if permissions are insufficient.</exception>
    internal static void ValidatePermissions(int requesterId, BO.Volunteer boVolunteer)
    {
        if (!(requesterId == boVolunteer.Id) && !(boVolunteer.Role == BO.Role.Manager))
            throw new BO.BlUnauthorizedAccessException("Only an admin or the volunteer themselves can perform this update.");
    }

    /// <summary>
    /// Checks if fields can be updated based on the original volunteer's role.
    /// </summary>
    /// <param name="original">The original volunteer object.</param>
    /// <param name="boVolunteer">The new volunteer object.</param>
    /// <returns>True if the update is allowed; otherwise, false.</returns>
    internal static bool CanUpdateFields(DO.Volunteer original, BO.Volunteer boVolunteer)
    {
        if ((BO.Role)original.Role != boVolunteer.Role)
        {
            if (boVolunteer.Role != BO.Role.Manager)
                return false;
        }
        return true;
    }

    /// <summary>
    /// Calculates the status of a call based on the time remaining.
    /// </summary>
    /// <param name="call">The call object.</param>
    /// <param name="riskThreshold">The threshold in minutes for at-risk status.</param>
    /// <returns>The status of the call.</returns>
    public static BO.CallStatusInProgress CalculateStatus(DO.Call call, int riskThreshold = 30)
    {

        var timeToEnd = call.MaxTimeToFinish - ClockManager.Now;
        if (timeToEnd?.TotalMinutes <= riskThreshold)
        {
            return BO.CallStatusInProgress.AtRisk;
        }
        return BO.CallStatusInProgress.InProgress;
    }
}


