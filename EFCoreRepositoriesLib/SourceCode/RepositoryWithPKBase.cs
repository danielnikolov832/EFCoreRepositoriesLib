using FluentQuery.Core;
using FluentQuery.SQLSupport;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace EFCoreRepositoriesLib;

public class RepositoryWithPKBase<TPrimaryKeyUser> : IRepositoryWithPKBase<TPrimaryKeyUser>, IRepositoryWithTransformations<TPrimaryKeyUser>
    where TPrimaryKeyUser : class, IReadOnlyPrimaryKeyUser
{
    public RepositoryWithPKBase(DbContext dbContext)
    {
        _table = dbContext.Set<TPrimaryKeyUser>();
        _dbContext = dbContext;
    }

    protected internal readonly DbSet<TPrimaryKeyUser> _table;
    protected internal readonly DbContext _dbContext;

    public virtual List<TPrimaryKeyUser> GetAll()
    {
        IQueryable<TPrimaryKeyUser> entities = ApplyTransformations(_table);

        return entities.ToList();
    }

    public virtual List<TPrimaryKeyUser> GetAll(QueryForSQLBase<TPrimaryKeyUser> query)
    {
        return _table.Query(query, this).ToList();
    }

    public virtual List<TPrimaryKeyUser> GetAll(IQuery<TPrimaryKeyUser> query)
    {
        return _table.Query(query, this).ToList();
    }

    public virtual TPrimaryKeyUser? GetById(int id)
    {
        return _table.ApplyTransformations(this).First(x => x.ID == id);
    }

    public virtual IQueryable<TPrimaryKeyUser> ApplyTransformations(IQueryable<TPrimaryKeyUser> entities) => entities;
}

public class CrudRepositoryWithPKBase<TPrimaryKeyUserModel> : RepositoryWithPKBase<TPrimaryKeyUserModel>, ICrudRepositoryWithPKBase<TPrimaryKeyUserModel>
    where TPrimaryKeyUserModel : class, IReadOnlyPrimaryKeyUser
{
    public CrudRepositoryWithPKBase(DbContext dbContext) : base(dbContext)
    {
    }

    public virtual void Insert(TPrimaryKeyUserModel model)
    {
        this.AddToTableAndSaveChanges(model);
    }

    public virtual void Update(TPrimaryKeyUserModel model)
    {
        this.UpdateTableAndSaveChanges(model);
    }

    public virtual void Remove(TPrimaryKeyUserModel model)
    {
        this.RemoveFromTableAndSaveChanges(model);
    }

    public virtual bool Remove(int id)
    {
        TPrimaryKeyUserModel? model = _table.Find(id);

        if (model is null)
        {
            return false;
        }

        this.RemoveFromTableAndSaveChanges(model);

        return true;
    }
}

public abstract class CrudRepositoryWithPKBase<TPrimaryKeyUser, TInsert, TUpdate> : RepositoryWithPKBase<TPrimaryKeyUser>,
    ICrudRepositoryWithPKBase<TPrimaryKeyUser, TInsert, TUpdate>
    where TPrimaryKeyUser : class, IReadOnlyPrimaryKeyUser
{
    protected CrudRepositoryWithPKBase(DbContext dbContext) : base(dbContext)
    {
    }

    public abstract TPrimaryKeyUser Insert(TInsert insert);

    public abstract void Update(TUpdate update);

    public virtual void Remove(TPrimaryKeyUser model)
    {
        this.RemoveFromTableAndSaveChanges(model);
    }

    public virtual bool Remove(int id)
    {
        TPrimaryKeyUser? model = _table.Find(id);

        if (model is null)
        {
            return false;
        }

        Remove(model);

        return true;
    }
}