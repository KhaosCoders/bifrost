using Bifrost.Data.Base;
using Bifrost.Features.PortalDefinitions.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DTO = Bifrost.DTOs;

namespace Bifrost.Features.PortalDefinitions;

using CreateResult = Results<Created, UnauthorizedHttpResult, ValidationProblem>;
using GetResult = Results<Ok<Model.PortalDefinition>, NotFound, UnauthorizedHttpResult>;
using DeleteResult = Results<NoContent, NotFound, UnauthorizedHttpResult>;
using PutResult = Results<NoContent, NotFound, UnauthorizedHttpResult, ValidationProblem>;

public static class PortalFeatureExtensions
{
    public static void AddPortalFeature(this IServiceCollection services)
    {
        // Repositories
        services.AddScoped<IPortalDefinitionRepository, PortalDefinitionRepository>();

        // Services
        services.AddScoped<IPortalDefinitionService, PortalDefinitionService>();

        // Server-Side-Requests
    }

    public static void MapPortalFeature(this IEndpointRouteBuilder app)
    {
        var api = app.MapGroup("/api")
            .MapGroup("")
                .RequireAuthorization("ApiPolicy");
        api.MapPost("/portals", async Task<CreateResult> (
            [FromBody] DTO.PortalRequest request,
            [FromServices] IPortalDefinitionService service,
            [FromServices] IHttpContextAccessor httpContextAccessor
            ) =>
        {
            if (httpContextAccessor?.HttpContext?.User?.Identity is not ClaimsIdentity user)
                return TypedResults.Unauthorized();

            var result = await service.CreatePortalAsync(request, user.Name!);
            return result.IsSuccess
                ? TypedResults.Created($"/api/portals/{result.Portal!.Id}")
                : result.CreateValidationProblem();
        });

        api.MapGet("/portals/{id}", async Task<GetResult> (
            [FromRoute] string id,
            [FromServices] IPortalDefinitionService service
            ) =>
        {
            var result = await service.GetPortalAsync(id);
            return result is null
                ? TypedResults.NotFound()
                : TypedResults.Ok(result);
        });

        api.MapDelete("/portals/{id}", async Task<DeleteResult> (
            [FromRoute] string id,
            [FromServices] IPortalDefinitionService service
            ) =>
        {
            try
            {
                await service.DeletePortalAsync(id);
            }
            catch (EntityNotFoundException)
            {
                return TypedResults.NotFound();
            }

            return TypedResults.NoContent();
        });

        api.MapPut("/portals/{id}", async Task<PutResult> (
            [FromRoute] string id,
            [FromBody] DTO.PortalRequest request,
            [FromServices] IPortalDefinitionService service
            ) =>
        {
            var result = await service.UpdatePortalAsync(id, request);
            return result.IsSuccess
                ? TypedResults.NoContent()
                : result.CreateValidationProblem();
        });
    }
}
