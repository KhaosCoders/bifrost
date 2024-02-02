using System.Runtime.CompilerServices;

namespace Bifrost.Client.Utils.Guards;

public static class GuardAgainstLessThan
{
    public static bool IsLessThan<T>(this IGuard _, T value, T minimum, [CallerArgumentExpression(nameof(value))] string? name = default, bool throws = true) where T : IComparable<T>
    {
        if (value.CompareTo(minimum) >= 0) return true;
        if (throws) throw new ArgumentOutOfRangeException(name ?? nameof(value), value, $"Value cannot be less than {minimum}");
        return false;
    }
}
