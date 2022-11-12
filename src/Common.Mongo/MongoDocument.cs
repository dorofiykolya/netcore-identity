using Common.Mongo.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Common.Mongo;

[BsonIgnoreExtraElements(Inherited = true)]
public abstract class MongoDocument : IMongoDocument
{
    [BsonId(Order = 1)] public ObjectId Id { get; set; }

    [BsonElement(Order = 2)] public DateTime CreatedAt => Id.CreationTime;

    protected MongoDocument()
    {
        Id = ObjectId.GenerateNewId(DateTime.UtcNow);
    }
}
