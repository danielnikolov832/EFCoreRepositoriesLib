using FluentValidation;

namespace EFCoreRepositoriesLib.FluentValidation;

public interface ICrudRepositoryWithPKAndValidationBase<TPrimaryKeyUser> : ICrudRepositoryWithPKBase<TPrimaryKeyUser>
    where TPrimaryKeyUser : ReadOnlyPrimaryKeyUser
{
    void Insert(TPrimaryKeyUser model, IValidator<TPrimaryKeyUser> currentValidator);
    void Update(TPrimaryKeyUser model, IValidator<TPrimaryKeyUser> currentValidator);
}

public interface ICrudRepositoryWithPKAndValidationBase<TPrimaryKeyUser, TInsert, TUpdate>
    : ICrudRepositoryWithPKBase<TPrimaryKeyUser, TInsert, TUpdate>
    where TPrimaryKeyUser : ReadOnlyPrimaryKeyUser
{
    TPrimaryKeyUser Insert(TInsert insert, IValidator<TInsert>? currentInsertValidator = null, IValidator<TPrimaryKeyUser>? currentModelValidator = null);
    void Update(TUpdate model, IValidator<TUpdate>? currentUpdateValidator = null, IValidator<TPrimaryKeyUser>? currentModelValidator = null);
}

public interface ICrudRepositoryWithPKAndMapperAndValidationBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO> : ICrudRepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO>
    where TPrimaryKeyUserModel : PrivatePrimaryKeyUser
    where TPrimaryKeyUserDAO : PublicPrimaryKeyUser
{
    void Insert(TPrimaryKeyUserModel model, IValidator<TPrimaryKeyUserModel>? currentModelValidator = null, IValidator<TPrimaryKeyUserDAO>? currentDaoValidator = null);
    void Update(TPrimaryKeyUserModel model, IValidator<TPrimaryKeyUserModel>? currentModelValidator = null, IValidator<TPrimaryKeyUserDAO>? currentDaoValidator = null);
}

public interface ICrudRepositoryWithPKAndMapperAndValidationBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO, TInsert, TUpdate>
    : ICrudRepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO, TInsert, TUpdate>
    where TPrimaryKeyUserModel : PrivatePrimaryKeyUser
    where TPrimaryKeyUserDAO : PublicPrimaryKeyUser
{
    TPrimaryKeyUserModel Insert(TInsert insert, IValidator<TInsert>? currentInsertValidator = null, IValidator<TPrimaryKeyUserModel>? currentModelValidator = null, IValidator<TPrimaryKeyUserDAO>? currentDaoValidator = null);
    void Update(TUpdate model, IValidator<TUpdate>? currentUpdateValidator = null, IValidator<TPrimaryKeyUserModel>? currentModelValidator = null, IValidator<TPrimaryKeyUserDAO>? currentDaoValidator = null);
}