using Bifrost.Commands;
using Bifrost.Features.Identity.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Bifrost.Features.Identity.Handler;

public class RegistrationHandler(IIdentityService identityService) : IRequestHandler<RegisterCommand, CommandResponse<RegisterResult>>
{
    private readonly IIdentityService identityService = identityService;

    public async Task<CommandResponse<RegisterResult>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await identityService.RegisterAsync(
                request.UserName,
                request.Password,
                request.Email) ?? throw new InvalidOperationException("Login failed");

            if (result.Succeeded)
            {
                return CommandResponse<RegisterResult>.Ok(new(true), "Login successful");
            }

            return CommandResponse<RegisterResult>.Ok(new(false, GetLoginErrorDetails(result)), "Login failed");
        }
        catch (Exception ex)
        {
            RegisterResult result = new (false, new Dictionary<string, string[]>{
                { "666", [ ex.GetType().Name ] }
            });
            return CommandResponse<RegisterResult>.Problem(result, ex.Message);
        }
    }

    private static Dictionary<string, string[]> GetLoginErrorDetails(IdentityResult result)
    {
        var errorDetails = new Dictionary<string, string[]>(1);

        foreach (var error in result.Errors)
        {
            string[] newDescriptions;

            if (errorDetails.TryGetValue(error.Code, out var descriptions))
            {
                newDescriptions = new string[descriptions.Length + 1];
                Array.Copy(descriptions, newDescriptions, descriptions.Length);
                newDescriptions[descriptions.Length] = error.Description;
            }
            else
            {
                newDescriptions = [error.Description];
            }

            errorDetails[error.Code] = newDescriptions;
        }

        return errorDetails;
    }
}
