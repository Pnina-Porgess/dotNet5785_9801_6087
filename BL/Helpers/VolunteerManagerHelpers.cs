using BO;
using DalApi;

using DO;
using Helpers;


internal static class VolunteerManagerHelpers
{
    private static IDal s_dal = Factory.Get; //stage 4
    public static IEnumerable<BO.VolunteerInList> GetVolunteerList(IEnumerable<DO.Volunteer> volunteers)
    {
        var volunteerInList = volunteers.Select(static v =>
          {
              var volunteerAssignments = s_dal.Assignment.ReadAll(a => a.VolunteerId == v.Id);
              var currentAssignment = volunteerAssignments.FirstOrDefault(a => a.EntryTime == null);
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
                  CurrentCallType = (TypeOfReading)(assignedResponseId.HasValue
                        ? (BO.CallType)(s_dal.Call.Read(assignedResponseId.Value)?.TypeOfReading ?? DO.TypeOfReading.None)
                        : BO.CallType.None) // אם אין קריאה, נחזיר None
              };
          }).ToList();

    }
    public static IEnumerable <BO.VolunteerInList> GetVolunteerList(IEnumerable<DO.Volunteer> volunteers)
    {
        var volunteerInList = volunteers.Select(static v =>
          {
              var volunteerAssignments = s_dal.Assignment.ReadAll(a => a.VolunteerId == v.Id);
              var currentAssignment = volunteerAssignments.FirstOrDefault(a => a.EntryTime == null);
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
                  CurrentCallType = (TypeOfReading)(assignedResponseId.HasValue
                        ? (BO.CallType)(s_dal.Call.Read(assignedResponseId.Value)?.TypeOfReading ?? DO.TypeOfReading.None)
                        : BO.CallType.None) // אם אין קריאה, נחזיר None
              };
          }).ToList();

    }   
    public static IEnumerable <BO.VolunteerInList> GetVolunteerList(IEnumerable<DO.Volunteer> volunteers)
    {
        var volunteerInList = volunteers.Select(static v =>
          {
              var volunteerAssignments = s_dal.Assignment.ReadAll(a => a.VolunteerId == v.Id);
              var currentAssignment = volunteerAssignments.FirstOrDefault(a => a.EntryTime == null);
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
                  CurrentCallType = (TypeOfReading)(assignedResponseId.HasValue
                        ? (BO.CallType)(s_dal.Call.Read(assignedResponseId.Value)?.TypeOfReading ?? DO.TypeOfReading.None)
                        : BO.CallType.None) // אם אין קריאה, נחזיר None
              };
          }).ToList();

    }   
}