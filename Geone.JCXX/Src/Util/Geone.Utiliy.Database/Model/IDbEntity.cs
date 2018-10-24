using Geone.Utiliy.Library;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Geone.Utiliy.Database
{
    public interface IDbEntity<TEntity> where TEntity : IEntity
    {
        IDbEntity<TEntity> SetName(string name);

        IDbEntity<TEntity> SetTable(string table);

        IDbEntity<TEntity> SetPrimaryKey(string key);

        IDbEntity<TEntity> SetParam(TEntity param);

        IDbEntity<TEntity> End();

        IDbEntity<TEntity> Empty();

        IDbEntity<TEntity> Plus(params Expression<Func<TEntity, dynamic>>[] funs);

        IDbEntity<TEntity> Minus(params Expression<Func<TEntity, dynamic>>[] funs);

        IDbEntity<TEntity> Count(string field = null, bool isDistinct = false);

        IDbEntity<TEntity> Select();

        IDbEntity<TEntity> Insert(TEntity value);

        IDbEntity<TEntity> Insert(List<TEntity> values);

        IDbEntity<TEntity> Update(TEntity value);

        IDbEntity<TEntity> Update(List<TEntity> values);

        IDbEntity<TEntity> UpdateByPKey(TEntity value);

        IDbEntity<TEntity> UpdateByPKey(List<TEntity> values);

        IDbEntity<TEntity> Delete();

        IDbEntity<TEntity> DeleteByPKey(string pkey);

        IDbEntity<TEntity> DeleteByPKey(List<string> pkeys);

        IDbEntity<TEntity> Where();

        IDbEntity<TEntity> And();

        IDbEntity<TEntity> Or();

        IDbEntity<TEntity> LBracket();

        IDbEntity<TEntity> RBracket();

        IDbEntity<TEntity> Where(Expression<Func<TEntity, dynamic>> fun);

        IDbEntity<TEntity> WhereAnd(params Expression<Func<TEntity, dynamic>>[] funs);

        IDbEntity<TEntity> WhereOr(params Expression<Func<TEntity, dynamic>>[] funs);

        IDbEntity<TEntity> And(params Expression<Func<TEntity, dynamic>>[] funs);

        IDbEntity<TEntity> Or(params Expression<Func<TEntity, dynamic>>[] funs);

        IDbEntity<TEntity> Where(string param);

        IDbEntity<TEntity> WhereAnd(params string[] param);

        IDbEntity<TEntity> WhereOr(params string[] param);

        IDbEntity<TEntity> And(params string[] param);

        IDbEntity<TEntity> Or(params string[] param);

        IDbEntity<TEntity> Union();

        IDbEntity<TEntity> GroupBy(Expression<Func<TEntity, dynamic>> fun);

        IDbEntity<TEntity> OrderBy(Expression<Func<TEntity, dynamic>> fun);

        IDbEntity<TEntity> OrderByDesc(Expression<Func<TEntity, dynamic>> fun);

        IDbEntity<TEntity> GroupBy(string param);

        IDbEntity<TEntity> OrderBy(string param);

        IDbEntity<TEntity> OrderByDesc(string param);

        int QueryCount();

        TEntity QueryFirst();

        List<TEntity> QueryList();

        Page<TEntity> QueryPage(int pi, int ps, params string[] orders);

        bool ExecInsert();

        bool ExecInsertBatch();

        bool ExecModify();

        bool ExecModifyBatch();

        bool ExecRemove();

        bool ExecRemoveBatch();

        bool ExecTrans();
    }
}