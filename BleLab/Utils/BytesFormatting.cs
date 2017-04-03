using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using BleLab.Model;

namespace BleLab.Utils
{
    internal static class BytesFormatting
    {
        public static string AsString(this byte[] bytes, BytesDisplayFormat format = BytesDisplayFormat.Decimal)
        {
            if (bytes == null)
                return "<no value>";

            if (bytes.Length == 0)
                return "<empty array>";

            switch (format)
            {
                case BytesDisplayFormat.Decimal:
                    return string.Join(" ", bytes.Select(t => t.ToString("D2")));
                case BytesDisplayFormat.Hexadecimal:
                    return string.Join(" ", bytes.Select(t => t.ToString("X2")));
                case BytesDisplayFormat.Utf8:
                    return Encoding.UTF8.GetString(bytes);
                case BytesDisplayFormat.Utf16:
                    return Encoding.Unicode.GetString(bytes);
                case BytesDisplayFormat.Utf16Be:
                    return Encoding.BigEndianUnicode.GetString(bytes);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public static BytesDisplayFormat DetectFormatAuto(string bytesString)
        {
            if (string.IsNullOrEmpty(bytesString))
                return BytesDisplayFormat.None;

            var parts = bytesString.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // checking decimal
            if (parts.All(x => byte.TryParse(x, out byte dummy)))
                return BytesDisplayFormat.Decimal;

            // checking hex
            foreach (var part in parts)
            {
                var str = part;

                if (str.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                    str = part.ToLowerInvariant().Replace("0x", string.Empty);

                // if any is not hex = UTF8
                if (!byte.TryParse(str, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out byte dummy))
                    return BytesDisplayFormat.Utf8;
            }

            return BytesDisplayFormat.Hexadecimal;
        }

        public static byte[] TryParse(string bytesString, BytesDisplayFormat format)
        {
            if (bytesString == null)
                return null;

            switch (format)
            {
                case BytesDisplayFormat.Auto:
                case BytesDisplayFormat.None:
                    return null;
                case BytesDisplayFormat.Utf8:
                    return Encoding.UTF8.GetBytes(bytesString);
                case BytesDisplayFormat.Utf16:
                    return Encoding.Unicode.GetBytes(bytesString);
                case BytesDisplayFormat.Utf16Be:
                    return Encoding.BigEndianUnicode.GetBytes(bytesString);
                case BytesDisplayFormat.Decimal:
                case BytesDisplayFormat.Hexadecimal:
                    break;
                default:
                    throw new NotImplementedException("Format parsing not implemented.");
            }

            var parts = bytesString.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var bytesList = new List<byte>();
            foreach (var part in parts)
            {
                var str = part;
                if (str.ToLowerInvariant().StartsWith("0x") && format == BytesDisplayFormat.Hexadecimal)
                    str = part.ToLowerInvariant().Replace("0x", string.Empty);

                byte result;
                if (byte.TryParse(str, format == BytesDisplayFormat.Hexadecimal ? NumberStyles.HexNumber : NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
                {
                    bytesList.Add(result);
                }
                else
                {
                    throw new FormatException($"Can't parse '{part}' as { format.AsString() }");
                }
            }

            if (bytesList.Count == 0)
                return null;

            return bytesList.ToArray();
        }
    }
}
