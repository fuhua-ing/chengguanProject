using System.Collections.Generic;
using System.Net;

namespace Geone.Utiliy.Library
{
    public interface IHttpAccess
    {
        RepModel Get(string url, string token = null);

        RepModel Post(string url, string param, string token = null);

        RepModel Put(string url, string param, string token = null);

        RepModel Delete(string url, string param, string token = null);

        RepModel Get<T>(string url, string token = null);

        RepModel Post<T>(string url, string param, string token = null);

        RepModel Put<T>(string url, string param, string token = null);

        RepModel Delete<T>(string url, string param, string token = null);

        RepModel Communicate(string url, string method, string param, Dictionary<string, string> header);

        RepModel Communicate<T>(string url, string method, string param, Dictionary<string, string> header);

        HttpWebResponse GetHttpWebResponse(string url, string method = "Get", string param = null, Dictionary<string, string> header = null, int timeout = 5000);

        string GetHttpWebContent(HttpWebResponse response);
    }
}