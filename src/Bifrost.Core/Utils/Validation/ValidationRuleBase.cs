namespace Bifrost.Utils.Validation;

public abstract class ValidationRuleBase
{
    public abstract ValidationFault? Validate();
}
