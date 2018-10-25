using Geone.Utiliy.Library;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Geone.Utiliy.Database
{
    public interface IDbLambda<TParam> where TParam : IParam
    {
        IDbLambda<TParam> SetName(string name);

        IDbLambda<TParam> SetTable(string table);

        IDbLambda<TParam> SetParam(TParam param);

        IDbLambda<TParam> SetSrid(int srid);

        IDbLambda<TParam> End();

        IDbLambda<TParam> Count(Expression<Func<TParam, dynamic>> fun = null, bool isDistinct = false);

        IDbLambda<TParam> Select(params Expression<Func<TParam, dynamic>>[] funs);

        IDbLambda<TParam> Select(params string[] @params);

        IDbLambda<TParam> Insert(params Expression<Func<TParam, dynamic>>[] funs);

        IDbLambda<TParam> Insert(JObject @params);

        IDbLambda<TParam> Update(params Expression<Func<TParam, dynamic>>[] funs);

        IDbLambda<TParam> Update(JObject @params);

        IDbLambda<TParam> Delete();

        IDbLambda<TParam> Where();

        IDbLambda<TParam> And();

        IDbLambda<TParam> Or();

        IDbLambda<TParam> LBracket();

        IDbLambda<TParam> RBracket();

        IDbLambda<TParam> Where(Expression<Func<TParam, dynamic>> fun);

        IDbLambda<TParam> WhereAnd(params Expression<Func<TParam, dynamic>>[] funs);

        IDbLambda<TParam> WhereOr(params Expression<Func<TParam, dynamic>>[] funs);

        IDbLambda<TParam> And(params Expression<Func<TParam, dynamic>>[] funs);

        IDbLambda<TParam> Or(params Expression<Func<TParam, dynamic>>[] funs);

        IDbLambda<TParam> Where(string param);

        IDbLambda<TParam> WhereAnd(params string[] param);

        IDbLambda<TParam> WhereOr(params string[] param);

        IDbLambda<TParam> And(params string[] param);

        IDbLambda<TParam> Or(params string[] param);

        IDbLambda<TParam> Union();

        IDbLambda<TParam> GroupBy(Expression<Func<TParam, dynamic>> fun);

        IDbLambda<TParam> OrderBy(Expression<Func<TParam, dynamic>> fun);

        IDbLambda<TParam> OrderByDesc(Expression<Func<TParam, dynamic>> fun);

        IDbLambda<TParam> GroupBy(string param);

        IDbLambda<TParam> OrderBy(string param);

        IDbLambda<TParam> OrderByDesc(string param);

        int QueryCount();

        TParam QueryFirst();

        List<TParam> QueryList();

        Page<TParam> QueryPage(int pi, int ps, params string[] orders);

        TEntity QueryFirst<TEntity>();

        List<TEntity> QueryList<TEntity>();

        Page<TEntity> QueryPage<TEntity>(int pi, int ps, params string[] orders);

        bool ExecInsert();

        bool ExecInsertBatch();

        bool ExecModify();

        bool ExecModifyBatch();

        bool ExecRemove();

        bool ExecRemoveBatch();

        bool ExecTrans();
    }
}