using Bifrost.Commands;
using Bifrost.Commands.Identity;
using Bifrost.Features.Identity.Services;
using MediatR;

namespace Bifrost.Features.Identity.Handler;

public class RefreshTokenHandler(IIdentityService identityService) : IRequestHandler<RefreshTokenCommand, CommandResponse<RefreshTokenResult>>
{
    private readonly IIdentityService identityService = identityService;

    public async Task<CommandResponse<RefreshTokenResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await identityService.RefreshAsync(request.RefreshToken);

            if (result != null)
            {
                return CommandResponse<RefreshTokenResult>.Ok(new(result), "Token refreshed");
            }

            return CommandResponse<RefreshTokenResult>.Problem(new(), "Token can't be refreshed");
        }
        catch (Exception ex) {
            return CommandResponse<RefreshTokenResult>.Problem(new(), ex.Message);
        }
    }
}
