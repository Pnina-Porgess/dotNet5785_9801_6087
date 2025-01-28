using BO;
using DalApi;
using DO;
using Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
namespace BlApi;

internal class VolunteerImplementation : IVolunteer
{
 private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void AddVolunteer(BO.Volunteer volunteer)
    {
        throw new NotImplementedException();
    }

    public void DeleteVolunteer(int id)
    {
        throw new NotImplementedException();
    }

    public BO.Volunteer GetVolunteerDetails(int volunteerId)
    {
        try
        {
            // קריאה לשכבת הנתונים (DO) על מנת לקבל פרטי המתנדב
            var volunteerDO = _dal.Volunteer.Read(volunteerId) ??
           throw new BO.BoDoesNotExistException($"Volunteer with ID={volunteerId} does not exist");
            var currentAssignment = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId && a.EndTime == null).FirstOrDefault();
            BO.CallInProgress? callInProgress = null;
            if (currentAssignment != null)
            {
                var callDetails = _dal.Call.Read(currentAssignment.CallId);
                if (callDetails != null)
                {
                    callInProgress = new BO.CallInProgress
                    {
                        Id = currentAssignment.Id,
                        CallId = currentAssignment.CallId,
                        CallType = callDetails.TypeOfReading,
                        Description = callDetails.Description,
                        Address = callDetails.Adress,
                        OpenTime = callDetails.TimeOfOpen,
                        MaxEndTime = callDetails.MaxTimeToFinish,
                        StartTime = currentAssignment.EntryTime,
                        DistanceFromVolunteer = Tools.CalculateDistance(volunteerDO.Latitude, volunteerDO.Longitude, callDetails.Latitude, callDetails.Longitude),
                        Status = Tools.CalculateStatus(currentAssignment, callDetails, 30)
                    };
                }
                // יצירת אובייקט BO.Volunteer והחזרת האובייקט
                var volunteerBO = new BO.Volunteer
                {
                    Id = volunteerDO.Id,
                    FullName = volunteerDO.Name,
                    Phone = volunteerDO.Phone,
                    Email = volunteerDO.Email,
                    Password = volunteerDO.Password,
                    CurrentAddress = volunteerDO.Adress,
                    Latitude = volunteerDO.Latitude,
                    Longitude = volunteerDO.Longitude,
                    Role = volunteerDO.Role,
                    IsActive = volunteerDO.IsActive,
                    MaxDistance = volunteerDO.MaximumDistance,
                    DistanceType = volunteerDO.DistanceType,
                    CurrentCall = callInProgress
                };

                return volunteerBO;
            }
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BoDoesNotExistException("Volunteer not found in data layer.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.GeneralDatabaseException("An unexpected error occurred while geting Volunteer details.", ex);
        }
    }




    public IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive = null, BO.VolunteerSortBy? sortBy = null)
    {

        try
        {
            IEnumerable<DO.Volunteer> volunteers = _dal.Volunteer.ReadAll(v =>
                !isActive.HasValue || v.IsActive == isActive.Value);

            var volunteerList = VolunteerManager.GetVolunteerList(volunteers);

            volunteerList = sortBy.HasValue ? sortBy.Value switch
            {
                BO.VolunteerSortBy.FullName => volunteerList.OrderBy(v => v.FullName).ToList(),
                BO.VolunteerSortBy.Phone => volunteerList.OrderByDescending(v => v.Phone).ToList(),
                BO.VolunteerSortBy.Email => volunteerList.OrderByDescending(v => v.Email).ToList(),
                BO.VolunteerSortBy.Role => volunteerList.OrderByDescending(v => v.Role).ToList(),
                BO.VolunteerSortBy.MaxDistance => volunteerList.OrderByDescending(v => v.MaxDistance).ToList(),
                BO.VolunteerSortBy.TotalHandledCalls => volunteerList.OrderBy(v => v.TotalHandledCalls).ToList(),
                BO.VolunteerSortBy.TotalCanceledCalls => volunteerList.OrderBy(v => v.TotalCanceledCalls).ToList(),
                BO.VolunteerSortBy.TotalExpiredCalls => volunteerList.OrderBy(v => v.TotalExpiredCalls).ToList(),
                _ => volunteerList.OrderBy(v => v.Id).ToList()
            } : volunteerList.OrderBy(v => v.Id).ToList();

            return volunteerList;
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.GeneralDatabaseException("Error accessing data.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.GeneralDatabaseException("An unexpected error occurred while getting Volunteers.", ex);
        }
    }
    public DO.Role Login(string username, string password)
    {

        try
        {
            // Search for a volunteer with the matching username
            var volunteerDO = _dal.Volunteer
                .ReadAll(v => v.Email == username)
                .FirstOrDefault();

            // If the user is not found, throw an exception
            if (volunteerDO == null)
                throw new ArgumentException($"User with username '{username}' does not exist.");

            // If the password does not match, throw an exception
            if (volunteerDO.Password != password)
                throw new ArgumentException("The password provided is incorrect.");

            // Return the user's role
            return volunteerDO.Role;
        }
        catch (Exception ex)
        {
            // Throw a general exception if something unexpected occurs
            throw new InvalidOperationException("An error occurred during login.", ex);
        }
    }

    public void UpdateVolunteerDetails(int id, BO.Volunteer volunteer)
    {
        throw new NotImplementedException();
    }
}
