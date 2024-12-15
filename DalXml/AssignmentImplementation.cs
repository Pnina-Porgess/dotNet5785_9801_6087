namespace Dal;
using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;



    internal class AssignmentImplementation : IAssignment
    {
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

        private static Assignment GetAssignmentFromElement(XElement element)
        {
            return new Assignment(
                element.ToIntNullable("Id") ?? throw new FormatException("Can't convert Id"),
                element.ToIntNullable("CallId") ?? throw new FormatException("Can't convert CallId"),
                element.ToIntNullable("VolunteerId") ?? throw new FormatException("Can't convert VolunteerId"),
                element.ToEnumNullable<TypeOfEndTime>("TypeOfEndTime") ?? throw new FormatException("Can't convert TypeOfEndTime"),
                element.ToDateTimeNullable("EntryTime") ?? throw new FormatException("Can't convert EntryTime"),
                element.ToDateTimeNullable("EndTime")
            );
        }

        public void Create(Assignment item)
        {
            XElement assignmentsRoot = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);
            if (assignmentsRoot.Elements().Any(assignment => (int?)assignment.Element("Id") == item.Id))
                throw new DalAlreadyExistsException($"Assignment with ID={item.Id} already exists.");

            assignmentsRoot.Add(CreateAssignmentElement(item));
            XMLTools.SaveListToXMLElement(assignmentsRoot, Config.s_assignments_xml);
        }

        public Assignment? Read(int id)
        {
            XElement? assignmentElement = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml)
                .Elements()
                .FirstOrDefault(a => (int?)a.Element("Id") == id);

            return assignmentElement == null ? null : GetAssignmentFromElement(assignmentElement);
        }

        public Assignment? Read(Func<Assignment, bool> filter)
        {
            return XMLTools.LoadListFromXMLElement(Config.s_assignments_xml)
                .Elements()
                .Select(GetAssignmentFromElement)
                .FirstOrDefault(filter);
        }

        public IEnumerable<Assignment> ReadAll(Func<Assignment, bool>? filter = null)
        {
            var assignments = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml)
                .Elements()
                .Select(GetAssignmentFromElement);

            return filter == null ? assignments : assignments.Where(filter);
        }

        public void Update(Assignment item)
        {
            XElement assignmentsRoot = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);

            XElement? assignmentElement = assignmentsRoot
                .Elements()
                .FirstOrDefault(a => (int?)a.Element("Id") == item.Id);

            if (assignmentElement == null)
                throw new DalDoesNotExistException($"Assignment with ID={item.Id} does not exist.");

            assignmentElement.Remove();
            assignmentsRoot.Add(CreateAssignmentElement(item));
            XMLTools.SaveListToXMLElement(assignmentsRoot, Config.s_assignments_xml);
        }

        public void Delete(int id)
        {
            XElement assignmentsRoot = XMLTools.LoadListFromXMLElement(Config.s_assignments_xml);

            XElement? assignmentElement = assignmentsRoot
                .Elements()
                .FirstOrDefault(a => (int?)a.Element("Id") == id);

            if (assignmentElement == null)
                throw new DalDoesNotExistException($"Assignment with ID={id} does not exist.");

            assignmentElement.Remove();
            XMLTools.SaveListToXMLElement(assignmentsRoot, Config.s_assignments_xml);
        }

        public void DeleteAll()
        {
            XMLTools.SaveListToXMLElement(new XElement("Assignments"), Config.s_assignments_xml);
        }
    }

