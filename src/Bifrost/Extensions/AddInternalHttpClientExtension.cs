namespace Bifrost.Extensions;

public static class AddInternalHttpClientExtension
{
    public static void AddInternalHttpClient(this IServiceCollection services, ConfigurationManager configuration)
    {
        string? baseURLs = configuration["ASPNETCORE_URLS"];
        if (string.IsNullOrWhiteSpace(baseURLs))
        {
            throw new Exception("ASPNETCORE_URLS environment variable is not set.");
        }

        services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(baseURLs.Split(';')[0]) });
    }
}
