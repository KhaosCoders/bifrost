namespace Bifrost.Shared;

public  class ErrorDetails : Dictionary<string, string[]>
{
    public static readonly ErrorDetails Empty = [];

    public static ErrorDetails SingleError(string key, string value) =>
        new()
        {
            { key, [value] }
        };
}
