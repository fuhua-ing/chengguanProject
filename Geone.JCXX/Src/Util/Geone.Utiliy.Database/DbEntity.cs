using Geone.Utiliy.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Geone.Utiliy.Database
{
    public class DbEntity<TEntity> : IDbEntity<TEntity>, IDisposable where TEntity : IEntity
    {
        //操作主键
        public string DbPrimaryKey { get; set; }

        //查询操作语句/查询操作参数
        public string DbSql { get; set; }

        //查询操作语句/查询操作参数（复数）
        public string[] DbSqls { get; set; }

        //键集合，实例化后不予变更
        public string[] DbKeys { get; set; }

        //键增集合
        public string[] DbKeysPlus { get; set; }

        //键减集合
        public string[] DbKeysMinus { get; set; }

        //表名/资源名
        public string DbTable { get; set; }

        //参数模型
        public TEntity DbParam { get; set; }

        //空间统一标识符
        public int GeoSrid { get; set; }

        private IDbConnect _conn;
        private ILogWriter _log;
        private IDbAction _action;

        public DbEntity(IDbConnect Conn, ILogWriter Log, IDbAction Action)
        {
            _conn = Conn;
            _log = Log;
            _action = Action;

            _action.SetName("Default");
            DbTable = string.Empty;
            GeoSrid = 0;
            DbSql = string.Empty;
            DbSqls = new string[] { };

            List<string> properties = new List<string>();
            Type type = typeof(TEntity);
            foreach (PropertyInfo pi in type.GetProperties())
            {
                List<Attribute> attrs = pi.GetCustomAttributes().ToList();
                bool check = true;

                if (attrs != null && attrs.Count > 0)
                {
                    foreach (Attribute attr in attrs)
                    {
                        if (attr.TypeId.ToString() == "Geone.Utiliy.Database.IgnoreAttribute")
                        {
                            check = false;
                        }

                        if (attr.TypeId.ToString() == "Geone.Utiliy.Database.PrimaryKeyAttribute")
                        {
                            DbPrimaryKey = pi.Name;
                        }
                    }
                }

                if (check)
                    properties.Add(pi.Name);
            }

            //操作主键-默认“ID”
            if (DbPrimaryKey == null)
                DbPrimaryKey = "ID";

            DbKeys = properties.ToArray();
            DbKeysPlus = null;
            DbKeysMinus = null;

            if (typeof(TEntity) == typeof(IEntity)) DbParam = default;
            else DbParam = (TEntity)Activator.CreateInstance(typeof(TEntity));
        }

        private void Clear()
        {
            DbSql = string.Empty;
            DbSqls = new string[] { };

            DbKeysPlus = null;
            DbKeysMinus = null;
        }

        //设置连接名称/模型/字符串
        public IDbEntity<TEntity> SetName(string name)
        {
            _action.SetName(name);
            return this;
        }

        //设置表名/资源名
        public IDbEntity<TEntity> SetTable(string table)
        {
            DbTable = table;
            return this;
        }

        //设置主键
        public IDbEntity<TEntity> SetPrimaryKey(string key)
        {
            DbPrimaryKey = key;
            return this;
        }

        //设置参数
        public IDbEntity<TEntity> SetParam(TEntity param)
        {
            if (param != null)
                DbParam = param;

            return this;
        }

        //设置设置空间统一标识
        public IDbEntity<TEntity> SetSrid(int srid)
        {
            GeoSrid = srid;
            return this;
        }

        //结束一次拼接
        public IDbEntity<TEntity> End()
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

        private void GetParam(out string name, out string[] final, Expression<Func<TEntity, dynamic>> fun)
        {
            name = string.Empty;

            int check = CheckOperation(fun);

            if (DbParam != null)
            {
                if (check == 1 || check == 2)
                {
                    string[] context = fun.Compile()((TEntity)DbParam);
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
                    string context = fun.Compile()((TEntity)DbParam);
                    name = context;

                    final = new string[0];
                }
                else if (check == 4 || check == 5)
                {
                    name = fun.Body.ToString().Split('.')[1];
                    name = name.Replace("(", null).Replace(")", null).Replace(" ", null).Replace(",Object", null);

                    string[] context = fun.Compile()((TEntity)DbParam);
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

        private int CheckOperation(Expression<Func<TEntity, dynamic>> fun)
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
                    if (body.Contains(".Val(") || body.Contains(".Eq(") || body.Contains(".Ne(") || body.Contains(".Lt(") || body.Contains(".Le(") || body.Contains(".Gt(") || body.Contains(".Ge(") || body.Contains(".Like(") || body.Contains(".Between(") || body.Contains(".In(") || body.Contains(".IsNull(") || body.Contains(".IsNotNull(") || body.Contains(".IsContains(") || body.Contains(".IsNotContains("))
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
                    if (body.Contains(".Val(") || body.Contains(".Eq(") || body.Contains(".Ne(") || body.Contains(".Lt(") || body.Contains(".Le(") || body.Contains(".Gt(") || body.Contains(".Ge(") || body.Contains(".Like(") || body.Contains(".Between(") || body.Contains(".In(") || body.Contains(".IsNull(") || body.Contains(".IsNotNull(") || body.Contains(".IsContains(") || body.Contains(".IsNotContains("))
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
                    if (body.Contains(".Val(") || body.Contains(".Eq(") || body.Contains(".Ne(") || body.Contains(".Lt(") || body.Contains(".Le(") || body.Contains(".Gt(") || body.Contains(".Ge(") || body.Contains(".Like(") || body.Contains(".Between(") || body.Contains(".In(") || body.Contains(".IsNull(") || body.Contains(".IsNotNull(") || body.Contains(".IsContains(") || body.Contains(".IsNotContains("))
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
                    if (body.Contains(".Val(") || body.Contains(".Eq(") || body.Contains(".Ne(") || body.Contains(".Lt(") || body.Contains(".Le(") || body.Contains(".Gt(") || body.Contains(".Ge(") || body.Contains(".Like(") || body.Contains(".Between(") || body.Contains(".In(") || body.Contains(".IsNull(") || body.Contains(".IsNotNull(") || body.Contains(".IsContains(") || body.Contains(".IsNotContains("))
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

        private string CheckSymbol(Expression<Func<TEntity, dynamic>> fun)
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
            else if (body.Contains(".IsNull"))
            {
                return "IS NULL";
            }
            else if (body.Contains(".IsNotNull"))
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

        //获取条件参数
        private string GetOperation(Expression<Func<TEntity, dynamic>> fun)
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
                    else if (body.Contains(".IsContains"))
                    {
                        value = $"{name}.STContains(GEOMETRY::STGeomFromText('{final[0]}',{GeoSrid}))=1 ";
                    }
                    else if (body.Contains(".IsNotContains"))
                    {
                        value = $"{name}.STContains(GEOMETRY::STGeomFromText('{final[0]}',{GeoSrid}))<>1 ";
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

        private string[] GetDbKeys()
        {
            string[] res = new string[] { };

            if (DbKeys != null)
                res = DbKeys;

            if (DbKeysPlus != null)
                res = res.Union(DbKeysPlus).ToArray();

            if (DbKeysMinus != null)
                res = res.Except(DbKeysMinus).ToArray();

            return res;
        }

        public IDbEntity<TEntity> Empty()
        {
            DbKeys = null;
            return this;
        }

        public IDbEntity<TEntity> Plus(params Expression<Func<TEntity, dynamic>>[] funs)
        {
            List<string> keys = new List<string>();
            //获取键名集合
            if (DbKeysMinus != null)
                keys = DbKeysPlus.ToList();

            foreach (Expression<Func<TEntity, dynamic>> fun in funs)
            {
                string name = fun.Body.ToString().Split('.').Last();
                name = name.Replace("(", null).Replace(")", null).Replace(" ", null).Replace(",Object", null);

                keys.Add(name);
            }
            DbKeysPlus = keys.ToArray();

            return this;
        }

        public IDbEntity<TEntity> Minus(params Expression<Func<TEntity, dynamic>>[] funs)
        {
            List<string> keys = new List<string>();
            //获取键名集合
            if (DbKeysMinus != null)
                keys = DbKeysMinus.ToList();

            foreach (Expression<Func<TEntity, dynamic>> fun in funs)
            {
                string name = fun.Body.ToString().Split('.').Last();
                name = name.Replace("(", null).Replace(")", null).Replace(" ", null).Replace(",Object", null);

                keys.Add(name);
            }
            DbKeysMinus = keys.ToArray();

            return this;
        }

        public IDbEntity<TEntity> Count(string field = null, bool isDistinct = false)
        {
            if (field != null)
            {
                if (isDistinct)
                    DbSql += $"SELECT COUNT(DISTINCT [{field}]) FROM {DbTable}";
                else
                    DbSql = $"SELECT COUNT([{field}]) FROM {DbTable}";
            }
            else
                DbSql = $"SELECT COUNT(*) FROM {DbTable}";

            return this;
        }

        private List<string> GetGeoKeys()
        {
            List<string> properties = new List<string>();
            Type type = typeof(TEntity);
            foreach (PropertyInfo pi in type.GetProperties())
            {
                List<Attribute> attrs = pi.GetCustomAttributes().ToList();

                bool check = false;
                if (attrs != null && attrs.Count > 0)
                {
                    foreach (Attribute attr in attrs)
                    {
                        if (attr.TypeId.ToString() == "Geone.Utiliy.Database.GeoAttribute")
                        {
                            check = true;
                        }
                    }
                }

                if (check)
                    properties.Add(pi.Name);
            }
            return properties;
        }

        public IDbEntity<TEntity> Select()
        {
            string[] e = GetDbKeys();
            if (e.Count() < 1) return this;

            string sql = "SELECT ";

            int length = e.Count();

            for (int i = 0; i < length; i++)
            {
                string value = string.Empty;

                if (GetGeoKeys().Contains(e[i]))
                    value = $"([{e[i]}]).AsTextZM() as [{e[i]}]";
                else
                    value = $"[{e[i]}]";

                if (i == length - 1)
                {
                    sql += $"{value} ";
                }
                else
                {
                    sql += $"{value},";
                }
            }

            sql += $" FROM {DbTable} ";

            DbSql += sql;
            return this;
        }

        public IDbEntity<TEntity> Insert(TEntity value)
        {
            string[] e = GetDbKeys();

            if (e.Count() < 1) return this;

            string keysql = string.Empty;
            string valuesql = string.Empty;
            Type type = typeof(TEntity);

            int length = e.Count();

            for (int i = 0; i < length; i++)
            {
                foreach (PropertyInfo pi in type.GetProperties())
                {
                    var va = pi.GetValue(value);
                    if (pi.Name == e[i])
                    {
                        if (va != null)
                        {
                            keysql += $"[{e[i]}]";
                            valuesql += $"'{va.ToString().Replace("'", "''")}'";
                            break;
                        }
                        else
                        {
                            keysql += $"[{e[i]}]";
                            valuesql += $"NULL";
                            break;
                        }
                    }
                }

                if (i != length - 1)
                {
                    keysql += ",";
                    valuesql += ",";
                }
            }

            string sql = $"INSERT INTO {DbTable} ({keysql}) VALUES ({valuesql}) ";

            DbSql += sql;

            if (DbParam == null)
                DbParam = value;
            return this;
        }

        public IDbEntity<TEntity> Insert(List<TEntity> values)
        {
            if (values.Count < 1) return this;

            foreach (TEntity value in values)
            {
                Insert(value).End();
            }

            return this;
        }

        public IDbEntity<TEntity> Update(TEntity value)
        {
            string[] e = GetDbKeys();

            if (e.Count() < 1) return this;

            string keyvaluesql = string.Empty;
            string pkeysql = string.Empty;
            Type type = typeof(TEntity);

            int length = e.Count();

            for (int i = 0; i < length; i++)
            {
                foreach (PropertyInfo pi in type.GetProperties())
                {
                    var va = pi.GetValue(value);
                    if (pi.Name == e[i])
                    {
                        if (va != null)
                        {
                            keyvaluesql += $"[{e[i]}] = '{va.ToString().Replace("'", "''")}'";
                            break;
                        }
                        else
                        {
                            keyvaluesql += $"[{e[i]}] = NULL";
                            break;
                        }
                    }
                }

                if (i != length - 1)
                {
                    keyvaluesql += $",";
                }
            }

            string sql = $"UPDATE {DbTable} SET {keyvaluesql} ";

            DbSql += sql;
            if (DbParam == null)
                DbParam = value;
            return this;
        }

        public IDbEntity<TEntity> Update(List<TEntity> values)
        {
            if (values.Count < 1) return this;

            foreach (TEntity value in values)
            {
                Update(value).End();
            }

            return this;
        }

        public IDbEntity<TEntity> UpdateByPKey(TEntity value)
        {
            string[] e = GetDbKeys();

            if (e.Count() < 1) return this;

            string keyvaluesql = string.Empty;
            string pkeysql = string.Empty;
            Type type = typeof(TEntity);

            int length = e.Count();

            for (int i = 0; i < length; i++)
            {
                foreach (PropertyInfo pi in type.GetProperties())
                {
                    var va = pi.GetValue(value);
                    if (pi.Name == e[i])
                    {
                        if (va != null)
                        {
                            keyvaluesql += $"[{e[i]}] = '{va.ToString().Replace("'", "''")}'";

                            break;
                        }
                        else
                        {
                            keyvaluesql += $"[{e[i]}] = NULL";
                            break;
                        }
                    }
                }

                if (i < length - 1)
                {
                    keyvaluesql += $",";
                }
            }

            foreach (PropertyInfo pi in type.GetProperties())
            {
                if (pi.Name == DbPrimaryKey)
                {
                    pkeysql = pi.GetValue(value).ToString();
                }
            }

            string sql = $"UPDATE {DbTable} SET {keyvaluesql} WHERE [{DbPrimaryKey}] = '{pkeysql.ToString().Replace("'", "''")}' ";

            DbSql += sql;
            if (DbParam == null)
                DbParam = value;
            return this;
        }

        public IDbEntity<TEntity> UpdateByPKey(List<TEntity> values)
        {
            if (values.Count < 1) return this;

            foreach (TEntity value in values)
            {
                UpdateByPKey(value).End();
            }

            return this;
        }

        public IDbEntity<TEntity> Delete()
        {
            string sql = $"DELETE FROM {DbTable} ";

            DbSql += sql;
            return this;
        }

        public IDbEntity<TEntity> DeleteByPKey(string pkey)
        {
            string sql = $"DELETE FROM {DbTable} WHERE {DbPrimaryKey} = '{pkey}' ";

            DbSql += sql;
            return this;
        }

        public IDbEntity<TEntity> DeleteByPKey(List<string> pkeys)
        {
            if (pkeys.Count < 1) return this;

            foreach (string pkey in pkeys)
            {
                DeleteByPKey(pkey).End();
            }

            return this;
        }

        public IDbEntity<TEntity> Where()
        {
            DbSql += "WHERE ";

            return this;
        }

        public IDbEntity<TEntity> And()
        {
            DbSql += $"AND ";

            return this;
        }

        public IDbEntity<TEntity> Or()
        {
            DbSql += $"OR ";

            return this;
        }

        public IDbEntity<TEntity> LBracket()
        {
            DbSql += $"( ";

            return this;
        }

        public IDbEntity<TEntity> RBracket()
        {
            DbSql += $") ";

            return this;
        }

        public IDbEntity<TEntity> Where(Expression<Func<TEntity, dynamic>> fun)
        {
            if (fun == null)
                DbSql += "WHERE ";
            else
                DbSql += $"WHERE {GetOperation(fun)}";

            return this;
        }

        public IDbEntity<TEntity> WhereAnd(params Expression<Func<TEntity, dynamic>>[] funs)
        {
            if (funs.Count() == 0)
            {
                DbSql += "WHERE ";
            }
            else if (funs.Count() == 1)
            {
                Expression<Func<TEntity, dynamic>> fun = funs[0];
                DbSql += $"WHERE {GetOperation(fun)}";
            }
            else
            {
                DbSql += $"WHERE ";
                And(funs);
            }

            return this;
        }

        public IDbEntity<TEntity> WhereOr(params Expression<Func<TEntity, dynamic>>[] funs)
        {
            if (funs.Count() == 0)
            {
                DbSql += "WHERE ";
            }
            else if (funs.Count() == 1)
            {
                Expression<Func<TEntity, dynamic>> fun = funs[0];
                DbSql += $"WHERE {GetOperation(fun)}";
            }
            else
            {
                DbSql += $"WHERE ";
                Or(funs);
            }

            return this;
        }

        public IDbEntity<TEntity> And(params Expression<Func<TEntity, dynamic>>[] funs)
        {
            if (funs.Count() == 0)
            {
                DbSql += "AND ";
            }
            else if (funs.Count() == 1)
            {
                Expression<Func<TEntity, dynamic>> fun = funs[0];
                DbSql += $"AND {GetOperation(fun)}";
            }
            else
            {
                List<string> e = new List<string>();

                foreach (Expression<Func<TEntity, dynamic>> fun in funs)
                {
                    e.Add($"{GetOperation(fun)}");
                }

                if (e.Count() != 0)
                    DbSql += $"({string.Join(" AND ", e)}) ";
            }

            return this;
        }

        public IDbEntity<TEntity> Or(params Expression<Func<TEntity, dynamic>>[] funs)
        {
            if (funs.Count() == 0)
            {
                DbSql += "OR ";
            }
            else if (funs.Count() == 1)
            {
                Expression<Func<TEntity, dynamic>> fun = funs[0];
                DbSql += $"OR {GetOperation(fun)}";
            }
            else
            {
                List<string> e = new List<string>();

                foreach (Expression<Func<TEntity, dynamic>> fun in funs)
                {
                    e.Add($"{GetOperation(fun)}");
                }

                if (e.Count() != 0)
                    DbSql += $"({string.Join(" OR ", e)}) ";
            }

            return this;
        }

        public IDbEntity<TEntity> Where(string param)
        {
            if (param == null)
                DbSql += "WHERE ";
            else
                DbSql += $"WHERE {param}";

            return this;
        }

        public IDbEntity<TEntity> WhereAnd(params string[] param)
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

        public IDbEntity<TEntity> WhereOr(params string[] param)
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

        public IDbEntity<TEntity> And(params string[] param)
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

        public IDbEntity<TEntity> Or(params string[] param)
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

        public IDbEntity<TEntity> Union()
        {
            DbSql += "UNION ";
            return this;
        }

        public IDbEntity<TEntity> GroupBy(Expression<Func<TEntity, dynamic>> fun)
        {
            GetParam(out string name, out string[] final, fun);
            if (name != null)
                DbSql += $"GROUP BY {name} ";

            return this;
        }

        public IDbEntity<TEntity> OrderBy(Expression<Func<TEntity, dynamic>> fun)
        {
            GetParam(out string name, out string[] final, fun);
            if (name != null)
                DbSql += $"ORDER BY {name} ";

            return this;
        }

        public IDbEntity<TEntity> OrderByDesc(Expression<Func<TEntity, dynamic>> fun)
        {
            GetParam(out string name, out string[] final, fun);
            if (name != null)
                DbSql += $"ORDER BY {name} DESC";

            return this;
        }

        public IDbEntity<TEntity> GroupBy(string param)
        {
            DbSql += $"GROUP BY {param} ";
            return this;
        }

        public IDbEntity<TEntity> OrderBy(string param)
        {
            DbSql += $"ORDER BY {param} ";
            return this;
        }

        public IDbEntity<TEntity> OrderByDesc(string param)
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

        public TEntity QueryFirst()
        {
            string sql = DbSql;
            Clear();
            return _action.QueryFirst<TEntity>(sql);
        }

        public List<TEntity> QueryList()
        {
            string sql = DbSql;
            Clear();
            return _action.QueryList<TEntity>(sql);
        }

        public Page<TEntity> QueryPage(int pi, int ps, params string[] orders)
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

    //public class SrvDbEntity<TEntity> : SrvDbAction, IDbEntity<TEntity> where TEntity : IEntity
    //{
    //    private IDbConnect conn;
    //    private ILogWriter log;

    //    public SrvDbEntity(IDbConnect Conn, ILogWriter Log)
    //        : base(Conn, Log)
    //    {
    //        conn = Conn;
    //        log = Log;

    //        DbName = "Default";
    //        DbTable = string.Empty;
    //        DbSql = string.Empty;
    //        DbSqls = new string[] { };
    //    }
    //}
}