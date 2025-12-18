namespace Errand.Exceptions;

/// <summary>
/// Base exception for Errand-specific errors.
/// "You shall not pass...  this exception uncaught!" - Gandalf
/// </summary>
public class ErrandException : Exception
{
    /// <summary>
    /// Initializes a new instance of ErrandException.
    /// </summary>
    public ErrandException() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of ErrandException with a message.
    /// </summary>
    /// <param name="message">The error message</param>
    public ErrandException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of ErrandException with a message and inner exception.
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="innerException">The inner exception</param>
    public ErrandException(string message, Exception innerException) : base(message, innerException)
    {
    }
}