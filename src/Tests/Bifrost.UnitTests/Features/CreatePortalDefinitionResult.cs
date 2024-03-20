using Bifrost.Models.Portals;

namespace Bifrost.UnitTests.Utils.Validation;

internal record CreatePortalDefinitionResult(bool IsSuccess, PortalDefinition? Portal, ValidationResult ValidationResult)
{
}
