﻿namespace Dal;
using DalApi;
using DO;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

/// <summary>
/// Implements ICall with CRUD operations for Call entities.
/// </summary>

internal class CallImplementation : ICall
{
    /// <summary> Adds a new Call with a unique ID. </summary>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7

    public void Create(Call item)
    {
        int id = Config.NextCallId;
        Call copy = item with { Id = id };
        DataSource.Calls.Add(copy);
    }

    /// <summary> Deletes a Call by its ID. </summary>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7

    public void Delete(int id)
    {
        int removeCount = DataSource.Calls.RemoveAll(c => c?.Id == id);
        if (removeCount == 0)
            throw new DalDoesNotExistException($"Call failed, Call with ID={id} not exists");
    }

    /// <summary> Deletes all Calls. </summary>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7

    public void DeleteAll()
    {
        DataSource.Calls.Clear();
    }

    /// <summary> Reads a Call by its ID. </summary>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7

    public Call? Read(int id)
    {
        return DataSource.Calls.FirstOrDefault(c => c.Id == id);
    }

    /// <summary> Reads all Calls, optionally filtered by a condition. </summary>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7

    public IEnumerable<Call> ReadAll(Func<Call, bool>? filter = null)
        => filter == null
            ? DataSource.Calls.Select(item => item)
            : DataSource.Calls.Where(filter);

    /// <summary> Updates an existing Call. </summary>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7

    public void Update(Call item)
    {
        var call = Read(item.Id);
        if (call == null)
            throw new DalDoesNotExistException($"Call failed, Call with ID={item.Id} not exists");
        DataSource.Calls.Remove(call);
        DataSource.Calls.Add(item);
    }

    /// <summary> Reads the first Call matching a given filter. </summary>
    [MethodImpl(MethodImplOptions.Synchronized)] //stage 7

    public Call? Read(Func<Call, bool> filter)
    {
        return DataSource.Calls.FirstOrDefault(filter);
    }
}
