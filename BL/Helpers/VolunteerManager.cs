
using DalApi;
using System.Security.Cryptography;
using System.Text;

namespace Helpers;
internal static class VolunteerManager
{
    private static IDal s_dal = Factory.Get; //stage 4
    //הופך מתנדב מDO לBO
    internal static BO.Volunteer MapVolunteer(DO.Volunteer volunteer)
    {
        return new BO.Volunteer
        {
            Id = volunteer.Id,
            FullName = volunteer.Name,
            Phone = volunteer.Phone,
            Email = volunteer.Email,
            IsActive = volunteer.IsActive,
           Role = (BO.Role)volunteer.Role,

        };
    }
    internal static bool VerifyPassword(string enteredPassword, string storedPassword)
    {
        var encryptedPassword = EncryptPassword(enteredPassword);
        return encryptedPassword == storedPassword;
    }
    internal static IEnumerable<BO.VolunteerInList> GetVolunteerList(IEnumerable<DO.Volunteer> volunteers)
    {
        if (volunteers is null)
        {
            throw new ArgumentNullException(nameof(volunteers));
        }

        var volunteerInList = volunteers.Select(static v =>
          {
              var volunteerAssignments = s_dal.Assignment.ReadAll(a => a.VolunteerId == v.Id);
              var currentAssignment = volunteerAssignments.FirstOrDefault(a => a?.EntryTime == null);
              var assignedResponseId = currentAssignment?.CallId;
              return new BO.VolunteerInList
              {
                  Id = v.Id,
                  FullName = v.Name, // ניתן לשנות את השם המלא אם יש צורך
                  IsActive = v.IsActive,
                  TotalHandledCalls = volunteerAssignments.Count(a => a.TypeOfEndTime == DO.TypeOfEndTime.treated), // חישוב מספר השיחות שנפגעו
                  TotalCancelledCalls = volunteerAssignments.Count(a => a.TypeOfEndTime == DO.TypeOfEndTime.SelfCancellation),  // חישוב מספר השיחות שבוטלו
                  TotalExpiredCalls = volunteerAssignments.Count(a => a.TypeOfEndTime == DO.TypeOfEndTime.CancellationHasExpired),  // חישוב מספר השיחות שזמן ההגשה שלהן פג
                  CurrentCallId = assignedResponseId,  // אם יש קריאה בשטח, נרצה להחזיר את מזהה הקריאה
                  CurrentCallType = (BO.TypeOfReading)(assignedResponseId.HasValue
                        ? (BO.CallType)(s_dal.Call.Read(assignedResponseId.Value)?.TypeOfReading ?? DO.TypeOfReading.None)
                        : BO.CallType.None) // אם אין קריאה, נחזיר None
              };
          }).ToList();
       return volunteerInList;

    }
    //ססמא חזקה
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
    //שדות מלאים ותקינים
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

        if (boVolunteer?.Password?.Length < 6 || !VolunteerManager.IsPasswordStrong(boVolunteer?.Password!))
            throw new BO.BlInvalidInputException("Password is too weak. It must have at least 6 characters, including uppercase, lowercase, and numbers.");
    }
//תז תקינה
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
    //הצפנת הססמא
    internal static string EncryptPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256?.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes!);
    }
    //יוצר VOLUNTEER מעביר מBO TO DO
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
    //בודק שבססמא חזקה ומחזיר כתובת בקווי אורך ורוחב
    internal static (double? Latitude, double? Longitude) logicalChecking(BO.Volunteer boVolunteer)
    {
        IsPasswordStrong(boVolunteer.Password!);
        return Tools.GetCoordinatesFromAddress(boVolunteer.CurrentAddress!);
    }

    //הרשאות למי שמוסמך
    internal static void ValidatePermissions(int requesterId, BO.Volunteer boVolunteer)
    {
        if (!(requesterId == boVolunteer.Id) && !(boVolunteer.Role == BO.Role.Manager))
            throw new UnauthorizedAccessException("Only an admin or the volunteer themselves can perform this update.");

        if (boVolunteer.Role != BO.Role.Manager && boVolunteer.Role !=BO.Role.Volunteer)
            throw new UnauthorizedAccessException("Only an admin can update the volunteer's role.");
    }
    internal static bool CanUpdateFields(int requesterId, DO.Volunteer original, BO.Volunteer boVolunteer)
    {
    
        if ((BO.Role)original.Role != boVolunteer.Role)
        {
            if (boVolunteer.Role != BO.Role.Manager|| requesterId!= original.Id)
                return false;
        }
        return true;
    }
}


