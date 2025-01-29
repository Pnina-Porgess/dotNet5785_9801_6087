namespace BlApi;
using BO;
using Helpers;


internal class VolunteerImplementation : IVolunteer
{
  
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void AddVolunteer(BO.Volunteer volunteer)
    {
        try
        {
            var existingVolunteer = _dal.Volunteer.Read(v => v.Id == volunteer.Id) ?? throw new DalAlreadyExistsException($"Volunteer with ID={volunteer.Id} already exists.");
            Helpers.VolunteerManager.ValidateInputFormat(volunteer);
            var (latitude, longitude) = VolunteerManager.logicalChecking(volunteer);
            if (latitude != null && longitude != null)
            {
                volunteer.Latitude = latitude;
                volunteer.Longitude = longitude;
            }
            DO.Volunteer doVolunteer = VolunteerManager.CreateDoVolunteer(volunteer);
            _dal.Volunteer.Create(doVolunteer);
        }
        catch (DO.DalAlreadyExistsException ex)
        {
            throw new BO.BLDoesNotExist($"Volunteer with ID={volunteer.Id} already exists", ex);
        }
        catch (Exception ex)
        {
            throw new BO.GeneralDatabaseException("An unexpected error occurred while adding the volunteer.", ex);
        }

    }

    public void DeleteVolunteer(int id)
    {
        try
        {
            var volunteer = _dal.Volunteer.Read(id) ?? throw new DO.DalDoesNotExistException($"Volunteer with ID={id} does not exist.");
            var currentAssignment = _dal.Assignment.ReadAll(a => a.VolunteerId == id && a.EndTime == null).FirstOrDefault();
            if (currentAssignment != null)
            {
                throw new InvalidOperationException("Cannot delete volunteer while they are handling a call.");
            }
            _dal.Volunteer.Delete(id);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.NotFoundException($"Volunteer with ID={id} was not found in the database.", ex);
        }
        catch (InvalidOperationException ex)
        {
            throw new BO.InvalidFormatException("The volunteer cannot be deleted as they are handling a call.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.GeneralDatabaseException("An unexpected error occurred while trying to delete the volunteer.", ex);
        }
    }

    public DO.Role Login(string username, string password)
    {
        try
        {
            var volunteer = _dal.Volunteer.ReadAll().Select(v => VolunteerManager.MapVolunteer(v)).FirstOrDefault(v => v.FullName == username);
            if (volunteer == null)
                throw new ArgumentException($"User with username '{username}' does not exist.");

            if (!VolunteerManager.VerifyPassword(password, volunteer.Password!))
                throw new ArgumentException("The password provided is incorrect.");

            return (DO.Role)volunteer.Role;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("An error occurred during login.", ex);
        }
    }
    public BO.Volunteer GetVolunteerDetails(int volunteerId)
    {
        try
        {
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
                        CallType = (BO.TypeOfReading)callDetails.TypeOfReading,
                        Description = callDetails.Description,
                        Address = callDetails.Adress,
                        OpenTime = callDetails.TimeOfOpen,
                        MaxEndTime = callDetails.MaxTimeToFinish,
                        StartTime = currentAssignment.EntryTime,
                        DistanceFromVolunteer = Tools.CalculateDistance(volunteerDO.Latitude, volunteerDO.Longitude, callDetails.Latitude, callDetails.Longitude),
                        Status = Tools.CalculateStatus(callDetails, 30)
                    };
                }
            }

            // יצירת האובייקט והחזרה תמידית
            return new BO.Volunteer
            {
                Id = volunteerDO.Id,
                FullName = volunteerDO.Name,
                Phone = volunteerDO.Phone,
                Email = volunteerDO.Email,
                Password = volunteerDO.Password,
                CurrentAddress = volunteerDO.Adress,
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
            throw new BO.BoDoesNotExistException("Volunteer not found in data layer.", ex);
        }
        catch (Exception ex)
        {
            throw new BO.GeneralDatabaseException("An unexpected error occurred while getting Volunteer details.", ex);
        }
    }
    public void UpdateVolunteerDetails(int requesterId, BO.Volunteer volunteerToUpdate)
    {
        try
        {
            Helpers.VolunteerManager.ValidateInputFormat(volunteerToUpdate);
            Helpers.VolunteerManager.ValidatePermissions(requesterId, volunteerToUpdate);
            var (latitude, longitude) = Tools.GetCoordinatesFromAddress(volunteerToUpdate.CurrentAddress!);
            volunteerToUpdate.Latitude = latitude;
            volunteerToUpdate.Longitude = longitude;
            var existingVolunteer = _dal.Volunteer.Read(volunteerToUpdate.Id);
            if (!Helpers.VolunteerManager.CanUpdateFields(requesterId, existingVolunteer!, volunteerToUpdate))
                throw new UnauthorizedAccessException("You do not have permission to update the Role field.");
            DO.Volunteer doVolunteer = Helpers.VolunteerManager.CreateDoVolunteer(volunteerToUpdate);
            _dal.Volunteer.Update(doVolunteer);
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.NotFoundException($"The volunteer with ID={volunteerToUpdate.Id} was not found.", ex);
        }
        catch (BO.InvalidFormatException ex)
        {
            throw new BO.InvalidFormatException($"Invalid data for volunteer update: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            throw new BO.GeneralDatabaseException("An unexpected error occurred while updating the volunteer.", ex);
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

               VolunteerSortBy.FullName => volunteerList.OrderBy(v => v.FullName).ToList(),
               VolunteerSortBy.TotalHandledCalls => volunteerList.OrderByDescending(v => v.TotalHandledCalls).ToList(),
              VolunteerSortBy.TotalCanceledCalls => volunteerList.OrderByDescending(v => v.TotalCancelledCalls).ToList(),
               VolunteerSortBy.TotalExpiredCalls => volunteerList.OrderByDescending(v => v.TotalExpiredCalls).ToList(),
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

  
   
 

}
