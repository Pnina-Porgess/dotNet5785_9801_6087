
namespace BO;
/// <summary>
/// Exception thrown for unexpected errors during database operations.
/// </summary>
[Serializable]
public class GeneralDatabaseException : Exception
{
    public GeneralDatabaseException() { }

    public GeneralDatabaseException(string message)
        : base(message) { }

    public GeneralDatabaseException(string message, Exception innerException)
        : base(message, innerException) { }

    protected GeneralDatabaseException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context)
        : base(info, context) { }
}
/// <summary>
/// Exception thrown when the volunteer with the specified ID is not found in the data layer.
/// </summary>
[Serializable]
    public class BoDoesNotExistException : Exception
    {
        public BoDoesNotExistException(string? message) : base(message) { }

        public BoDoesNotExistException(string? message, Exception? innerException)
            : base(message, innerException) { }
    }



