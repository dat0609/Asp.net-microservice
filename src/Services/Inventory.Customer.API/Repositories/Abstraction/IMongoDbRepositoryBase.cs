using Inventory.Customer.API.Entities.Abstraction;
using MongoDB.Driver;

namespace Inventory.Customer.API.Repositories.Abstraction;

public interface IMongoDbRepositoryBase<T> where T : MongoEntity
{
    IMongoCollection<T> FindAll(ReadPreference? readPreference = null);
    Task CreateAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(string id);
}