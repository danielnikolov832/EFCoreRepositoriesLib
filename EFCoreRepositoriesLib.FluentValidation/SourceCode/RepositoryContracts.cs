using FluentValidation;
using FluentValidation.Results;

namespace EFCoreRepositoriesLib.FluentValidation;

public interface ICrudRepositoryWithPKAndValidationBase<TPrimaryKeyUser> : ICrudRepositoryWithPKBase<TPrimaryKeyUser>
    where TPrimaryKeyUser : ReadOnlyPrimaryKeyUser
{
    Action<ValidationResult, IValidator<TPrimaryKeyUser>, TPrimaryKeyUser>? getset_handleValidationOnModelFail { get; set; }

    void Insert(TPrimaryKeyUser model, IValidator<TPrimaryKeyUser> currentValidator);
    void Update(TPrimaryKeyUser model, IValidator<TPrimaryKeyUser> currentValidator);
}

public interface ICrudRepositoryWithPKAndValidationBase<TPrimaryKeyUser, TInsert, TUpdate>
    : ICrudRepositoryWithPKBase<TPrimaryKeyUser, TInsert, TUpdate>
    where TPrimaryKeyUser : ReadOnlyPrimaryKeyUser
{
    Action<ValidationResult, IValidator<TPrimaryKeyUser>, TPrimaryKeyUser>? getset_handleValidationOnModelFail { get; set; }
    Action<ValidationResult, IValidator<TInsert>, TInsert>? getset_handleValidationOnInsertFail { get; set; }
    Action<ValidationResult, IValidator<TUpdate>, TUpdate>? getset_handleValidationOnUpdateFail { get; set; }

    TPrimaryKeyUser Insert(TInsert insert, IValidator<TInsert>? currentInsertValidator = null, IValidator<TPrimaryKeyUser>? currentModelValidator = null);
    TPrimaryKeyUser? TryInsert(TInsert insert, IValidator<TInsert>? currentInsertValidator = null, IValidator<TPrimaryKeyUser>? currentModelValidator = null);
    void Update(TUpdate update, IValidator<TUpdate>? currentUpdateValidator = null, IValidator<TPrimaryKeyUser>? currentModelValidator = null);
}

public interface ICrudRepositoryWithPKAndMapperAndValidationBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO> : ICrudRepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO>
    where TPrimaryKeyUserModel : PrivatePrimaryKeyUser
    where TPrimaryKeyUserDAO : PublicPrimaryKeyUser
{
    Action<ValidationResult, IValidator<TPrimaryKeyUserModel>, TPrimaryKeyUserModel>? getset_handleValidationOnModelFail { get; set; }
    Action<ValidationResult, IValidator<TPrimaryKeyUserDAO>, TPrimaryKeyUserDAO>? getset_handleValidationOnDaoFail { get; set; }

    void Insert(TPrimaryKeyUserModel model, IValidator<TPrimaryKeyUserModel>? currentModelValidator = null, IValidator<TPrimaryKeyUserDAO>? currentDaoValidator = null);
    void Update(TPrimaryKeyUserModel model, IValidator<TPrimaryKeyUserModel>? currentModelValidator = null, IValidator<TPrimaryKeyUserDAO>? currentDaoValidator = null);
}

public interface ICrudRepositoryWithPKAndMapperAndValidationBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO, TInsert, TUpdate>
    : ICrudRepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO, TInsert, TUpdate>
    where TPrimaryKeyUserModel : PrivatePrimaryKeyUser
    where TPrimaryKeyUserDAO : PublicPrimaryKeyUser
{
    Action<ValidationResult, IValidator<TInsert>, TInsert>? getset_handleValidationOnInsertFail { get; set; }
    Action<ValidationResult, IValidator<TUpdate>, TUpdate>? getset_handleValidationOnUpdateFail { get; set; }
    Action<ValidationResult, IValidator<TPrimaryKeyUserModel>, TPrimaryKeyUserModel>? getset_handleValidationOnModelFail { get; set; }
    Action<ValidationResult, IValidator<TPrimaryKeyUserDAO>, TPrimaryKeyUserDAO>? getset_handleValidationOnDaoFail { get; set; }

    TPrimaryKeyUserModel Insert(TInsert insert, IValidator<TInsert>? currentInsertValidator = null, IValidator<TPrimaryKeyUserModel>? currentModelValidator = null, IValidator<TPrimaryKeyUserDAO>? currentDaoValidator = null);
    TPrimaryKeyUserModel? TryInsert(TInsert insert, IValidator<TInsert>? currentInsertValidator = null, IValidator<TPrimaryKeyUserModel>? currentModelValidator = null, IValidator<TPrimaryKeyUserDAO>? currentDaoValidator = null);
    void Update(TUpdate update, IValidator<TUpdate>? currentUpdateValidator = null, IValidator<TPrimaryKeyUserModel>? currentModelValidator = null, IValidator<TPrimaryKeyUserDAO>? currentDaoValidator = null);
}