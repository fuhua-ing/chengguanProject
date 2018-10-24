using MessagePack;
using Newtonsoft.Json;

namespace Geone.Utiliy.Library
{
    /// <summary>
    /// 通用响应数据模型--支持MessagePack格式传输
    /// </summary>
    [MessagePackObject(true)]
    public class RepModel
    {
        public double StatusCode { get; set; }  //标识（成功：200/错误：999）
        public string Message { get; set; }     //描述
        public dynamic Details { get; set; }    //信息
        public dynamic Data { get; set; }       //数据

        //生成相应实例
        public static RepModel Create(double _StatusCode, dynamic _Data, string _Msg, dynamic _Info)
        {
            return new RepModel() { StatusCode = _StatusCode, Data = _Data, Message = _Msg, Details = _Info };
        }

        //请求成功响应参数
        public static RepModel Success(dynamic data, string msg = "请求成功", dynamic info = null)
        {
            return Create(200, data, msg, info);
        }

        //请求失败响应参数
        public static RepModel Error(string msg = "发生错误", double code = 999, dynamic info = null)
        {
            return Create(code, null, msg, info);
        }

        //请求失败响应参数
        public static RepModel Error(LogShow info)
        {
            double code = 999;
            if (!string.IsNullOrWhiteSpace(info.流程编码))
            {
                try
                {
                    code = double.Parse(info.流程编码);
                }
                catch
                {
                    code = 999;
                }
            }
            return Create(code, null, info.日志信息, info);
        }

        //转换Json字符串--忽略null
        public static string AsJson(RepModel res)
        {
            return JsonConvert.SerializeObject(res, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });
        }

        //请求成功响应参数--转换Json字符串--忽略null
        public static string SuccessAsJson(dynamic data, string msg = "请求成功", dynamic info = null)
        {
            return JsonConvert.SerializeObject(Create(200, data, msg, info), Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });
        }

        //请求失败响应参数--转换Json字符串--忽略null
        public static string ErrorAsJson(string msg = "发生错误", double code = 999, dynamic info = null)
        {
            return JsonConvert.SerializeObject(Create(code, null, msg, info), Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });
        }

        //请求失败响应参数--转换Json字符串--忽略null
        public static string ErrorAsJson(LogShow info)
        {
            double code = 999;
            if (!string.IsNullOrWhiteSpace(info.流程编码))
            {
                try
                {
                    code = double.Parse(info.流程编码);
                }
                catch
                {
                    code = 999;
                }
            }
            return JsonConvert.SerializeObject(Create(code, null, info.日志信息, info), Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            });
        }
    }
}