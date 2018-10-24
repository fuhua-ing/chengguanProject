using System;
using System.Reflection;

namespace Geone.Utiliy.Library
{
    public class EnumHelper
    {
        public static string GetDescription(Enum value)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }

            try
            {
                string description = value.ToString();

                FieldInfo fieldInfo = value.GetType().GetField(description);

                EnumDescAttribute[] attributes = (EnumDescAttribute[])fieldInfo.GetCustomAttributes(typeof(EnumDescAttribute), false);

                if (attributes != null && attributes.Length > 0)
                {
                    description = attributes[0].Description;
                }

                return description;
            }
            catch
            {
                return "";
            }
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class EnumDescAttribute : Attribute
    {
        public String Description { get { return description; } }

        protected String description;

        public EnumDescAttribute(String Description_in)
        {
            description = Description_in;
        }
    }
}