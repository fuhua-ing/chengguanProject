using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using System.Net;


namespace Geone.Utiliy.Library
{
    /// <summary>
    /// Http访问
    /// </summary>
    public class HttpAccess : IHttpAccess
    {
        private static ILogWriter _log;

        public HttpAccess(ILogWriter log)
        {
            _log = log;
        }

        public RepModel Get(string url, string token = null)
        {
            try
            {
                HttpWebResponse response = null;

                if (!string.IsNullOrWhiteSpace(token))
                {
                    Dictionary<string, string> header = new Dictionary<string, string>()
                    {
                        { "Authorization" , token}
                    };

                    response = GetHttpWebResponse(url, "Get", null, header);
                }
                else
                    response = GetHttpWebResponse(url);

                if (response == null) return RepModel.Error("服务访问失败！");

                string content = GetHttpWebContent(response);

                if (content == null) return RepModel.Error("获取返回值失败！");

                return RepModel.Success(content);
            }
            catch (Exception ex)
            {
                _log.WriteException($"{url}——Get发生错误", ex);

                return RepModel.Error(ex.Message.ToString());
            }
        }

        public RepModel Post(string url, string param, string token = null)
        {
            try
            {
                HttpWebResponse response = null;

                if (!string.IsNullOrWhiteSpace(token))
                {
                    Dictionary<string, string> header = new Dictionary<string, string>()
                    {
                        { "Authorization" , token}
                    };

                    response = GetHttpWebResponse(url, "Post", param, header);
                }
                else
                    response = GetHttpWebResponse(url, "Post", param);

                if (response == null) return RepModel.Error("服务访问失败！");

                string content = GetHttpWebContent(response);

                if (content == null) return RepModel.Error("获取返回值失败！");

                return RepModel.Success(content);
            }
            catch (Exception ex)
            {
                _log.WriteException($"{url}——Post发生错误", ex);

                return RepModel.Error(ex.Message.ToString());
            }
        }

        public RepModel Put(string url, string param, string token = null)
        {
            try
            {
                HttpWebResponse response = null;

                if (!string.IsNullOrWhiteSpace(token))
                {
                    Dictionary<string, string> header = new Dictionary<string, string>()
                    {
                        { "Authorization" , token}
                    };

                    response = GetHttpWebResponse(url, "Put", param, header);
                }
                else
                    response = GetHttpWebResponse(url, "Put", param);

                if (response == null) return RepModel.Error("服务访问失败！");

                string content = GetHttpWebContent(response);

                if (content == null) return RepModel.Error("获取返回值失败！");

                return RepModel.Success(content);
            }
            catch (Exception ex)
            {
                _log.WriteException($"{url}——Put发生错误", ex);

                return RepModel.Error(ex.Message.ToString());
            }
        }

        public RepModel Delete(string url, string param, string token = null)
        {
            try
            {
                HttpWebResponse response = null;

                if (!string.IsNullOrWhiteSpace(token))
                {
                    Dictionary<string, string> header = new Dictionary<string, string>()
                    {
                        { "Authorization" , token}
                    };

                    response = GetHttpWebResponse(url, "Delete", param, header);
                }
                else
                    response = GetHttpWebResponse(url, "Delete", param);

                if (response == null) return RepModel.Error("服务访问失败！");

                string content = GetHttpWebContent(response);

                if (content == null) return RepModel.Error("获取返回值失败！");

                return RepModel.Success(content);
            }
            catch (Exception ex)
            {
                _log.WriteException($"{url}——Delete发生错误", ex);

                return RepModel.Error(ex.Message.ToString());
            }
        }

        public RepModel Get<T>(string url, string token = null)
        {
            try
            {
                HttpWebResponse response = null;

                if (!string.IsNullOrWhiteSpace(token))
                {
                    Dictionary<string, string> header = new Dictionary<string, string>()
                    {
                        { "Authorization" , token}
                    };

                    response = GetHttpWebResponse(url, "Get", null, header);
                }
                else
                    response = GetHttpWebResponse(url);

                if (response == null) return RepModel.Error("服务访问失败！");

                string content = GetHttpWebContent(response);

                if (content == null) return RepModel.Error("获取返回值失败！");

                return RepModel.Success(JsonConvert.DeserializeObject<T>(content));
            }
            catch (Exception ex)
            {
                _log.WriteException($"{url}——Get发生错误", ex);

                return RepModel.Error(ex.Message.ToString());
            }
        }

        public RepModel Post<T>(string url, string param, string token = null)
        {
            try
            {
                HttpWebResponse response = null;

                if (!string.IsNullOrWhiteSpace(token))
                {
                    Dictionary<string, string> header = new Dictionary<string, string>()
                    {
                        { "Authorization" , token}
                    };

                    response = GetHttpWebResponse(url, "Post", param, header);
                }
                else
                    response = GetHttpWebResponse(url, "Post", param);

                if (response == null) return RepModel.Error("服务访问失败！");

                string content = GetHttpWebContent(response);

                if (content == null) return RepModel.Error("获取返回值失败！");

                return RepModel.Success(JsonConvert.DeserializeObject<T>(content));
            }
            catch (Exception ex)
            {
                _log.WriteException($"{url}——Post发生错误", ex);

                return RepModel.Error(ex.Message.ToString());
            }
        }

