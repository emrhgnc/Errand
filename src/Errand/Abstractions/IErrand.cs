namespace Errand.Abstractions;

/// <summary>
/// Defines the main Errand mediator interface to encapsulate request/response interaction patterns.
/// "Not all those who wander are lost" - but all requests need an Errand to reach their handlers.
/// </summary>
public interface IErrand
{
    /// <summary>
    /// Asynchronously sends a request to a single handler.
    /// </summary>
    /// <typeparam name="TResponse">The type of response from the handler</typeparam>
    /// <param name="request">The request object</param>
    /// <param name="cancellationToken">Optional cancellation token</param>
    /// <returns>A task that represents the send operation.  The task result contains the handler response.</returns>
    /// <exception cref="ArgumentNullException">Thrown when request is null</exception>
    /// <exception cref="HandlerNotFoundException">Thrown when no handler is found for the request</exception>
    Task<TResponse> Send<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default);
}