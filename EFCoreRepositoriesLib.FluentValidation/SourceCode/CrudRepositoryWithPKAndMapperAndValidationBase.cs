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
    where TPrimaryKeyUserModel : PrivatePrimaryKeyUser
    where TPrimaryKeyUserDAO : PublicPrimaryKeyUser
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

    private bool ValidateModel(TPrimaryKeyUserModel objectToValidate, IValidator<TPrimaryKeyUserModel>? currentValidator = null) => ValidationStaticLib.Validate(HandleValidationOnModelFail, objectToValidate, _defaultModelValidator, currentValidator);
    private bool ValidateDAO(TPrimaryKeyUserDAO objectToValidate, IValidator<TPrimaryKeyUserDAO>? currentValidator = null) => ValidationStaticLib.Validate(HandleValidationOnDAOFail, objectToValidate, _defaultDaoValidator, currentValidator);

    protected virtual void HandleValidationOnModelFail(ValidationResult result, IValidator<TPrimaryKeyUserModel> validator, TPrimaryKeyUserModel validatedObject)
    {
        ValidationStaticLib.DefaultResultFail(result);
    }

    protected virtual void HandleValidationOnDAOFail(ValidationResult result, IValidator<TPrimaryKeyUserDAO> validator, TPrimaryKeyUserDAO validatedObject)
    {
        ValidationStaticLib.DefaultResultFail(result);
    }
    private (bool isValid, TPrimaryKeyUserDAO? dao) ValidateModelAndDAO(TPrimaryKeyUserModel model, IValidator<TPrimaryKeyUserModel>? currentModelValidator = null, IValidator<TPrimaryKeyUserDAO>? currentDaoValidator = null)
    {
        return ValidationStaticLib.ValidateModelAndDao(model, ValidateModel, ValidateDAO, Adapt, currentModelValidator, currentDaoValidator);
    }

    public virtual void Insert(TPrimaryKeyUserModel model, IValidator<TPrimaryKeyUserModel>? currentModelValidator = null, IValidator<TPrimaryKeyUserDAO>? currentDaoValidator = null)
    {
        (bool isValidModel, TPrimaryKeyUserDAO? entity) = ValidateModelAndDAO(model, currentModelValidator, currentDaoValidator);

        if (!isValidModel || entity is null) return;

        _table.Add(entity);

        _dbContext.SaveChanges();
    }

    public override void Insert(TPrimaryKeyUserModel model)
    {
        Insert(model, null, null);
    }

    public virtual void Update(TPrimaryKeyUserModel model, IValidator<TPrimaryKeyUserModel>? currentModelValidator = null, IValidator<TPrimaryKeyUserDAO>? currentDaoValidator = null)
    {
        (bool isValidModel, TPrimaryKeyUserDAO? entity) = ValidateModelAndDAO(model, currentModelValidator, currentDaoValidator);

        if (!isValidModel || entity is null) return;

        _table.Update(entity);

        _dbContext.SaveChanges(isValidModel);
    }

    public override void Update(TPrimaryKeyUserModel model)
    {
        Update(model, null, null);
    }
}

