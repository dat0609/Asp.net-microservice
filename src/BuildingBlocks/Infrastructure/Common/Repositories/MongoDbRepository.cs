using Contracts.Domains;
using Contracts.Domains.Interface;
using Infrastructure.Extensions;
using MongoDB.Driver;
using Shared.Configurations;

namespace Infrastructure.Common.Repositories;

public class MongoDbRepository<T> : IMongoDbRepositoryBase<T> where T : MongoEntity
{
    private readonly IMongoDatabase _database;

    public MongoDbRepository(IMongoClient client, MongoDbSettings settings)
    {
        _database = client.GetDatabase(settings.DatabaseName);
    }

    public IMongoCollection<T> FindAll(ReadPreference? readPreference = null)
        => _database.WithReadPreference(readPreference ?? ReadPreference.Primary)
            .GetCollection<T>(GetCollectionName());
    
    protected virtual IMongoCollection<T> GetCollection => _database.GetCollection<T>(GetCollectionName());

    public Task CreateAsync(T entity) => GetCollection.InsertOneAsync(entity);

    public Task UpdateAsync(T entity)
    {
        var filter = Builders<T>.Filter.Eq(x => x.Id, entity.Id);
        return GetCollection.ReplaceOneAsync(filter, entity);
    }

    public Task DeleteAsync(string id)
    {
        var filter = Builders<T>.Filter.Eq(x => x.Id, id);
        return GetCollection.DeleteOneAsync(filter);
    }
    
    private static string? GetCollectionName()
    {
        return (typeof(T).GetCustomAttributes(typeof(BsonCollection), true)
            .FirstOrDefault() as BsonCollection)?.CollectionName;
    }
}