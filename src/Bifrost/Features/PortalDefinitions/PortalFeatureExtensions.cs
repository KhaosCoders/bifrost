using Bifrost.Commands.Portals;
using Bifrost.Features.PortalDefinitions.Services;
using Bifrost.Models.Portals;
using Bifrost.Queries.Portals;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Bifrost.Features.PortalDefinitions;

using CreateResult = Results<Created, UnauthorizedHttpResult, ValidationProblem, ProblemHttpResult>;
using DeleteResult = Results<NoContent, NotFound, UnauthorizedHttpResult, ProblemHttpResult>;
using GetPortalResult = Results<Ok<PortalDefinition>, NotFound, UnauthorizedHttpResult>;
using GetPortalsResult = Results<Ok<IEnumerable<PortalDefinition>>, UnauthorizedHttpResult, ProblemHttpResult>;
using PutResult = Results<NoContent, NotFound, UnauthorizedHttpResult, ValidationProblem, ProblemHttpResult>;

public static class PortalFeatureExtensions
{
    public static void AddPortalFeature(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<IPortalDefinitionRepository, PortalDefinitionRepository>();

        // Services
        services.AddScoped<IPortalDefinitionService, PortalDefinitionService>();
    }

    public static void MapPortalFeature(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api")
            .MapGroup("")
                .RequireAuthorization("ApiPolicy");
        api.MapPost("/portals", async Task<CreateResult> (
            [FromBody] CreatePortalCommand request,
            [FromServices] IMediator mediator
            ) =>
        {
            var result = await mediator.Send(request);

            if (result.Data.UnauthorizedRequest)
            {
                return TypedResults.Unauthorized();
            }
            else if (result.Success && !string.IsNullOrWhiteSpace(result.Data.PortalId))
            {
                return TypedResults.Created($"/api/portals/{result.Data.PortalId}");
            }
            else if (result.Data.ErrorDetails != null)
            {
                return TypedResults.ValidationProblem(result.Data.ErrorDetails);
            }

            return TypedResults.Problem($"Unknown Error: {result.Description}");
        });

        api.MapPut("/portals/{id}", async Task<PutResult> (
            [FromRoute] string id,
            [FromBody] UpdatePortalCommand request,
            [FromServices] IMediator mediator
            ) =>
        {
            var result = await mediator.Send(request with { Id = id });

            if (result.Success)
            {
                return TypedResults.NoContent();
            }
            else if (result.Data.ErrorDetails != null)
            {
                return TypedResults.ValidationProblem(result.Data.ErrorDetails);
            }

            return TypedResults.Problem($"Unknown Error: {result.Description}");
        });

        api.MapGet("/portals", async Task<GetPortalsResult>(
            [FromQuery] int? limit,
            [FromQuery] int? offset,
            [FromServices] IMediator mediator
            ) =>
        {
            var result = await mediator.Send(new GetPortalsQuery(limit, offset));

            return result.Success
                ? TypedResults.Ok((IEnumerable<PortalDefinition>)result.Data.Portals)
                : TypedResults.Problem(result.Description);
        });

        api.MapGet("/portals/{id}", async Task<GetPortalResult> (
            [FromRoute] string id,
            [FromServices] IMediator mediator
            ) =>
        {
            var result = await mediator.Send(new GetPortalQuery(id));

            return result.Success && result.Data.Portal != null
                ? TypedResults.Ok(result.Data.Portal)
                : TypedResults.NotFound();
        });

        api.MapDelete("/portals/{id}", async Task<DeleteResult> (
            [FromRoute] string id,
            [FromServices] IMediator mediator
            ) =>
        {
            var result = await mediator.Send(new DeletePortalCommand(id));

            if (result.Success)
            {
                return TypedResults.NoContent();
            }
            else if (result.Data.NotFound)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.Problem(result.Description);
        });
    }
}
