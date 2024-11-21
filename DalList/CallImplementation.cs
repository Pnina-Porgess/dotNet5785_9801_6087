namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

public class CallImplementation : ICall
{
    public void Create(Call item)
    {

        if (DataSource.Calls.Any(c => c?.Id == item.Id))
            throw new NotImplementedException("Call with the same id already exists.");
        DataSource.Calls.Add(item);
    }

    public void Delete(int id)
    {
        int removeCount = DataSource.Calls.RemoveAll(c => c?.Id == id);
        if (removeCount == 0)
            throw new NotImplementedException("Call not found");
    }

    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }

    public Call? Read(int id)
    {
        var call = DataSource.Calls.Find(c => c.Id == id);
        return call;
    }

    public List<Call> ReadAll()
    {
        return new List<Call>(DataSource.Calls);
    }

    public void Update(Call item)
    {
        int index = DataSource.Calls.FindIndex(c => c?.Id == item.Id);
        if (index < 0)
            throw new NotImplementedException("Call not found");
        DataSource.Calls[index] = item;
    }
}
