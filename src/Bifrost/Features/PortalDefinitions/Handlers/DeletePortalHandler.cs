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

            return CommandResponse<DeletePortalResult>.Problem(new(false, NotFound: result.NotFound, ErrorDetails: result.ErrorDetails), "Portal not deleted");
        }
        catch (Exception ex)
        {
            DeletePortalResult result = new(false, ErrorDetails: ErrorDetails.SingleError(ex.GetType().Name, ex.Message));
            return CommandResponse<DeletePortalResult>.Problem(result, ex.Message);
        }
    }

    private async Task<(bool IsSuccess, bool NotFound, ErrorDetails? ErrorDetails)> DeletePortalAsync(DeletePortalCommand request)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return new(false, false, validationResult.ToErrorDetails());
        }

        try
        {
            await repository.DeleteAsync(request.Id);
        }
        catch (Exception ex) when (RepositoryExceptionHandler.ToErrorDetails(ex) is ErrorDetails error)
        {
            return new(false, true,  error);
        }

        return new(true, false, default);
    }
}
