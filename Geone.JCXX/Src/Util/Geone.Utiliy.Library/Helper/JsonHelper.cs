using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Geone.Utiliy.Library
{
    public static class JsonHelper
    {
        public static string JsonDllSerialize<T>(T t, JsonDateTimeFormat? TimeFormat = null)
        {
            if (TimeFormat == null)
                return JsonConvert.SerializeObject(t);
            else
            {
                IsoDateTimeConverter timeFormat = new IsoDateTimeConverter
                {
                    DateTimeFormat = EnumHelper.GetDescription(TimeFormat)
                };
                return JsonConvert.SerializeObject(t, Formatting.Indented, timeFormat);
            }
        }

        public static string JsonDllSerializeList<T>(List<T> tl, JsonDateTimeFormat? TimeFormat = null)
        {
            if (TimeFormat == null)
                return JsonConvert.SerializeObject(tl);
            else
            {
                IsoDateTimeConverter timeFormat = new IsoDateTimeConverter
                {
                    DateTimeFormat = EnumHelper.GetDescription(TimeFormat)
                };
                return JsonConvert.SerializeObject(tl, Formatting.Indented, timeFormat);
            }
        }

        public static string JsonDllTableSerialize(DataTable dt, JsonDateTimeFormat? TimeFormat = null)
        {
            if (TimeFormat == null)
                return JsonConvert.SerializeObject(dt);
            else
            {
                IsoDateTimeConverter timeFormat = new IsoDateTimeConverter
                {
                    DateTimeFormat = EnumHelper.GetDescription(TimeFormat)
                };
                return JsonConvert.SerializeObject(dt, Formatting.Indented, timeFormat);
            }
        }

        public static List<T> JsonDllDeserializeList<T>(string value)
        {
            return JsonConvert.DeserializeObject<List<T>>(value);
        }

        public static T JsonDllDeserialize<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }

        public static T JsonDllTableToObject<T>(DataTable dt)
        {
            string temp = JsonDllTableSerialize(dt);
            string value = temp.Substring(1, temp.Length - 2);
            T t = JsonConvert.DeserializeObject<T>(value);
            return t;
        }

        public static List<T> JsonDllTableToObjectList<T>(DataTable dt)
        {
            string temp = JsonDllTableSerialize(dt);
            List<T> tl = JsonConvert.DeserializeObject<List<T>>(temp);
            return tl;
        }

        public static string FromDictionaryToJson(this Dictionary<string, string> dictionary)
        {
            var kvs = dictionary.Select(kvp => string.Format("\"{0}\":\"{1}\"", kvp.Key, string.Concat(",", kvp.Value)));
            return string.Concat("{", string.Join(",", kvs), "}");
        }

        public static Dictionary<string, string> FromJsonToDictionary(this string json)
        {
            string[] keyValueArray = json.Replace("{", string.Empty).Replace("}", string.Empty).Replace("\"", string.Empty).Split(',');
            return keyValueArray.ToDictionary(item => item.Split(':')[0], item => item.Split(':')[1]);
        }
    }

    public enum JsonDateTimeFormat
    {
        /// <summary>
        /// yyyy-MM-dd
        /// </summary>
        [EnumDesc("yyyy-MM-dd")]
        Date = 1,

        /// <summary>
        /// yyyy/MM/dd HH:mm:ss
        /// </summary>
        [EnumDesc("yyyy/MM/dd HH:mm:ss")]
        DateTime = 2,

        /// <summary>
        /// yyyy-MM-dd HH:mm:ss
        /// </summary>
        [EnumDesc("yyyy-MM-dd HH:mm:ss")]
        DateTime2 = 3
    }
}