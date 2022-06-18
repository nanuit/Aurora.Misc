using System;
using System.Collections.Generic;
using System.Linq;

namespace Aurora.Misc.Encoding
{
    public class EncodingHandler
    {
        #region Private Members
        private static List<System.Text.Encoding> m_Encodings = null;
        public static List<System.Text.Encoding> Encodings
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

        public static List<string> GetNamesFromEncodings(List<System.Text.Encoding> list = null)
        {
            if (list == null)
            {
                list = Encodings;
            }
            return (list.Select(GetNameFromEncoding).ToList());
        }
        public static string GetNameFromEncoding(System.Text.Encoding encoding)
        {
            return (encoding.HeaderName);
        }

        public static List<System.Text.Encoding> GetEncodingsFromNames(List<string> listOfNames)
        {
            return (Encodings.FindAll((item) => listOfNames.Contains(GetNameFromEncoding(item), StringComparer.CurrentCultureIgnoreCase)));
        }
        public static System.Text.Encoding GetEncodingFromName(string nameOfEncoding)
        {
            return (Encodings.Find((item) => nameOfEncoding == GetNameFromEncoding(item)));
        }
        #endregion
    }
}
