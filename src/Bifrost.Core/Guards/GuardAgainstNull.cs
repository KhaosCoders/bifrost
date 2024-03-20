using System.Runtime.CompilerServices;

namespace Bifrost.Guards;

public static class GuardAgainstNull
{
    public static bool IsNull<T>(this IGuard _, T? value, [CallerArgumentExpression(nameof(value))] string? name = default, bool throws = true)
    {
        if (value is not null) return true;
        if (throws) throw new ArgumentNullException(name ?? nameof(value));
        return false;
    }
}
