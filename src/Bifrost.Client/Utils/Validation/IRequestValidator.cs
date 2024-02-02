namespace Bifrost.Client.Utils.Validation;

public interface IRequestValidator<T>
{
    ValidationResult ValidateRequest(T request);
}
