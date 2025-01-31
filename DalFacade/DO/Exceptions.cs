
namespace DO;
/// <summary>
/// The exception will be thrown when trying to delete an object with a non-existent ID number from the object list
/// </summary>
[Serializable]
public class DalDoesNotExistException : Exception
{
    public DalDoesNotExistException(string? message) : base(message) { }
}
/// <summary>
/// The exception will be thrown when trying to add an object with an ID number that already exists to the object list
/// </summary>
[Serializable]
public class DalAlreadyExistsException : Exception
{
    public DalAlreadyExistsException(string? message) : base(message) { }
}
/// <summary>
/// The exception will be thrown when attempting to fill in a field that does not conform to the required format.
/// </summary>
[Serializable]
public class DalInvalidFormatException : Exception
{
    public DalInvalidFormatException(string? message) : base(message) { }
}
/// <summary>
/// Exception thrown when loading a DalXML file
/// </summary>
[Serializable]
public class DalXMLFileLoadCreateException : Exception
{
    public DalXMLFileLoadCreateException(string? message) : base(message) { }
}
/// <summary>
/// Exception for configuration-related errors
/// </summary>
[Serializable]
public class DalConfigException : Exception
{
    public DalConfigException(string message) : base(message) { }
    public DalConfigException(string message, Exception innerException) : base(message, innerException) { }
}
