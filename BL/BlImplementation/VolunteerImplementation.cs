namespace BlApi;
using Helpers;

/// <summary>
/// Implementation for managing volunteers in the business layer.
/// </summary>
internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    /// <summary>
    /// Adds a new volunteer.
    /// </summary>
    /// <param name="volunteer">The volunteer object to be added.</param>
    /// <exception cref="BO.BlAlreadyExistsException">Thrown if a volunteer with the same ID already exists.</exception>
    /// <exception cref="BO.BlDatabaseException">Thrown if an unexpected error occurs while adding the volunteer.</exception>
    public void AddVolunteer(BO.Volunteer volunteer)
    {
        try
        {
            var existingVolunteer = _dal.Volunteer.Read(v => v.Id == volunteer.Id);
            if (existingVolunteer != null)
            {
                throw new BO.BlAlreadyExistsException($"Volunteer with ID={volunteer.Id} already exists.");
            }

            VolunteerManager.ValidateInputFormat(volunteer);
             (volunteer.Latitude, volunteer.Longitude) = Tools.GetCoordinatesFromAddress(volunteer.CurrentAddress!);
            DO.Volunteer doVolunteer =( VolunteerManager.CreateDoVolunteer(volunteer));
            _dal.Volunteer.Create(doVolunteer);
            VolunteerManager.Observers.NotifyListUpdated(); //stage 5                                                    

        }
        catch (Exception ex)
        {
            throw new BO.BlDatabaseException("An unexpected error occurred while adding the volunteer.", ex);
        }
    }

    /// <summary>
    /// Deletes a volunteer by ID.
    /// </summary>
    /// <param name="id">The ID of the volunteer to delete.</param>
    /// <exception cref="BO.BlNotFoundException">Thrown if the volunteer does not exist.</exception>
    /// <exception cref="BO.InvalidOperationException">Thrown if the volunteer is currently handling a call.</exception>
    /// <exception cref="BO.BlDatabaseException">Thrown if an unexpected error occurs while deleting the volunteer.</exception>
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
            VolunteerManager.Observers.NotifyListUpdated(); //stage 5                                                    

        }
        catch (Exception ex)
        {
            throw new BO.BlDatabaseException($"An error occurred while deleting volunteer with ID={id}.", ex);
        }
    }

    /// <summary>
    /// Logs in a volunteer.
    /// </summary>
    /// <param name="username">The username (volunteer name).</param>
    /// <param name="password">The password to verify.</param>
    /// <returns>The role of the logged-in volunteer.</returns>
    /// <exception cref="BO.BlNotFoundException">Thrown if the username or password is incorrect.</exception>
    public BO.Role Login(string username, string password)
    {
        try
        {
            var volunteer = _dal.Volunteer.ReadAll().FirstOrDefault(v => v.Name == username)
                ?? throw new BO.BlNotFoundException("Username or password is not correct.");

            if (!(VolunteerManager.EncryptPassword(password) == volunteer.Password))
            {
                throw new BO.BlNotFoundException("Username or password is not correct.");
            }

            return (BO.Role)volunteer.Role;
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlNotFoundException($"Volunteer with ID={username} was not found in the database.", ex);
        }
    }

    /// <summary>
    /// Gets the details of a specific volunteer.
    /// </summary>
    /// <param name="volunteerId">The ID of the volunteer.</param>
    /// <returns>The details of the volunteer.</returns>
    /// <exception cref="BO.BlNotFoundException">Thrown if the volunteer is not found.</exception>
    public BO.Volunteer GetVolunteerDetails(int volunteerId)
    {
        try
        {
            var volunteerDO = _dal.Volunteer.Read(volunteerId) ??
                throw new BO.BlNotFoundException($"Volunteer with ID={volunteerId} does not exist.");

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

    /// <summary>
    /// Updates the details of a volunteer.
    /// </summary>
    /// <param name="requesterId">The ID of the requester.</param>
    /// <param name="volunteerToUpdate">The volunteer object containing updated details.</param>
    /// <exception cref="BO.BlNotFoundException">Thrown if the volunteer does not exist.</exception>
    /// <exception cref="BO.BlInvalidInputException">Thrown if the input is invalid.</exception>
    /// <exception cref="BO.BlUnauthorizedAccessException">Thrown if the requester does not have permission to update the volunteer.</exception>
    public void UpdateVolunteerDetails(int requesterId, BO.Volunteer volunteerToUpdate)
    {
        try
        {
            VolunteerManager.ValidatePermissions(requesterId, volunteerToUpdate);
            VolunteerManager.ValidateInputFormat(volunteerToUpdate);
            (volunteerToUpdate.Latitude, volunteerToUpdate.Longitude) = VolunteerManager.LogicalChecking(volunteerToUpdate);

            var existingVolunteer = _dal.Volunteer.Read(volunteerToUpdate.Id)
                ?? throw new BO.BlNotFoundException($"Volunteer with ID={volunteerToUpdate.Id} does not exist.");

            if (!VolunteerManager.CanUpdateFields(existingVolunteer!, volunteerToUpdate))
                throw new BO.BlUnauthorizedAccessException("You do not have permission to update the Role field.");

            DO.Volunteer doVolunteer = VolunteerManager.CreateDoVolunteer(volunteerToUpdate);
            _dal.Volunteer.Update(doVolunteer);
            VolunteerManager.Observers.NotifyItemUpdated(doVolunteer.Id);  //stage 5
            VolunteerManager.Observers.NotifyListUpdated();  //stage 5

        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlNotFoundException($"The volunteer with ID={volunteerToUpdate.Id} was not found.", ex);
        }
        catch (BO.BlInvalidInputException ex)
        {
            throw new BO.BlInvalidInputException($"Invalid data for volunteer update: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Retrieves a list of volunteers.
    /// </summary>
    /// <param name="isActive">Filter by active status.</param>
    /// <param name="sortBy">Sorting criteria.</param>
    /// <returns>A sorted and filtered list of volunteers.</returns>
    /// <exception cref="BO.BlDatabaseException">Thrown if an error occurs while accessing data.</exception>
   
        public IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive = null, BO.VolunteerSortBy? sortBy = null)
    {

        try
        {
            IEnumerable<DO.Volunteer> volunteers = _dal.Volunteer.ReadAll();
            if (isActive.HasValue)
            {
                volunteers = volunteers.Where(c => c.IsActive == isActive.Value);
            }

            var volunteerList = VolunteerManager.GetVolunteerList(volunteers);

            volunteerList = sortBy.HasValue ? sortBy.Value switch
            {
                BO.VolunteerSortBy.FullName => volunteerList.OrderBy(v => v.FullName).ToList(),
                BO.VolunteerSortBy.TotalHandledCalls => volunteerList.OrderBy(v => v.TotalHandledCalls).ToList(),
                BO.VolunteerSortBy.TotalCanceledCalls => volunteerList.OrderBy(v => v.TotalCancelledCalls).ToList(),
                BO.VolunteerSortBy.TotalExpiredCalls => volunteerList.OrderBy(v => v.TotalExpiredCalls).ToList(),
                _ => volunteerList.OrderBy(v => v.Id).ToList()
            } : volunteerList.OrderBy(v => v.Id).ToList();

            return volunteerList;
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDatabaseException("Error accessing data.", ex);
        }
    }
    #region Stage 5
    public void AddObserver(Action listObserver) =>
    VolunteerManager.Observers.AddListObserver(listObserver); //stage 5
    public void AddObserver(int id, Action observer) =>
VolunteerManager.Observers.AddObserver(id, observer); //stage 5
    public void RemoveObserver(Action listObserver) =>
VolunteerManager.Observers.RemoveListObserver(listObserver); //stage 5
    public void RemoveObserver(int id, Action observer) =>
VolunteerManager.Observers.RemoveObserver(id, observer); //stage 5
    #endregion Stage 5


}
