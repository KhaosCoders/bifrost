namespace Bifrost.Client.Utils.Validation;

public class ValidationResult
{
    public required bool IsValid { get; init; }

    public required IEnumerable<ValidationFault> Faults { get; init; }
}
