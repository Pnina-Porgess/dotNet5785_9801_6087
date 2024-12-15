namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;

internal class CallImplementation : ICall
{
    /// <summary>
    /// Adds a new Call to the XML file with a unique ID.
    /// </summary>
    public void Create(Call item)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        int id = Config.NextCallId;
        Call copy = item with { Id = id };
        calls.Add(copy);
        XMLTools.SaveListToXMLSerializer(calls, Config.s_calls_xml);
    }

    /// <summary>
    /// Updates an existing Call in the XML file.
    /// </summary>
    public void Update(Call item)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        if (calls.RemoveAll(it => it.Id == item.Id) == 0)
            throw new DalDoesNotExistException($"Call with ID={item.Id} does not exist.");
        calls.Add(item);
        XMLTools.SaveListToXMLSerializer(calls, Config.s_calls_xml);
    }

    /// <summary>
    /// Deletes a Call by ID from the XML file.
    /// </summary>
    public void Delete(int id)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        if (calls.RemoveAll(it => it.Id == id) == 0)
            throw new DalDoesNotExistException($"Call with ID={id} does not exist.");
        XMLTools.SaveListToXMLSerializer(calls, Config.s_calls_xml);
    }

    /// <summary>
    /// Deletes all Calls from the XML file.
    /// </summary>
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLSerializer(new List<Call>(), Config.s_calls_xml);
    }

    /// <summary>
    /// Reads a Call by ID from the XML file.
    /// </summary>
    public Call? Read(int id)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        return calls.FirstOrDefault(call => call.Id == id);
    }

    /// <summary>
    /// Reads the first Call matching a filter.
    /// </summary>
    public Call? Read(Func<Call, bool> filter)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        return calls.FirstOrDefault(filter);
    }

    /// <summary>
    /// Reads all Calls, optionally filtered by a predicate.
    /// </summary>
    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
    {
        List<Call> calls = XMLTools.LoadListFromXMLSerializer<Call>(Config.s_calls_xml);
        return filter == null ? calls : calls.Where(filter);
    }

    /// <summary>
    /// Reads all Volunteers matching a filter (not implemented).
    /// </summary>
    public IEnumerable<Volunteer> ReadAll(Func<Assignment, bool>? filter = null)
    {
        throw new NotImplementedException();
    }
}
