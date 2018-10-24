using Geone.Utiliy.Library;
using MagicOnion;

namespace Geone.Utiliy.Component
{
    public interface IConfig : IService<IConfig>
    {
        #region 服务调用

        //查询单个
        UnaryResult<RepModel> GetModel(RpcReq req);

        //查询批量
        UnaryResult<RepModel> GetModels(RpcReq req);

        //查询所有
        UnaryResult<RepModel> GetAll(RpcReq req);

        //分页查询
        UnaryResult<RepModel> GetPage(RpcReq req);

        //分页查询-带简单搜索
        UnaryResult<RepModel> GetPageEx(RpcReq req);

        //新增单个
        UnaryResult<RepModel> PostModel(RpcReq req);

        //新增批量
        UnaryResult<RepModel> PostModels(RpcReq req);

        //修改单个
        UnaryResult<RepModel> PutModel(RpcReq req);

        //修改批量
        UnaryResult<RepModel> PutModels(RpcReq req);

        //删除单个
        UnaryResult<RepModel> DeleteModel(RpcReq req);

        //删除批量
        UnaryResult<RepModel> DeleteModels(RpcReq req);

        //清空
        UnaryResult<RepModel> Empty(RpcReq req);

        #endregion 服务调用
    }
}