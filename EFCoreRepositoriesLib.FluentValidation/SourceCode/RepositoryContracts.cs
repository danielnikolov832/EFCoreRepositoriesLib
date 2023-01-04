using FluentValidation;

namespace EFCoreRepositoriesLib.FluentValidation;

public interface ICrudRepositoryWithPKAndValidationBase<TPrimaryKeyUser> : ICrudRepositoryWithPKBase<TPrimaryKeyUser>
    where TPrimaryKeyUser : ReadOnlyPrimaryKeyUser
{
    void Insert(TPrimaryKeyUser model, IValidator<TPrimaryKeyUser> currentValidator);
    void Update(TPrimaryKeyUser model, IValidator<TPrimaryKeyUser> currentValidator);
}

public interface ICrudRepositoryWithPKAndMapperAndValidationBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO> : ICrudRepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO>
    where TPrimaryKeyUserModel : PrivatePrimaryKeyUser
    where TPrimaryKeyUserDAO : PublicPrimaryKeyUser
{
    void Insert(TPrimaryKeyUserModel model, IValidator<TPrimaryKeyUserModel>? currentModelValidator = null, IValidator<TPrimaryKeyUserDAO>? currentDaoValidator = null);
    void Update(TPrimaryKeyUserModel model, IValidator<TPrimaryKeyUserModel>? currentModelValidator = null, IValidator<TPrimaryKeyUserDAO>? currentDaoValidator = null);
}