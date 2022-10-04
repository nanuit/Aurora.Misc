using System;
using System.Collections.Generic;
using System.Linq;

namespace Aurora.Misc.Encoding
{
    public class EncodingHandler
    {
        #region Private Members
        private static List<System.Text.Encoding>? m_Encodings = null;
        /// <summary>
        /// Retrieve a list of encodings
        /// </summary>
        public static List<System.Text.Encoding>? Encodings
        {
            get
            {
                if (m_Encodings == null || m_Encodings.Count < 1)
                    m_Encodings = System.Text.Encoding.GetEncodings().Select(encoding => encoding.GetEncoding()).ToList();
                return m_Encodings;
            }
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// get all encoding names 
        /// </summary>
        /// <param name="list">list to be filled with encoding names</param>
        /// <returns>list of encoding names</returns>
        public static List<string>? GetNamesFromEncodings(List<System.Text.Encoding>? list = null)
        {
            list ??= Encodings;
            return (list?.Select(GetNameFromEncoding).ToList());
        }
        /// <summary>
        /// select method to get the name of an encoding
        /// </summary>
        /// <param name="encoding">encoding to get the name from</param>
        /// <returns>name of the encoding</returns>
        public static string GetNameFromEncoding(System.Text.Encoding encoding)
        {
            return (encoding.HeaderName);
        }

        /// <summary>
        /// Get all Encoding identified by their names
        /// </summary>
        /// <param name="listOfNames"></param>
        /// <returns></returns>
        public static List<System.Text.Encoding>? GetEncodingsFromNames(List<string> listOfNames)
        {
            List<System.Text.Encoding>? retVal = null;
            if (Encodings != null)
                retVal = Encodings.FindAll((item) => listOfNames.Contains(GetNameFromEncoding(item), StringComparer.CurrentCultureIgnoreCase));
            return retVal;
        }
        /// <summary>
        /// Get the Encoding identified by the encoding name
        /// </summary>
        /// <param name="nameOfEncoding">name of Encoding to get</param>
        /// <returns>Encoding found</returns>
        public static System.Text.Encoding? GetEncodingFromName(string nameOfEncoding)
        {
            System.Text.Encoding? retVal = null;
            if (Encodings != null)
                retVal = Encodings.Find((item) => nameOfEncoding == GetNameFromEncoding(item));
            return retVal;
        }
        #endregion
    }
}
