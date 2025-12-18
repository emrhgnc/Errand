using Errand.Core;

namespace Errand.Abstractions;

/// <summary>
/// Defines a handler for a request with a response.
/// "All we have to decide is what to do with the request that is given us." - Gandalf (adapted)
/// </summary>
/// <typeparam name="TRequest">The type of request being handled</typeparam>
/// <typeparam name="TResponse">The type of response from the handler</typeparam>
public interface IRequestHandler<in TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Handles a request and returns a response.
    /// </summary>
    /// <param name="request">The request to handle</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The response from the handler</returns>
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}

/// <summary>
/// Defines a handler for a request without a response (void).
/// </summary>
/// <typeparam name="TRequest">The type of request being handled</typeparam>
public interface IRequestHandler<in TRequest> : IRequestHandler<TRequest, Unit>
    where TRequest : IRequest<Unit>
{
    /// <summary>
    /// Handles a request without a response. 
    /// </summary>
    /// <param name="request">The request to handle</param>
    /// <param name="cancellationToken">Cancellation token</param>
    async Task<Unit> IRequestHandler<TRequest, Unit>.Handle(TRequest request, CancellationToken cancellationToken)
    {
        await Handle(request, cancellationToken);
        return Unit.Value;
    }

    /// <summary>
    /// Handles a request. 
    /// </summary>
    /// <param name="request">The request to handle</param>
    /// <param name="cancellationToken">Cancellation token</param>
    new Task Handle(TRequest request, CancellationToken cancellationToken);
}