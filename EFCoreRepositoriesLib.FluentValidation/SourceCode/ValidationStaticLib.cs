using FluentValidation;
using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreRepositoriesLib.FluentValidation;

internal static class ValidationStaticLib
{
    internal static IValidator<T>? GetCurrentValidator<T>(IValidator<T>? defaultValidator = null, IValidator<T>? currentValidator = null)
    {
        return currentValidator ?? defaultValidator;
    }

    internal static bool Validate<T>(Action<ValidationResult, IValidator<T>, T> handleFailAction,
        T objectToValidate, IValidator<T>? defaultValidator = null, IValidator<T>? currentValidator = null)
    {
        IValidator<T>? validator = GetCurrentValidator(defaultValidator, currentValidator);

        if (validator is null) return true;

        ValidationResult result = validator.Validate(objectToValidate);

        if (!result.IsValid)
        {
            handleFailAction(result, validator, objectToValidate);
        }

        return result.IsValid;
    }

    internal static bool Validate<T>(Action<ValidationResult, IValidator<T>, T> handleFailAction,
        Action<ValidationResult, IValidator<T>, T>? injectedHandleFailAction,
        T objectToValidate, IValidator<T>? defaultValidator = null, IValidator<T>? currentValidator = null)
    {
        IValidator<T>? validator = GetCurrentValidator(defaultValidator, currentValidator);

        if (validator is null) return true;

        ValidationResult result = validator.Validate(objectToValidate);

        if (!result.IsValid)
        {
            handleFailAction(result, validator, objectToValidate);
            injectedHandleFailAction?.Invoke(result, validator, objectToValidate);
        }

        return result.IsValid;
    }

    internal static (bool isValid, TPrimaryKeyUserDAO? dao) ValidateModelAndDao<TPrimaryKeyUserModel, TPrimaryKeyUserDAO>(TPrimaryKeyUserModel model,
        Func<TPrimaryKeyUserModel, IValidator<TPrimaryKeyUserModel>?, bool> validateModelAction,
        Func<TPrimaryKeyUserDAO, IValidator<TPrimaryKeyUserDAO>?, bool> validateDaoAction,
        Func<TPrimaryKeyUserModel, TPrimaryKeyUserDAO> adaptAction,
        IValidator<TPrimaryKeyUserModel>? currentModelValidator = null, IValidator<TPrimaryKeyUserDAO>? currentDaoValidator = null)
        where TPrimaryKeyUserModel : PrivatePrimaryKeyUser
        where TPrimaryKeyUserDAO : PublicPrimaryKeyUser
    {
        bool isValidModel = validateModelAction(model, currentModelValidator);

        if (!isValidModel) return (false, null);

        TPrimaryKeyUserDAO entity = adaptAction(model);

        bool isValidDao = validateDaoAction(entity, currentDaoValidator);

        return (isValidDao, entity);
    }

    internal static void DefaultResultFail(ValidationResult result)
    {
        throw new ValidationException(result.Errors);
    }
}