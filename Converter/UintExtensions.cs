using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurora.Misc.Converter
{
    /// <summary>
    /// Extension Methods for uint data type
    /// </summary>
    public static  class UintExtensions
    {
        /// <summary>
        /// convert a uint to a BCD byte Array
        /// </summary>
        /// <param name="toConvert">number to convert</param>
        /// <param name="howManyBytes">number of bytes in the array</param>
        /// <returns>byte array containing the BCD Code</returns>
        public static byte[] ConvertToBcd(this uint toConvert, int howManyBytes)
        {
            var convertedNumber = new byte[howManyBytes];
            var strNumber = toConvert.ToString();
            var currentNumber = string.Empty;

            for (var index = 0; index < howManyBytes; index++)
                convertedNumber[index] = 0xff;

            for (var index = 0; index < strNumber.Length; index++)
            {
                currentNumber += strNumber[index];

                if (index == strNumber.Length - 1 && index % 2 == 0)
                {
                    convertedNumber[index / 2] = 0xf;
                    convertedNumber[index / 2] |= (byte)((int.Parse(currentNumber) % 10) << 4);
                }
                if (index % 2 == 0) continue;
                var value = int.Parse(currentNumber);
                convertedNumber[(index - 1) / 2] = (byte)(value % 10);
                convertedNumber[(index - 1) / 2] |= (byte)((value / 10) << 4);
                currentNumber = string.Empty;
            }

            return convertedNumber;
        }
    }
}
