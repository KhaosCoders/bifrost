using Bifrost.Commands;
using Bifrost.Features.Identity.Services;
using Microsoft.AspNetCore.Authentication.BearerToken;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using DTO = Bifrost.DTOs;

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
    public static IEndpointConventionBuilder MapIdentityApiWithUsername<TUser>(this IEndpointRouteBuilder endpoints)
       where TUser : class, new()
    {
        ArgumentNullException.ThrowIfNull(endpoints);

        var routeGroup = endpoints.MapGroup("");

        routeGroup.MapPost("/register", async Task<RegisterResult> (
            [FromBody] DTO.RegisterRequest registration,
            [FromServices] IIdentityService identityService) =>
        {
            var result = await identityService.RegisterAsync(registration.UserName, registration.Password, registration.Email);
            return !result.Succeeded
                ? (RegisterResult)CreateValidationProblem(result)
                : (RegisterResult)TypedResults.Created();
        });

        routeGroup.MapPost("/login", async Task<LoginResult> (
            [FromBody] LoginCommand login,
            [FromQuery] bool? useCookies,
            [FromQuery] bool? useSessionCookies,
            [FromServices] IIdentityService identityService) =>
        {
            var result = await identityService.LoginAsync(login.Username, login.Password, login.TwoFactorCode, login.TwoFactorRecoveryCode, useCookies, useSessionCookies);
            return !result.Succeeded
                ? (LoginResult)TypedResults.Problem(result.ToString(), statusCode: StatusCodes.Status401Unauthorized)
                : (LoginResult)TypedResults.Empty;
        });

        routeGroup.MapPost("/refresh", async Task<RefreshResult> (
            [FromBody] DTO.RefreshRequest refreshRequest,
            [FromServices] IIdentityService identityService) =>
        {
            var newPrincipal = await identityService.RefreshAsync(refreshRequest.RefreshToken);
            return newPrincipal == null
                ? (RefreshResult)TypedResults.Challenge()
                : (RefreshResult)TypedResults.SignIn(newPrincipal, authenticationScheme: IdentityConstants.BearerScheme);
        });

        return new IdentityEndpointsConventionBuilder(routeGroup);
    }

    private static ValidationProblem CreateValidationProblem(IdentityResult result)
    {
        // We expect a single error code and description in the normal case.
        // This could be golfed with GroupBy and ToDictionary, but perf! :P
        Debug.Assert(!result.Succeeded);
        var errorDictionary = new Dictionary<string, string[]>(1);

        foreach (var error in result.Errors)
        {
            string[] newDescriptions;

            if (errorDictionary.TryGetValue(error.Code, out var descriptions))
            {
                newDescriptions = new string[descriptions.Length + 1];
                Array.Copy(descriptions, newDescriptions, descriptions.Length);
                newDescriptions[descriptions.Length] = error.Description;
            }
            else
            {
                newDescriptions = [error.Description];
            }

            errorDictionary[error.Code] = newDescriptions;
        }

        return TypedResults.ValidationProblem(errorDictionary);
    }

    private sealed class IdentityEndpointsConventionBuilder(RouteGroupBuilder inner) : IEndpointConventionBuilder
    {
        private IEndpointConventionBuilder InnerAsConventionBuilder => inner;

        public void Add(Action<EndpointBuilder> convention) => InnerAsConventionBuilder.Add(convention);
        public void Finally(Action<EndpointBuilder> finallyConvention) => InnerAsConventionBuilder.Finally(finallyConvention);
    }
}
