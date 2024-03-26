using Bifrost.Commands;
using Bifrost.Data;
using Bifrost.Extensions;
using Bifrost.Features.PortalDefinitions.Services;
using Bifrost.Models.Portals;
using Bifrost.Queries.Portals;
using Bifrost.Shared;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Bifrost.Features.PortalDefinitions.Handlers;

internal class GetPortalsHandler(
    IPortalDefinitionRepository repository,
    IValidator<GetPortalsQuery> validator) : IRequestHandler<GetPortalsQuery, CommandResponse<GetPortalsResult>>
{
    private readonly IValidator<GetPortalsQuery> validator = validator;
    private readonly IPortalDefinitionRepository repository = repository;

    public async Task<CommandResponse<GetPortalsResult>> Handle(GetPortalsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = GetPortals(request);

            if (result.IsSuccess && result.Portals != default)
            {
                var portals = await result.Portals.ToListAsync(cancellationToken: cancellationToken);
                return CommandResponse<GetPortalsResult>.Ok(new(portals), "Got portal list");
            }

            return CommandResponse<GetPortalsResult>.Problem(new([], ErrorDetails: result.ErrorDetails), "Portal not found");
        }
        catch (Exception ex)
        {
            GetPortalsResult result = new([], ErrorDetails: ErrorDetails.SingleError(ex.GetType().Name, ex.Message));
            return CommandResponse<GetPortalsResult>.Problem(result, ex.Message);
        }
    }

    private (bool IsSuccess, IQueryable<PortalDefinition>? Portals, ErrorDetails? ErrorDetails) GetPortals(GetPortalsQuery request)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return new(false, null, validationResult.ToErrorDetails());
        }

        try
        {
            var result = repository.QueryAll().Skip(request.Offset).Take(request.Limit);
            return new(true, result, default);
        }
        catch (Exception ex) when (RepositoryExceptionHandler.ToErrorDetails(ex) is ErrorDetails error)
        {
            return new(false, null, error);
        }
    }
}
