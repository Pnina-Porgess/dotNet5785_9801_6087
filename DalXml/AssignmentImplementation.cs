namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

internal class AssignmentImplementation : IAssignment
{
    /// <summary>
    /// Converts an XElement to an Assignment object.
    /// </summary>
    static Assignment GetAssignment(XElement a)
    {
        return new DO.Assignment
        {
            Id = a.ToIntNullable("Id") ?? throw new FormatException("Can't convert Id"),
            CallId = a.ToIntNullable("CallId") ?? throw new FormatException("Can't convert CallId"),
            VolunteerId = a.ToIntNullable("VolunteerId") ?? throw new FormatException("Can't convert VolunteerId"),
            TypeOfEndTime = a.ToEnumNullable<TypeOfEndTime>("TypeOfEndTime") ?? throw new FormatException("Can't convert TypeOfEndTime"),
            EntryTime = a.ToDateTimeNullable("EntryTime") ?? throw new FormatException("Can't convert EntryTime"),
            EndTime = a.ToDateTimeNullable("EndTime") ?? null
        };
    }

    /// <summary>
    /// Creates an XElement from an Assignment object.
    /// </summary>
    private static XElement CreateAssignmentElement(Assignment item)
    {
        return new XElement("Assignment",
            new XElement("Id", item.Id),
            new XElement("CallId", item.CallId),
            new XElement("VolunteerId", item.VolunteerId),
            new XElement("TypeOfEndTime", item.TypeOfEndTime),
            new XElement("EntryTime", item.EntryTime),
            new XElement("EndTime", item.EndTime)
        );
    }

    /// <summary>
    /// Adds a new Assignment to the XML file.
    /// </summary>
    public void Create(Assignment item)
    {
        XElement assignmentsRoot = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);
        item = item with { Id = Config.NextAssignmentId };
        if (assignmentsRoot.Elements().Any(assignment => (int?)assignment.Element("Id") == item.Id))
            throw new DalAlreadyExistsException($"Assignment with ID={item.Id} already exists.");
        assignmentsRoot.Add(CreateAssignmentElement(item));
        XMLTools.SaveListToXMLElement(assignmentsRoot, Config.s_assignments_xml);
    }

    /// <summary>
    /// Reads an Assignment by ID from the XML file.
    /// </summary>
    public Assignment? Read(int id)
    {
        XElement? assignmentElement = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml)
            .Elements()
            .FirstOrDefault(a => (int?)a.Element("Id") == id);
        return assignmentElement == null ? null : GetAssignment(assignmentElement);
    }

    /// <summary>
    /// Reads the first Assignment matching a filter.
    /// </summary>
    public Assignment? Read(Func<Assignment, bool> filter)
    {
        return XMLTools.LoadListFromXMLElement(Config.s_assignments_xml).Elements()
            .Select(GetAssignment).FirstOrDefault(filter);
    }

    /// <summary>
    /// Reads all Assignments, optionally filtered by a predicate.
    /// </summary>
    public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
    {
        var assignments = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml)
            .Elements()
            .Select(GetAssignment);
        return filter == null ? assignments : assignments.Where(filter);
    }

    /// <summary>
    /// Updates an existing Assignment in the XML file.
    /// </summary>
    public void Update(Assignment item)
    {
        XElement assignmentsRoot = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);
        XElement? assignmentElement = assignmentsRoot.Elements().FirstOrDefault(a => (int?)a.Element("Id") == item.Id);
        if (assignmentElement == null)
            throw new DalDoesNotExistException($"Assignment with ID={item.Id} does not exist.");
        assignmentElement.Remove();
        assignmentsRoot.Add(CreateAssignmentElement(item));
        XMLTools.SaveListToXMLElement(assignmentsRoot, Config.s_assignments_xml);
    }

    /// <summary>
    /// Deletes an Assignment by ID from the XML file.
    /// </summary>
    public void Delete(int id)
    {
        XElement assignmentsRoot = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);
        XElement? assignmentElement = assignmentsRoot.Elements().FirstOrDefault(a => (int?)a.Element("Id") == id);
        if (assignmentElement == null)
            throw new DO.DalDoesNotExistException($"Assignment with ID={id} does not exist.");
        assignmentElement.Remove();
        XMLTools.SaveListToXMLElement(assignmentsRoot, Config.s_assignments_xml);
    }

    /// <summary>
    /// Deletes all Assignments from the XML file.
    /// </summary>
    public void DeleteAll()
    {
        XMLTools.SaveListToXMLElement(new XElement("Assignments"), Config.s_assignments_xml);
    }
}
