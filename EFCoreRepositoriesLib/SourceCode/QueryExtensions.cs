using FluentQuery.Core;
using FluentQuery.SQLSupport;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EFCoreRepositoriesLib;

public static class QueryExtensions
{
    public static IEnumerable<T> Query<T>(this DbSet<T> table, IQuery<T> query)
    where T : class
    {
        Expression<Func<T, bool>> predicate = QueryToExpression.QueryToExp(query);

        return table.AsExpandableEFCore().Where(predicate.Compile());
    }

    public static IEnumerable<T> Query<T>(this DbSet<T> table, QueryForSQLBase<T> query)
        where T : class
    {
        Expression<Func<T, bool>> predicate = QueryToExpression.QueryToExp(query);

        return table.AsExpandableEFCore().Where(predicate.Compile());
    }

    public static IEnumerable<T> Query<T>(this DbSet<T> table, IQuery<T> query, IRepositoryWithTransformations<T> repositoryWithTransformations)
        where T : ReadOnlyPrimaryKeyUser
    {
        Expression<Func<T, bool>> predicate = QueryToExpression.QueryToExp(query);

        return table.AsExpandableEFCore().ApplyTransformations(repositoryWithTransformations).Where(predicate.Compile());
    }

    public static IEnumerable<T> Query<T>(this DbSet<T> table, QueryForSQLBase<T> query, IRepositoryWithTransformations<T> repositoryWithTransformations)
        where T : ReadOnlyPrimaryKeyUser
    {
        Expression<Func<T, bool>> predicate = QueryToExpression.QueryToExp(query);

        return table.AsExpandableEFCore().ApplyTransformations(repositoryWithTransformations).Where(predicate.Compile());
    }
}