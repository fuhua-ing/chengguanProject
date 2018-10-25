using System;
using System.Collections.Generic;

namespace Geone.Utiliy.Database
{
    public static class ExtendObject
    {
        public static string Set(this object source, dynamic value)
        {
            return Convert.ToString(value);
        }

        public static string[] Val(this object source, dynamic value)
        {
            string[] final = new string[]
            {
                Convert.ToString(source),
                Convert.ToString(value)
            };

            return final;
        }

        public static string[] Sub(this object source, dynamic value)
        {
            string[] final = new string[]
            {
                Convert.ToString(source),
                Convert.ToString(value)
            };

            return final;
        }

        public static string[] Eq(this object source, dynamic value)
        {
            string[] final = new string[]
            {
                Convert.ToString(source),
                Convert.ToString(value)
            };

            return final;
        }

        public static string[] IsNull(this object source)
        {
            string[] final = new string[]
            {
                Convert.ToString(source),
                Convert.ToString("")
            };

            return final;
        }

        public static string[] IsNotNull(this object source)
        {
            string[] final = new string[]
            {
                Convert.ToString(source),
                Convert.ToString("")
            };

            return final;
        }

        public static string[] IsContains(this object source, dynamic value)
        {
            string[] final = new string[]
            {
                Convert.ToString(source),
                Convert.ToString(value)
            };

            return final;
        }

        public static string[] IsNotContains(this object source, dynamic value)
        {
            string[] final = new string[]
            {
                Convert.ToString(source),
                Convert.ToString(value)
            };

            return final;
        }

        public static string[] Ne(this object source, dynamic value)
        {
            string[] final = new string[]
            {
                Convert.ToString(source),
                Convert.ToString(value)
            };

            return final;
        }

        public static string[] Lt(this object source, dynamic value)
        {
            string[] final = new string[]
            {
                Convert.ToString(source),
                Convert.ToString(value)
            };

            return final;
        }

        public static string[] Le(this object source, dynamic value)
        {
            string[] final = new string[]
            {
                Convert.ToString(source),
                Convert.ToString(value)
            };

            return final;
        }

        public static string[] Gt(this object source, dynamic value)
        {
            string[] final = new string[]
            {
                Convert.ToString(source),
                Convert.ToString(value)
            };

            return final;
        }

        public static string[] Ge(this object source, dynamic value)
        {
            string[] final = new string[]
            {
                Convert.ToString(source),
                Convert.ToString(value)
            };

            return final;
        }

        public static string[] Like(this object source, dynamic value)
        {
            string[] final = new string[]
            {
                Convert.ToString(source),
                Convert.ToString(value)
            };

            return final;
        }

        public static string[] Between(this object source, dynamic value1, dynamic value2)
        {
            string[] final = new string[]
            {
                Convert.ToString(source),
                Convert.ToString(value1),
                Convert.ToString(value2)
            };

            return final;
        }

        public static string[] In(this object source, params dynamic[] value)
        {
            List<string> list = new List<string>
            {
                Convert.ToString(source)
            };

            foreach (dynamic v in value)
            {
                list.Add(Convert.ToString(v));
            }

            string[] final = list.ToArray();

            return final;
        }
    }
}