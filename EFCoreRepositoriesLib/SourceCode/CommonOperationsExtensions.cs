using Microsoft.EntityFrameworkCore;

namespace EFCoreRepositoriesLib;

public static class CommonOperationsExtensions
{
    internal static IQueryable<TPrimaryKeyUser> ApplyTransformations<TPrimaryKeyUser>(
        this IQueryable<TPrimaryKeyUser> queryable,
        IRepositoryWithTransformations<TPrimaryKeyUser> repositoryWithTransformations)
        where TPrimaryKeyUser : class, IReadOnlyPrimaryKeyUser
    {
        return repositoryWithTransformations.ApplyTransformations(queryable);
    }

    public static void AddToTableAndSaveChanges<TPrimaryKeyUser>(this RepositoryWithPKBase<TPrimaryKeyUser> repositoryWithPKBase,
        TPrimaryKeyUser model,
        bool acceptAllChangesOnSuccess = true)
        where TPrimaryKeyUser : class, IReadOnlyPrimaryKeyUser
    {
        repositoryWithPKBase._table.Add(model);

        repositoryWithPKBase._dbContext.SaveChanges(acceptAllChangesOnSuccess);
    }

    public static void AddToTableAndSaveChanges<TPrimaryKeyUserModel, TPrimaryKeyUserDAO>(this RepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO> repositoryWithPKBase,
        TPrimaryKeyUserDAO dao,
        bool acceptAllChangesOnSuccess = true)
        where TPrimaryKeyUserModel : IPrivatePrimaryKeyUser
        where TPrimaryKeyUserDAO : class, IPublicPrimaryKeyUser
    {
        repositoryWithPKBase._table.Add(dao);

        repositoryWithPKBase._dbContext.SaveChanges(acceptAllChangesOnSuccess);
    }

    public static void UpdateTableAndSaveChanges<TPrimaryKeyUser>(
        this RepositoryWithPKBase<TPrimaryKeyUser> repositoryWithPKBase,
        TPrimaryKeyUser model,
        bool acceptAllChangesOnSuccess = true)
        where TPrimaryKeyUser : class, IReadOnlyPrimaryKeyUser
    {
        repositoryWithPKBase._table.Update(model);

        repositoryWithPKBase._dbContext.SaveChanges(acceptAllChangesOnSuccess);
    }

    public static void UpdateTableAndSaveChanges<TPrimaryKeyUserModel, TPrimaryKeyUserDAO>(
        this RepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO> repositoryWithPKBase,
        TPrimaryKeyUserDAO dao,
        bool acceptAllChangesOnSuccess = true)
        where TPrimaryKeyUserModel : IPrivatePrimaryKeyUser
        where TPrimaryKeyUserDAO : class, IPublicPrimaryKeyUser
    {
        repositoryWithPKBase._table.Update(dao);

        repositoryWithPKBase._dbContext.SaveChanges(acceptAllChangesOnSuccess);
    }

    public static void RemoveFromTableAndSaveChanges<TPrimaryKeyUser>(
        this RepositoryWithPKBase<TPrimaryKeyUser> repositoryWithPKBase,
        TPrimaryKeyUser model,
        bool acceptAllChangesOnSuccess = true)
        where TPrimaryKeyUser : class, IReadOnlyPrimaryKeyUser
    {
        repositoryWithPKBase._table.Remove(model);

        repositoryWithPKBase._dbContext.SaveChanges(acceptAllChangesOnSuccess);
    }

    public static void RemoveFromTableAndSaveChanges<TPrimaryKeyUserModel, TPrimaryKeyUserDAO>(
        this RepositoryWithPKAndMapperBase<TPrimaryKeyUserModel, TPrimaryKeyUserDAO> repositoryWithPKBase,
        TPrimaryKeyUserDAO dao,
        bool acceptAllChangesOnSuccess = true)
        where TPrimaryKeyUserModel : IPrivatePrimaryKeyUser
        where TPrimaryKeyUserDAO : class, IPublicPrimaryKeyUser
    {
        repositoryWithPKBase._table.Remove(dao);

        repositoryWithPKBase._dbContext.SaveChanges(acceptAllChangesOnSuccess);
    }
}