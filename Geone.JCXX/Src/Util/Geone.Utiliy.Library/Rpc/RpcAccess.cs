using Grpc.Core;
using MagicOnion;
using MagicOnion.Client;
using MagicOnion.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Geone.Utiliy.Library
{
    /// <summary>
    /// 远程调用其他服务
    /// </summary>
    public class RpcAccess : IRpcAccess
    {
        private static ILogWriter _log;

        public RpcAccess(ILogWriter log)
        {
            _log = log;
        }

        //获取服务端
        public Server GetServer(string host, int port)
        {
            try
            {
                MagicOnionServiceDefinition magicOnionServiceDefinition = MagicOnionEngine.BuildServerServiceDefinition(new MagicOnionOptions(true)
                {
                    MagicOnionLogger = new MagicOnionLogToGrpcLogger()
                });

                Server server = new Server
                {
                    Services = { magicOnionServiceDefinition },
                    Ports = { new ServerPort(host, port, ServerCredentials.Insecure) }
                };

                return server;
            }
            catch (Exception ex)
            {
                _log.WriteException($"获取服务端{host}:{port}发生错误！", ex);
                return null;
            }
        }

        //开启服务端
        public void Start(Server server)
        {
            try
            {
                server.Start();
            }
            catch (Exception ex)
            {
                _log.WriteException("开启服务端发生错误！", ex);
            }
        }

        //关闭服务端
        public async void Close(Server server)
        {
            try
            {
                await server.ShutdownAsync();
            }
            catch (Exception ex)
            {
                _log.WriteException("关闭服务端发生错误！", ex);
            }
        }

        //客户端连接
        public Channel GetChannel(string host, int port)
        {
            try
            {
                return new Channel(host, port, ChannelCredentials.Insecure);
            }
            catch (Exception ex)
            {
                _log.WriteException("客户端连接发生错误！", ex);
                return default;
            }
        }

        //获取服务
        public T GetService<T>(Channel channel) where T : IService<T>
        {
            try
            {
                return MagicOnionClient.Create<T>(channel);
            }
            catch (Exception ex)
            {
                _log.WriteException("获取服务发生错误！", ex);
                return default;
            }
        }

        //获取服务
        public T GetService<T>(string host, int port) where T : IService<T>
        {
            try
            {
                Channel channel = new Channel(host, port, ChannelCredentials.Insecure);
                return MagicOnionClient.Create<T>(channel);
            }
            catch (Exception ex)
            {
                _log.WriteException("获取服务发生错误！", ex);
                return default;
            }
        }

        //进行访问
        public RepModel Communicate(Channel channel, string service, string method, RpcReq req)
        {
            try
            {
                //进行访问
                //获取服务类型
                Type type = typeof(IRpcStorage).Assembly.GetTypes().Where(t => t.Name == service).FirstOrDefault();

                //调用RpcHelper.GetService<T>(string host, int port)
                //RpcAccess rt = new RpcAccess(_log);
                //先获取到GetService<T>的MethodInfo反射对象
                MethodInfo mi = GetType().GetMethod("GetService");
                //然后使用MethodInfo反射对象调用RpcHelper类的GetService<T>方法，这时要使用MethodInfo的MakeGenericMethod函数指定函数GetService<T>的泛型类型T
                object resource = mi.MakeGenericMethod(new Type[] { type }).Invoke(this, new object[] { channel });

                //获取参数格式
                object[] parameters = new object[] { req };

                //反射调用Rpc服务
                object Method = resource.GetType().GetMethod(method).Invoke(resource, parameters);
                UnaryResult<RepModel> urep = (UnaryResult<RepModel>)Method;
                return urep.ResponseAsync.Result;
            }
            catch (Exception ex)
            {
                return RepModel.Error(_log.WriteServiceException(service + "-" + method, "RpcAccess<T>-Simple", "远程调用访问发生错误！", ex));
            }
        }

        //进行访问
        public RepModel Communicate(string host, int post, string service, string method, RpcReq req)
        {
            try
            {
                //进行访问
                //获取服务类型
                Type type = typeof(IRpcStorage).Assembly.GetTypes().Where(t => t.Name == service).FirstOrDefault();

                //调用RpcHelper.GetService<T>(string host, int port)
                //RpcAccess rt = new RpcAccess(_log);
                //先获取到GetService<T>的MethodInfo反射对象
                MethodInfo mi = this.GetType().GetMethod("GetService");
                //然后使用MethodInfo反射对象调用RpcHelper类的GetService<T>方法，这时要使用MethodInfo的MakeGenericMethod函数指定函数GetService<T>的泛型类型T
                object resource = mi.MakeGenericMethod(new Type[] { type }).Invoke(this, new object[] { host, post });

                //获取参数格式
                object[] parameters = new object[] { req };

                //反射调用Rpc服务
                object Method = resource.GetType().GetMethod(method).Invoke(resource, parameters);
                UnaryResult<RepModel> urep = (UnaryResult<RepModel>)Method;
                return urep.ResponseAsync.Result;
            }
            catch (Exception ex)
            {
                return RepModel.Error(_log.WriteServiceException(service + "-" + method, "RpcAccess<T>-Simple", "远程调用访问发生错误！", ex).错误信息);
            }
        }

        //前置执行操作
        public RepModel Do(Event @event, RpcReq req)
        {
            string srvid = "";

            try
            {
                //请求头（Srvid/Identity/Ticket/Token）
                Dictionary<string, string> Header = JsonConvert.DeserializeObject<Dictionary<string, string>>(req.Header);
                //请求参数（可为空）
                Dictionary<string, object> Param = JsonConvert.DeserializeObject<Dictionary<string, object>>(req.Param);

                if (Header.ContainsKey("Srvid"))
                {
                    srvid = Header["Srvid"];
                }

                if (Header.ContainsKey("Ticket"))
                {
                    Ticket.Verify(Header["Ticket"]);
                }

                RepModel res = @event(Param, _log);

                if (!res.Data.GetType().IsPrimitive)
                    res.Data = JsonSpecify.CamelCase(res.Data);

                return res;
            }
            catch (Exception ex)
            {
                return RepModel.Error(_log.WriteServiceException(srvid, "Do", "远程调用服务发生错误！", ex));
            }
        }
    }
}