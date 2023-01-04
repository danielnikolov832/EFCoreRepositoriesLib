using FluentQuery.Core;
using FluentQuery.SQLSupport;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace EFCoreRepositoriesLib;

public class RepositoryWithPKBase<TPrimaryKeyUser> : IRepositoryWithPKBase<TPrimaryKeyUser>, IRepositoryWithTransformations<TPrimaryKeyUser>
    where TPrimaryKeyUser : ReadOnlyPrimaryKeyUser
{
    public RepositoryWithPKBase(DbContext dbContext)
    {
        _table = dbContext.Set<TPrimaryKeyUser>();
        _dbContext = dbContext;
    }

    protected readonly DbSet<TPrimaryKeyUser> _table;
    protected readonly DbContext _dbContext;

    public virtual List<TPrimaryKeyUser> GetAll()
    {
        IQueryable<TPrimaryKeyUser> entities = ApplyTransformations(_table);

        return entities.ToList();
    }

    public virtual List<TPrimaryKeyUser> GetAll(QueryForSQLBase<TPrimaryKeyUser> query)
    {
        Expression<Func<TPrimaryKeyUser, bool>> predicate = QueryToExpression.QueryToExp(query);

        IEnumerable<TPrimaryKeyUser> filteredTable = _table.AsExpandableEFCore().ApplyTransformations(this).Where(predicate.Compile());

        return filteredTable.ToList();
    }

    public virtual List<TPrimaryKeyUser> GetAll(IQuery<TPrimaryKeyUser> query)
    {
        Expression<Func<TPrimaryKeyUser, bool>> predicate = QueryToExpression.QueryToExp(query);

        IEnumerable<TPrimaryKeyUser> filteredTable = _table.AsExpandableEFCore().ApplyTransformations(this).Where(predicate.Compile());

        return filteredTable.ToList();
    }

    public virtual TPrimaryKeyUser? GetById(int id)
    {
        return _table.ApplyTransformations(this).First(x => x.ID == id);
    }

    public virtual IQueryable<TPrimaryKeyUser> ApplyTransformations(IQueryable<TPrimaryKeyUser> entities) => entities;
}

public class CrudRepositoryWithPKBase<TPrimaryKeyUserModel> : RepositoryWithPKBase<TPrimaryKeyUserModel>, ICrudRepositoryWithPKBase<TPrimaryKeyUserModel>
    where TPrimaryKeyUserModel : ReadOnlyPrimaryKeyUser
{
    public CrudRepositoryWithPKBase(DbContext dbContext) : base(dbContext)
    {
    }

    public virtual void Insert(TPrimaryKeyUserModel model)
    {
        _table.Add(model);

        _dbContext.SaveChanges();
    }

    public virtual void Update(TPrimaryKeyUserModel model)
    {
        _table.Update(model);

        _dbContext.SaveChanges();
    }

    public virtual void Remove(TPrimaryKeyUserModel model)
    {
        _table.Remove(model);

        _dbContext.SaveChanges();
    }

    public virtual bool Remove(int id)
    {
        TPrimaryKeyUserModel? model = _table.Find(id);

        if (model is null)
        {
            return false;
        }

        _table.Remove(model);

        _dbContext.SaveChanges();

        return true;
    }
}

public abstract class CrudRepositoryWithPKBase<TPrimaryKeyUser, TInsert, TUpdate> : RepositoryWithPKBase<TPrimaryKeyUser>,
    ICrudRepositoryWithPKBase<TPrimaryKeyUser, TInsert, TUpdate>
    where TPrimaryKeyUser : ReadOnlyPrimaryKeyUser
{
    protected CrudRepositoryWithPKBase(DbContext dbContext) : base(dbContext)
    {
    }

    public abstract TPrimaryKeyUser Insert(TInsert insert);

    public abstract void Update(TUpdate update);

    public virtual void Remove(TPrimaryKeyUser model)
    {
        _table.Remove(model);

        _dbContext.SaveChanges();
    }

    public virtual bool Remove(int id)
    {
        TPrimaryKeyUser? model = _table.Find(id);

        if (model is null)
        {
            return false;
        }

        _table.Remove(model);

        _dbContext.SaveChanges();

        return true;
    }
}