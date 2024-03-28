using Bifrost.Models.Portals;
using Bifrost.Shared;
using FluentValidation;

namespace Bifrost.Queries.Portals;

public record GetPortalsQuery(
    int Limit = GetPortalsQuery.DefaultLimit,
    int Offset = GetPortalsQuery.DefaultOffset,
    string? Order = default) : IQuery<GetPortalsResult>
{
    public const int DefaultLimit = 50;
    public const int DefaultOffset = 0;
}

public record GetPortalsResult(IList<PortalDefinition>? Portals, int Total, ErrorDetails? ErrorDetails = default);

public class GetPortalsQueryValidator : AbstractValidator<GetPortalsQuery>
{
    public GetPortalsQueryValidator()
    {
        RuleFor(x => x.Limit)
            .GreaterThan(0).WithMessage("Limit must be greater than 0");

        RuleFor(x => x.Offset)
            .GreaterThanOrEqualTo(0).WithMessage("Offset must be greater than or equal to 0");
    }
}