public abstract class CrudRepositoryWithPKAndMapperAndValidationBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO, TInsert, TUpdate> : CrudRepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO, TInsert, TUpdate>,
    ICrudRepositoryWithPKAndMapperAndValidationBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO, TInsert, TUpdate>
    where TPrimaryKeyUserModel : PrivatePrimaryKeyUser
    where TPrimaryKeyUserDAO : PublicPrimaryKeyUser
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

    private readonly IValidator<TPrimaryKeyUserModel>? _defaultIModelValidator;
    private readonly IValidator<TPrimaryKeyUserDAO>? _defaultDaoValidator;
    private readonly IValidator<TInsert>? _defaultInsertValidator;
    private readonly IValidator<TUpdate>? _defaultUpdateValidator;

    protected bool ValidateModel(TPrimaryKeyUserModel objectToValidate, IValidator<TPrimaryKeyUserModel>? currentValidator = null) => ValidationStaticLib.Validate(HandleValidationOnModelFail, objectToValidate, _defaultIModelValidator, currentValidator);
    protected bool ValidateDAO(TPrimaryKeyUserDAO objectToValidate, IValidator<TPrimaryKeyUserDAO>? currentValidator = null) => ValidationStaticLib.Validate(HandleValidationOnDaoFail, objectToValidate, _defaultDaoValidator, currentValidator);
    private bool ValidateInsert(TInsert objectToValidate, IValidator<TInsert>? currentValidator = null) => ValidationStaticLib.Validate(HandleValidationOnInsertFail, objectToValidate, _defaultInsertValidator, currentValidator);
    private bool ValidateUpdate(TUpdate objectToValidate, IValidator<TUpdate>? currentValidator = null) => ValidationStaticLib.Validate(HandleValidationOnUpdateFail, objectToValidate, _defaultUpdateValidator, currentValidator);

    protected virtual void HandleValidationOnModelFail(ValidationResult result, IValidator<TPrimaryKeyUserModel> validator, TPrimaryKeyUserModel validatedObject)
    {
        ValidationStaticLib.DefaultResultFail(result);
    }

    protected virtual void HandleValidationOnDaoFail(ValidationResult result, IValidator<TPrimaryKeyUserDAO> validator, TPrimaryKeyUserDAO validatedObject)
    {
        ValidationStaticLib.DefaultResultFail(result);
    }

    protected virtual void HandleValidationOnInsertFail(ValidationResult result, IValidator<TInsert> validator, TInsert validatedObject)
    {
        ValidationStaticLib.DefaultResultFail(result);
    }

    protected virtual void HandleValidationOnUpdateFail(ValidationResult result, IValidator<TUpdate> validator, TUpdate validatedObject)
    {
        ValidationStaticLib.DefaultResultFail(result);
    }

    private IValidator<TPrimaryKeyUserModel>? GetModelValidator(IValidator<TPrimaryKeyUserModel>? currentModelValidator = null)
    {
        return currentModelValidator ?? _defaultIModelValidator;
    }

    private IValidator<TPrimaryKeyUserDAO>? GetDaoValidator(IValidator<TPrimaryKeyUserDAO>? currentModelValidator = null)
    {
        return currentModelValidator ?? _defaultDaoValidator;
    }

    public virtual TPrimaryKeyUserModel Insert(TInsert insert, IValidator<TInsert>? currentInsertValidator = null,
        IValidator<TPrimaryKeyUserModel>? currentModelValidator = null, IValidator<TPrimaryKeyUserDAO>? currentDaoValidator = null)
    {
        bool isValidModel = ValidateInsert(insert, currentInsertValidator);

        if (isValidModel)
        {
            return InsertInternal(insert, GetModelValidator(currentModelValidator), GetDaoValidator(currentDaoValidator));
        }

        throw new ValidationException($"Model {nameof(insert)} is invalid");
    }

    public override TPrimaryKeyUserModel Insert(TInsert insert)
    {
        return Insert(insert, null, null, null);
    }

    public virtual void Update(TUpdate model, IValidator<TUpdate>? currentUpdateValidator = null,
        IValidator<TPrimaryKeyUserModel>? currentModelValidator = null, IValidator<TPrimaryKeyUserDAO>? currentDaoValidator = null)
    {
        bool isValidModel = ValidateUpdate(model, currentUpdateValidator);

        if (isValidModel)
        {
            UpdateInternal(model, GetModelValidator(currentModelValidator), GetDaoValidator(currentDaoValidator));
        }
    }

    public override void Update(TUpdate update)
    {
        Update(update, null, null, null);
    }

    protected abstract TPrimaryKeyUserModel InsertInternal(TInsert insert, IValidator<TPrimaryKeyUserModel>? modelValidator = null, IValidator<TPrimaryKeyUserDAO>? daoValidator = null);
    protected abstract void UpdateInternal(TUpdate update, IValidator<TPrimaryKeyUserModel>? modelValidator = null, IValidator<TPrimaryKeyUserDAO>? daoValidator = null);
}