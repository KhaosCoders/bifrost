namespace Bifrost.Client.Utils.Validation;

public abstract class ValidationRuleBase
{
    public abstract ValidationFault? Validate();
}
