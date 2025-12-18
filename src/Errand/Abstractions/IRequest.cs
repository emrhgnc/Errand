using Errand.Core;

namespace Errand.Abstractions;

/// <summary>
/// Marker interface to represent a request with a response.
/// "I will take the request, though I do not know the way." - Frodo
/// </summary>
/// <typeparam name="TResponse">The type of response returned by the handler</typeparam>
public interface IRequest<out TResponse>
{
}

/// <summary>
/// Marker interface to represent a request without a response (void).
/// </summary>
public interface IRequest : IRequest<Unit>
{
}