namespace BO;



public class BlLogicalException : Exception
{
    public BlLogicalException(string message) : base(message) { }

    public BlLogicalException(string message, Exception innerException) : base(message, innerException) { }

}

    public class InvalidOperationException : Exception
{
    public InvalidOperationException(string message) : base(message) { }

    public InvalidOperationException(string message, Exception innerException) : base(message, innerException) { }

}
/// <summary>
/// Exception for invalid data formats.
/// </summary>
[Serializable]
public class BlInvalidInputException : Exception
{
    public BlInvalidInputException(string message) : base(message) { }

    public BlInvalidInputException(string message, Exception innerException) : base(message, innerException) { }

}
/// <summary>
/// Exception for cases where a business layer object does not exist.
/// </summary>
[Serializable]
public class BlNotFoundException : Exception
{
    public BlNotFoundException(string message) : base(message) { }

    public BlNotFoundException(string message, Exception innerException) : base(message, innerException) { }

}
/// <summary>
/// Exception for general database errors.
/// </summary>
[Serializable]
public class BlDatabaseException : Exception
{
    public BlDatabaseException(string message) : base(message) { }
    public BlDatabaseException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception for unauthorized access attempts.
/// </summary>
[Serializable]
public class BlUnauthorizedAccessException : Exception
{
    public BlUnauthorizedAccessException(string message) : base(message) { }
    public BlUnauthorizedAccessException(string message, Exception innerException) : base(message, innerException) { }
}
// <summary>
/// Exception for cases where an object already exists
/// </summary>
[Serializable]
public class BlAlreadyExistsException : Exception
{
    public BlAlreadyExistsException(string message) : base(message) { }
    public BlAlreadyExistsException(string message, Exception innerException) : base(message, innerException) { }
}
/// <summary>
/// Exception for configuration-related errors
/// </summary>
[Serializable]
public class BlConfigException : Exception
{
    public BlConfigException(string message) : base(message) { }
    public BlConfigException(string message, Exception innerException) : base(message, innerException) { }
}



/// <summary>
/// General exception in the Bl.
/// </summary>
[Serializable]
public class BlGeneralException : Exception
{
    public BlGeneralException(string message) : base(message) { }
    public BlGeneralException(string message, Exception innerException) : base(message, innerException) { }
}
/// <summary>
/// Exception thrown when a temporary condition prevents operation (e.g., simulator is running).
/// </summary>
[Serializable]
public class BLTemporaryNotAvailableException : Exception
{
    public BLTemporaryNotAvailableException(string message) : base(message) { }

    public BLTemporaryNotAvailableException(string message, Exception innerException) : base(message, innerException) { }
}
