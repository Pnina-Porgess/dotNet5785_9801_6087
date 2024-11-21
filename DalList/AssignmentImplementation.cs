

namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

public class AssignmentImplementation : IAssignment
{
    public void Create(Assignment item)
    {
        if(DataSource.Assignments.Any(a=>a?.Id == item.Id))
            throw new NotImplementedException("Assignment with the same id already exists.");
        DataSource.Assignments.Add(item);
    }

    public void Delete(int id)
    {
        int removeCount=DataSource.Assignments.RemoveAll(a=>a?.Id==id);
        if (removeCount == 0)
            throw new NotImplementedException("Assignment not found");
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

