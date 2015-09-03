using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Utils {
    public static class Enums {
        public static string StringValue(this Enum value) {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            StringValueAttribute[] attributes = (StringValueAttribute[])fi.GetCustomAttributes(typeof(StringValueAttribute), false);
            if (attributes.Length > 0) {
                return attributes[0].Value;
            }
            else {
                return value.ToString();
            }
        }
        public static object OfStringValue(string value, Type enumType) {
            string[] names = Enum.GetNames(enumType);
            foreach (string name in names) {
                if (StringValue((Enum)Enum.Parse(enumType, name)).Equals(value)) {
                    return Enum.Parse(enumType, name);
                }
            }

            throw new ArgumentException("The string is not a description or value of the specified enum.");
        }
        public static object Of(string value, Type enumType) {
            return Enum.Parse(enumType, value);
        }
    }
}