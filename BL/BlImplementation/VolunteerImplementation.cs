namespace BlApi;
using Helpers;




internal class VolunteerImplementation : IVolunteer
{
  
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void AddVolunteer(BO.Volunteer volunteer)
    {
        try
        {
            //var existingVolunteer = _dal.Volunteer.Read(v => v.Id == volunteer.Id) ??
            //    throw new BO.BlNotFoundException($"Volunteer with ID={volunteer.Id} already exists.");
            var existingVolunteer = _dal.Volunteer.Read(v => v.Id == volunteer.Id);
            if (existingVolunteer != null)
            {
                throw new BO.BlAlreadyExistsException($"Volunteer with ID={volunteer.Id} already exists.");
            }
            VolunteerManager.ValidateInputFormat(volunteer);
             (volunteer.Latitude, volunteer.Longitude) = Tools.GetCoordinatesFromAddress(volunteer.CurrentAddress);
            DO.Volunteer doVolunteer =( VolunteerManager.CreateDoVolunteer(volunteer));
            _dal.Volunteer.Create(doVolunteer);
        }
        catch (Exception ex)
        {
            throw new BO.BlDatabaseException($"Volunt", ex);
        }
        //catch (Exception ex)
        //{
        //    throw new BO.BlDatabaseException("An unexpected error occurred while adding the volunteer.", ex);
        //}

    }

    public void DeleteVolunteer(int id)
    {
        try
        {
            var volunteer = _dal.Volunteer.Read(id) ?? throw new BO.BlNotFoundException($"Volunteer with ID={id} does not exist.");
            var currentAssignment = _dal.Assignment.ReadAll(a => a.VolunteerId == id && a.EndTime == null).FirstOrDefault();
            if (currentAssignment != null)
            {
                throw new BO.InvalidOperationException("Cannot delete volunteer while they are handling a call.");
            }
            _dal.Volunteer.Delete(id);
        }
        catch (Exception ex)
        {
            throw new BO.BlDatabaseException($"Volunteer with ID={id} was not found in the database.", ex);
        }
    }

    public BO.Role Login(string username, string password)
    {
        try
        {
            var volunteer = _dal.Volunteer.ReadAll().FirstOrDefault(v => v.Name == username)
                ?? throw new BO.BlNotFoundException($"Username or password is not correct.");
            if (!(VolunteerManager.EncryptPassword(password) == volunteer.Password))
            {
                throw new BO.BlNotFoundException($"Username or password is not correct.");
            }
            return (BO.Role)volunteer.Role;
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlNotFoundException($"Volunteer with ID={username} was not found in the database.", ex);
        }
    }
    public BO.Volunteer GetVolunteerDetails(int volunteerId)
    {
        try
        {
            var volunteerDO = _dal.Volunteer.Read(volunteerId) ??
              throw new BO.BlNotFoundException($"Volunteer with ID={volunteerId} does not exist");

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
                        CallType = (BO.TypeOfReading)callDetails.TypeOfReading,
                        Description = callDetails.Description,
                        Address = callDetails.Adress,
                        OpenTime = callDetails.TimeOfOpen,
                        MaxEndTime = callDetails.MaxTimeToFinish,
                        StartTime = currentAssignment.EntryTime,
                        DistanceFromVolunteer = Tools.CalculateDistance(volunteerDO.Latitude!, volunteerDO.Longitude!, callDetails.Latitude, callDetails.Longitude),
                        Status = VolunteerManager.CalculateStatus(callDetails, 30)
                    };
                }
            }
            return new BO.Volunteer
            {
                Id = volunteerDO.Id,
                FullName = volunteerDO.Name,
                Phone = volunteerDO.Phone,
                Email = volunteerDO.Email,
                Password = volunteerDO.Password,
                CurrentAddress = volunteerDO.Address,
                Latitude = volunteerDO.Latitude,
                Longitude = volunteerDO.Longitude,
                Role = (BO.Role)volunteerDO.Role,
                IsActive = volunteerDO.IsActive,
                MaxDistance = volunteerDO.MaximumDistance,
                DistanceType = (BO.DistanceType)volunteerDO.DistanceType,
                CurrentCall = callInProgress
            };
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlNotFoundException("Volunteer not found in data layer.", ex);
        }
    }
    public void UpdateVolunteerDetails(int requesterId, BO.Volunteer volunteerToUpdate)
    {
        try
        {
            VolunteerManager.ValidatePermissions(requesterId, volunteerToUpdate);
            VolunteerManager.ValidateInputFormat(volunteerToUpdate);
            (volunteerToUpdate.Latitude, volunteerToUpdate.Longitude) = VolunteerManager.logicalChecking(volunteerToUpdate);
            var existingVolunteer = _dal.Volunteer.Read(volunteerToUpdate.Id)
                ??throw new BO.BlNotFoundException($"Volunteer with ID={volunteerToUpdate.Id} does not exist.");
            if (!VolunteerManager.CanUpdateFields( existingVolunteer!, volunteerToUpdate))
                throw new BO.BlUnauthorizedAccessException("You do not have permission to update the Role field.");
            DO.Volunteer doVolunteer = VolunteerManager.CreateDoVolunteer(volunteerToUpdate);
            _dal.Volunteer.Update(doVolunteer);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlNotFoundException($"The volunteer with ID={volunteerToUpdate.Id} was not found.", ex);
        }
        catch (BO.BlInvalidInputException ex)
        {
            throw new BO.BlInvalidInputException($"Invalid data for volunteer update: {ex.Message}", ex);
        }
        catch (BO.BlUnauthorizedAccessException ex)
        {
            throw new BO.BlInvalidInputException($"Invalid data for volunteer update: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new BO.BlDatabaseException("An unexpected error occurred while updating the volunteer.", ex);
        }
    }
    public IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive = null, BO.VolunteerSortBy? sortBy = null)
    {

        try
        {
            IEnumerable<DO.Volunteer> volunteers = _dal.Volunteer.ReadAll(v =>
                !isActive.HasValue || v.IsActive == isActive);

            var volunteerList = VolunteerManager.GetVolunteerList(volunteers);
           
            volunteerList = sortBy.HasValue ? sortBy.Value switch
            {
              BO.VolunteerSortBy.FullName => volunteerList.OrderBy(v => v.FullName).ToList(),
              BO.VolunteerSortBy.TotalHandledCalls => volunteerList.OrderByDescending(v => v.TotalHandledCalls).ToList(),
              BO.VolunteerSortBy.TotalCanceledCalls => volunteerList.OrderByDescending(v => v.TotalCancelledCalls).ToList(),
              BO.VolunteerSortBy.TotalExpiredCalls => volunteerList.OrderByDescending(v => v.TotalExpiredCalls).ToList(),
                _ => volunteerList.OrderBy(v => v.Id).ToList()
            } : volunteerList.OrderBy(v => v.Id).ToList();

            return volunteerList;
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDatabaseException("Error accessing data.", ex);
        }
    }
}
