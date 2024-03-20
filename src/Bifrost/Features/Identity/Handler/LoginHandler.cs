using Bifrost.Commands;
using Bifrost.Commands.Identity;
using Bifrost.Features.Identity.Services;
using MediatR;

namespace Bifrost.Features.Identity.Handler;

public class LoginHandler(IIdentityService identityService) : IRequestHandler<LoginCommand, CommandResponse<LoginResult>>
{
    private readonly IIdentityService identityService = identityService;

    public async Task<CommandResponse<LoginResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await identityService.LoginAsync(
                request.Username,
                request.Password,
                request.TwoFactorCode,
                request.TwoFactorRecoveryCode,
                request.UseCookie,
                request.UseSession);

            return CommandResponse<LoginResult>.Ok(new(result.Succeeded, result.RequiresTwoFactor, result.IsLockedOut));
        }
        catch(Exception ex)
        {
            return CommandResponse<LoginResult>.Problem(new(Succeeded: false, RequiresTwoFactor: false, IsLockedOut: false), ex.Message);
        }
    }
}
