using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;

namespace BleLab.Utils
{
    public static class UtilExtensions
    {
        public static string ToMacAddressString(this ulong macAddress)
        {
            return string.Format("{0:X2}:{1:X2}:{2:X2}:{3:X2}:{4:X2}:{5:X2}",
                (macAddress >> 40) & 0xff,
                (macAddress >> 32) & 0xff,
                (macAddress >> 24) & 0xff,
                (macAddress >> 16) & 0xff,
                (macAddress >> 8) & 0xff,
                macAddress & 0xff);
        }

        public static byte[] ToArraySafe(this IBuffer buffer)
        {
            if (buffer?.Length > 0)
                return buffer.ToArray();

            return null;
        }

        public static bool IsStandartGattUuid(this Guid uuid)
        {
            return uuid.ToString("d").EndsWith("0000-1000-8000-00805f9b34fb");
        }

        public static string GetStandartGattAssignedNumber(this Guid uuid)
        {
            var bytes = uuid.ToByteArray();
            return $"0x{bytes[1]:X2}{bytes[0]:X2}";
        }
    }
}
