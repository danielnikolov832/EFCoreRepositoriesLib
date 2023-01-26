using FluentQuery.Core;
using FluentQuery.SQLSupport;
using LinqKit;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EFCoreRepositoriesLib;

public abstract class RepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO> : IRepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO>,
    IRepositoryWithTransformations<TPrimaryKeyUserDAO>
    where TPrimaryKeyUserModel : PrivatePrimaryKeyUser
    where TPrimaryKeyUserDAO : PublicPrimaryKeyUser
{
    protected RepositoryWithPKAndMapperBase(DbContext dbContext, IMapper mapper)
    {
        _table = dbContext.Set<TPrimaryKeyUserDAO>();
        _dbContext = dbContext;
        _mapper = mapper;
    }

    protected readonly DbSet<TPrimaryKeyUserDAO> _table;
    protected readonly DbContext _dbContext;
    protected readonly IMapper _mapper;

    protected TPrimaryKeyUserModel Adapt(TPrimaryKeyUserDAO dao)
    {
        return _mapper.Map<TPrimaryKeyUserDAO, TPrimaryKeyUserModel>(dao);
    }

    protected TPrimaryKeyUserDAO Adapt(TPrimaryKeyUserModel model)
    {
        return _mapper.Map<TPrimaryKeyUserModel, TPrimaryKeyUserDAO>(model);
    }

    public virtual List<TPrimaryKeyUserModel> GetAll()
    {
        List<TPrimaryKeyUserModel> output = new();

        IQueryable<TPrimaryKeyUserDAO> entities = ApplyTransformations(_table);

        foreach (var item in entities.ToList())
        {
            TPrimaryKeyUserModel mappedReturn = Adapt(item);

            output.Add(mappedReturn);
        }

        return output;
    }

    public virtual List<TPrimaryKeyUserModel> GetAll(IQuery<TPrimaryKeyUserModel> query)
    {
        List<TPrimaryKeyUserModel> output = new();

        IQueryable<TPrimaryKeyUserDAO> entities = _table.ApplyTransformations(this);

        output.AddRange(from item in entities.ToList()
                        let mappedReturn = Adapt(item)
                        where query.IsValid(mappedReturn)
                        select mappedReturn);

        return output;
    }

    public virtual List<TPrimaryKeyUserModel> GetAll(QueryForSQLBase<TPrimaryKeyUserDAO> query)
    {
        IEnumerable<TPrimaryKeyUserDAO> filteredTable = _table.Query(query, this);

        IEnumerable<TPrimaryKeyUserModel> output = from TPrimaryKeyUserDAO dao in filteredTable
                                                   select Adapt(dao);

        return output.ToList();
    }

    public virtual List<TPrimaryKeyUserModel> GetAll(IQuery<TPrimaryKeyUserDAO> query)
    {
        IEnumerable<TPrimaryKeyUserDAO> filteredTable = _table.Query(query, this);

        IEnumerable<TPrimaryKeyUserModel> output = from TPrimaryKeyUserDAO dao in filteredTable
                                                   select Adapt(dao);

        return output.ToList();
    }

    public virtual TPrimaryKeyUserModel? GetById(int id)
    {
        TPrimaryKeyUserDAO? entity = _table.ApplyTransformations(this).First(x => x.ID == id);

        if (entity is null) return null;

        return Adapt(entity);
    }

    public virtual IQueryable<TPrimaryKeyUserDAO> ApplyTransformations(IQueryable<TPrimaryKeyUserDAO> entities) => entities;
}

public class CrudRepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO> : RepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO>,
    ICrudRepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO>
        where TPrimaryKeyUserModel : PrivatePrimaryKeyUser
        where TPrimaryKeyUserDAO : PublicPrimaryKeyUser
{
    public CrudRepositoryWithPKAndMapperBase(DbContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {
    }

    public virtual void Insert(TPrimaryKeyUserModel model)
    {
        TPrimaryKeyUserDAO entity = Adapt(model);

        _table.Add(entity);

        _dbContext.SaveChanges();
    }

    public virtual void Update(TPrimaryKeyUserModel model)
    {
        TPrimaryKeyUserDAO entity = Adapt(model);

        _table.Update(entity);

        _dbContext.SaveChanges();
    }

    public virtual void Remove(TPrimaryKeyUserModel model)
    {
        TPrimaryKeyUserDAO entity = Adapt(model);

        _table.Remove(entity);

        _dbContext.SaveChanges();
    }

    public virtual bool Remove(int id)
    {
        TPrimaryKeyUserDAO? model = _table.Find(id);

        if (model is null)
        {
            return false;
        }

        _table.Remove(model);

        _dbContext.SaveChanges();

        return true;
    }
}

public abstract class CrudRepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO, TInsert, TUpdate> : RepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO>,
    ICrudRepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO, TInsert, TUpdate>,
    ICrudRepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TInsert, TUpdate>
        where TPrimaryKeyUserModel : PrivatePrimaryKeyUser
        where TPrimaryKeyUserDAO : PublicPrimaryKeyUser
{
    protected CrudRepositoryWithPKAndMapperBase(DbContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {
    }

    public abstract TPrimaryKeyUserModel Insert(TInsert insert);

    public abstract void Update(TUpdate update);

    public virtual void Remove(TPrimaryKeyUserModel model)
    {
        TPrimaryKeyUserDAO entity = Adapt(model);

        _table.Remove(entity);

        _dbContext.SaveChanges();
    }

    public virtual bool Remove(int id)
    {
        TPrimaryKeyUserDAO? model = _table.Find(id);

        if (model is null)
        {
            return false;
        }

        _table.Remove(model);

        _dbContext.SaveChanges();

        return true;
    }
}