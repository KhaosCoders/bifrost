using Bifrost.Commands;
using System.Text.Json.Serialization;

namespace Bifrost.Queries;

public class QueryResponse<T> : CommandResponse
{
    public T Data { get; }

    [JsonConstructor]
    protected QueryResponse(T data, bool success, string? description)
        : base(success, description)
    {
        Data = data;
    }

    public static QueryResponse<T> Ok(T data, string? detail = default)
    {
        return new QueryResponse<T>(data, true, detail);
    }

    public static QueryResponse<T> Problem(T data, string detail)
    {
        return new QueryResponse<T>(data, false, detail);
    }
}