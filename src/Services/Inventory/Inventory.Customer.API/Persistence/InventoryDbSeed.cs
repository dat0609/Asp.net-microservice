using Inventory.Customer.API.Entities;
using Inventory.Customer.API.Extension;
using MongoDB.Driver;
using Shared.Configurations;
using Shared.Enum.Inventory;

namespace Inventory.Customer.API.Persistence;

public class InventoryDbSeed
{
    public async Task SeedDataAsync(IMongoClient mongoClient, MongoDbSettings settings)
    {
        var databaseName = settings.DatabaseName;
        var database = mongoClient.GetDatabase(databaseName);
        var inventoryCollection = database.GetCollection<InventoryEntry>("InventoryEntries");
        if (await inventoryCollection.EstimatedDocumentCountAsync() == 0)
        {
            await inventoryCollection.InsertManyAsync(GetPreconfiguredInventories());
        }
    }

    private IEnumerable<InventoryEntry> GetPreconfiguredInventories()
    {
        return new List<InventoryEntry>
        {
            new()
            {
                Quantity = 10,
                DocumentNo = Guid.NewGuid().ToString(),
                ItemNo = "Lotus",
                ExternalDocumentNo = Guid.NewGuid().ToString(),
                DocumentType = EDocumentType.Purchase
            },
            new()
            {
                ItemNo = "Cadillac",
                Quantity = 10,
                DocumentNo = Guid.NewGuid().ToString(),
                ExternalDocumentNo = Guid.NewGuid().ToString(),
                DocumentType = EDocumentType.Purchase
            },
        };
    }
}