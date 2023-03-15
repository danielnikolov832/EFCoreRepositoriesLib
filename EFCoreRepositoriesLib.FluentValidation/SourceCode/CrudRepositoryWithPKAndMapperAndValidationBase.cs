using FluentValidation;
using FluentValidation.Results;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreRepositoriesLib.FluentValidation;

public class CrudRepositoryWithPKAndMapperAndValidationBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO> : CrudRepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO>,
    ICrudRepositoryWithPKAndMapperAndValidationBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO>
    where TPrimaryKeyUserModel : IPrivatePrimaryKeyUser
    where TPrimaryKeyUserDAO : class, IPublicPrimaryKeyUser
{
    public CrudRepositoryWithPKAndMapperAndValidationBase(DbContext dbContext, IMapper mapper,
        IValidator<TPrimaryKeyUserModel>? modelValidator = null, IValidator<TPrimaryKeyUserDAO>? daoValidator = null)
        : base(dbContext, mapper)
    {
        _defaultModelValidator = modelValidator;
        _defaultDaoValidator = daoValidator;
    }

    private readonly IValidator<TPrimaryKeyUserModel>? _defaultModelValidator;
    private readonly IValidator<TPrimaryKeyUserDAO>? _defaultDaoValidator;

    public Action<ValidationResult, IValidator<TPrimaryKeyUserModel>, TPrimaryKeyUserModel>? getset_handleValidationOnModelFail { get; set; }
    public Action<ValidationResult, IValidator<TPrimaryKeyUserDAO>, TPrimaryKeyUserDAO>? getset_handleValidationOnDaoFail { get; set; }

    private bool ValidateModel(TPrimaryKeyUserModel objectToValidate, IValidator<TPrimaryKeyUserModel>? currentValidator = null) => ValidationStaticLib.Validate(HandleValidationOnModelFail, getset_handleValidationOnModelFail, objectToValidate, _defaultModelValidator, currentValidator);
    private bool ValidateDAO(TPrimaryKeyUserDAO objectToValidate, IValidator<TPrimaryKeyUserDAO>? currentValidator = null) => ValidationStaticLib.Validate(HandleValidationOnDAOFail, getset_handleValidationOnDaoFail, objectToValidate, _defaultDaoValidator, currentValidator);

    protected virtual void HandleValidationOnModelFail(ValidationResult result, IValidator<TPrimaryKeyUserModel> validator, TPrimaryKeyUserModel validatedObject)
    {
        ValidationStaticLib.DefaultResultFail(result, validator, validatedObject);
    }

    protected virtual void HandleValidationOnDAOFail(ValidationResult result, IValidator<TPrimaryKeyUserDAO> validator, TPrimaryKeyUserDAO validatedObject)
    {
        ValidationStaticLib.DefaultResultFail(result, validator, validatedObject);
    }
    private (bool isValid, TPrimaryKeyUserDAO? dao) ValidateModelAndDAO(TPrimaryKeyUserModel model, IValidator<TPrimaryKeyUserModel>? currentModelValidator = null, IValidator<TPrimaryKeyUserDAO>? currentDaoValidator = null)
    {
        return ValidationStaticLib.ValidateModelAndDao(model, ValidateModel, ValidateDAO, Adapt, currentModelValidator, currentDaoValidator);
    }

    public virtual bool TryInsert(TPrimaryKeyUserModel model, IValidator<TPrimaryKeyUserModel>? currentModelValidator = null, IValidator<TPrimaryKeyUserDAO>? currentDaoValidator = null)
    {
        (bool isValidModel, TPrimaryKeyUserDAO? entity) = ValidateModelAndDAO(model, currentModelValidator, currentDaoValidator);

        if (!isValidModel || entity is null) return false;

        this.AddToTableAndSaveChanges(entity);

        return true;
    }

    public virtual void Insert(TPrimaryKeyUserModel model, IValidator<TPrimaryKeyUserModel>? currentModelValidator = null, IValidator<TPrimaryKeyUserDAO>? currentDaoValidator = null)
    {
        if(!TryInsert(model, currentModelValidator, currentDaoValidator))
        {
            throw new ValidationException("There was an invalidation of a model in the InsertInternal method");
        }
    }

    public override void Insert(TPrimaryKeyUserModel model)
    {
        Insert(model, null, null);
    }

    public virtual void Update(TPrimaryKeyUserModel model, IValidator<TPrimaryKeyUserModel>? currentModelValidator = null, IValidator<TPrimaryKeyUserDAO>? currentDaoValidator = null)
    {
        (bool isValidModel, TPrimaryKeyUserDAO? entity) = ValidateModelAndDAO(model, currentModelValidator, currentDaoValidator);

        if (!isValidModel || entity is null) return;

        this.UpdateTableAndSaveChanges(entity);
    }

    public override void Update(TPrimaryKeyUserModel model)
    {
        Update(model, null, null);
    }
}

