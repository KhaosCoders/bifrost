﻿using Bifrost.Serialization;
using MediatR;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using System.Reflection;

namespace Bifrost;

public static class RpcBehavior
{
    public const string MediatRRpcEndpoint = "/mediatr-rpc";
}

public class RpcBehavior<TRequest, TResponse>(HttpClient httpClient) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly HttpClient _httpClient = httpClient;
    private readonly Assembly thisAssembly = typeof(RpcBehavior).Assembly;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Don't send requests to the server if they are defined and handled in this assembly
        if (typeof(TRequest).Assembly == thisAssembly)
        {
            return await next();
        }

        await using Stream stream = (await Serializer.SerializeAsync(request)) ?? throw new InvalidOperationException($"Request of Type {typeof(TRequest)} can't be serialized.");

        using StreamContent content = new(stream);
        content.Headers.ContentType = new("application/json");

        HttpRequestMessage message = new(HttpMethod.Post, RpcBehavior.MediatRRpcEndpoint)
        {
            Content = content
        };
        message.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

        var response = await _httpClient.SendAsync(message, cancellationToken);

        await using Stream responseStream = await response.Content.ReadAsStreamAsync(cancellationToken);

        var responseObject = await Serializer.TypedDeserializeAsync(responseStream);

        return responseObject is TResponse tRes ? tRes : throw new InvalidOperationException($"Response of Type {typeof(TResponse)} can't be deserialized.");
    }
}
