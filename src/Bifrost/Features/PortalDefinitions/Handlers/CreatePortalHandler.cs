using Bifrost.Commands;
using Bifrost.Commands.Portals;
using Bifrost.Data;
using Bifrost.Extensions;
using Bifrost.Features.PortalDefinitions.Services;
using Bifrost.Models.Portals;
using Bifrost.Shared;
using FluentValidation;
using MediatR;
using System.Security.Claims;

namespace Bifrost.Features.PortalDefinitions.Handlers;

internal class CreatePortalHandler(
    IHttpContextAccessor httpContextAccessor,
    IPortalDefinitionRepository repository,
    IValidator<CreatePortalCommand> validator) : IRequestHandler<CreatePortalCommand, CommandResponse<CreatePortalResult>>
{
    private readonly IValidator<CreatePortalCommand> validator = validator;
    private readonly IPortalDefinitionRepository repository = repository;
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;

    public async Task<CommandResponse<CreatePortalResult>> Handle(CreatePortalCommand request, CancellationToken cancellationToken)
    {
        try
        {
            if (httpContextAccessor?.HttpContext?.User?.Identity is not ClaimsIdentity user
                || string.IsNullOrWhiteSpace(user.Name))
            {
                return CommandResponse<CreatePortalResult>.Problem(new(UnauthorizedRequest: true), "User not logged in");
            }

            var result = await CreatePortalAsync(request, user.Name);

            if (result.IsSuccess && result.Portal != null)
            {
                return CommandResponse<CreatePortalResult>.Ok(new(result.Portal.Id), "Portal created");
            }

            return CommandResponse<CreatePortalResult>.Problem(new(ErrorDetails: result.ErrorDetails), "Portal not created");
        }
        catch (Exception ex)
        {
            CreatePortalResult result = new(ErrorDetails: ErrorDetails.SingleError(ex.GetType().Name, ex.Message));
            return CommandResponse<CreatePortalResult>.Problem(result, ex.Message);
        }
    }

    private async Task<(bool IsSuccess, PortalDefinition? Portal, ErrorDetails? ErrorDetails)> CreatePortalAsync(CreatePortalCommand request, string creatorName)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return new(false, null, validationResult.ToErrorDetails());
        }

        PortalDefinition definition = new()
        {
            CreationDate = DateTime.UtcNow,
            CreationUser = creatorName,
            Name = request.Name,
            MaxInstanceCount = request.MaxInstanceCount,
            VpnType = request.VpnType,
            VpnConfig = request.VpnConfig ?? string.Empty
        };

        try
        {
            await repository.CreateAsync(definition);
        }
        catch (Exception ex) when (RepositoryExceptionHandler.ToErrorDetails(ex) is ErrorDetails error)
        {
            return new(false, null, error);
        }

        return new(true, definition, default);
    }
}
