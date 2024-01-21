using Bifrost.Client.Utils.Validation;

namespace Bifrost.Features;

internal record ServiceResultBase(bool IsSuccess, IEnumerable<ValidationFault> Faults);
