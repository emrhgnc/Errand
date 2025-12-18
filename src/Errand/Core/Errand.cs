using Errand.Abstractions;
using Errand.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Errand.Core;

/// <summary>
/// Default implementation of <see cref="IErrand"/>.
/// "The errand is mine, and I will see it done." - Errand
/// </summary>
public sealed class Errand : IErrand
{
    private readonly IServiceProvider _serviceProvider;
    // Change the type of _requestHandlers from ConcurrentDictionary<Type, object> to ConcurrentDictionary<RequestHandlerKey, object>
    private static readonly ConcurrentDictionary<RequestHandlerKey, object> _requestHandlers = new();

    /// <summary>
    /// Initializes a new instance of Errand.
    /// </summary>
    /// <param name="serviceProvider">Service provider for resolving handlers</param>
    public Errand(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <inheritdoc />
    public async Task<TResponse> Send<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        var requestType = request.GetType();
        var responseType = typeof(TResponse);

        var handler = GetOrAddRequestHandler(requestType, responseType);

        if (handler == null)
        {
            throw new HandlerNotFoundException(
                $"No handler found for request type '{requestType.FullName}' with response type '{responseType.FullName}'.  " +
                $"Make sure you have registered the handler using AddErrand().");
        }

        var result = await InvokeHandler(handler, request, cancellationToken);
        return (TResponse)result;
    }

    private object? GetOrAddRequestHandler(Type requestType, Type responseType)
    {
        var cacheKey = new RequestHandlerKey(requestType, responseType);

        if (_requestHandlers.TryGetValue(cacheKey, out var cachedWrapper))
        {
            return cachedWrapper;
        }

        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, responseType);
        var handler = _serviceProvider.GetService(handlerType);

        if (handler == null)
        {
            return null;
        }

        var wrapperType = typeof(RequestHandlerWrapper<,>).MakeGenericType(requestType, responseType);
        var wrapper = Activator.CreateInstance(wrapperType, handler, _serviceProvider);

        _requestHandlers.TryAdd(cacheKey, wrapper!);
        return wrapper;
    }

    private static async Task<object?> InvokeHandler(object handler, object request, CancellationToken cancellationToken)
    {
        var handleMethod = handler.GetType().GetMethod(nameof(RequestHandlerWrapper<IRequest<Unit>, Unit>.Handle));

        if (handleMethod == null)
        {
            throw new InvalidOperationException($"Handle method not found on handler wrapper.");
        }

        var task = (Task<object?>)handleMethod.Invoke(handler, new[] { request, cancellationToken })!;
        return await task.ConfigureAwait(false);
    }

    private readonly struct RequestHandlerKey : IEquatable<RequestHandlerKey>
    {
        public readonly Type RequestType;
        public readonly Type ResponseType;

        public RequestHandlerKey(Type requestType, Type responseType)
        {
            RequestType = requestType;
            ResponseType = responseType;
        }

        public bool Equals(RequestHandlerKey other) =>
            RequestType == other.RequestType && ResponseType == other.ResponseType;

        public override bool Equals(object? obj) =>
            obj is RequestHandlerKey other && Equals(other);

        public override int GetHashCode() =>
            HashCode.Combine(RequestType, ResponseType);
    }
}

internal abstract class RequestHandlerWrapperBase
{
    public abstract Task<object?> Handle(object request, CancellationToken cancellationToken);
}

internal class RequestHandlerWrapper<TRequest, TResponse> : RequestHandlerWrapperBase
    where TRequest : IRequest<TResponse>
{
    private readonly IRequestHandler<TRequest, TResponse> _handler;
    private readonly IServiceProvider _serviceProvider;

    public RequestHandlerWrapper(IRequestHandler<TRequest, TResponse> handler, IServiceProvider serviceProvider)
    {
        _handler = handler;
        _serviceProvider = serviceProvider;
    }

    public override async Task<object?> Handle(object request, CancellationToken cancellationToken)
    {
        var typedRequest = (TRequest)request;

        var behaviors = _serviceProvider
            .GetServices<IPipelineBehavior<TRequest, TResponse>>()
            .Reverse()
            .ToList();

        async Task<TResponse> Handler() => await _handler.Handle(typedRequest, cancellationToken);

        if (behaviors.Count == 0)
        {
            return await Handler();
        }

        RequestHandlerDelegate<TResponse> pipeline = Handler;

        foreach (var behavior in behaviors)
        {
            var currentBehavior = behavior;
            var next = pipeline;
            pipeline = () => currentBehavior.Handle(typedRequest, next, cancellationToken);
        }

        return await pipeline();
    }
}