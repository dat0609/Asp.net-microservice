using System.Reflection;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Infrastructure.Common.Models;
using Microsoft.EntityFrameworkCore;
using Shared.SeedWork;

namespace Infrastructure.Extensions;

public static class AutoMapperExtension
{
    public static IMappingExpression<TSource, TDestination> IgnoreAllNonExisting<TSource, TDestination>
        (this IMappingExpression<TSource, TDestination> expression)
    {
        var flags = BindingFlags.Public | BindingFlags.Instance;
        var sourceType = typeof(TSource);
        var destinationProperties = typeof(TDestination).GetProperties(flags);

        foreach (var property in destinationProperties)
            if (sourceType.GetProperty(property.Name, flags) == null)
                expression.ForMember(property.Name, opt => opt.Ignore());
        return expression;
    }
    
    public static Task<PagedList<TDestination>> PaginatedListAsync<TDestination>(this IQueryable<TDestination> queryable, int pageIndex, int pageSize) where TDestination : class
        => PagedList<TDestination>.ToPagedList(queryable, pageIndex, pageSize);

}