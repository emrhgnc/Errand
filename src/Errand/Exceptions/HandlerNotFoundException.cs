namespace Errand.Exceptions;

/// <summary>
/// Exception thrown when no handler is found for a request.
/// "One does not simply forget to register a handler." - Boromir
/// </summary>
public class HandlerNotFoundException : ErrandException
{
    /// <summary>
    /// Initializes a new instance of HandlerNotFoundException. 
    /// </summary>
    public HandlerNotFoundException() : base()
    {
    }

    /// <summary>
    /// Initializes a new instance of HandlerNotFoundException with a message.
    /// </summary>
    /// <param name="message">The error message</param>
    public HandlerNotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of HandlerNotFoundException with a message and inner exception. 
    /// </summary>
    /// <param name="message">The error message</param>
    /// <param name="innerException">The inner exception</param>
    public HandlerNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}