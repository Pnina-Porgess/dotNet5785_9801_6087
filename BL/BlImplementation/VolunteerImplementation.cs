using BlApi;
using Helpers;

namespace BlImplementation;

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
            VolunteerManager.ValidateInputFormat(volunteer);

            // Step 1: Set dummy coordinates (null or default)
            volunteer.Latitude = null;
            volunteer.Longitude = null;

            // Step 2: Encrypt password and create DO object
            volunteer.Password = VolunteerManager.EncryptPassword(volunteer.Password!);
            DO.Volunteer doVolunteer = VolunteerManager.CreateDoVolunteer(volunteer);

            // Step 3: Check existence and create
            lock (AdminManager.BlMutex)
            {
                var existing = _dal.Volunteer.Read(v => v.Id == volunteer.Id);
                if (existing != null)
                    throw new BO.BlAlreadyExistsException($"Volunteer with ID={volunteer.Id} already exists.");

                _dal.Volunteer.Create(doVolunteer);
            }

            VolunteerManager.Observers.NotifyListUpdated();

            // Step 4: Async coordinate update
            _ = UpdateCoordinatesForVolunteerAsync(volunteer.Id, volunteer.CurrentAddress!);
        }
        catch (Exception ex)
        {
            throw new BO.BlDatabaseException("An unexpected error occurred while adding the volunteer.", ex);
        }
    }

    private static async Task UpdateCoordinatesForVolunteerAsync(int volunteerId, string address)
    {
        try
        {
            var coordinates = await Tools.GetCoordinatesFromAddressAsync(address);
            if (coordinates == null)
            {
                // טיפול במקרה של null, למשל:
                throw new Exception("Coordinates not found.");
            }

            var Latitude = coordinates.Value.Latitude;
            var Longitude = coordinates.Value.Longitude;


            lock (AdminManager.BlMutex)
            {
                var volunteer = DalApi.Factory.Get.Volunteer.Read(volunteerId);
                if (volunteer is null)
                    return;

                var updated = volunteer with
                {
                    Latitude = Latitude,
                    Longitude = Longitude
                };

                DalApi.Factory.Get.Volunteer.Update(updated);
            }

            VolunteerManager.Observers.NotifyItemUpdated(volunteerId);
            VolunteerManager.Observers.NotifyListUpdated();
        }
        catch
        {
            // Optional: log or ignore address error
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
            DO.Volunteer volunteer;
            lock (AdminManager.BlMutex)
            {
                volunteer = _dal.Volunteer.ReadAll().FirstOrDefault(v => v.Id == id)
                    ?? throw new BO.BlNotFoundException("Username or password is not correct.");
            }

            var encrypted = VolunteerManager.EncryptPassword(password);
            if (encrypted != volunteer.Password)
                throw new BO.BlNotFoundException("Username or password is not correct.");

            return (BO.Role)volunteer.Role;
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

    public async Task UpdateVolunteerDetails(int requesterId, BO.Volunteer volunteerToUpdate)
    {
        try
        {
            AdminManager.ThrowOnSimulatorIsRunning();
            VolunteerManager.ValidatePermissions(requesterId, volunteerToUpdate);
            VolunteerManager.ValidateInputFormat(volunteerToUpdate);

            var coords = await VolunteerManager.LogicalCheckingAsync(volunteerToUpdate);
            if (coords is null)
                throw new BO.BlInvalidInputException("Could not determine coordinates for the volunteer's address.");

            (volunteerToUpdate.Latitude, volunteerToUpdate.Longitude) = coords.Value;

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
            IEnumerable<BO.VolunteerInList> volunteerList;
            lock (AdminManager.BlMutex)
            {
                IEnumerable<DO.Volunteer> volunteers = _dal.Volunteer.ReadAll(v => !isActive.HasValue || v.IsActive == isActive.Value);
                volunteerList = VolunteerManager.GetVolunteerList(volunteers);
                volunteerList = volunteerList.Where(vol => !filterField.HasValue || vol.CurrentCallType == filterField);
            }

            volunteerList = sortBy.HasValue ? sortBy.Value switch
            {
                BO.VolunteerSortBy.FullName => volunteerList.OrderBy(v => v.FullName),
                BO.VolunteerSortBy.TotalHandledCalls => volunteerList.OrderBy(v => v.TotalHandledCalls),
                BO.VolunteerSortBy.TotalCanceledCalls => volunteerList.OrderBy(v => v.TotalCancelledCalls),
                BO.VolunteerSortBy.TotalExpiredCalls => volunteerList.OrderBy(v => v.TotalExpiredCalls),
                _ => volunteerList.OrderBy(v => v.Id)
            } : volunteerList.OrderBy(v => v.Id);

            return volunteerList;
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
