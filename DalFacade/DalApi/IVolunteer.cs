namespace DalApi;
using DO;
public interface IVolunteer
{
    void Create(IVolunteer item); //Creates new entity object in DAL
    IVolunteer? Read(int id); //Reads entity object by its ID
    List<IVolunteer> ReadAll(); //stage 1 only, Reads all entity objects
    void Update(IVolunteer item); //Updates entity object
    void Delete(int id); //Deletes an object by is Id
    void DeleteAll(); //Delete all entity objects
}
