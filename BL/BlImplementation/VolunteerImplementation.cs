namespace BlApi;
using Helpers;

/// <summary>
/// Implementation for managing volunteers in the business layer.
/// </summary>
internal class VolunteerImplementation : IVolunteer
{
    private readonly DalApi.IDal _dal = DalApi.Factory.Get;

    public void AddVolunteer(BO.Volunteer volunteer)
    {
        try
        {
            AdminManager.ThrowOnSimulatorIsRunning();
            lock (AdminManager.BlMutex)
            {
                var existingVolunteer = _dal.Volunteer.Read(v => v.Id == volunteer.Id);
                if (existingVolunteer != null)
                    throw new BO.BlAlreadyExistsException($"Volunteer with ID={volunteer.Id} already exists.");

                VolunteerManager.ValidateInputFormat(volunteer);
                (volunteer.Latitude, volunteer.Longitude) = Tools.GetCoordinatesFromAddress(volunteer.CurrentAddress!);
                volunteer.Password = VolunteerManager.EncryptPassword(volunteer.Password!);
                DO.Volunteer doVolunteer = VolunteerManager.CreateDoVolunteer(volunteer);
                VolunteerManager.ValidatePassword(volunteer);
                _dal.Volunteer.Create(doVolunteer);
            }
            VolunteerManager.Observers.NotifyListUpdated();
        }
        catch (Exception ex)
        {
            throw new BO.BlDatabaseException("An unexpected error occurred while adding the volunteer.", ex);
        }
    }

    public void DeleteVolunteer(int id)
    {
        try
        {
            AdminManager.ThrowOnSimulatorIsRunning();
            lock (AdminManager.BlMutex)
            {
                var volunteer = _dal.Volunteer.Read(id) ?? throw new BO.BlNotFoundException($"Volunteer with ID={id} does not exist.");
                var currentAssignment = _dal.Assignment.ReadAll(a => a.VolunteerId == id && a.EndTime == null).FirstOrDefault();
                if (currentAssignment != null)
                    throw new BO.InvalidOperationException("Cannot delete volunteer while they are handling a call.");

                _dal.Volunteer.Delete(id);
            }
            VolunteerManager.Observers.NotifyListUpdated();
        }
        catch (Exception ex)
        {
            throw new BO.BlDatabaseException($"An error occurred while deleting volunteer with ID={id}.", ex);
        }
    }

    public BO.Role Login(int id, string password)
    {
        try
        {
            lock (AdminManager.BlMutex)
            {
                var volunteer = _dal.Volunteer.ReadAll().FirstOrDefault(v => v.Id == id)
                    ?? throw new BO.BlNotFoundException("Username or password is not correct.");

                var pass = VolunteerManager.EncryptPassword(password);
                if (pass != volunteer.Password)
                    throw new BO.BlNotFoundException("Username or password is not correct.");

                return (BO.Role)volunteer.Role;
            }
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlNotFoundException($"Volunteer with ID={id} was not found in the database.", ex);
        }
    }

    public BO.Volunteer GetVolunteerDetails(int volunteerId)
    {
        try
        {
            lock (AdminManager.BlMutex)
            {
                var volunteerDO = _dal.Volunteer.Read(volunteerId)
                    ?? throw new BO.BlNotFoundException($"Volunteer with ID={volunteerId} does not exist.");
                var currentAssignment = _dal.Assignment.ReadAll(a => a.VolunteerId == volunteerId && a?.TypeOfEndTime == null).FirstOrDefault();
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
                            Status = VolunteerManager.CalculateStatus(callDetails)
                        };
                    }
                }

                return new BO.Volunteer
                {
                    Id = volunteerDO.Id,
                    FullName = volunteerDO.Name,
                    Phone = volunteerDO.Phone,
                    Email = volunteerDO.Email,
                    Password = "",
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
            AdminManager.ThrowOnSimulatorIsRunning();
            VolunteerManager.ValidatePermissions(requesterId, volunteerToUpdate);
            VolunteerManager.ValidateInputFormat(volunteerToUpdate);
            (volunteerToUpdate.Latitude, volunteerToUpdate.Longitude) = VolunteerManager.LogicalChecking(volunteerToUpdate);

            lock (AdminManager.BlMutex)
            {
                var existingVolunteer = _dal.Volunteer.Read(volunteerToUpdate.Id)
                    ?? throw new BO.BlNotFoundException($"Volunteer with ID={volunteerToUpdate.Id} does not exist.");
                var volunteerRequester = _dal.Volunteer.Read(requesterId);

                if (!VolunteerManager.CanUpdateFields(existingVolunteer!, volunteerRequester!))
                    throw new BO.BlUnauthorizedAccessException("You do not have permission to update the Role field.");
                if (volunteerToUpdate.IsActive == false && volunteerToUpdate.CurrentCall != null)
                    throw new BO.BlInvalidInputException("Cannot set volunteer to inactive while they have an active call.");

                if (string.IsNullOrEmpty(volunteerToUpdate.Password))
                {
                    volunteerToUpdate.Password = existingVolunteer.Password;
                }
                else
                {
                    VolunteerManager.ValidatePassword(volunteerToUpdate);
                    volunteerToUpdate.Password = VolunteerManager.EncryptPassword(volunteerToUpdate.Password);
                }

                DO.Volunteer doVolunteer = VolunteerManager.CreateDoVolunteer(volunteerToUpdate);
                _dal.Volunteer.Update(doVolunteer);
            }
            VolunteerManager.Observers.NotifyItemUpdated(volunteerToUpdate.Id);
            VolunteerManager.Observers.NotifyListUpdated();
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

    public IEnumerable<BO.VolunteerInList> GetVolunteersList(bool? isActive = null, BO.VolunteerSortBy? sortBy = null, BO.TypeOfReading? filterField = null)
    {
        try
        {
            lock (AdminManager.BlMutex)
            {
                IEnumerable<DO.Volunteer> volunteers = _dal.Volunteer.ReadAll(v => !isActive.HasValue || v.IsActive == isActive.Value);
                var volunteerList = VolunteerManager.GetVolunteerList(volunteers);
                volunteerList = volunteerList.Where(vol => !filterField.HasValue || vol.CurrentCallType == filterField);

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
        }
        catch (DO.DalDoesNotExistException ex)
        {
            throw new BO.BlDatabaseException("Error accessing data.", ex);
        }
    }

    #region Stage 5 Observers
    public void AddObserver(Action listObserver) => VolunteerManager.Observers.AddListObserver(listObserver);
    public void AddObserver(int id, Action observer) => VolunteerManager.Observers.AddObserver(id, observer);
    public void RemoveObserver(Action listObserver) => VolunteerManager.Observers.RemoveListObserver(listObserver);
    public void RemoveObserver(int id, Action observer) => VolunteerManager.Observers.RemoveObserver(id, observer);
    #endregion
}
