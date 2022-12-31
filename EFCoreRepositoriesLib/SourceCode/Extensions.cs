namespace EFCoreRepositoriesLib;

internal static class Extensions
{
    public static IQueryable<TPrimaryKeyUser> ApplyTransformations<TPrimaryKeyUser>(
        this IQueryable<TPrimaryKeyUser> queryable,
        IRepositoryWithTransformations<TPrimaryKeyUser> repositoryWithTransformations)
        where TPrimaryKeyUser : ReadOnlyPrimaryKeyUser
    {
        return repositoryWithTransformations.ApplyTransformations(queryable);
    }
}