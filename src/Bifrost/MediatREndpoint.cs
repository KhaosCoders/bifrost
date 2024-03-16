
using Bifrost.Serialization;
using MediatR;
using System.Diagnostics;
using MediatoRResult = Microsoft.AspNetCore.Http.HttpResults.Results<
                        Microsoft.AspNetCore.Http.HttpResults.ContentHttpResult,
                        Microsoft.AspNetCore.Http.HttpResults.ProblemHttpResult>;

namespace Bifrost;

public static class MediatREndpoint
{
    public static void UseMediatRRpc(this WebApplication app)
    {
        app.MapPost(RpcBehavior.MediatRRpcEndpoint, (Delegate)HandleRpc);
    }

    private static async Task<MediatoRResult> HandleRpc(HttpContext context)
    {
        try
        {
            object request = (await Serializer.TypedDeserializeAsync(context.Request.Body))
                ?? throw new InvalidOperationException("Request can't be deserialized");

            IMediator? mediator = context.RequestServices.GetService<IMediator>();
            Debug.Assert(mediator != null, nameof(mediator) + " != null");

            object? commandQueryResponse = await mediator.Send(request);

            Stream response = (await Serializer.SerializeAsync(commandQueryResponse))
                ?? throw new InvalidOperationException("Response can't be serialized");

            using StreamReader reader = new(response);
            return TypedResults.Content(await reader.ReadToEndAsync(), "application/json");
        }
        catch(Exception ex)
        {
            return TypedResults.Problem(ex.Message);
        }
    }
}
