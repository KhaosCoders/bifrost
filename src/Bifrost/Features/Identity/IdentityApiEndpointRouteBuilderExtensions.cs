using Bifrost.Commands.Identity;
using MediatR;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Bifrost.Features.Identity;

using LoginResult = Results<Ok<AccessTokenResponse>, EmptyHttpResult, ProblemHttpResult>;
using RefreshResult = Results<Ok<AccessTokenResponse>, UnauthorizedHttpResult, SignInHttpResult, ChallengeHttpResult>;
using RegisterResult = Results<Created, ValidationProblem>;

/// <summary>
/// Provides extension methods for <see cref="IEndpointRouteBuilder"/> to add identity endpoints.
/// </summary>
public static class IdentityApiEndpointRouteBuilderExtensions
{
    /// <summary>
    /// Add endpoints for registration, logging in and logging out using ASP.NET Core Identity.
    /// </summary>
    /// <typeparam name="TUser">The type describing the user. This should match the generic parameter in <see cref="UserManager{TUser}"/>.</typeparam>
    /// <param name="endpoints">
    /// The <see cref="IEndpointRouteBuilder"/> to add the identity endpoints to.
    /// Call <see cref="EndpointRouteBuilderExtensions.MapGroup(IEndpointRouteBuilder, string)"/> to add a prefix to all the endpoints.
    /// </param>
    /// <returns>An <see cref="IEndpointConventionBuilder"/> to further customize the added endpoints.</returns>
    public static void MapIdentityApiWithUsername<TUser>(this IEndpointRouteBuilder endpoints)
       where TUser : class, new()
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        endpoints.MapPost("/register", async Task<RegisterResult> (
            [FromBody] RegisterCommand registration,
            [FromServices] IMediator mediator) =>
        {
            var result = await mediator.Send(registration);
            return !result.Success || !result.Data.Success
                ? (RegisterResult)TypedResults.ValidationProblem(result.Data.ErrorDetails!)
                : (RegisterResult)TypedResults.Created();
        });

        endpoints.MapPost("/login", async Task<LoginResult> (
            [FromBody] LoginCommand login,
            [FromServices] IMediator mediator) =>
        {
            var result = await mediator.Send(login);
            return !result.Success || !result.Data.Succeeded
                ? (LoginResult)TypedResults.Problem(result.Description, statusCode: StatusCodes.Status401Unauthorized)
                : (LoginResult)TypedResults.Empty;
        });

        endpoints.MapPost("/refresh", async Task<RefreshResult> (
            [FromBody] RefreshTokenCommand refreshRequest,
            [FromServices] IMediator mediator) =>
        {
            var result = await mediator.Send(refreshRequest);
            return !result.Success || result.Data.NewPrincipal == default
                ? (RefreshResult)TypedResults.Challenge()
                : (RefreshResult)TypedResults.SignIn(result.Data.NewPrincipal, authenticationScheme: IdentityConstants.BearerScheme);
        });
    }
}
