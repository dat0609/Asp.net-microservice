using MongoDB.Bson.Serialization.Attributes;

namespace Contracts.Domains;

public abstract class MongoEntity
{
    [BsonId]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    [BsonElement("_id")]
    public virtual string Id { get; protected set; }

    [BsonElement("createDate")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedDate { get; set; }
    
    [BsonElement("lastModifiedDate")]
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime LastModifiedDate { get; set; }
}