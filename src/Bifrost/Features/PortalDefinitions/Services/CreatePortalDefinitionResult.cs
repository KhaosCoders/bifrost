using Bifrost.Client.Utils.Validation;
using Bifrost.Features.PortalDefinitions.Model;

namespace Bifrost.Features.PortalDefinitions.Services;

internal record CreatePortalDefinitionResult(bool IsSuccess, PortalDefinition? Portal, IEnumerable<ValidationFault> Faults)
    : ServiceResultBase(IsSuccess, Faults);
