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
