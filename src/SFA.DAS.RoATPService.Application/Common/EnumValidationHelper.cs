using System;

namespace SFA.DAS.RoATPService.Application.Common;

public static class EnumValidationHelper
{
    public static bool IsValidEnumValue(this object value, Type enumType)
    {
        if (!enumType.IsEnum) return false;

        if (value is int intVal)
            return Enum.IsDefined(enumType, intVal);

        if (value is string strVal)
            return Enum.TryParse(enumType, strVal, true, out var enumVal) && Enum.IsDefined(enumType, enumVal);

        return false;
    }

    public static bool IsValidEnumValue<TEnum>(this object value) where TEnum : struct, Enum
        => IsValidEnumValue(value, typeof(TEnum));
}
