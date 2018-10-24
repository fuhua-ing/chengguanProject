using Geone.Utiliy.Library;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Geone.Utiliy.Database
{
    public class DbLambda<TParam> : IDbLambda<TParam>, IDisposable where TParam : IParam
    {
        //查询操作语句/查询操作参数
        public string DbSql { get; set; }

        //查询操作语句/查询操作参数（复数）
        public string[] DbSqls { get; set; }

        //连接名称/模型/字符串
        public string DbName { get; set; }

        //表名/资源名
        public string DbTable { get; set; }

        //参数模型
        public TParam DbParam { get; set; }

        private IDbConnect _conn;
        private ILogWriter _log;
        private IDbAction _action;

        public DbLambda(IDbConnect Conn, ILogWriter Log, IDbAction Action)
        {
            _conn = Conn;
            _log = Log;
            _action = Action;

            DbName = "Default";
            DbTable = string.Empty;
            DbSql = string.Empty;
            DbSqls = new string[] { };

            if (typeof(TParam) == typeof(IParam)) DbParam = default;
            else DbParam = (TParam)Activator.CreateInstance(typeof(TParam));
        }

        private void Clear()
        {
            DbSql = string.Empty;
            DbSqls = new string[] { };
        }

        //设置连接名称/模型/字符串
        public IDbLambda<TParam> SetName(string name)
        {
            DbName = name;
            return this;
        }

        //设置表名/资源名
        public IDbLambda<TParam> SetTable(string table)
        {
            DbTable = table;
            return this;
        }

        //设置参数
        public IDbLambda<TParam> SetParam(TParam param)
        {
            if (param != null)
                DbParam = param;

            return this;
        }

        //结束一次拼接
        public IDbLambda<TParam> End()
        {
            //把DbSql加入DbSqls
            List<string> sqls = DbSqls.ToList();
            sqls.Add(DbSql);
            DbSqls = sqls.ToArray();
            //清空DbSql
            DbSql = string.Empty;

            return this;
        }

        #region //私有方法

        private void GetParam(out string name, out string[] final, Expression<Func<TParam, dynamic>> fun)
        {
            name = string.Empty;

            int check = CheckOperation(fun);

            if (DbParam != null)
            {
                if (check == 1 || check == 2)
                {
                    string[] context = fun.Compile()((TParam)DbParam);
                    if (context == null) final = new string[0] { };
                    else
                    {
                        name = context[0];

                        List<string> e = context.ToList();
                        e.Remove(e[0]);

                        final = e.ToArray();
                    }
                }
                else if (check == 3)
                {
                    string context = fun.Compile()((TParam)DbParam);
                    name = context;

                    final = new string[0];
                }
                else if (check == 4 || check == 5)
                {
                    name = fun.Body.ToString().Split('.')[1];
                    name = name.Replace("(", null).Replace(")", null).Replace(" ", null).Replace(",Object", null);

                    string[] context = fun.Compile()((TParam)DbParam);
                    if (context == null) final = new string[1] { null };
                    else
                    {
                        List<string> e = context.ToList();
                        e.Remove(e[0]);

                        final = e.ToArray();
                    }
                }
                else if (check == 6)
                {
                    name = fun.Body.ToString().Split('.')[1];
                    name = name.Replace("(", null).Replace(")", null).Replace(" ", null).Replace(",Object", null);

                    final = new string[0];
                }
                else
                {
                    final = new string[0];
                }
            }
            else
            {
                string body = fun.Body.ToString();

                string[] split = body.Split('"');

                if (check == 1 || check == 2)
                {
                    string iset = split[1];
                    string lset = split[3];

                    name = iset;
                    final = new string[] { lset };
                }
                else if (check == 3)
                {
                    string iset = split[1];

                    name = iset;
                    final = new string[0];
                }
                else if (check == 4 || check == 5)
                {
                    string lset = split[1];

                    name = fun.Body.ToString().Split('.')[1];
                    name = name.Replace("(", null).Replace(")", null).Replace(" ", null);

                    final = new string[] { lset };
                }
                else if (check == 6)
                {
                    name = fun.Body.ToString().Split('.')[1];
                    name = name.Replace("(", null).Replace(")", null).Replace(" ", null);

                    final = new string[0];
                }
                else
                {
                    final = new string[0];
                }
            }
        }

        private int CheckOperation(Expression<Func<TParam, dynamic>> fun)
        {
            //Set|Sub|Other| = Result
            //√ |√ | √  | =   0
            //√ |√ | ×  | =   1
            //√ |× | √  | =   2
            //√ |× | ×  | =   3
            //× |√ | √  | =   0
            //× |√ | ×  | =   4
            //× |× | √  | =   5
            //× |× | ×  | =   6
            string body = fun.Body.ToString();

            if (body.Contains(".Set("))
            {
                if (body.Contains(".Sub("))
                {
                    if (body.Contains(".Val(") || body.Contains(".Eq(") || body.Contains(".Ne(") || body.Contains(".Lt(") || body.Contains(".Le(") || body.Contains(".Gt(") || body.Contains(".Ge(") || body.Contains(".Like(") || body.Contains(".Between(") || body.Contains(".In(") || body.Contains(".Isnull(") || body.Contains(".Isnotnull("))
                    {
                        return 0;
                    }
                    else
                    {
                        return 1;
                    }
                }
                else
                {
                    if (body.Contains(".Val(") || body.Contains(".Eq(") || body.Contains(".Ne(") || body.Contains(".Lt(") || body.Contains(".Le(") || body.Contains(".Gt(") || body.Contains(".Ge(") || body.Contains(".Like(") || body.Contains(".Between(") || body.Contains(".In(") || body.Contains(".Isnull(") || body.Contains(".Isnotnull("))
                    {
                        return 2;
                    }
                    else
                    {
                        return 3;
                    }
                }
            }
            else
            {
                if (body.Contains(".Sub("))
                {
                    if (body.Contains(".Val(") || body.Contains(".Eq(") || body.Contains(".Ne(") || body.Contains(".Lt(") || body.Contains(".Le(") || body.Contains(".Gt(") || body.Contains(".Ge(") || body.Contains(".Like(") || body.Contains(".Between(") || body.Contains(".In(") || body.Contains(".Isnull(") || body.Contains(".Isnotnull("))
                    {
                        return 0;
                    }
                    else
                    {
                        return 4;
                    }
                }
                else
                {
                    if (body.Contains(".Val(") || body.Contains(".Eq(") || body.Contains(".Ne(") || body.Contains(".Lt(") || body.Contains(".Le(") || body.Contains(".Gt(") || body.Contains(".Ge(") || body.Contains(".Like(") || body.Contains(".Between(") || body.Contains(".In(") || body.Contains(".Isnull(") || body.Contains(".Isnotnull("))
                    {
                        return 5;
                    }
                    else
                    {
                        return 6;
                    }
                }
            }
        }

        private string CheckSymbol(Expression<Func<TParam, dynamic>> fun)
        {
            string body = fun.Body.ToString();

            if (body.Contains(".Eq"))
            {
                return "=";
            }
            else if (body.Contains(".Ne"))
            {
                return "<>";
            }
            else if (body.Contains(".Lt"))
            {
                return "<";
            }
            else if (body.Contains(".Le"))
            {
                return "<=";
            }
            else if (body.Contains(".Gt"))
            {
                return ">";
            }
            else if (body.Contains(".Ge"))
            {
                return ">=";
            }
            else if (body.Contains(".Like"))
            {
                return "LIKE";
            }
            else if (body.Contains(".Isnull"))
            {
                return "IS NULL";
            }
            else if (body.Contains(".Isnotnull"))
            {
                return "IS NOT NULL";
            }
            else
            {
                return null;
            }
        }

        #endregion //私有方法

        #region //获取私有方法

        //获取查询参数--string
        private string GetSelectParam(params string[] @params)
        {
            string sql = string.Empty;

            if (@params.Count() != 0)
            {
                for (int i = 0; i < @params.Count(); i++)
                {
                    if (i == @params.Count() - 1)
                    {
                        sql += $"{@params[i]} ";
                    }
                    else
                    {
                        sql += $"{@params[i]},";
                    }
                }
            }

            return sql;
        }

        //获取查询参数--Expression<Func<TParam, dynamic>>
        private string GetSelectParam(params Expression<Func<TParam, dynamic>>[] funs)
        {
            string sql = string.Empty;
            List<string> e = new List<string>();

            //如果未传参，默认查询*
            if (funs.Count() != 0)
            {
                //获取参数String列表
                foreach (Expression<Func<TParam, dynamic>> fun in funs)
                {
                    string value = string.Empty;

                    GetParam(out string name, out string[] final, fun);

                    if (name != null && final.Count() != 0)
                    {
                        value = $"({final[0]}) AS {name}";
                    }
                    else if (name != null && final.Count() == 0)
                    {
                        value = $"{name}";
                    }

                    if (!string.IsNullOrWhiteSpace(value))
                        e.Add(value);
                }

                //参数String数
                int length = e.Count();

                if (length != 0)
                {
                    for (int i = 0; i < length; i++)
                    {
                        if (i == length - 1)
                        {
                            sql += $"{e[i]} ";
                        }
                        else
                        {
                            sql += $"{e[i]},";
                        }
                    }
                }
            }

            return sql;
        }

        //获取查询参数--Expression<Func<TParam, dynamic>>
        private string GetCountParam(Expression<Func<TParam, dynamic>> fun)
        {
            string sql = string.Empty;
            List<string> e = new List<string>();

            //如果未传参，默认查询*
            if (fun != null)
            {
                string value = string.Empty;
                GetParam(out string name, out string[] final, fun);

                if (name != null && final.Count() != 0)
                {
                    value = $"{final[0]}";
                }
                else if (name != null && final.Count() == 0)
                {
                    value = $"{name}";
                }

                if (!string.IsNullOrWhiteSpace(value))
                    sql = value;
            }

            return sql;
        }

        //获取参数返回字典形kv值--Expression<Func<TParam, dynamic>>
        private Dictionary<string, string> GetDicParam(params Expression<Func<TParam, dynamic>>[] funs)
        {
            Dictionary<string, string> e = new Dictionary<string, string>();

            if (funs.Count() == 0)
            {
                Type type = DbParam.GetType();

                //反射获取键名集合
                foreach (PropertyInfo pi in type.GetProperties())
                {
                    object Value = pi.GetValue(DbParam);

                    if (Value == null)
                        e.Add(pi.Name, null);
                    else
                        e.Add(pi.Name, Value.ToString());
                }
            }
            else
            {
                foreach (Expression<Func<TParam, dynamic>> fun in funs)
                {
                    GetParam(out string name, out string[] final, fun);

                    if (fun.Body.ToString().Contains(".Val("))
                    {
                        e.Add(name, final[0]);
                    }
                    else
                    {
                        Type type = DbParam.GetType();
                        //反射获取键名集合
                        foreach (PropertyInfo pi in type.GetProperties())
                        {
                            if (pi.Name == name)
                            {
                                try
                                {
                                    e.Add(pi.Name, pi.GetValue(DbParam).ToString());
                                }
                                catch
                                {
                                    e.Add(pi.Name, null);
                                }
                            }
                        }
                    }
                }
            }

            return e;
        }

        //获取参数返回字典形kv值--JObject
        private Dictionary<string, string> GetDicParam(JObject @params)
        {
            Dictionary<string, string> e = new Dictionary<string, string>();

            Dictionary<string, string> @object = @params.ToObject<Dictionary<string, string>>();
            if (@object != null)
                e = @object;

            return e;
        }

        //获取条件参数
        private string GetOperation(Expression<Func<TParam, dynamic>> fun)
        {
            string value = string.Empty;

            GetParam(out string name, out string[] final, fun);
            string body = fun.Body.ToString();

            string symbol = CheckSymbol(fun);

            if (name != null && final.Count() != 0)
            {
                if (symbol != null)
                {
                    if (symbol != "IS NULL" && symbol != "IS NOT NULL")
                    {
                        value = $"{name} {symbol} '{final[0]}' ";
                    }
                    else
                    {
                        value = $"{name} {symbol} ";
                    }
                }
                else
                {
                    if (body.Contains(".Sub("))
                    {
                        value = $"({final[0]}) AS {name} ";
                    }
                    else if (body.Contains(".Between("))
                    {
                        if (final.Count() == 1)
                        {
                            string[] split = final[0].Split(',');
                            value = $"{name} BETWEEN '{split[0]}' AND '{split[1]}' ";
                        }
                        else
                        {
                            value = $"{name} BETWEEN '{final[0]}' AND '{final[1]}' ";
                        }
                    }
                    else if (body.Contains(".In("))
                    {
                        if (final.Count() == 1)
                        {
                            value = $"{name} IN ('{final[0]}') ";
                        }
                        else
                        {
                            string str = string.Empty;
                            for (int i = 0; i < final.Count(); i++)
                            {
                                if (i == final.Count() - 1)
                                {
                                    str += $"'{final[i]}' ";
                                }
                                else
                                {
                                    str += $"'{final[i]}',";
                                }
                            }
                            value = $"{name} IN ({str}) ";
                        }
                    }
                }
            }
            else if (name != null && final.Count() == 0)
            {
                value = $"{name} ";
            }

            return value;
        }

        //获取条件参数
        private string GetOperation(string param)
        {
            string sql = string.Empty;

            sql += $"{ param } ";

            return sql;
        }

        #endregion //获取私有方法

        public IDbLambda<TParam> Count(Expression<Func<TParam, dynamic>> fun = null, bool isDistinct = false)
        {
            string param = GetCountParam(fun);

            if (string.IsNullOrWhiteSpace(param))
                DbSql += $"SELECT COUNT(*) FROM {DbTable} ";
            else
            {
                if (isDistinct)
                    DbSql += $"SELECT COUNT(DISTINCT {param}) FROM {DbTable} ";
                else
                    DbSql = $"SELECT COUNT({param}) FROM {DbTable} ";
            }

            return this;
        }

        public IDbLambda<TParam> Select(params Expression<Func<TParam, dynamic>>[] funs)
        {
            string param = GetSelectParam(funs);

            if (string.IsNullOrWhiteSpace(param))
                DbSql += $"SELECT * FROM {DbTable} ";
            else
                DbSql += $"SELECT {param} FROM {DbTable} ";

            return this;
        }

        public IDbLambda<TParam> Select(params string[] @params)
        {
            string param = GetSelectParam(@params);

            if (string.IsNullOrWhiteSpace(param))
                DbSql += $"SELECT * FROM {DbTable} ";
            else
                DbSql += $"SELECT {param} FROM {DbTable} ";

            return this;
        }

        public IDbLambda<TParam> Insert(params Expression<Func<TParam, dynamic>>[] funs)
        {
            string sql = string.Empty;
            string keysql = string.Empty;
            string valuesql = string.Empty;

            Dictionary<string, string> e = GetDicParam(funs);

            if (e.Count() != 0)
            {
                foreach (KeyValuePair<string, string> kvp in e)
                {
                    var va = kvp.Value;
                    keysql += $"[{kvp.Key}]";
                    if (va == null)
                        valuesql += $"NULL";
                    else
                        valuesql += $"'{va.ToString().Replace("'", "''")}'";

                    if (!kvp.Equals(e.Last()))
                    {
                        keysql += ",";
                        valuesql += ",";
                    }
                }

                sql = $"INSERT INTO {DbTable} ({keysql}) VALUES ({valuesql}) ";
            }

            DbSql += sql;

            return this;
        }

        public IDbLambda<TParam> Insert(JObject @params)
        {
            string sql = string.Empty;
            string keysql = string.Empty;
            string valuesql = string.Empty;

            Dictionary<string, string> e = GetDicParam(@params);

            if (e.Count() != 0)
            {
                foreach (KeyValuePair<string, string> kvp in e)
                {
                    var va = kvp.Value;
                    keysql += $"{kvp.Key}";
                    if (va == null)
                        valuesql += $"NULL";
                    else
                        valuesql += $"'{va.ToString().Replace("'", "''")}'";

                    if (!kvp.Equals(e.Last()))
                    {
                        keysql += ",";
                        valuesql += ",";
                    }
                }

                sql = $"INSERT INTO {DbTable} ({keysql}) VALUES ({valuesql}) ";
            }

            DbSql += sql;

            return this;
        }

        public IDbLambda<TParam> Update(params Expression<Func<TParam, dynamic>>[] funs)
        {
            string sql = string.Empty;
            string keyvaluesql = string.Empty;

            Dictionary<string, string> e = GetDicParam(funs);

            if (e.Count() != 0)
            {
                foreach (KeyValuePair<string, string> kvp in e)
                {
                    var va = kvp.Value;
                    if (va == null)
                        keyvaluesql += $"[{kvp.Key}] = NULL";
                    else
                        keyvaluesql += $"[{kvp.Key}] = '{va.ToString().Replace("'", "''")}'";

                    if (!kvp.Equals(e.Last()))
                    {
                        keyvaluesql += ",";
                    }
                }

                sql = $"UPDATE {DbTable} SET {keyvaluesql} ";
            }

            DbSql += sql;

            return this;
        }

        public IDbLambda<TParam> Update(JObject @params)
        {
            string sql = string.Empty;
            string keyvaluesql = string.Empty;

            Dictionary<string, string> e = GetDicParam(@params);

            if (e.Count() != 0)
            {
                foreach (KeyValuePair<string, string> kvp in e)
                {
                    var va = kvp.Value;
                    if (va == null)
                        keyvaluesql += $"[{kvp.Key}] = NULL";
                    else
                        keyvaluesql += $"[{kvp.Key}] = '{va.ToString().Replace("'", "''")}'";

                    if (!kvp.Equals(e.Last()))
                    {
                        keyvaluesql += ",";
                    }
                }

                sql = $"UPDATE {DbTable} SET {keyvaluesql} ";
            }

            DbSql += sql;

            return this;
        }

        public IDbLambda<TParam> Delete()
        {
            string sql = $"DELETE FROM {DbTable} ";

            DbSql += sql;

            return this;
        }

        public IDbLambda<TParam> Where()
        {
            DbSql += "WHERE ";

            return this;
        }

        public IDbLambda<TParam> And()
        {
            DbSql += $"AND ";

            return this;
        }

        public IDbLambda<TParam> Or()
        {
            DbSql += $"OR ";

            return this;
        }

        public IDbLambda<TParam> LBracket()
        {
            DbSql += $"( ";

            return this;
        }

        public IDbLambda<TParam> RBracket()
        {
            DbSql += $") ";

            return this;
        }

        public IDbLambda<TParam> Where(Expression<Func<TParam, dynamic>> fun)
        {
            if (fun == null)
                DbSql += "WHERE ";
            else
                DbSql += $"WHERE {GetOperation(fun)}";

            return this;
        }

        public IDbLambda<TParam> WhereAnd(params Expression<Func<TParam, dynamic>>[] funs)
        {
            if (funs.Count() == 0)
            {
                DbSql += "WHERE ";
            }
            else if (funs.Count() == 1)
            {
                Expression<Func<TParam, dynamic>> fun = funs[0];
                DbSql += $"WHERE {GetOperation(fun)}";
            }
            else
            {
                DbSql += $"WHERE ";
                And(funs);
            }

            return this;
        }

        public IDbLambda<TParam> WhereOr(params Expression<Func<TParam, dynamic>>[] funs)
        {
            if (funs.Count() == 0)
            {
                DbSql += "WHERE ";
            }
            else if (funs.Count() == 1)
            {
                Expression<Func<TParam, dynamic>> fun = funs[0];
                DbSql += $"WHERE {GetOperation(fun)}";
            }
            else
            {
                DbSql += $"WHERE ";
                Or(funs);
            }

            return this;
        }

        public IDbLambda<TParam> And(params Expression<Func<TParam, dynamic>>[] funs)
        {
            if (funs.Count() == 0)
            {
                DbSql += "AND ";
            }
            else if (funs.Count() == 1)
            {
                Expression<Func<TParam, dynamic>> fun = funs[0];
                DbSql += $"AND {GetOperation(fun)}";
            }
            else
            {
                List<string> e = new List<string>();

                foreach (Expression<Func<TParam, dynamic>> fun in funs)
                {
                    e.Add($"{GetOperation(fun)}");
                }

                if (e.Count() != 0)
                    DbSql += $"({string.Join(" AND ", e)}) ";
            }

            return this;
        }

        public IDbLambda<TParam> Or(params Expression<Func<TParam, dynamic>>[] funs)
        {
            if (funs.Count() == 0)
            {
                DbSql += "OR ";
            }
            else if (funs.Count() == 1)
            {
                Expression<Func<TParam, dynamic>> fun = funs[0];
                DbSql += $"OR {GetOperation(fun)}";
            }
            else
            {
                List<string> e = new List<string>();

                foreach (Expression<Func<TParam, dynamic>> fun in funs)
                {
                    e.Add($"{GetOperation(fun)}");
                }

                if (e.Count() != 0)
                    DbSql += $"({string.Join(" OR ", e)}) ";
            }

            return this;
        }

        public IDbLambda<TParam> Where(string param)
        {
            if (param == null)
                DbSql += "WHERE ";
            else
                DbSql += $"WHERE {param}";

            return this;
        }

        public IDbLambda<TParam> WhereAnd(params string[] param)
        {
            if (param.Count() == 0)
            {
                DbSql += "WHERE ";
            }
            else if (param.Count() == 1)
            {
                string p = param[0];
                DbSql += $"WHERE {GetOperation(p)}";
            }
            else
            {
                DbSql += $"WHERE ";
                And(param);
            }

            return this;
        }

        public IDbLambda<TParam> WhereOr(params string[] param)
        {
            if (param.Count() == 0)
            {
                DbSql += "WHERE ";
            }
            else if (param.Count() == 1)
            {
                string p = param[0];
                DbSql += $"WHERE {GetOperation(p)}";
            }
            else
            {
                DbSql += $"WHERE ";
                Or(param);
            }

            return this;
        }

        public IDbLambda<TParam> And(params string[] param)
        {
            if (param.Count() == 0)
            {
                DbSql += "AND ";
            }
            else if (param.Count() == 1)
            {
                string p = param[0];
                DbSql += $"AND {GetOperation(p)}";
            }
            else
            {
                List<string> e = new List<string>();

                foreach (string p in param)
                {
                    e.Add($"{GetOperation(p)}");
                }

                if (e.Count() != 0)
                    DbSql += $"({string.Join(" AND ", e)}) ";
            }

            return this;
        }

        public IDbLambda<TParam> Or(params string[] param)
        {
            if (param.Count() == 0)
            {
                DbSql += "OR ";
            }
            else if (param.Count() == 1)
            {
                string p = param[0];
                DbSql += $"OR {GetOperation(p)}";
            }
            else
            {
                List<string> e = new List<string>();

                foreach (string p in param)
                {
                    e.Add($"{GetOperation(p)}");
                }

                if (e.Count() != 0)
                    DbSql += $"({string.Join(" OR ", e)}) ";
            }

            return this;
        }

        public IDbLambda<TParam> Union()
        {
            DbSql += "UNION ";
            return this;
        }

        public IDbLambda<TParam> GroupBy(Expression<Func<TParam, dynamic>> fun)
        {
            GetParam(out string name, out string[] final, fun);
            if (name != null)
                DbSql += $"GROUP BY {name} ";

            return this;
        }

        public IDbLambda<TParam> OrderBy(Expression<Func<TParam, dynamic>> fun)
        {
            GetParam(out string name, out string[] final, fun);
            if (name != null)
                DbSql += $"ORDER BY {name} ";

            return this;
        }

        public IDbLambda<TParam> OrderByDesc(Expression<Func<TParam, dynamic>> fun)
        {
            GetParam(out string name, out string[] final, fun);
            if (name != null)
                DbSql += $"ORDER BY {name} DESC";

            return this;
        }

        public IDbLambda<TParam> GroupBy(string param)
        {
            DbSql += $"GROUP BY {param} ";
            return this;
        }

        public IDbLambda<TParam> OrderBy(string param)
        {
            DbSql += $"ORDER BY {param} ";
            return this;
        }

        public IDbLambda<TParam> OrderByDesc(string param)
        {
            DbSql += $"ORDER BY {param} DESC ";
            return this;
        }

        public int QueryCount()
        {
            string sql = DbSql;
            Clear();
            return _action.QueryCount(sql);
        }

        public TParam QueryFirst()
        {
            string sql = DbSql;
            Clear();
            return _action.QueryFirst<TParam>(sql);
        }

        public List<TParam> QueryList()
        {
            string sql = DbSql;
            Clear();
            return _action.QueryList<TParam>(sql);
        }

        public Page<TParam> QueryPage(int pi, int ps, params string[] orders)
        {
            string sql = DbSql;
            Clear();
            return _action.QueryPage<TParam>(pi, ps, sql, orders);
        }

        public TEntity QueryFirst<TEntity>()
        {
            string sql = DbSql;
            Clear();
            return _action.QueryFirst<TEntity>(sql);
        }

        public List<TEntity> QueryList<TEntity>()
        {
            string sql = DbSql;
            Clear();
            return _action.QueryList<TEntity>(sql);
        }

        public Page<TEntity> QueryPage<TEntity>(int pi, int ps, params string[] orders)
        {
            string sql = DbSql;
            Clear();
            return _action.QueryPage<TEntity>(pi, ps, sql, orders);
        }

        public bool ExecInsert()
        {
            string sql = DbSql;
            Clear();
            return _action.Insert(sql);
        }

        public bool ExecInsertBatch()
        {
            List<string> sqls = new List<string>();
            foreach (string sql in DbSqls)
            {
                sqls.Add(sql + DbSql);
            };
            Clear();
            return _action.InsertBatch(sqls.ToArray());
        }

        public bool ExecModify()
        {
            string sql = DbSql;
            Clear();
            return _action.Modify(sql);
        }

        public bool ExecModifyBatch()
        {
            List<string> sqls = new List<string>();
            foreach (string sql in DbSqls)
            {
                sqls.Add(sql + DbSql);
            };
            Clear();
            return _action.ModifyBatch(sqls.ToArray());
        }

        public bool ExecRemove()
        {
            string sql = DbSql;
            Clear();
            return _action.Remove(sql);
        }

        public bool ExecRemoveBatch()
        {
            List<string> sqls = new List<string>();
            foreach (string sql in DbSqls)
            {
                sqls.Add(sql + DbSql);
            };
            Clear();
            return _action.RemoveBatch(sqls.ToArray());
        }

        public bool ExecTrans()
        {
            List<string> sqls = new List<string>();
            foreach (string sql in DbSqls)
            {
                sqls.Add(sql + DbSql);
            };
            Clear();
            return _action.Trans(sqls.ToArray());
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

    //public class SrvDbLambda : SrvDbAction, IDbLambda
    //{
    //    private IDbConnect conn;
    //    private ILogWriter log;

    //    public SrvDbLambda(IDbConnect Conn, ILogWriter Log)
    //        : base(Conn, Log)
    //    {
    //        conn = Conn;
    //        log = Log;

    //        DbName = "Default";
    //        DbTable = string.Empty;
    //    }
    //}
}