using System.Linq.Expressions;

namespace Bifrost.Client.Utils.Validation;

public static class Validator
{
    public static ValidationRuleBase Rule<T>(Expression<Func<T>> accessor, Func<T, bool> validator) =>
        new ValidationRule<T>(accessor, validator);

    public static ValidationResult Validate(params ValidationRuleBase[] rules)
    {
        var faults = rules
            .Select(rule => rule.Validate())
            .Where(fault => fault is not null)
            .Cast<ValidationFault>();
        return new ValidationResult { IsValid = !faults.Any(), Faults = faults };
    }
}
