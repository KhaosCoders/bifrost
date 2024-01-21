using Bifrost.Client.Utils.Validation;

namespace Bifrost.Features.Portals.Services;

internal record UpdatePortalResult(bool IsSuccess, IEnumerable<ValidationFault> Faults)
    : ServiceResultBase(IsSuccess, Faults);
