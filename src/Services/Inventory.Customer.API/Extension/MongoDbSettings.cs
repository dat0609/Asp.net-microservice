using Shared.Configurations;

namespace Inventory.Customer.API.Extension;

public class MongoDbSettings : DatabaseSettings
{
    public string DatabaseName { get; set; }
}