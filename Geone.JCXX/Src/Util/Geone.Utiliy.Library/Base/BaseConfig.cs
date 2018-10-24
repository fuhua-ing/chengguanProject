using System;

namespace Geone.Utiliy.Library
{
    public class BaseConfig
    {
        public BaseConfig()
        {
            Id = Guid.NewGuid().ToString("N");
            Isdelete = false;
        }

        //编号-唯一标识
        public string Id { get; set; }

        //是否已删除
        public bool Isdelete { get; set; }
    }
}