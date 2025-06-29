

namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

/// <summary>
/// Implements IAssignment with CRUD operations for Assignments.
/// </summary>
public class AssignmentImplementation : IAssignment
{

    ///  Adds a new Assignment.
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7

    public void Create(Assignment item)
    {
        int id = Config.NextAssignmentId;
        Assignment copy = item with { Id = id };
        DataSource.Assignments.Add(copy);
    }
    /// Deletes an Assignment by ID.
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7

    public void Delete(int id)
    {
        int removeCount=DataSource.Assignments.RemoveAll(a=>a?.Id==id);
        if (removeCount == 0)
        throw new DalDoesNotExistException($"Assignment faild,Volunteer with ID={id} not exists");
    }
    /// <summary> Deletes all Assignments. </summary>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7

    public void DeleteAll()
    {
       DataSource.Assignments.Clear();
    }

    /// <summary> Reads an Assignment by ID. </summary>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7

    public Assignment? Read(int id)
    {
        return DataSource.Assignments.FirstOrDefault(a=>a.Id==id);
  
    }
    /// <summary> Reads all Assignments, optionally filtered. </summary>
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null) //stage 2
        => filter == null
            ? DataSource.Assignments.Select(item => item)
            : DataSource.Assignments.Where(filter);


    /// <summary> Updates an existing Assignment. </summary>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7

    public void Update(Assignment item)
    {
     var assignment =Read(item.Id);
     if(assignment ==null)
        throw new DalDoesNotExistException($"Assignments faild,Volunteer with ID={item.Id} not exists");
        DataSource.Assignments.Remove(assignment);
        DataSource.Assignments.Add(assignment);
    }

    /// <summary> Reads the first Assignment matching a condition. </summary>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7

    public Assignment? Read(Func<Assignment, bool> filter)
    {
        return DataSource.Assignments.FirstOrDefault(filter);
    }

}

