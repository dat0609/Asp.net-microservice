namespace Inventory.Customer.API.Extension;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class BsonCollection : Attribute
{
    public string? CollectionName { get; set; }
    public BsonCollection(string? collectionName)
    {
        CollectionName = collectionName;
    }
}