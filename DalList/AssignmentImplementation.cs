

namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

public class AssignmentImplementation : IAssignment
{
    public int Create(Assignment item)
    {
        int id = DataSource.Config.NextCallId;
        Assignment copy = item with { Id = id };
        DataSource.Assignments.Add(copy);
        return copy.Id;
    }

    public void Delete(int id)
    {
        int removeCount=DataSource.Assignments.RemoveAll(a=>a?.Id==id);
        if (removeCount == 0)
            throw new NotImplementedException($"Assignments with ID={id} not exists");
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
       int index = DataSource.Assignments.FindIndex(a=>a?.Id==item.Id);
     if(index<0)
        throw new NotImplementedException("Assignment not found");
        DataSource.Assignments[index] = item;   
    }
}

