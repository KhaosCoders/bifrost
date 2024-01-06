namespace Bifrost.Client.Utils.Guards;

public static class GuardAgainstNull
{
    public static bool ObjectIsNull<T>(this IGuard _, T? value, string? name = default, bool @throw = false)
    {
        if (value is not null) return true;
        if (@throw) throw new ArgumentNullException(name ?? nameof(value));
        return false;
    }
}
