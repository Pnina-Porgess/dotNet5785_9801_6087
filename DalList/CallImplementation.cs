namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;

internal class CallImplementation : ICall
{
    public void Create(Call item)
    {
        int id = Config.NextCallId;
        Call copy = item with { Id = id };
        DataSource.Calls.Add(copy);
    }

    public void Delete(int id)
    {
        int removeCount = DataSource.Calls.RemoveAll(c => c?.Id == id);
        if (removeCount == 0)
            throw new Exception($"Call faild,Volunteer with ID={id} not exists");
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
        var call = Read(item.Id);
        if (call == null)
            throw new Exception($"Call faild,Volunteer with ID={item.Id} not exists");
        DataSource.Calls.Remove(call);
        DataSource.Calls.Add(call);
    }

   
}
