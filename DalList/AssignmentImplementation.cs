

namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

public class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {
        int id = Config.NextAssignmentId;
        Assignment copy = item with { Id = id };
        DataSource.Assignments.Add(copy);
       // return copy.Id;
    }

    public void Delete(int id)
    {
        int removeCount=DataSource.Assignments.RemoveAll(a=>a?.Id==id);
        if (removeCount == 0)
            throw new Exception($"Assignment faild,Volunteer with ID={id} not exists");
    }

    public void DeleteAll()
    {
       DataSource.Assignments.Clear();
    }

    public Assignment? Read(int id)
    {
        return DataSource.Assignments.FirstOrDefault(a=>a.Id==id);
  
    }

    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null) //stage 2
        => filter == null
            ? DataSource.Assignments.Select(item => item)
            : DataSource.Assignments.Where(filter);


    public void Update(Assignment item)
    {
       var assignment =Read(item.Id);
     if(assignment ==null)
        throw new Exception($"Assignments faild,Volunteer with ID={item.Id} not exists");
        DataSource.Assignments.Remove(assignment);
        DataSource.Assignments.Add(assignment);
    }

}

