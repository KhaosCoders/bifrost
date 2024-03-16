using System.Text.Json.Serialization.Metadata;

namespace Bifrost.Serialization;

public static class JsonTypeModifier
{
    public static void AddTypeInfoModifier(JsonTypeInfo typeInfo)
    {
        if (typeInfo.Kind != JsonTypeInfoKind.Object)
        {
            return;
        }

        string? typeName = typeInfo.Type.AssemblyQualifiedName;
        if (typeName is null)
        {
            return;
        }

        var typeProperty = typeInfo.CreateJsonPropertyInfo(typeof(string), "$type");
        typeProperty.Get = (_) => typeName;
        if (typeInfo.Properties.Any())
        {
            typeProperty.Order = typeInfo.Properties.Max(x => x.Order) + 1;
        }
        typeInfo.Properties.Add(typeProperty);
    }
}
