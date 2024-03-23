using Bifrost.Shared;
using FluentValidation.Results;

namespace Bifrost.Extensions;

public static class ValidationResultExtensions
{
    public static ErrorDetails ToErrorDetails(this ValidationResult result)
    {
        var dict = result.Errors
            .GroupBy(x => x.PropertyName)
            .ToDictionary(g => g.Key, g => g.Select(x => x.ErrorMessage).ToArray());

        ErrorDetails details = [];

        foreach (var x in dict)
        {
            details.Add(x.Key, x.Value);
        }

        return details;
    }
}
