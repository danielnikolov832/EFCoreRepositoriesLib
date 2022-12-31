using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EFCoreRepositoriesLib.FluentValidation;

public class CrudRepositoryWithPKAndValidationBase<TPrimaryKeyUser> : CrudRepositoryWithPKBase<TPrimaryKeyUser>
    where TPrimaryKeyUser : ReadOnlyPrimaryKeyUser
{
    public CrudRepositoryWithPKAndValidationBase(DbContext dbContext, IValidator<TPrimaryKeyUser> defaultValidator) : base(dbContext)
    {
        _defaultValidator = defaultValidator;
    }

    private readonly IValidator<TPrimaryKeyUser> _defaultValidator;

    public override void Insert(TPrimaryKeyUser model)
    {
        base.Insert(model);
    }
}