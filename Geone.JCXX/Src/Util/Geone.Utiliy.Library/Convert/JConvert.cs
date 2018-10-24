using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Geone.Utiliy.Library
{
    public class JConvert
    {
        public static Page<T> GetPage<T>(int index, int size, List<T> list)
        {
            try
            {
                int count = list.Count;

                int start = (index - 1) * size;
                int end = index * size - 1;

                List<T> Results = new List<T>();

                for (int i = 0; i < count; i++)
                {
                    if (i >= start && i < end)
                    {
                        Results.Add(list[i]);
                    }
                }

                Page<T> page = new Page<T>()
                {
                    Total = count,
                    Rows = Results
                };

                return page;
            }
            catch
            {
                return default;
            }
        }

        public static string GetParamStr<T>(T param)
        {
            Type t = typeof(T);
            if (t.IsPrimitive)
                return param.ToString();
            else
                return JsonConvert.SerializeObject(param);
        }

        public static JObject GetJObject<T>(ref JObject obj, T param)
        {
            Type type = param.GetType();

            //反射获取键名集合
            foreach (PropertyInfo pi in type.GetProperties())
            {
                object value = pi.GetValue(param);
                if (value != null)
                {
                    Type btype = value.GetType();
                    if (btype.Assembly == typeof(object).Assembly)
                        obj.Add(new JProperty(pi.Name, value));
                    else
                    {
                        JObject child = new JObject();
                        obj.Add(new JProperty(pi.Name, GetJObject(ref child, value)));
                    }
                }
                else
                    obj.Add(new JProperty(pi.Name, value));
            }

            return obj;
        }

        public static JObject GetJObject(ref JObject obj, Dictionary<string, object> kvs)
        {
            foreach (KeyValuePair<string, object> kv in kvs)
            {
                obj.Add(new JProperty(kv.Key, kv.Value));
            }

            return obj;
        }

        public static JObject GetJObject(ref JObject obj, List<KVPair> kvs)
        {
            foreach (KVPair kv in kvs)
            {
                obj.Add(new JProperty(kv.Key, kv.Value));
            }

            return obj;
        }

        public static JObject GetJObject(ref JObject obj, string name, JArray array)
        {
            obj.Add(new JProperty(name, array));

            return obj;
        }

        public static JObject GetJObject<T>(ref JObject obj, string name, List<T> list)
        {
            JArray arr = new JArray();
            foreach (T t in list)
            {
                JObject @object = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(t));
                arr.Add(@object);
            }
            return GetJObject(ref obj, name, arr);
        }

        public static JArray GetJArray<T>(ref JArray arr, List<T> list)
        {
            foreach (T t in list)
            {
                JObject obj = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(t));
                arr.Add(obj);
            }

            return arr;
        }
    }
}