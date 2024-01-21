using Bifrost.Features.PortalDefinitions.Model;

namespace Bifrost.UnitTests.Utils.Validation;

internal record CreatePortalDefinitionResult(bool IsSuccess, PortalDefinition? Portal, ValidationResult ValidationResult)
{
}
