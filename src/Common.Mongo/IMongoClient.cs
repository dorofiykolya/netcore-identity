using MongoDB.Driver;

namespace Common.Mongo;

public interface IMongoClient
{
    IMongoDatabase Database { get; }
}

public class MongoClientImpl : IMongoClient
{
    public IMongoDatabase Database { get; }
    public MongoClient Client { get; }

    public MongoClientImpl(MongoOptions options)
    {
        var value = options;
        Client = new MongoClient(value.ConnectionString);
        Database = Client.GetDatabase(value.DatabaseName);
    }
}
