using MongoDB.Driver;
using System.Linq.Expressions;
using Contracts.Domains;
using Contracts.Domains.Interfaces;
using Infrastructure.Extensions;
using Shared.Configurations;

namespace Infrastructure.Common;

public class MongoDbRepository<T> : IMongoDbRepositoryBase<T> where T : MongoEntity
{
    public IMongoDatabase Database { get; }

    public MongoDbRepository(IMongoClient client, MongoDbSettings settings)
    {
        Database = client.GetDatabase(settings.DatabaseName);
    }

    public IMongoCollection<T> FindAll(ReadPreference? readPreference = null)
    {
        return Database
            .WithReadPreference(readPreference ?? ReadPreference.Primary)
            .GetCollection<T>(GetCollectionName());
    }

    protected virtual IMongoCollection<T> Collection =>
        Database.GetCollection<T>(GetCollectionName());

    public Task CreateAsync(T entity) => Collection.InsertOneAsync(entity);

    public Task UpdateAsync(T entity)
    {
        Expression<Func<T, string>> func = f => f.Id;
        var value = (string)entity.GetType()
            .GetProperty(func.Body.ToString()
                .Split(".")[1])?.GetValue(entity, null);
        var filter = Builders<T>.Filter.Eq(func, value);

        return Collection.ReplaceOneAsync(filter, entity);
    }

    public Task DeleteAsync(string id) => Collection.DeleteOneAsync(x => x.Id.Equals(id));

    private static string GetCollectionName()
    {
        return (typeof(T).GetCustomAttributes(typeof(BsonCollectionAttribute), true).FirstOrDefault() as
            BsonCollectionAttribute)?.CollectionName;
    }
}