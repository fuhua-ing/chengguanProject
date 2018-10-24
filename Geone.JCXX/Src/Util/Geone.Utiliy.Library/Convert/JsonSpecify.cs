using Newtonsoft.Json;

namespace Geone.Utiliy.Library
{
    public class JsonSpecify
    {
        public static string CamelCase<T>(T t)
        {
            return JsonConvert.SerializeObject(t, Formatting.Indented, new JsonSerializerSettings
            {
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });
        }
    }
}