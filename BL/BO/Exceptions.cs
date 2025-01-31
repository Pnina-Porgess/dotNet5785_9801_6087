namespace BO;



public class BlLogicalException : Exception
{
    public BlLogicalException(string message) : base(message) { }

    public BlLogicalException(string message, Exception innerException) : base(message, innerException) { }

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
/// Exception thrown when the provided address is invalid
/// </summary>
[Serializable]
public class InvalidAddressException : Exception
{
    public InvalidAddressException(string message) : base(message) { }
    public InvalidAddressException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when geolocation coordinates are not found for the given address
/// </summary>
[Serializable]
public class BlGeolocationNotFoundException : Exception
{
    public BlGeolocationNotFoundException(string message) : base(message) { }
    public BlGeolocationNotFoundException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when there is an error in API request
/// </summary>
[Serializable]
public class BlApiRequestException : Exception
{
    public BlApiRequestException(string message) : base(message) { }
    public BlApiRequestException(string message, Exception innerException) : base(message, innerException) { }
}

/// <summary>
/// Exception thrown when there is an error in distance calculation between points
/// </summary>
[Serializable]
public class DistanceCalculationException : Exception
{
    public DistanceCalculationException(string message) : base(message) { }
    public DistanceCalculationException(string message, Exception innerException) : base(message, innerException) { }
}