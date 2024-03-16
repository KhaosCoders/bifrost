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
