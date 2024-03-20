using Bifrost.Shared;

namespace Bifrost.Utils.Validation;

public class ValidationResult
{
    public required bool IsValid { get; init; }

    public required IEnumerable<ValidationFault> Faults { get; init; }

    public ErrorDetails GetErrorDetails()
    {
        ErrorDetails errorDetails = [];

        foreach (var fault in Faults)
        {
            string[] newDescriptions;

            if (errorDetails.TryGetValue(fault.Property, out var descriptions))
            {
                newDescriptions = new string[descriptions.Length + 1];
                Array.Copy(descriptions, newDescriptions, descriptions.Length);
                newDescriptions[descriptions.Length] = fault.Message;
            }
            else
            {
                newDescriptions = [fault.Message];
            }

            errorDetails[fault.Property] = newDescriptions;
        }

        return errorDetails;
    }
}
