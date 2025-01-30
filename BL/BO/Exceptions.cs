namespace BO;

/// <summary>
/// Exception for unexpected database errors.
/// </summary>
[Serializable]
public class DbException : Exception
{
    public DbException() { }

    public DbException(string message) : base(message) { }

    public DbException(string message, Exception innerException) : base(message, innerException) { }

    protected DbException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        : base(info, context) { }
}

/// <summary>
/// Exception for cases where an object is not found.
/// </summary>
[Serializable]
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }

    public NotFoundException(string message, Exception innerException) : base(message, innerException) { }

    protected NotFoundException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        : base(info, context) { }
}

/// <summary>
/// Exception for invalid data formats.
/// </summary>
[Serializable]
public class InvalidFormatException : Exception
{
    public InvalidFormatException(string message) : base(message) { }

    public InvalidFormatException(string message, Exception innerException) : base(message, innerException) { }

    protected InvalidFormatException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        : base(info, context) { }
}
/// <summary>
/// Exception for timeout during the API request.
/// </summary>
[Serializable]
public class TimeoutException : Exception
{
    public TimeoutException(string message) : base(message) { }

    public TimeoutException(string message, Exception innerException) : base(message, innerException) { }

    protected TimeoutException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        : base(info, context) { }
}
/// <summary>
/// Exception for cases where a business layer object does not exist.
/// </summary>
[Serializable]
public class BLDoesNotExist : Exception
{
    public BLDoesNotExist(string message) : base(message) { }

    public BLDoesNotExist(string message, Exception innerException) : base(message, innerException) { }

    protected BLDoesNotExist(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        : base(info, context) { }
}
/// <summary>
/// Exception for general database errors.
/// </summary>
[Serializable]
public class GeneralDatabaseException : Exception
{
    public GeneralDatabaseException(string message) : base(message) { }

    public GeneralDatabaseException(string message, Exception innerException) : base(message, innerException) { }

    protected GeneralDatabaseException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        : base(info, context) { }
}
/// <summary>
/// Exception for cases where a business object does not exist.
/// </summary>
[Serializable]
public class BoDoesNotExistException : Exception
{
    public BoDoesNotExistException(string message) : base(message) { }

    public BoDoesNotExistException(string message, Exception innerException) : base(message, innerException) { }

    protected BoDoesNotExistException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        : base(info, context) { }
}
public class LogicalException : Exception
{
    public LogicalException(string message) : base(message) { }

    public LogicalException(string message, Exception innerException) : base(message, innerException) { }

    protected LogicalException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        : base(info, context) { }
}

public class InvalidOperationException : Exception
{
    public InvalidOperationException(string message) : base(message) { }

    public InvalidOperationException(string message, Exception innerException) : base(message, innerException) { }

    protected InvalidOperationException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        : base(info, context) { }
}


