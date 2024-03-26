using Bifrost.Shared;
using FluentValidation;

namespace Bifrost.Commands.Portals;

public record DeletePortalCommand(string Id) : ICommand<DeletePortalResult>;

public record DeletePortalResult(bool Deleted, bool NotFound = false, ErrorDetails? ErrorDetails = default);

public class DeletePortalCommandValidator : AbstractValidator<DeletePortalCommand>
{
    public DeletePortalCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");
    }
}
