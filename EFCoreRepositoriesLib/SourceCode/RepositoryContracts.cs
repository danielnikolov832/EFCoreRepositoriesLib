using FluentQuery.Core;
using FluentQuery.SQLSupport;

namespace EFCoreRepositoriesLib;

public interface IRepositoryWithTransformations<TPrimaryKeyUser>
        where TPrimaryKeyUser : class, IReadOnlyPrimaryKeyUser
{
    IQueryable<TPrimaryKeyUser> ApplyTransformations(IQueryable<TPrimaryKeyUser> entities);
}

public interface IRepositoryWithPKCommon<TPrimaryKeyUser>
    where TPrimaryKeyUser : IPrivatePrimaryKeyUser
{
    List<TPrimaryKeyUser> GetAll();
    List<TPrimaryKeyUser> GetAll(IQuery<TPrimaryKeyUser> query);
    TPrimaryKeyUser? GetById(int id);
}

public interface ICrudRepositoryWithPKCommon<TPrimaryKeyUser> : IRepositoryWithPKCommon<TPrimaryKeyUser>
    where TPrimaryKeyUser : IPrivatePrimaryKeyUser
{
    void Insert(TPrimaryKeyUser model);
    bool Remove(int id);
    void Remove(TPrimaryKeyUser model);
    void Update(TPrimaryKeyUser model);
}

//Expl : Requests should not just by default support inheritance
public interface ICrudRepositoryWithPKCommon<TPrimaryKeyUser, TInsert, TUpdate> : IRepositoryWithPKCommon<TPrimaryKeyUser>
    where TPrimaryKeyUser : IPrivatePrimaryKeyUser
{
    TPrimaryKeyUser Insert(TInsert insert);
    bool Remove(int id);
    void Remove(TPrimaryKeyUser model);
    void Update(TUpdate update);
}

public interface IRepositoryWithPKBase<TPrimaryKeyUser> : IRepositoryWithPKCommon<TPrimaryKeyUser>
        where TPrimaryKeyUser : IReadOnlyPrimaryKeyUser
{
    List<TPrimaryKeyUser> GetAll(QueryForSQLBase<TPrimaryKeyUser> query);
}

public interface ICrudRepositoryWithPKBase<TPrimaryKeyUser> : IRepositoryWithPKBase<TPrimaryKeyUser>, ICrudRepositoryWithPKCommon<TPrimaryKeyUser>
    where TPrimaryKeyUser : IReadOnlyPrimaryKeyUser
{
}

public interface ICrudRepositoryWithPKBase<TPrimaryKeyUser, TInsert, TUpdate> : ICrudRepositoryWithPKCommon<TPrimaryKeyUser, TInsert, TUpdate>
    where TPrimaryKeyUser : IReadOnlyPrimaryKeyUser
{
}

public interface IRepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO> : IRepositoryWithPKCommon<TPrimaryKeyUserModel>
    where TPrimaryKeyUserModel : IPrivatePrimaryKeyUser
    where TPrimaryKeyUserDAO : IPublicPrimaryKeyUser
{
    List<TPrimaryKeyUserModel> GetAll(QueryForSQLBase<TPrimaryKeyUserDAO> query);
    List<TPrimaryKeyUserModel> GetAll(IQuery<TPrimaryKeyUserDAO> query);
}

public interface ICrudRepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO> : IRepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO>,
    ICrudRepositoryWithPKCommon<TPrimaryKeyUserModel>
    where TPrimaryKeyUserModel : IPrivatePrimaryKeyUser
    where TPrimaryKeyUserDAO : IPublicPrimaryKeyUser
{
}

public interface ICrudRepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO, TInsert, TUpdate> : IRepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO>,
    ICrudRepositoryWithPKCommon<TPrimaryKeyUserModel, TInsert, TUpdate>
    where TPrimaryKeyUserModel : IPrivatePrimaryKeyUser
    where TPrimaryKeyUserDAO : IPublicPrimaryKeyUser
{
}

public interface ICrudRepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TInsert, TUpdate> : ICrudRepositoryWithPKCommon<TPrimaryKeyUserModel, TInsert, TUpdate>
    where TPrimaryKeyUserModel : IPrivatePrimaryKeyUser
{
}