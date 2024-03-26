using Bifrost.Models.Portals;
using Bifrost.Shared;
using FluentValidation;

namespace Bifrost.Queries.Portals;

public record GetPortalQuery(string Id) : IQuery<GetPortalResult>;

public record GetPortalResult(PortalDefinition? Portal = default, ErrorDetails? ErrorDetails = default);

public class GetPortalQueryValidator : AbstractValidator<GetPortalQuery>
{
    public GetPortalQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");
    }
}
