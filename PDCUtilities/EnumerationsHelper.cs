using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace PDCUtility
{
    public static class EnumerationsHelper
    {
        public static TAttribute GetAttribute<TAttribute>(Enum value) where TAttribute : Attribute
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            return type.GetField(name).GetCustomAttributes(false).OfType<TAttribute>().SingleOrDefault();
        }

        public static TEnum GetDefaultValue<TEnum>() where TEnum : struct
        {
            Type t = typeof(TEnum);
            DefaultValueAttribute[] attributes = (DefaultValueAttribute[])t.GetCustomAttributes(typeof(DefaultValueAttribute), false);
            if (attributes != null &&
                attributes.Length > 0)
            {
                return (TEnum)attributes[0].Value;
            }
            else
            {
                return default(TEnum);
            }
        }

        public static string GetDescription<T>(this T enumerationValue) where T : struct
        {
            string strRet = enumerationValue.ToString();

            Type type = enumerationValue.GetType();

            if (!type.IsEnum)
                throw new ArgumentException("GetDescription<T>(): Must be of enum type", "T");

            // Tries to find A DescriptionAttribute for A potential friendly name for the enum
            MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if ((null != attrs) &&
                    (0 < attrs.Length) &&
                    (null != attrs.Where(t => t.GetType() == typeof(DescriptionAttribute)).FirstOrDefault()))
                    //Pull out the description value
                    strRet = ((DescriptionAttribute)attrs.Where(t => t.GetType() == typeof(DescriptionAttribute)).FirstOrDefault()).Description;
            }

            return strRet;
        }

        public static T ToEnumValue<T>(this string enumerationDescription) where T : struct
        {
            Type type = typeof(T);

            if (!type.IsEnum)
                throw new ArgumentException("ToEnumValue<T>(): Must be of enum type", "T");

            foreach (object val in System.Enum.GetValues(type))
                try
                {
                    string str = GetDescription<T>((T)val);
                    if (str.Equals(enumerationDescription, StringComparison.OrdinalIgnoreCase))
                        return (T)val;

                    str = val.ToString();
                    if (str.Equals(enumerationDescription, StringComparison.OrdinalIgnoreCase))
                        return (T)val;
                }
                catch (System.Exception ex) {  }

            throw new ArgumentException("ToEnumValue<T>(): Invalid description for enum " + type.Name, "enumerationDescription");
        }
    }
}