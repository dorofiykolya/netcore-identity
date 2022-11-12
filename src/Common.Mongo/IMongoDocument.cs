using Common.Mongo.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Common.Mongo;

public interface IMongoDocument
{
    [BsonId(Order = 1)]
    ObjectId Id { get; set; }

    DateTime CreatedAt { get; }
}
