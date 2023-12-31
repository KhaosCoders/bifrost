﻿namespace Bifrost.Client.Utils.Guards;

public static class GuardAgainstEmptyString
{
    public static bool StringIsNullOrWhitespace(this IGuard _, string? value, string? name = default, bool throws = true)
    {
        if (!string.IsNullOrWhiteSpace(value)) return true;
        if (throws) throw new ArgumentException("String cannot be null or whitespace", name ?? nameof(value));
        return false;
    }
}
