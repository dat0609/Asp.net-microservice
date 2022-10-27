using MongoDB.Driver;
using Shared.SeedWork;

namespace Infrastructure.Extensions;

public static class MongoCollectionExtension
{
    public static Task<PagedList<T>> PagedListAsync<T>(this IMongoCollection<T> collection, FilterDefinition<T> filter,
        int pageIndex, int pageSize) where T : class 
        => PagedList<T>.ToPagedList(collection, filter, pageIndex, pageSize);
}