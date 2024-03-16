using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Bifrost.Serialization;

public static class Serializer
{
    public static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        DefaultIgnoreCondition = JsonIgnoreCondition.Never,
        IncludeFields = true,
        TypeInfoResolver = new DefaultJsonTypeInfoResolver()
        {
            Modifiers = { JsonTypeModifier.AddTypeInfoModifier }
        },
    };

    public static async Task<Stream> SerializeAsync<T>(T request)
    {
        MemoryStream stream = new();
        await JsonSerializer.SerializeAsync(stream, request, JsonOptions);
        stream.Position = 0;
        return stream;
    }

    public static ValueTask<T?> DeserializeAsync<T>(Stream data) =>
        JsonSerializer.DeserializeAsync<T>(data, JsonOptions);

    public static async ValueTask<object?> TypedDeserializeAsync(Stream data)
    {
        await using MemoryStream memoryStream = new();
        await data.CopyToAsync(memoryStream);
        memoryStream.Position = 0;

        // Get the type name from the JSON object
        if (await DeserializeAsync<JsonObject>(memoryStream) is not JsonObject root
            || root["$type"] is not JsonValue typeProperty
            || !typeProperty.TryGetValue(out string? typeName)
            || string.IsNullOrWhiteSpace(typeName)
            || Type.GetType(typeName) is not Type type)
        {
            return default;
        }

        memoryStream.Position = 0;
        return await JsonSerializer.DeserializeAsync(memoryStream, type, JsonOptions);
    }
}
