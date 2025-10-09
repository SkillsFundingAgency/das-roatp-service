#nullable enable

using System;

namespace SFA.DAS.RoATPService.Application.Api.Serialization;

public readonly struct Optional<T> : IEquatable<Optional<T>>
{
    public bool HasValue { get; }
    public T? Value { get; }

    private Optional(T? value, bool hasValue)
    {
        Value = value;
        HasValue = hasValue;
    }

    public static Optional<T> Missing() => new Optional<T>(default, false);
    public static Optional<T> FromValue(T? value) => new Optional<T>(value, true);

    // 🔹 Implicit conversions
    public static implicit operator Optional<T>(T? value) => FromValue(value);
    public static explicit operator T?(Optional<T> optional) => optional.Value;

    // 🔹 Helper: Get value or default if missing
    public T? GetValueOrDefault(T? defaultValue = default) => HasValue ? Value : defaultValue;

    // 🔹 Equality + ToString
    public bool Equals(Optional<T> other)
    {
        if (HasValue != other.HasValue) return false;
        if (!HasValue) return true; // both missing
        return Equals(Value, other.Value);
    }

    public override bool Equals(object? obj) => obj is Optional<T> other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(HasValue, Value);
    public override string ToString() => HasValue ? Value?.ToString() ?? "null" : "<missing>";
}


