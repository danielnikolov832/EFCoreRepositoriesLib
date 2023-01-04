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
    where TPrimaryKeyUser : ReadOnlyPrimaryKeyUser
{
    public CrudRepositoryWithPKAndValidationBase(DbContext dbContext, IValidator<TPrimaryKeyUser> defaultValidator) : base(dbContext)
    {
        _defaultValidator = defaultValidator;
    }

    private readonly IValidator<TPrimaryKeyUser> _defaultValidator;

    private bool ValidateModel(TPrimaryKeyUser objectToValidate, IValidator<TPrimaryKeyUser>? currentValidator = null) => ValidationStaticLib.Validate(HandleValidationOnModelFail, objectToValidate, _defaultValidator, currentValidator);

    protected virtual void HandleValidationOnModelFail(ValidationResult result, IValidator<TPrimaryKeyUser> validator, TPrimaryKeyUser validatedObject)
    {
        ValidationStaticLib.DefaultResultFail(result);
    }

    public override void Insert(TPrimaryKeyUser model)
    {
        bool isValidModel = ValidateModel(model);

        if (isValidModel)
        {
            base.Insert(model);
        }
    }
    public void Insert(TPrimaryKeyUser model, IValidator<TPrimaryKeyUser> currentValidator)
    {
        bool isValidModel = ValidateModel(model, currentValidator);

        if (isValidModel)
        {
            base.Insert(model);
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

public abstract class CrudRepositoryWithPKAndValidationBase<TPrimaryKeyUser, TInsert, TUpdate> : CrudRepositoryWithPKBase<TPrimaryKeyUser, TInsert, TUpdate>
    where TPrimaryKeyUser : ReadOnlyPrimaryKeyUser
{
    protected CrudRepositoryWithPKAndValidationBase(DbContext dbContext, IValidator<TInsert>? defaultValidator = null, IValidator<TUpdate>? defaultUpdateValidator = null) : base(dbContext)
    {
        _defaultInsertValidator = defaultValidator;
        _defaultUpdateValidator = defaultUpdateValidator;
    }

    private readonly IValidator<TInsert>? _defaultInsertValidator;
    private readonly IValidator<TUpdate>? _defaultUpdateValidator;

    private bool ValidateInsert(TInsert objectToValidate, IValidator<TInsert>? currentValidator = null) => ValidationStaticLib.Validate(HandleValidationOnInsertFail, objectToValidate, _defaultInsertValidator, currentValidator);
    private bool ValidateUpdate(TUpdate objectToValidate, IValidator<TUpdate>? currentValidator = null) => ValidationStaticLib.Validate(HandleValidationOnUpdateFail, objectToValidate, _defaultUpdateValidator, currentValidator);

    protected virtual void HandleValidationOnInsertFail(ValidationResult result, IValidator<TInsert> validator, TInsert validatedObject)
    {
        ValidationStaticLib.DefaultResultFail(result);
    }

    protected virtual void HandleValidationOnUpdateFail(ValidationResult result, IValidator<TUpdate> validator, TUpdate validatedObject)
    {
        ValidationStaticLib.DefaultResultFail(result);
    }

    public override TPrimaryKeyUser Insert(TInsert insert)
    {
        bool isValidModel = ValidateInsert(insert);

        if (isValidModel)
        {
            return InsertInternal(insert);
        }

        throw new ValidationException($"Model {nameof(insert)} is invalid");
    }

    public virtual TPrimaryKeyUser Insert(TInsert insert, IValidator<TInsert> currentValidator)
    {
        bool isValidModel = ValidateInsert(insert, currentValidator);

        if (isValidModel)
        {
            return InsertInternal(insert);
        }

        throw new ValidationException($"Model {nameof(insert)} is invalid");
    }

    public override void Update(TUpdate update)
    {
        bool isValidModel = ValidateUpdate(update);

        if (isValidModel)
        {
            UpdateInternal(update);
        }
    }

    public virtual void Update(TUpdate model, IValidator<TUpdate> currentValidator)
    {
        bool isValidModel = ValidateUpdate(model, currentValidator);

        if (isValidModel)
        {
            UpdateInternal(model);
        }
    }

    protected abstract TPrimaryKeyUser InsertInternal(TInsert insert);
    protected abstract void UpdateInternal(TUpdate update);
}