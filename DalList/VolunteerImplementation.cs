namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

public class VolunteerImplementation : IVolunteer
{
    public void Create(Volunteer item)
    {
        if(DataSource.Volunteers.Any(v=>v.Id == item.Id)) 
         throw new NotImplementedException($"Volunteer with ID={item.Id} already exists");
        DataSource.Volunteers.Add(item);
    }

    public void Delete(int id)
    {
        int removeCount = DataSource.Volunteers.RemoveAll(c => c?.Id == id);
        if (removeCount == 0)
            throw new NotImplementedException($"Volunteer with ID={id} not exists");
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
        var volunteer = Read(item.Id);
        if (volunteer == null)
            throw new Exception($"Volunteer with ID={item.Id} not exists");
        DataSource.Volunteers.Remove(volunteer);
        DataSource.Volunteers.Add(item);
    }
}
