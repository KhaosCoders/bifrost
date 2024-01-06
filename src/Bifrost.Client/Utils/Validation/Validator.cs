namespace Bifrost.Client.Utils.Validation;

public static class Validator
{
    public static ValidationResult Validate(params ValidationRule[] rules)
    {
        var faults = rules
            .Select(rule => rule.Validate())
            .Where(fault => fault is not null)
            .Cast<ValidationFault>();
        return new ValidationResult { IsValid = !faults.Any(), Faults = faults };
    }
}
