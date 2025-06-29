namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

internal class VolunteerImplementation : IVolunteer
{
    /// <summary> Adds a new Volunteer if it doesn't already exist. </summary>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7

    public void Create(Volunteer item)
    {
        if (DataSource.Volunteers.Any(v => v.Id == item.Id))
            throw new DalAlreadyExistsException($"Volunteer with ID={item.Id} already exists");
        DataSource.Volunteers.Add(item);
    }

    /// <summary> Deletes a Volunteer by ID. </summary>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7

    public void Delete(int id)
    {
        int removeCount = DataSource.Volunteers.RemoveAll(c => c?.Id == id);
        if (removeCount == 0)
            throw new DalDoesNotExistException($"Volunteer with ID={id} not exists");
    }

    /// <summary> Deletes all Volunteers. </summary>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7

    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();
    }

    /// <summary> Reads a Volunteer by ID. </summary>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7

    public Volunteer? Read(int id)
    {
        return DataSource.Volunteers.FirstOrDefault(v => v.Id == id);
    }

    /// <summary> Reads all Volunteers, optionally filtered. </summary>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7

    public IEnumerable<Volunteer> ReadAll(Func<Volunteer, bool>? filter = null)
        => filter == null
            ? DataSource.Volunteers.Select(item => item)
            : DataSource.Volunteers.Where(filter);

    /// <summary> Updates an existing Volunteer. </summary>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7

    public void Update(Volunteer item)
    {
        var volunteer = Read(item.Id);
        if (volunteer == null)
            throw new DalDoesNotExistException($"Volunteer with ID={item.Id} not exists");
        DataSource.Volunteers.Remove(volunteer);
        DataSource.Volunteers.Add(item);
    }

    /// <summary> Reads the first Volunteer matching a condition. </summary>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7

    public Volunteer? Read(Func<Volunteer, bool> filter)
    {
        return DataSource.Volunteers.FirstOrDefault(filter);
    }
}
