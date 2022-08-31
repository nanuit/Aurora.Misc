using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurora.Misc.String
{
    /// <summary>
    /// String ExtensionMethod
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// convert a binary string into a readable string. all value below 32 will be translated to hexadecimal notation
        /// </summary>
        /// <param name="bytes">string of bytes</param>
        /// <returns>hexadecimal readable string</returns>
        public static string StringFromByte(this string bytes)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var digit in bytes)
                builder.Append(TranslateByte((byte)digit));

            return builder.ToString();
        }
        /// <summary>
        /// convert a byte array into a readable string. all value below 32 will be translated to hexadecimal notation
        /// </summary>
        /// <param name="bytes">bytearray</param>
        /// <returns>hexadecimal readable string</returns>
        public static string StringFromByte(this byte[] bytes)
        {
            StringBuilder builder = new StringBuilder();
            if (bytes != null && bytes.Length > 0)
            {
                foreach (var digit in bytes)
                    builder.Append(TranslateByte(digit));
            }
            return builder.ToString();
        }
        private static string TranslateByte(byte digit)
        {
            return digit < 32 ? $"\\x{digit:X2}" : $"{(char)digit}";
        }

    }
}