public abstract class CrudRepositoryWithPKAndMapperAndValidationBase<TPrimaryKeyUserModel, TInsert, TUpdate> : CrudRepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TInsert, TUpdate>,
    ICrudRepositoryWithPKAndMapperAndValidationBase<TPrimaryKeyUserModel, TInsert, TUpdate>
    where TPrimaryKeyUserModel : class, IReadOnlyPrimaryKeyUser
{
    protected CrudRepositoryWithPKAndMapperAndValidationBase(DbContext dbContext, IMapper mapper,
        IValidator<TPrimaryKeyUserModel>? defaultIModelValidator = null,
        IValidator<TInsert>? defaultInsertValidator = null, IValidator<TUpdate>? defaultUpdateValidator = null) : base(dbContext, mapper)
    {
        _defaultInsertValidator = defaultInsertValidator;
        _defaultUpdateValidator = defaultUpdateValidator;
        _defaultIModelValidator = defaultIModelValidator;
    }

    private readonly IValidator<TInsert>? _defaultInsertValidator;
    private readonly IValidator<TUpdate>? _defaultUpdateValidator;
    private readonly IValidator<TPrimaryKeyUserModel>? _defaultIModelValidator;

    public Action<ValidationResult, IValidator<TInsert>, TInsert>? getset_handleValidationOnInsertFail { get; set; }
    public Action<ValidationResult, IValidator<TUpdate>, TUpdate>? getset_handleValidationOnUpdateFail { get; set; }
    public Action<ValidationResult, IValidator<TPrimaryKeyUserModel>, TPrimaryKeyUserModel>? getset_handleValidationOnModelFail { get; set; }

    private bool ValidateInsert(TInsert objectToValidate, IValidator<TInsert>? currentValidator = null) => ValidationStaticLib.Validate(HandleValidationOnInsertFail, getset_handleValidationOnInsertFail, objectToValidate, _defaultInsertValidator, currentValidator);
    private bool ValidateUpdate(TUpdate objectToValidate, IValidator<TUpdate>? currentValidator = null) => ValidationStaticLib.Validate(HandleValidationOnUpdateFail, getset_handleValidationOnUpdateFail, objectToValidate, _defaultUpdateValidator, currentValidator);
    protected bool ValidateModel(TPrimaryKeyUserModel objectToValidate, IValidator<TPrimaryKeyUserModel>? currentValidator = null) => ValidationStaticLib.Validate(HandleValidationOnModelFail, getset_handleValidationOnModelFail, objectToValidate, _defaultIModelValidator, currentValidator);

    protected virtual void HandleValidationOnModelFail(ValidationResult result, IValidator<TPrimaryKeyUserModel> validator, TPrimaryKeyUserModel validatedObject)
    {
        ValidationStaticLib.DefaultResultFail(result, validator, validatedObject);
    }

    protected virtual void HandleValidationOnInsertFail(ValidationResult result, IValidator<TInsert> validator, TInsert validatedObject)
    {
        ValidationStaticLib.DefaultResultFail(result, validator, validatedObject);
    }

    protected virtual void HandleValidationOnUpdateFail(ValidationResult result, IValidator<TUpdate> validator, TUpdate validatedObject)
    {
        ValidationStaticLib.DefaultResultFail(result, validator, validatedObject);
    }

    public virtual TPrimaryKeyUserModel? TryInsert(TInsert insert, IValidator<TInsert>? currentInsertValidator = null,
        IValidator<TPrimaryKeyUserModel>? currentModelValidator = null)
    {
        bool isValidModel = ValidateInsert(insert, currentInsertValidator);

        if (isValidModel)
        {
            return InsertInternal(insert, ValidationStaticLib.GetCurrentValidator(currentModelValidator, _defaultIModelValidator));
        }

        return null;
    }

    public virtual TPrimaryKeyUserModel Insert(TInsert insert, IValidator<TInsert>? currentInsertValidator = null,
       IValidator<TPrimaryKeyUserModel>? currentModelValidator = null)
    {
        return TryInsert(insert, currentInsertValidator, currentModelValidator) ??
            throw new ValidationException($"Argument '{nameof(insert)}' is invalid, or there was an invalidation of a model in the InsertInternal method");
    }

    public override TPrimaryKeyUserModel Insert(TInsert insert)
    {
        return Insert(insert, null, null);
    }

    public virtual void Update(TUpdate update, IValidator<TUpdate>? currentUpdateValidator = null,
        IValidator<TPrimaryKeyUserModel>? currentModelValidator = null)
    {
        bool isValidModel = ValidateUpdate(update, currentUpdateValidator);

        if (isValidModel)
        {
            UpdateInternal(update, ValidationStaticLib.GetCurrentValidator(currentModelValidator, _defaultIModelValidator));
        }
    }

    public override void Update(TUpdate update)
    {
        Update(update, null, null);
    }

    protected abstract TPrimaryKeyUserModel? InsertInternal(TInsert insert, IValidator<TPrimaryKeyUserModel>? modelValidator = null);
    protected abstract void UpdateInternal(TUpdate update, IValidator<TPrimaryKeyUserModel>? modelValidator = null);
}

