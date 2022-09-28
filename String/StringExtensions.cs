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
        /// <summary>
        /// Create a bytes array from a hexadecimal string
        /// the hexadecimal bytes can be separated by  '-' or ':'. If no separator every 2 characters will be converted to a byte
        /// </summary>
        /// <param name="bytesString">string with a number of hexadecimal bytes</param>
        /// <returns>resulting byte array</returns>
        public static byte[] BytesFromHexString(this string bytesString)
        {
            bytesString = bytesString.Replace("-", "");
            bytesString = bytesString.Replace(":", "");

            int numberChars = bytesString.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(bytesString.Substring(i, 2), 16);
            return bytes;
        }

        private static string TranslateByte(byte digit)
        {
            return digit < 32 ? $"\\x{digit:X2}" : $"{(char)digit}";
        }
    }
}
