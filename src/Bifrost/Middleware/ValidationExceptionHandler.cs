using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;

namespace Bifrost.Middleware;

public class ValidationExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not ValidationException validationException)
        {
            return false;
        }

        var problemDetails = ToProblemDetails(validationException, httpContext);

        HttpResponse response = httpContext.Response;

        response.StatusCode = StatusCodes.Status400BadRequest;
        response.ContentType = "application/problem+json";
        await response.WriteAsJsonAsync(problemDetails, cancellationToken);

        return true;
    }

    private static ProblemDetails ToProblemDetails(ValidationException exception, HttpContext httpContext)
    {
        ProblemDetails problemDetails = new()
        {
            Title = "Validation Error",
            Status = StatusCodes.Status400BadRequest,
            Detail = "One or more validation errors occurred.",
        };

        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;
        problemDetails.Extensions["errors"] =
            exception.Errors
                .Select(err => new ErrorDetails(err.PropertyName, err.ErrorMessage, err.AttemptedValue))
                .ToArray();

        Uri baseUri = new($"https://{httpContext.Request.Host.Value}");
        if (httpContext.Features.Get<IHttpRequestFeature>() is IHttpRequestFeature requestFeature
            && Uri.TryCreate(baseUri, requestFeature.RawTarget, out Uri? uri))
        {
            problemDetails.Instance = uri.LocalPath;
        }

        return problemDetails;
    }

    internal record ErrorDetails(string PropertyName, string ErrorMessage, object AttemptedValue);
}