        public RepModel Put<T>(string url, string param, string token = null)
        {
            try
            {
                HttpWebResponse response = null;

                if (!string.IsNullOrWhiteSpace(token))
                {
                    Dictionary<string, string> header = new Dictionary<string, string>()
                    {
                        { "Authorization" , token}
                    };

                    response = GetHttpWebResponse(url, "Put", param, header);
                }
                else
                    response = GetHttpWebResponse(url, "Put", param);

                if (response == null) return RepModel.Error("服务访问失败！");

                string content = GetHttpWebContent(response);

                if (content == null) return RepModel.Error("获取返回值失败！");

                return RepModel.Success(JsonConvert.DeserializeObject<T>(content));
            }
            catch (Exception ex)
            {
                _log.WriteException($"{url}——Put发生错误", ex);

                return RepModel.Error(ex.Message.ToString());
            }
        }

        public RepModel Delete<T>(string url, string param, string token = null)
        {
            try
            {
                HttpWebResponse response = null;

                if (!string.IsNullOrWhiteSpace(token))
                {
                    Dictionary<string, string> header = new Dictionary<string, string>()
                    {
                        { "Authorization" , token}
                    };

                    response = GetHttpWebResponse(url, "Delete", param, header);
                }
                else
                    response = GetHttpWebResponse(url, "Delete", param);

                if (response == null) return RepModel.Error("服务访问失败！");

                string content = GetHttpWebContent(response);

                if (content == null) return RepModel.Error("获取返回值失败！");

                return RepModel.Success(JsonConvert.DeserializeObject<T>(content));
            }
            catch (Exception ex)
            {
                _log.WriteException($"{url}——Delete发生错误", ex);

                return RepModel.Error(ex.Message.ToString());
            }
        }

        public RepModel Communicate(string url, string method, string param, Dictionary<string, string> header)
        {
            try
            {
                HttpWebResponse response = GetHttpWebResponse(url, method, param, header);

                if (response == null) return RepModel.Error("服务访问失败！");

                string content = GetHttpWebContent(response);

                if (content == null) return RepModel.Error("获取返回值失败！");

                return RepModel.Success(content);
            }
            catch (Exception ex)
            {
                _log.WriteException($"{url}——Communicate发生错误", ex);

                return RepModel.Error(ex.Message.ToString());
            }
        }

        public RepModel Communicate<T>(string url, string method, string param, Dictionary<string, string> header)
        {
            try
            {
                HttpWebResponse response = GetHttpWebResponse(url, method, param, header);

                if (response == null) return RepModel.Error("服务访问失败！");

                string content = GetHttpWebContent(response);

                if (content == null) return RepModel.Error("获取返回值失败！");

                return RepModel.Success(JsonConvert.DeserializeObject<T>(content));
            }
            catch (Exception ex)
            {
                _log.WriteException($"{url}——Get发生错误", ex);

                return RepModel.Error(ex.Message.ToString());
            }
        }

        /// <summary>
        /// 发送请求，获取响应
        /// </summary>
        /// <param name="url">路由</param>
        /// <param name="method">方法/谓词</param>
        /// <param name="param">参数</param>
        /// <param name="header">请求头</param>
        /// <param name="timeout">超时</param>
        /// <returns></returns>
        public HttpWebResponse GetHttpWebResponse(string url, string method = "Get", string param = null, Dictionary<string, string> header = null, int timeout = 5000)
        {
            try
            {
                //1.发送地址
                HttpWebRequest hwp = (HttpWebRequest)WebRequest.Create(url);

                //2.设置提交方式
                hwp.Method = method;
                //3.写入参数
                if (param != null)
                {
                    if (!string.IsNullOrWhiteSpace(param))
                    {
                        byte[] byteArray = Encoding.UTF8.GetBytes(param);
                        hwp.ContentLength = byteArray.Length;
                        using (Stream newStream = hwp.GetRequestStream())
                        {
                            newStream.Write(byteArray, 0, byteArray.Length);
                            newStream.Close();
                        }
                    }
                }

                //4.写入请求头
                //4.1.通用跨域设置
                hwp.Headers.Add("Access-Control-Allow-Origin", "*");
                hwp.Headers.Add("Access-Control-Allow-Methods", "GET,POST,PUT,DELETE,OPTIONS");
                //4.2.允许设置头
                hwp.Headers.Add("Access-Control-Allow-Headers", "Origin, X-Requested-With, Accept, Content-Type, Authorization, Token, Ticket, Access-Token, Srvid, Identity");

                if (method == "Post")
                    hwp.Headers.Add("Content-Type", "application/json;charset=utf-8");

                if (header != null)
                {
                    if (header.Count > 0)
                    {
                        foreach (KeyValuePair<string, string> kv in header)
                        {
                            hwp.Headers.Add(kv.Key, kv.Value);
                        }
                    }
                }

                //5.设置超时
                hwp.Timeout = timeout;
                hwp.ReadWriteTimeout = timeout;

                HttpWebResponse response = (HttpWebResponse)hwp.GetResponse();//获取响应
                return response;
            }
            catch (Exception ex)
            {
                _log.WriteException($"{url}——GetHttpWebResponse发生错误", ex);
                return null;
            }
        }

        /// <summary>
        /// 根据响应，获取请求值
        /// </summary>
        /// <param name="response">响应</param>
        /// <returns></returns>
        public string GetHttpWebContent(HttpWebResponse response)
        {
            StreamReader sr = null;

            try
            {
                if (response != null)
                {
                    sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    string content = sr.ReadToEnd();
                    return content;
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                _log.WriteException($"{response.ResponseUri.AbsoluteUri}——GetHttpWebContent发生错误", ex);
                return null;
            }
            finally
            {
                if (sr != null)
                {
                    sr.Close();
                    sr.Dispose();
                }
            }
        }
    }
}