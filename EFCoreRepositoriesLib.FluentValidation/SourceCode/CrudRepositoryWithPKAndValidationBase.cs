using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;

namespace EFCoreRepositoriesLib.FluentValidation;

public class CrudRepositoryWithPKAndValidationBase<TPrimaryKeyUser> : CrudRepositoryWithPKBase<TPrimaryKeyUser>, ICrudRepositoryWithPKAndValidationBase<TPrimaryKeyUser>
    where TPrimaryKeyUser : class, IReadOnlyPrimaryKeyUser
{
    public CrudRepositoryWithPKAndValidationBase(DbContext dbContext, IValidator<TPrimaryKeyUser> defaultValidator) : base(dbContext)
    {
        _defaultValidator = defaultValidator;
    }

    private readonly IValidator<TPrimaryKeyUser> _defaultValidator;

    public Action<ValidationResult, IValidator<TPrimaryKeyUser>, TPrimaryKeyUser>? getset_handleValidationOnModelFail { get; set; }

    private bool ValidateModel(TPrimaryKeyUser objectToValidate, IValidator<TPrimaryKeyUser>? currentValidator = null) => ValidationStaticLib.Validate(HandleValidationOnModelFail, getset_handleValidationOnModelFail, objectToValidate, _defaultValidator, currentValidator);

    protected virtual void HandleValidationOnModelFail(ValidationResult result, IValidator<TPrimaryKeyUser> validator, TPrimaryKeyUser validatedObject)
    {
        ValidationStaticLib.DefaultResultFail(result, validator, validatedObject);
    }

    public bool TryInsert(TPrimaryKeyUser model, IValidator<TPrimaryKeyUser>? currentModelValidator = null)
    {
        bool isValidModel = ValidateModel(model, currentModelValidator);

        if (isValidModel)
        {
            base.Insert(model);
        }

        return isValidModel;
    }

    public override void Insert(TPrimaryKeyUser model)
    {
        if(!TryInsert(model))
        {
            throw new ValidationException($"Argument {model} is invalid");
        }
    }

    public void Insert(TPrimaryKeyUser model, IValidator<TPrimaryKeyUser> currentValidator)
    {
        if (!TryInsert(model, currentValidator))
        {
            throw new ValidationException($"Argument {model} is invalid");
        }
    }

    public override void Update(TPrimaryKeyUser model)
    {
        bool isValidModel = ValidateModel(model);

        if (isValidModel)
        {
            base.Update(model);
        }
    }
    public void Update(TPrimaryKeyUser model, IValidator<TPrimaryKeyUser> currentValidator)
    {
        bool isValidModel = ValidateModel(model, currentValidator);

        if (isValidModel)
        {
            base.Update(model);
        }
    }
}

public abstract class CrudRepositoryWithPKAndValidationBase<TPrimaryKeyUser, TInsert, TUpdate> : CrudRepositoryWithPKBase<TPrimaryKeyUser, TInsert, TUpdate>,
    ICrudRepositoryWithPKAndValidationBase<TPrimaryKeyUser, TInsert, TUpdate>
    where TPrimaryKeyUser : class, IReadOnlyPrimaryKeyUser
{
    protected CrudRepositoryWithPKAndValidationBase(DbContext dbContext, IValidator<TPrimaryKeyUser>? defaultModelValidator = null,
        IValidator<TInsert>? defaultInsertValidator = null, IValidator<TUpdate>? defaultUpdateValidator = null) : base(dbContext)
    {
        _defaultModelValidator = defaultModelValidator;
        _defaultInsertValidator = defaultInsertValidator;
        _defaultUpdateValidator = defaultUpdateValidator;
    }

    private readonly IValidator<TPrimaryKeyUser>? _defaultModelValidator;
    private readonly IValidator<TInsert>? _defaultInsertValidator;
    private readonly IValidator<TUpdate>? _defaultUpdateValidator;

    public Action<ValidationResult, IValidator<TPrimaryKeyUser>, TPrimaryKeyUser>? getset_handleValidationOnModelFail { get; set; }
    public Action<ValidationResult, IValidator<TInsert>, TInsert>? getset_handleValidationOnInsertFail { get; set; }
    public Action<ValidationResult, IValidator<TUpdate>, TUpdate>? getset_handleValidationOnUpdateFail { get; set; }

    private bool ValidateInsert(TInsert objectToValidate, IValidator<TInsert>? currentValidator = null) => ValidationStaticLib.Validate(HandleValidationOnInsertFail, HandleValidationOnInsertFail, objectToValidate, _defaultInsertValidator, currentValidator);
    private bool ValidateUpdate(TUpdate objectToValidate, IValidator<TUpdate>? currentValidator = null) => ValidationStaticLib.Validate(HandleValidationOnUpdateFail, HandleValidationOnUpdateFail, objectToValidate, _defaultUpdateValidator, currentValidator);

    protected virtual void HandleValidationOnModelFail(ValidationResult result, IValidator<TPrimaryKeyUser> validator, TPrimaryKeyUser validatedObject)
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

    private IValidator<TPrimaryKeyUser>? GetValidator(IValidator<TPrimaryKeyUser>? currentModelValidator)
    {
        return ValidationStaticLib.GetCurrentValidator(_defaultModelValidator, currentModelValidator);
    }

    public virtual TPrimaryKeyUser? TryInsert(TInsert insert, IValidator<TInsert>? currentInsertValidator = null, IValidator<TPrimaryKeyUser>? currentModelValidator = null)
    {
        bool isValidModel = ValidateInsert(insert, currentInsertValidator);

        if (isValidModel)
        {
            return InsertInternal(insert, GetValidator(currentModelValidator));
        }

        return null;
    }

    public virtual TPrimaryKeyUser Insert(TInsert insert, IValidator<TInsert>? currentInsertValidator = null, IValidator<TPrimaryKeyUser>? currentModelValidator = null)
    {
        return TryInsert(insert, currentInsertValidator, currentModelValidator) ??
            throw new ValidationException($"Argument {insert} is invalid, or there was an invalidation of a model in the InsertInternal method");
    }

    public override TPrimaryKeyUser Insert(TInsert insert)
    {
        return Insert(insert, null, null);
    }

    public virtual void Update(TUpdate update, IValidator<TUpdate>? currentUpdateValidator = null, IValidator<TPrimaryKeyUser>? currentModelValidator = null)
    {
        bool isValidModel = ValidateUpdate(update, currentUpdateValidator);

        if (isValidModel)
        {
            UpdateInternal(update, GetValidator(currentModelValidator));
        }
    }

    public override void Update(TUpdate update)
    {
        Update(update, null, null);
    }

    protected abstract TPrimaryKeyUser? InsertInternal(TInsert insert, IValidator<TPrimaryKeyUser>? modelValidator = null);
    protected abstract void UpdateInternal(TUpdate update, IValidator<TPrimaryKeyUser>? modelValidator = null);
}