using Microsoft.FluentUI.AspNetCore.Components;

namespace Bifrost.Extensions;

public static class GridItemsProviderRequestExtensions
{
    public static string? GetOrder<T>(this GridItemsProviderRequest<T> request) =>
        request.GetSortByProperties() is IReadOnlyCollection<SortedProperty> { Count: > 0 } collection
            && collection.FirstOrDefault() is SortedProperty property
                ? $"{(property.Direction == SortDirection.Descending ? '-' : string.Empty)}{property.PropertyName}" : null;
}
