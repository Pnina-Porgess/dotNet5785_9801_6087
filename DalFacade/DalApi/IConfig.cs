namespace DalApi;
using DO;

public interface IConfig
{
    void Create(IConfig item); //Creates new entity object in DAL
    IConfig? Read(int id); //Reads entity object by its ID
    List<IConfig> ReadAll(); //stage 1 only, Reads all entity objects
    void Update(IConfig item); //Updates entity object
    void Delete(int id); //Deletes an object by is Id
    void DeleteAll(); //Delete all entity objects
}

