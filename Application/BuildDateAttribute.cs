using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurora.Misc.Application
{
    /// <summary>
    /// Custom Assembly Attribute to store the current Build date 
    /// 
    /// To use this class you have to add the following item group to the project file you want to use the attribute
    /// <example>
    /// <ItemGroup>
    ///    <AssemblyAttribute Include="Aurora.Misc.Application.BuildDateAttribute">
    ///      <_Parameter1>$([System.DateTime]::UtcNow.ToString("u"))</_Parameter1>
    ///   </AssemblyAttribute>
    ///  </ItemGroup>
    /// </example>
    /// </summary>

    [AttributeUsage(AttributeTargets.Assembly)]
    public class BuildDateAttribute : Attribute
    {
        /// <summary>
        /// Attribute containing the build timestamp
        /// </summary>
        /// <param name="value"></param>
        public BuildDateAttribute(string value)
        {
            UtcBuildTimeStamp = DateTime.ParseExact(value, "u", CultureInfo.InvariantCulture, DateTimeStyles.None);
        }
        /// <summary>
        /// on compile stored build timestamp UTC
        /// </summary>
        public DateTime UtcBuildTimeStamp { get; }
    }
}
