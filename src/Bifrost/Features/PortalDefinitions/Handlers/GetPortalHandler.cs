using Bifrost.Commands;
using Bifrost.Data;
using Bifrost.Extensions;
using Bifrost.Features.PortalDefinitions.Services;
using Bifrost.Models.Portals;
using Bifrost.Queries.Portals;
using Bifrost.Shared;
using FluentValidation;
using MediatR;

namespace Bifrost.Features.PortalDefinitions.Handlers;

internal class GetPortalHandler(
    IPortalDefinitionRepository repository,
    IValidator<GetPortalQuery> validator) : IRequestHandler<GetPortalQuery, CommandResponse<GetPortalResult>>
{
    private readonly IValidator<GetPortalQuery> validator = validator;
    private readonly IPortalDefinitionRepository repository = repository;

    public async Task<CommandResponse<GetPortalResult>> Handle(GetPortalQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await GetPortalAsync(request);

            if (result.IsSuccess && result.Portal != default)
            {
                return CommandResponse<GetPortalResult>.Ok(new(result.Portal), "Found Portal");
            }

            return CommandResponse<GetPortalResult>.Problem(new(ErrorDetails: result.ErrorDetails), "Portal not found");
        }
        catch (Exception ex)
        {
            GetPortalResult result = new(ErrorDetails: ErrorDetails.SingleError(ex.GetType().Name, ex.Message));
            return CommandResponse<GetPortalResult>.Problem(result, ex.Message);
        }
    }

    private async Task<(bool IsSuccess, PortalDefinition? Portal, ErrorDetails? ErrorDetails)> GetPortalAsync(GetPortalQuery request)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return new(false, null, validationResult.ToErrorDetails());
        }

        try
        {
            var result = await repository.GetByIdAsync(request.Id);
            return new(true, result, default);
        }
        catch (Exception ex) when (RepositoryExceptionHandler.ToErrorDetails(ex) is ErrorDetails error)
        {
            return new(false, null, error);
        }
    }

}
