using FluentQuery.Core;
using FluentQuery.SQLSupport;

namespace EFCoreRepositoriesLib;

public interface IRepositoryWithTransformations<TPrimaryKeyUser>
        where TPrimaryKeyUser : ReadOnlyPrimaryKeyUser
{
    IQueryable<TPrimaryKeyUser> ApplyTransformations(IQueryable<TPrimaryKeyUser> entities);
}

public interface IRepositoryWithPKCommon<TPrimaryKeyUser>
    where TPrimaryKeyUser : PrivatePrimaryKeyUser
{
    List<TPrimaryKeyUser> GetAll();
    List<TPrimaryKeyUser> GetAll(IQuery<TPrimaryKeyUser> query);
    TPrimaryKeyUser? GetById(int id);
}

public interface ICrudRepositoryWithPKCommon<TPrimaryKeyUser> : IRepositoryWithPKCommon<TPrimaryKeyUser>
    where TPrimaryKeyUser : PrivatePrimaryKeyUser
{
    void Insert(TPrimaryKeyUser model);
    bool Remove(int id);
    void Remove(TPrimaryKeyUser model);
    void Update(TPrimaryKeyUser model);
}

//Expl : Requests should not just by default support inheritance
public interface ICrudRepositoryWithPKCommon<TPrimaryKeyUser, TInsert, TUpdate> : IRepositoryWithPKCommon<TPrimaryKeyUser>
    where TPrimaryKeyUser : PrivatePrimaryKeyUser
{
    TPrimaryKeyUser Insert(TInsert insert);
    bool Remove(int id);
    void Remove(TPrimaryKeyUser model);
    void Update(TUpdate update);
}

public interface IRepositoryWithPKBase<TPrimaryKeyUser> : IRepositoryWithPKCommon<TPrimaryKeyUser>
        where TPrimaryKeyUser : ReadOnlyPrimaryKeyUser
{
    List<TPrimaryKeyUser> GetAll(QueryForSQLBase<TPrimaryKeyUser> query);
}

public interface ICrudRepositoryWithPKBase<TPrimaryKeyUser> : IRepositoryWithPKBase<TPrimaryKeyUser>, ICrudRepositoryWithPKCommon<TPrimaryKeyUser>
    where TPrimaryKeyUser : ReadOnlyPrimaryKeyUser
{
}

public interface ICrudRepositoryWithPKBase<TPrimaryKeyUser, TInsert, TUpdate> : ICrudRepositoryWithPKCommon<TPrimaryKeyUser, TInsert, TUpdate>
    where TPrimaryKeyUser : ReadOnlyPrimaryKeyUser
{
}

public interface IRepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO> : IRepositoryWithPKCommon<TPrimaryKeyUserModel>
    where TPrimaryKeyUserModel : PrivatePrimaryKeyUser
    where TPrimaryKeyUserDAO : PublicPrimaryKeyUser
{
    List<TPrimaryKeyUserModel> GetAll(QueryForSQLBase<TPrimaryKeyUserDAO> query);
    List<TPrimaryKeyUserModel> GetAll(IQuery<TPrimaryKeyUserDAO> query);
}

public interface ICrudRepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO> : IRepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO>,
    ICrudRepositoryWithPKCommon<TPrimaryKeyUserModel>
    where TPrimaryKeyUserModel : PrivatePrimaryKeyUser
    where TPrimaryKeyUserDAO : PublicPrimaryKeyUser
{
}

public interface ICrudRepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO, TInsert, TUpdate> : IRepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO>,
    ICrudRepositoryWithPKCommon<TPrimaryKeyUserModel, TInsert, TUpdate>
    where TPrimaryKeyUserModel : PrivatePrimaryKeyUser
    where TPrimaryKeyUserDAO : PublicPrimaryKeyUser
{
}

public interface ICrudRepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TInsert, TUpdate> : ICrudRepositoryWithPKCommon<TPrimaryKeyUserModel, TInsert, TUpdate>
    where TPrimaryKeyUserModel : PrivatePrimaryKeyUser
{
}