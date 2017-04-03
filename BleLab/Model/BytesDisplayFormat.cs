using System;

namespace BleLab.Model
{
    public enum BytesDisplayFormat
    {
        None,
        Auto,
        Decimal,
        Hexadecimal,
        Utf8,
        Utf16,
        Utf16Be,
    }

    public static class BytesDisplayFormatExtensions
    {
        public static string AsString(this BytesDisplayFormat format)
        {
            switch (format)
            {
                case BytesDisplayFormat.Auto:
                    return "Auto";
                case BytesDisplayFormat.Decimal:
                    return "Decimal";
                case BytesDisplayFormat.Hexadecimal:
                    return "Hex";
                case BytesDisplayFormat.Utf8:
                    return "UTF8";
                case BytesDisplayFormat.Utf16:
                    return "UTF16";
                case BytesDisplayFormat.Utf16Be:
                    return "UTF16BE";
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }
        }
    }
}