using Bifrost.Client.Utils.Validation;

namespace Bifrost.Features.PortalDefinitions.Services;

internal record UpdatePortalResult(bool IsSuccess, IEnumerable<ValidationFault> Faults)
    : ServiceResultBase(IsSuccess, Faults);
