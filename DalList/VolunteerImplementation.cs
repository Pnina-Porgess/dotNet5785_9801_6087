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
       
    }

    public void DeleteAll()
    {

        throw new NotImplementedException();
    }

    public Volunteer? Read(int id)
    {
        return DataSource.Volunteers.Find(v => v.Id == id);
    }

    public List<Volunteer> ReadAll()
    {
        throw new NotImplementedException();
    }

    public void Update(Volunteer item)
    {
        throw new NotImplementedException();
    }
}
