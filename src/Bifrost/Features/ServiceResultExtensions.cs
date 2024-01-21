using Microsoft.AspNetCore.Http.HttpResults;
using System.Diagnostics;

namespace Bifrost.Features;

internal static class ServiceResultExtensions
{
    internal static ValidationProblem CreateValidationProblem(this ServiceResultBase result)
    {
        Debug.Assert(!result.IsSuccess);
        var errorDictionary = new Dictionary<string, string[]>(1);

        foreach (var fault in result.Faults)
        {
            string[] newDescriptions;

            if (errorDictionary.TryGetValue(fault.Property, out var descriptions))
            {
                newDescriptions = new string[descriptions.Length + 1];
                Array.Copy(descriptions, newDescriptions, descriptions.Length);
                newDescriptions[descriptions.Length] = fault.Message;
            }
            else
            {
                newDescriptions = [fault.Message];
            }

            errorDictionary[fault.Property] = newDescriptions;
        }

        return TypedResults.ValidationProblem(errorDictionary);
    }
}
