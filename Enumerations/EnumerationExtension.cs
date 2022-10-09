using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Markup;

namespace Aurora.Misc.Enumerations
{
    public class EnumerationExtension : MarkupExtension
    {
        private Type m_EnumType;


        public EnumerationExtension(Type enumType)
        {
            EnumType = enumType ?? throw new ArgumentNullException("enumType");
        }

        public Type EnumType
        {
            get => m_EnumType;
            private set
            {
                if (m_EnumType == value)
                    return;

                var enumType = Nullable.GetUnderlyingType(value) ?? value;

                if (enumType.IsEnum == false)
                    throw new ArgumentException("Type must be an Enum.");

                m_EnumType = value;
            }
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var enumValues = Enum.GetValues(EnumType);

            return (
                from object enumValue in enumValues
                select new EnumerationMember
                {
                    Value = enumValue,
                    Description = GetDescription(enumValue) ?? string.Empty
                }).ToArray();
        }

        private string? GetDescription(object enumValue)
        {
            if (enumValue == null)
                throw new ArgumentNullException("enumValue");
            var fieldInfo = EnumType.GetField(enumValue.ToString()!);
            string? retVal = fieldInfo?.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault() is DescriptionAttribute descriptionAttribute
                ? descriptionAttribute.Description
                : enumValue.ToString();
            return retVal;
        }

        public class EnumerationMember
        {
            public string Description { get; set; }
            public object Value { get; set; }
        }
    }
}
