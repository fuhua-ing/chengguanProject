using Dapper;
using Geone.Utiliy.Library;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace Geone.Utiliy.Database
{
    /// <summary>
    /// 通用数据库模型
    /// </summary>
    public class DbAction : IDbAction, IDisposable
    {
        //连接名称/模型/字符串
        public string DbName { get; set; }

        //表名/资源名
        public string DbTable { get; set; }

        private IDbConnect conn;
        private ILogWriter log;

        //构造函数
        public DbAction(IDbConnect Conn, ILogWriter Log)
        {
            conn = Conn;
            log = Log;

            DbName = "Default";
            DbTable = string.Empty;
        }

        //设置连接名称/模型/字符串
        public IDbAction SetName(string name)
        {
            DbName = name;
            return this;
        }

        //设置表名/资源名
        public IDbAction SetTable(string table)
        {
            DbTable = table;
            return this;
        }

        //清理查询/操作字符串
        private string CleanSql(string sql)
        {
            //去除可能有的转义符
            sql = sql.Replace("\r", "").Replace("\n", "").Replace("\t", "");
            //去除多余空格
            sql = new Regex("\\s+").Replace(sql, " ");

            return sql;
        }

        //查询数量
        public int QueryCount(string sql)
        {
            string ressql = CleanSql(sql);

            log.WriteSql(DbName, "Query", sql);

            using (IDbConnection db = conn.OpenConn(DbName))
            {
                return db.Query<int>(sql).First();
            }
        }

        //查询单个
        public T QueryFirst<T>(string sql)
        {
            string ressql = CleanSql(sql);

            log.WriteSql(DbName, "Query", ressql);

            using (IDbConnection db = conn.OpenConn(DbName))
            {
                return db.Query<T>(sql).FirstOrDefault();
            }
        }

        //查询列表
        public List<T> QueryList<T>(string sql)
        {
            string ressql = CleanSql(sql);

            log.WriteSql(DbName, "Query", ressql);

            using (IDbConnection db = conn.OpenConn(DbName))
            {
                return db.Query<T>(ressql).ToList();
            }
        }

        //查询分页
        public Page<T> QueryPage<T>(int pi, int ps, string sql, params string[] orders)
        {
            Page<T> page = new Page<T>();

            string ressql = CleanSql(sql);

            if (orders.Length >= 1)
            {
                using (IDbConnection db = conn.OpenConn(DbName))
                {
                    log.WriteSql(DbName, "QueryPage", ressql);
                    List<T> clist = db.Query<T>(ressql).ToList();

                    if (clist != null && clist.Count > 0)
                    {
                        page.Total = clist.Count;

                        string order = $"temp.{orders[0]}";
                        for (int i = 1; i < orders.Length; i++)
                        {
                            order += $",temp.{orders[i]}";
                        }
                        string pagesql = $"select tempo.* from (select temp.*, ROW_NUMBER() over(order by {order}) as Num from ({ressql}) temp )tempo where tempo.Num between {(pi - 1) * ps + 1} and {pi * ps}";

                        log.WriteSql(DbName, "QueryPage", pagesql);
                        page.Rows = db.Query<T>(pagesql).ToList();
                    }
                    else
                    {
                        page.Total = 0;
                        page.Rows = new List<T>();
                    }
                }
            }

            return page;
        }

        //查询
        public List<dynamic> Query(string sql)
        {
            string ressql = CleanSql(sql);

            log.WriteSql(DbName, "Query", ressql);

            using (IDbConnection db = conn.OpenConn(DbName))
            {
                return db.Query<dynamic>(ressql).ToList();
            }
        }

        //添加
        public bool Insert(string sql)
        {
            string ressql = CleanSql(sql);
            log.WriteSql(DbName, "Insert", ressql);

            return Execute(ressql);
        }

        //批量添加
        public bool InsertBatch(params string[] sqls)
        {
            return Trans(sqls);
        }

        //修改
        public bool Modify(string sql)
        {
            string ressql = CleanSql(sql);
            log.WriteSql(DbName, "Modify", ressql);

            return Execute(ressql);
        }

        //批量修改
        public bool ModifyBatch(params string[] sqls)
        {
            return Trans(sqls);
        }

        //删除
        public bool Remove(string sql)
        {
            string ressql = CleanSql(sql);
            log.WriteSql(DbName, "Delete", ressql);

            return Execute(ressql);
        }

        //批量删除
        public bool RemoveBatch(params string[] sqls)
        {
            return Trans(sqls);
        }

        //操作
        private bool Execute(string sql)
        {
            using (IDbConnection db = conn.OpenConn(DbName))
            {
                int res = db.Execute(sql);
                if (res > 0)
                    return true;
                else
                    return false;
            }
        }

        //事务
        public bool Trans(params string[] sqls)
        {
            using (IDbConnection db = conn.OpenConn(DbName))
            {
                using (IDbTransaction trans = db.BeginTransaction())
                {
                    try
                    {
                        foreach (string sql in sqls)
                        {
                            string ressql = CleanSql(sql);
                            log.WriteSql(DbName, "Trans", ressql);
                            db.Execute(ressql, null, trans);
                        }
                        trans.Commit();
                        return true;
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                        return false;
                    }
                }
            }
        }

        #region IDisposable Support

        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~DBActionHelper() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }

    /// <summary>
    /// 通用数据库模型
    /// </summary>
    public class MyDbAction : IDbAction, IDisposable
    {
        //连接名称/模型/字符串
        public string DbName { get; set; }

        //表名/资源名
        public string DbTable { get; set; }

        private IDbConnect conn;
        private ILogWriter log;

        //构造函数
        public MyDbAction(IDbConnect Conn, ILogWriter Log)
        {
            conn = Conn;
            log = Log;

            DbName = "Default";
            DbTable = string.Empty;
        }

        //设置连接名称/模型/字符串
        public IDbAction SetName(string name)
        {
            DbName = name;
            return this;
        }

        //设置表名/资源名
        public IDbAction SetTable(string table)
        {
            DbTable = table;
            return this;
        }

        //清理查询/操作字符串
        private string CleanSql(string sql)
        {
            //去除可能有的转义符
            sql = sql.Replace("\r", "").Replace("\n", "").Replace("\t", "");
            //去除多余空格
            sql = new Regex("\\s+").Replace(sql, " ");

            return sql;
        }

        //查询数量
        public int QueryCount(string sql)
        {
            string ressql = CleanSql(sql);

            log.WriteSql(DbName, "Query", sql);

            using (IDbConnection db = conn.OpenConn(DbName))
            {
                return db.Query<int>(sql).First();
            }
        }

        //查询单个
        public T QueryFirst<T>(string sql)
        {
            string ressql = CleanSql(sql);

            log.WriteSql(DbName, "Query", ressql);

            using (IDbConnection db = conn.OpenConn(DbName))
            {
                return db.Query<T>(sql).FirstOrDefault();
            }
        }

        //查询列表
        public List<T> QueryList<T>(string sql)
        {
            string ressql = CleanSql(sql);

            log.WriteSql(DbName, "Query", ressql);

            using (IDbConnection db = conn.OpenConn(DbName))
            {
                return db.Query<T>(ressql).ToList();
            }
        }

        //查询分页
        public Page<T> QueryPage<T>(int pi, int ps, string sql, params string[] orders)
        {
            Page<T> page = new Page<T>();

            string ressql = CleanSql(sql);

            if (orders.Length >= 1)
            {
                using (IDbConnection db = conn.OpenConn(DbName))
                {
                    log.WriteSql(DbName, "QueryPage", ressql);
                    List<T> clist = db.Query<T>(ressql).ToList();

                    if (clist != null && clist.Count > 0)
                    {
                        page.Total = clist.Count;

                        string order = $"temp.{orders[0]}";
                        for (int i = 1; i < orders.Length; i++)
                        {
                            order += $",temp.{orders[i]}";
                        }
                        string pagesql = $"select temp.* from ({ressql}) temp order by {order} limit {(pi - 1) * ps},{ps}";

                        log.WriteSql(DbName, "QueryPage", pagesql);
                        page.Rows = db.Query<T>(pagesql).ToList();
                    }
                    else
                    {
                        page.Total = 0;
                        page.Rows = new List<T>();
                    }
                }
            }

            return page;
        }

        //查询
        public List<dynamic> Query(string sql)
        {
            string ressql = CleanSql(sql);

            log.WriteSql(DbName, "Query", ressql);

            using (IDbConnection db = conn.OpenConn(DbName))
            {
                return db.Query<dynamic>(ressql).ToList();
            }
        }

        //添加
        public bool Insert(string sql)
        {
            string ressql = CleanSql(sql);
            log.WriteSql(DbName, "Insert", ressql);

            return Execute(ressql);
        }

        //批量添加
        public bool InsertBatch(params string[] sqls)
        {
            return Trans(sqls);
        }

        //修改
        public bool Modify(string sql)
        {
            string ressql = CleanSql(sql);
            log.WriteSql(DbName, "Modify", ressql);

            return Execute(ressql);
        }

        //批量修改
        public bool ModifyBatch(params string[] sqls)
        {
            return Trans(sqls);
        }

        //删除
        public bool Remove(string sql)
        {
            string ressql = CleanSql(sql);
            log.WriteSql(DbName, "Delete", ressql);

            return Execute(ressql);
        }

        //批量删除
        public bool RemoveBatch(params string[] sqls)
        {
            return Trans(sqls);
        }

        //操作
        private bool Execute(string sql)
        {
            using (IDbConnection db = conn.OpenConn(DbName))
            {
                int res = db.Execute(sql);
                if (res > 0)
                    return true;
                else
                    return false;
            }
        }

        //事务
        public bool Trans(params string[] sqls)
        {
            using (IDbConnection db = conn.OpenConn(DbName))
            {
                using (IDbTransaction trans = db.BeginTransaction())
                {
                    try
                    {
                        foreach (string sql in sqls)
                        {
                            string ressql = CleanSql(sql);
                            log.WriteSql(DbName, "Trans", ressql);
                            db.Execute(ressql, null, trans);
                        }
                        trans.Commit();
                        return true;
                    }
                    catch (Exception)
                    {
                        trans.Rollback();
                        return false;
                    }
                }
            }
        }

        #region IDisposable Support

        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~DBActionHelper() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }

    /// <summary>
    /// 通用数据库模型
    /// </summary>
    public class SrvDbAction : IDbAction, IDisposable
    {
        //连接名称/模型/字符串
        public string DbName { get; set; }

        //表名/资源名
        public string DbTable { get; set; }

        //查询操作语句/查询操作参数
        public string DbSql { get; set; }

        //查询操作语句/查询操作参数（复数）
        public string[] DbSqls { get; set; }

        private ISrvDbConnect conn;
        private ILogWriter log;

        public SrvDbAction(ISrvDbConnect Conn, ILogWriter Log)
        {
            conn = Conn;
            log = Log;

            DbName = "Default";
            DbTable = string.Empty;
            DbSql = string.Empty;
            DbSqls = new string[] { };
        }

        public IDbAction SetName(string name)
        {
            DbName = name;
            return this;
        }

        public IDbAction SetTable(string table)
        {
            DbTable = table;
            return this;
        }

        public string CleanSql(string sql)
        {
            sql = DbSql + sql;
            //去除可能有的转义符
            sql = sql.Replace("\r", "").Replace("\n", "").Replace("\t", "");
            //去除多余空格
            sql = new Regex("\\s+").Replace(sql, " ");

            return sql;
        }

        private string SpecifyResource()
        {
            return DbTable.Replace("_", "").ToLower();
        }

        private void Clear()
        {
            DbName = "Default";
            DbTable = string.Empty;
            DbSql = string.Empty;
            DbSqls = new string[] { };
        }

        public int QueryCount(string sql = null)
        {
            string ressql = CleanSql(sql);
            Clear();

            log.WriteSql(DbName, "QueryCount", sql);

            using (ISrvDbConnection srv = conn.OpenSrv(DbName))
            {
                return srv.Count(SpecifyResource(), JObject.Parse(sql));
            }
        }

        public T QueryFirst<T>(string sql = null)
        {
            string ressql = CleanSql(sql);
            Clear();

            log.WriteSql(DbName, "QueryFirst", sql);

            using (ISrvDbConnection srv = conn.OpenSrv(DbName))
            {
                return srv.Query<T>(SpecifyResource(), JObject.Parse(sql)).Rows.FirstOrDefault();
            }
        }

        public List<T> QueryList<T>(string sql = null)
        {
            string ressql = CleanSql(sql);
            Clear();

            log.WriteSql(DbName, "QueryList", ressql);

            using (ISrvDbConnection srv = conn.OpenSrv(DbName))
            {
                return srv.Query<T>(SpecifyResource(), JObject.Parse(sql)).Rows.ToList();
            }
        }

        public Page<T> QueryPage<T>(int pi, int ps, string sql = null, params string[] orders)
        {
            string ressql = CleanSql(sql);
            Clear();

            log.WriteSql(DbName, "QueryPage", ressql);

            using (ISrvDbConnection srv = conn.OpenSrv(DbName))
            {
                return srv.Query<T>(SpecifyResource(), JObject.Parse(sql), pi, ps);
            }
        }

        public List<dynamic> Query(string sql = null)
        {
            string ressql = CleanSql(sql);
            Clear();

            log.WriteSql(DbName, "Query", ressql);

            using (ISrvDbConnection srv = conn.OpenSrv(DbName))
            {
                return srv.Query<dynamic>(SpecifyResource(), JObject.Parse(sql)).Rows.ToList();
            }
        }

        public bool Insert(string sql = null)
        {
            string ressql = CleanSql(sql);
            Clear();

            log.WriteSql(DbName, "Insert", ressql);

            using (ISrvDbConnection srv = conn.OpenSrv(DbName))
            {
                return srv.Insert(SpecifyResource(), JObject.Parse(sql));
            }
        }

        public bool InsertBatch(params string[] sqls)
        {
            Clear();
            JArray arr = new JArray();
            foreach (string sql in sqls)
            {
                string ressql = CleanSql(sql);
                log.WriteSql(DbName, "InsertBatch", ressql);
                arr.Add(JObject.Parse(ressql));
            }

            using (ISrvDbConnection srv = conn.OpenSrv(DbName))
            {
                return srv.InsertBatch(SpecifyResource(), arr);
            }
        }

        public bool Modify(string sql = null)
        {
            string ressql = CleanSql(sql);
            Clear();

            log.WriteSql(DbName, "Modify", ressql);

            using (ISrvDbConnection srv = conn.OpenSrv(DbName))
            {
                return srv.Insert(SpecifyResource(), JObject.Parse(sql));
            }
        }

        public bool ModifyBatch(params string[] sqls)
        {
            Clear();
            JArray arr = new JArray();
            foreach (string sql in sqls)
            {
                string ressql = CleanSql(sql);
                log.WriteSql(DbName, "ModifyBatch", ressql);
                arr.Add(JObject.Parse(ressql));
            }

            using (ISrvDbConnection srv = conn.OpenSrv(DbName))
            {
                return srv.ModifyBatch(SpecifyResource(), arr);
            }
        }

        public bool Remove(string sql = null)
        {
            string ressql = CleanSql(sql);
            Clear();

            log.WriteSql(DbName, "Delete", ressql);

            using (ISrvDbConnection srv = conn.OpenSrv(DbName))
            {
                return srv.Insert(SpecifyResource(), JObject.Parse(sql));
            }
        }

        public bool RemoveBatch(params string[] sqls)
        {
            Clear();
            JArray arr = new JArray();
            foreach (string sql in sqls)
            {
                string ressql = CleanSql(sql);
                log.WriteSql(DbName, "DeleteBatch", ressql);
                arr.Add(JObject.Parse(ressql));
            }

            using (ISrvDbConnection srv = conn.OpenSrv(DbName))
            {
                return srv.DeleteBatch(SpecifyResource(), arr);
            }
        }

        public bool Trans(params string[] sqls)
        {
            return false;
        }

        #region IDisposable Support

        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~DBActionHelper() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}