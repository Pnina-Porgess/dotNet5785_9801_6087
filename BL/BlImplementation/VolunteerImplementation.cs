using BO;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
namespace BlApi
{
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
                // Retrieve volunteer details from the data layer
                var volunteerDO = _dal.Volunteer.Read(volunteerId);
                if (volunteerDO == null) {
                    throw new Exception("sss");
                        }

                // Retrieve the current call in progress for the volunteer, if it exists
                var callDO = _dal.GetCallInProgressByVolunteerId(volunteerId);

                // Map the data layer objects (DO) to business objects (BO)
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
                    TotalHandledCalls = _dal.GetTotalHandledCalls(volunteerId),
                    TotalCanceledCalls = _dal.GetTotalCanceledCalls(volunteerId),
                    TotalExpiredCalls = _dal.GetTotalExpiredCalls(volunteerId),
                    CurrentCall = callDO == null ? null : new BO.CallInProgress
                    {
                        CallId = callDO.CallId,
                        CallerName = callDO.CallerName,
                        CallerPhone = callDO.CallerPhone,
                        Location = callDO.Location,
                        TimeOfCall = callDO.TimeOfCall,
                        Description = callDO.Description,
                        IsUrgent = callDO.IsUrgent
                    }
                };

                return volunteerBO;
            }
            //catch (DO.DalDoesNotExistException ex)
            //{
            //    // Wrap and rethrow as a business layer exception
            //    throw new BO.BoEntityNotFoundException($"Volunteer with ID {volunteerId} does not exist.", ex);
            //}
            catch (Exception ex)
            {
                // Wrap and rethrow any unexpected exception
                throw new BO.BoGeneralException("An unexpected error occurred while retrieving volunteer details.", ex);
            }
        }
        public IEnumerable<VolunteerInList> GetVolunteersList(bool? isActive = null, VolunteerSortBy? sortBy = null)
        {
            throw new NotImplementedException();
        }

        public DO.Role Login(string username, string password)
        {
            // Check for invalid input
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be null or empty.");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty.");
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
}
