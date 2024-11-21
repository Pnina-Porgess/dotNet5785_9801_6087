namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

public class VolunteerImplementation : IVolunteer
{
    public void Create(Volunteer item)
    {
        if(DataSource.Volunteers.Any(v=>v.Id == item.Id)) 
         throw new NotImplementedException("An object of type Volunteer with such ID already exists.");
        DataSource.Volunteers.Add(item);
    }

    public void Delete(int id)
    {
        int removeCount = DataSource.Volunteers.RemoveAll(c => c?.Id == id);
        if (removeCount == 0)
            throw new NotImplementedException("Call not found");
    }

    public void DeleteAll()
    {
        DataSource.Volunteers.Clear();

    }

    public Volunteer? Read(int id)
    {
        return DataSource.Volunteers.Find(v => v.Id == id);
    }

    public List<Volunteer> ReadAll()
    {
        return new List<Volunteer>(DataSource.Volunteers);
    }

    public void Update(Volunteer item)
    {
        int index = DataSource.Volunteers.FindIndex(v => v.Id == item.Id);
        if(index==-1)
           throw new NotImplementedException();
        DataSource.Volunteers[index] = item;
    }
}