public abstract class CrudRepositoryWithPKAndMapperAndValidationBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO, TInsert, TUpdate> : CrudRepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO, TInsert, TUpdate>,
    ICrudRepositoryWithPKAndMapperAndValidationBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO, TInsert, TUpdate>
    where TPrimaryKeyUserModel : IPrivatePrimaryKeyUser
    where TPrimaryKeyUserDAO : class, IPublicPrimaryKeyUser
{
    protected CrudRepositoryWithPKAndMapperAndValidationBase(DbContext dbContext, IMapper mapper,
        IValidator<TPrimaryKeyUserModel>? defaultIModelValidator = null, IValidator<TPrimaryKeyUserDAO>? defaultDaoValidator = null,
        IValidator<TInsert>? defaultInsertValidator = null, IValidator<TUpdate>? defaultUpdateValidator = null) : base(dbContext, mapper)
    {
        _defaultInsertValidator = defaultInsertValidator;
        _defaultUpdateValidator = defaultUpdateValidator;
        _defaultIModelValidator = defaultIModelValidator;
        _defaultDaoValidator = defaultDaoValidator;
    }

    private readonly IValidator<TInsert>? _defaultInsertValidator;
    private readonly IValidator<TUpdate>? _defaultUpdateValidator;
    private readonly IValidator<TPrimaryKeyUserModel>? _defaultIModelValidator;
    private readonly IValidator<TPrimaryKeyUserDAO>? _defaultDaoValidator;

    public Action<ValidationResult, IValidator<TInsert>, TInsert>? getset_handleValidationOnInsertFail { get; set; }
    public Action<ValidationResult, IValidator<TUpdate>, TUpdate>? getset_handleValidationOnUpdateFail { get; set; }
    public Action<ValidationResult, IValidator<TPrimaryKeyUserModel>, TPrimaryKeyUserModel>? getset_handleValidationOnModelFail { get; set; }
    public Action<ValidationResult, IValidator<TPrimaryKeyUserDAO>, TPrimaryKeyUserDAO>? getset_handleValidationOnDaoFail { get; set; }

    private bool ValidateInsert(TInsert objectToValidate, IValidator<TInsert>? currentValidator = null) => ValidationStaticLib.Validate(HandleValidationOnInsertFail, getset_handleValidationOnInsertFail, objectToValidate, _defaultInsertValidator, currentValidator);
    private bool ValidateUpdate(TUpdate objectToValidate, IValidator<TUpdate>? currentValidator = null) => ValidationStaticLib.Validate(HandleValidationOnUpdateFail, getset_handleValidationOnUpdateFail, objectToValidate, _defaultUpdateValidator, currentValidator);
    protected bool ValidateModel(TPrimaryKeyUserModel objectToValidate, IValidator<TPrimaryKeyUserModel>? currentValidator = null) => ValidationStaticLib.Validate(HandleValidationOnModelFail, getset_handleValidationOnModelFail, objectToValidate, _defaultIModelValidator, currentValidator);
    protected bool ValidateDAO(TPrimaryKeyUserDAO objectToValidate, IValidator<TPrimaryKeyUserDAO>? currentValidator = null) => ValidationStaticLib.Validate(HandleValidationOnDaoFail, getset_handleValidationOnDaoFail, objectToValidate, _defaultDaoValidator, currentValidator);

    protected virtual void HandleValidationOnModelFail(ValidationResult result, IValidator<TPrimaryKeyUserModel> validator, TPrimaryKeyUserModel validatedObject)
    {
        ValidationStaticLib.DefaultResultFail(result, validator, validatedObject);
    }

    protected virtual void HandleValidationOnDaoFail(ValidationResult result, IValidator<TPrimaryKeyUserDAO> validator, TPrimaryKeyUserDAO validatedObject)
    {
        ValidationStaticLib.DefaultResultFail(result, validator, validatedObject);
    }

    protected virtual void HandleValidationOnInsertFail(ValidationResult result, IValidator<TInsert> validator, TInsert validatedObject)
    {
        ValidationStaticLib.DefaultResultFail(result, validator, validatedObject);
    }

    protected virtual void HandleValidationOnUpdateFail(ValidationResult result, IValidator<TUpdate> validator, TUpdate validatedObject)
    {
        ValidationStaticLib.DefaultResultFail(result, validator, validatedObject);
    }

    public virtual TPrimaryKeyUserModel? TryInsert(TInsert insert, IValidator<TInsert>? currentInsertValidator = null,
        IValidator<TPrimaryKeyUserModel>? currentModelValidator = null, IValidator<TPrimaryKeyUserDAO>? currentDaoValidator = null)
    {
        bool isValidModel = ValidateInsert(insert, currentInsertValidator);

        if (isValidModel)
        {
            return InsertInternal(insert, ValidationStaticLib.GetCurrentValidator(currentModelValidator, _defaultIModelValidator), ValidationStaticLib.GetCurrentValidator(currentDaoValidator, _defaultDaoValidator));
        }

        return default;
    }

    public virtual TPrimaryKeyUserModel Insert(TInsert insert, IValidator<TInsert>? currentInsertValidator = null,
       IValidator<TPrimaryKeyUserModel>? currentModelValidator = null, IValidator<TPrimaryKeyUserDAO>? currentDaoValidator = null)
    {
        return TryInsert(insert, currentInsertValidator, currentModelValidator, currentDaoValidator) ??
            throw new ValidationException($"Argument '{nameof(insert)}' is invalid, or there was an invalidation of a model in the InsertInternal method");
    }

    public override TPrimaryKeyUserModel Insert(TInsert insert)
    {
        return Insert(insert, null, null, null);
    }

    public virtual void Update(TUpdate update, IValidator<TUpdate>? currentUpdateValidator = null,
        IValidator<TPrimaryKeyUserModel>? currentModelValidator = null, IValidator<TPrimaryKeyUserDAO>? currentDaoValidator = null)
    {
        bool isValidModel = ValidateUpdate(update, currentUpdateValidator);

        if (isValidModel)
        {
            UpdateInternal(update, ValidationStaticLib.GetCurrentValidator(currentModelValidator, _defaultIModelValidator), ValidationStaticLib.GetCurrentValidator(currentDaoValidator, _defaultDaoValidator));
        }
    }

    public override void Update(TUpdate update)
    {
        Update(update, null, null, null);
    }

    protected abstract TPrimaryKeyUserModel? InsertInternal(TInsert insert, IValidator<TPrimaryKeyUserModel>? modelValidator = null, IValidator<TPrimaryKeyUserDAO>? daoValidator = null);
    protected abstract void UpdateInternal(TUpdate update, IValidator<TPrimaryKeyUserModel>? modelValidator = null, IValidator<TPrimaryKeyUserDAO>? daoValidator = null);
}