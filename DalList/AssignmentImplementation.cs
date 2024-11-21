

namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

public class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {
        int id = Config.NextCallId;
        Assignment copy = item with { Id = id };
        DataSource.Assignments.Add(copy);
       // return copy.Id;
    }

    public void Delete(int id)
    {
        int removeCount=DataSource.Assignments.RemoveAll(a=>a?.Id==id);
        if (removeCount == 0)
            throw new Exception($"Assignments with ID={id} not exists");
    }

    public void DeleteAll()
    {
       DataSource.Assignments.Clear();
    }

    public Assignment? Read(int id)
    {
        var assignment=DataSource.Assignments.Find(a=>a.Id==id);
        return assignment;
    }

    public List<Assignment> ReadAll()
    {
        return new List<Assignment>(DataSource.Assignments);
    }

    public void Update(Assignment item)
    {
       var assignment =Read(item.Id);
     if(assignment ==null)
        throw new Exception($"Assignments with ID={item.Id} not exists");
        DataSource.Assignments.Remove(assignment);
        DataSource.Assignments.Add(assignment);
    }

}

