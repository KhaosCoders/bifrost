using System.Text.Json.Serialization;

namespace Bifrost.Commands;

public class CommandResponse
{
    public bool Success { get; }
    public string? Description { get; }

    [JsonConstructor]
    protected CommandResponse(bool success, string? description)
    {
        Success = success;
        Description = description;
    }

    public static CommandResponse Ok(string? detail)
    {
        return new CommandResponse(true, detail);
    }

    public static CommandResponse Problem(string detail)
    {
        return new CommandResponse(false, detail);
    }
}

public class CommandResponse<T> : CommandResponse
{
    public T Data { get; }

    [JsonConstructor]
    protected CommandResponse(T data, bool success, string? description)
        : base(success, description)
    {
        Data = data;
    }

    public static CommandResponse<T> Ok(T data, string? detail = default)
    {
        return new CommandResponse<T>(data, true, detail);
    }

    public static CommandResponse<T> Problem(T data, string detail)
    {
        return new CommandResponse<T>(data, false, detail);
    }
}