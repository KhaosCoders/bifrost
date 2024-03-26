using Bifrost.Commands;
using Bifrost.Commands.Portals;
using Bifrost.Data;
using Bifrost.Data.Base;
using Bifrost.Extensions;
using Bifrost.Features.PortalDefinitions.Services;
using Bifrost.Shared;
using FluentValidation;
using MediatR;

namespace Bifrost.Features.PortalDefinitions.Handlers;

internal class DeletePortalHandler(
    IPortalDefinitionRepository repository,
    IValidator<DeletePortalCommand> validator) : IRequestHandler<DeletePortalCommand, CommandResponse<DeletePortalResult>>
{
    private readonly IPortalDefinitionRepository repository = repository;
    private readonly IValidator<DeletePortalCommand> validator = validator;

    public async Task<CommandResponse<DeletePortalResult>> Handle(DeletePortalCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await DeletePortalAsync(request);

            if (result.IsSuccess)
            {
                return CommandResponse<DeletePortalResult>.Ok(new(true), "Portal deleted");
            }

            return CommandResponse<DeletePortalResult>.Problem(new(false, ErrorDetails: result.ErrorDetails), "Portal not deleted");
        }
        catch (EntityNotFoundException)
        {
            DeletePortalResult result = new(false, true, ErrorDetails: ErrorDetails.SingleError("NotFound", $"Portal with ID {request.Id} not found"));
            return CommandResponse<DeletePortalResult>.Problem(result, "Portal not found");
        }
        catch (Exception ex)
        {
            DeletePortalResult result = new(false, ErrorDetails: ErrorDetails.SingleError(ex.GetType().Name, ex.Message));
            return CommandResponse<DeletePortalResult>.Problem(result, ex.Message);
        }
    }

    private async Task<(bool IsSuccess, ErrorDetails? ErrorDetails)> DeletePortalAsync(DeletePortalCommand request)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return new(false, validationResult.ToErrorDetails());
        }

        try
        {
            await repository.DeleteAsync(request.Id);
        }
        catch (Exception ex) when (RepositoryExceptionHandler.ToErrorDetails(ex) is ErrorDetails error)
        {
            return new(false,  error);
        }

        return new(true, default);
    }
}
