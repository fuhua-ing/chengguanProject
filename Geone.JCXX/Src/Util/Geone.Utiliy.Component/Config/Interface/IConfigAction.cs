using Geone.Utiliy.Library;
using System.Collections.Generic;

namespace Geone.Utiliy.Component
{
    public interface IConfigAction
    {
        void Init(string path);

        T GetModel<T>(string name, string id) where T : BaseConfig;

        List<T> GetModels<T>(string name, string[] ids) where T : BaseConfig;

        List<T> GetAll<T>(string name) where T : BaseConfig;

        Page<T> GetPage<T>(string name, int pi, int ps) where T : BaseConfig;

        Page<T> GetPageEx<T>(string name, int pi, int ps, string content) where T : BaseConfig;

        bool PostModel<T>(string name, dynamic value) where T : BaseConfig;

        bool PostModels<T>(string name, dynamic[] values) where T : BaseConfig;

        bool PutModel<T>(string name, dynamic value) where T : BaseConfig;

        bool PutModels<T>(string name, dynamic[] values) where T : BaseConfig;

        bool DeleteModel<T>(string name, string key) where T : BaseConfig;

        bool DeleteModels<T>(string name, string[] keys) where T : BaseConfig;

        bool Empty<T>(string name) where T : BaseConfig;
    }
}