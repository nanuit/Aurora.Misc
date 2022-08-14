using System;
using System.IO;
using System.Reflection;

namespace Aurora.Misc.Application
{
    public static class ApplicationInfo
    {
        /// <summary>
        /// Retrieves the linker timestamp.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns></returns>
        /// <remarks>http://www.codinghorror.com/blog/2005/04/determining-build-date-the-hard-way.html</remarks>
        private static System.DateTime RetrieveLinkerTimestamp(string filePath)
        {
            const int peHeaderOffset = 60;
            const int linkerTimestampOffset = 8;
            var linkerTimestampBytes = new byte[2048];
            System.IO.FileStream s = null;
            using (s = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    s.Read(linkerTimestampBytes, 0, 2048);
            
            var linkerTimeStampUtc = new System.DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(System.BitConverter.ToInt32(linkerTimestampBytes, System.BitConverter.ToInt32(linkerTimestampBytes, peHeaderOffset) + linkerTimestampOffset));
            
            return TimeZoneInfo.ConvertTimeFromUtc(linkerTimeStampUtc, TimeZoneInfo.Local);
        }
        /// <summary>
        /// Retrieves the linker timestamp.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        /// <remarks>https://www.meziantou.net/getting-the-date-of-build-of-a-dotnet-assembly-at-runtime.htm</remarks>
        public static DateTime GetLinkerTimestamp(string filePath)
        {
            const int peHeaderOffset = 60;
            const int linkerTimestampOffset = 8;
            var bytes = new byte[2048];

            using (var file = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                file.Read(bytes, 0, bytes.Length);
            }
            var headerPos = BitConverter.ToInt32(bytes, peHeaderOffset);
            var secondsSince1970 = BitConverter.ToInt32(bytes, headerPos + linkerTimestampOffset);
            var netLinkerTimestamp = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);            
            netLinkerTimestamp = TimeZoneInfo.ConvertTimeFromUtc(netLinkerTimestamp.AddSeconds(secondsSince1970), TimeZoneInfo.Local); 
            return netLinkerTimestamp;
        }
        /// <summary>
        /// get the linked timestamp for the assembly
        /// </summary>
        /// <param name="assembly">assembly to get the date for</param>
        /// <returns></returns>
        public static DateTime CompiledDate(this Assembly assembly)
        {
            //string path = Path.GetDirectoryName(new Uri(assembly.GetName().CodeBase).LocalPath);
            string path = new Uri(assembly.GetName().CodeBase).LocalPath;
            return (GetLinkerTimestamp(path));
        }
        /// <summary>
        /// Retreive the version 
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static Version GetVersion(this Assembly assembly)
        {
            return (assembly.GetName().Version);
        }
        /// <summary>
        /// Read the build timestamp from the custom assembly attribute for Utc
        /// </summary>
        /// <param name="assembly">assembly the attribute is to be read from</param>
        /// <returns></returns>
        public static DateTime GetCustomAssemblyLinkDateUtc(this Assembly assembly)
        {            
            var attr = assembly.GetCustomAttribute<BuildDateAttribute>();            
            return attr != null ? attr.UtcBuildTimeStamp : default(DateTime); 
        }
        /// <summary>
        /// Read the build timestamp from the custom assembly attribute
        /// </summary>
        /// <param name="assembly">assembly the attribute is to be read from</param>
        /// <returns></returns>
        public static DateTime GetCustomAssemblyLinkDateLocal(this Assembly assembly)
        {               
            return TimeZoneInfo.ConvertTimeFromUtc(GetCustomAssemblyLinkDateUtc(assembly), TimeZoneInfo.Local); 
        }
    }
}
