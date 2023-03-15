using FluentQuery.Core;
using FluentQuery.SQLSupport;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace EFCoreRepositoriesLib;

public abstract class RepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO> : IRepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO>,
    IRepositoryWithTransformations<TPrimaryKeyUserDAO>
    where TPrimaryKeyUserModel : IPrivatePrimaryKeyUser
    where TPrimaryKeyUserDAO :  class, IPublicPrimaryKeyUser
{
    protected RepositoryWithPKAndMapperBase(DbContext dbContext, IMapper mapper)
    {
        _table = dbContext.Set<TPrimaryKeyUserDAO>();
        _dbContext = dbContext;
        _mapper = mapper;
    }

    protected internal readonly DbSet<TPrimaryKeyUserDAO> _table;
    protected internal readonly DbContext _dbContext;
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

        if (entity is null) return default;

        return Adapt(entity);
    }

    public virtual IQueryable<TPrimaryKeyUserDAO> ApplyTransformations(IQueryable<TPrimaryKeyUserDAO> entities) => entities;
}

public class CrudRepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO> : RepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO>,
    ICrudRepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO>
        where TPrimaryKeyUserModel : IPrivatePrimaryKeyUser
        where TPrimaryKeyUserDAO : class, IPublicPrimaryKeyUser
{
    public CrudRepositoryWithPKAndMapperBase(DbContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {
    }

    public virtual void Insert(TPrimaryKeyUserModel model)
    {
        TPrimaryKeyUserDAO entity = Adapt(model);

        this.AddToTableAndSaveChanges(entity);
    }

    public virtual void Update(TPrimaryKeyUserModel model)
    {
        TPrimaryKeyUserDAO entity = Adapt(model);

        this.UpdateTableAndSaveChanges(entity);
    }

    public virtual void Remove(TPrimaryKeyUserModel model)
    {
        TPrimaryKeyUserDAO entity = Adapt(model);

        this.RemoveFromTableAndSaveChanges(entity);
    }

    public virtual bool Remove(int id)
    {
        TPrimaryKeyUserDAO? model = _table.Find(id);

        if (model is null)
        {
            return false;
        }

        this.RemoveFromTableAndSaveChanges(model);

        return true;
    }
}

public abstract class CrudRepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TInsert, TUpdate> : RepositoryWithPKBase<TPrimaryKeyUserModel>,
    ICrudRepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TInsert, TUpdate>
        where TPrimaryKeyUserModel : class, IReadOnlyPrimaryKeyUser
{
    protected CrudRepositoryWithPKAndMapperBase(DbContext dbContext, IMapper mapper) : base(dbContext)
    {
        _mapper = mapper;
    }

    protected readonly IMapper _mapper;

    public abstract TPrimaryKeyUserModel Insert(TInsert insert);

    public abstract void Update(TUpdate update);

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

public abstract class CrudRepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO, TInsert, TUpdate> : RepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO>,
    ICrudRepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO, TInsert, TUpdate>,
    ICrudRepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TInsert, TUpdate>
        where TPrimaryKeyUserModel : IPrivatePrimaryKeyUser
        where TPrimaryKeyUserDAO : class, IPublicPrimaryKeyUser
{
    protected CrudRepositoryWithPKAndMapperBase(DbContext dbContext, IMapper mapper) : base(dbContext, mapper)
    {
    }

    public abstract TPrimaryKeyUserModel Insert(TInsert insert);

    public abstract void Update(TUpdate update);

    public virtual void Remove(TPrimaryKeyUserModel model)
    {
        TPrimaryKeyUserDAO entity = Adapt(model);

        this.RemoveFromTableAndSaveChanges(entity);
    }

    public virtual bool Remove(int id)
    {
        TPrimaryKeyUserDAO? model = _table.Find(id);

        if (model is null)
        {
            return false;
        }

        this.RemoveFromTableAndSaveChanges(model);

        return true;
    }
}