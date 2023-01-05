using Infrastructure.Common.Models;
using MongoDB.Driver;

namespace Infrastructure.Extensions;

public static class MongoCollectionExtensions
{
    public static Task<PagedList<TDestination>> PaginatedListAsync<TDestination>(
        this IMongoCollection<TDestination> collection,
        FilterDefinition<TDestination> filter,
        int pageIndex, int pageNumber) where TDestination : class
        => PagedList<TDestination>.ToPagedList(collection, filter, pageIndex, pageNumber);
}