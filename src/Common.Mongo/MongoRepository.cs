using System.Linq.Expressions;
using System.Reflection;
using Common.Mongo.Attributes;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Common.Mongo;

public class MongoRepository<TDocument> : IMongoRepository<TDocument>
    where TDocument : IMongoDocument
{
    public ILogger<TDocument> Logger { get; }
    public Type DocumentType => typeof(TDocument);
    public IMongoDatabase Database { get; }
    public IMongoCollection<TDocument> Collection { get; }

    public MongoRepository(IMongoClient client, ILogger<TDocument> logger, string? customCollectionName = null, IndexBuilder? customIndexBuilder = null)
    {
        Logger = logger;
        Database = client.Database;

        var documentCollectionName = DocumentType.GetCustomAttribute<CollectionAttribute>(true)?.Name;
        if (string.IsNullOrWhiteSpace(customCollectionName) && string.IsNullOrWhiteSpace(documentCollectionName))
        {
            throw new InvalidOperationException($"You must set a collection's name");
        }

        Collection = Database.GetCollection<TDocument>(customCollectionName ?? documentCollectionName);

        var indexAttribute = DocumentType.GetCustomAttribute<IndexAttribute>();
        if (indexAttribute != null)
        {
            customIndexBuilder = Activator.CreateInstance(indexAttribute.IndexesType) as IndexBuilder;
        }
        if (customIndexBuilder != null)
        {
            var task = customIndexBuilder.BuildAsync(this);
            task.Wait();
        }
    }

    public virtual IQueryable<TDocument> AsQueryable()
    {
        return Collection.AsQueryable();
    }

    public virtual IEnumerable<TDocument> FilterBy(
        Expression<Func<TDocument, bool>> filterExpression)
    {
        return Collection.Find(filterExpression).ToEnumerable();
    }

    public virtual IEnumerable<TProjected> FilterBy<TProjected>(
        Expression<Func<TDocument, bool>> filterExpression,
        Expression<Func<TDocument, TProjected>> projectionExpression)
    {
        return Collection.Find(filterExpression).Project(projectionExpression).ToEnumerable();
    }

    public virtual TDocument FindOne(Expression<Func<TDocument, bool>> filterExpression)
    {
        return Collection.Find(filterExpression).FirstOrDefault();
    }

    public virtual async Task<TDocument?> FindOneAsync(Expression<Func<TDocument, bool>> filterExpression)
    {
        return (await Collection.FindAsync(filterExpression)).FirstOrDefault();
    }

    public virtual TDocument FindById(string id)
    {
        var objectId = new ObjectId(id);
        var filter = Builders<TDocument>.Filter.Eq(doc => doc!.Id, objectId);
        return Collection.Find(filter).SingleOrDefault();
    }

    public virtual TDocument FindById(ObjectId id)
    {
        var filter = Builders<TDocument>.Filter.Eq(doc => doc!.Id, id);
        return Collection.Find(filter).SingleOrDefault();
    }

    public virtual async Task<TDocument?> FindByIdAsync(string id)
    {
        var objectId = new ObjectId(id);
        var filter = Builders<TDocument>.Filter.Eq(doc => doc!.Id, objectId);
        return (await Collection.FindAsync(filter)).SingleOrDefault();
    }

    public virtual async Task<TDocument?> FindByIdAsync(ObjectId id)
    {
        var filter = Builders<TDocument>.Filter.Eq(doc => doc!.Id, id);
        return (await Collection.FindAsync(filter)).SingleOrDefault();
    }

    public virtual void InsertOne(TDocument document)
    {
        Collection.InsertOne(document);
    }

    public virtual Task InsertOneAsync(TDocument document)
    {
        return Task.Run(() => Collection.InsertOneAsync(document));
    }

    public void InsertMany(ICollection<TDocument> documents)
    {
        Collection.InsertMany(documents);
    }


    public virtual async Task InsertManyAsync(ICollection<TDocument> documents)
    {
        await Collection.InsertManyAsync(documents);
    }

    public void ReplaceOne(TDocument document)
    {
        var filter = Builders<TDocument>.Filter.Eq(doc => doc!.Id, document.Id);
        Collection.FindOneAndReplace(filter, document);
    }

    public virtual async Task ReplaceOneAsync(TDocument document)
    {
        var filter = Builders<TDocument>.Filter.Eq(doc => doc!.Id, document.Id);
        await Collection.FindOneAndReplaceAsync(filter, document);
    }

    public void DeleteOne(Expression<Func<TDocument, bool>> filterExpression)
    {
        Collection.FindOneAndDelete(filterExpression);
    }

    public Task DeleteOneAsync(Expression<Func<TDocument, bool>> filterExpression)
    {
        return Task.Run(() => Collection.FindOneAndDeleteAsync(filterExpression));
    }

    public void DeleteById(string id)
    {
        var objectId = new ObjectId(id);
        var filter = Builders<TDocument>.Filter.Eq(doc => doc!.Id, objectId);
        Collection.FindOneAndDelete(filter);
    }

    public Task DeleteByIdAsync(ObjectId id)
    {
        return Task.Run(() =>
        {
            var filter = Builders<TDocument>.Filter.Eq(doc => doc!.Id, id);
            Collection.FindOneAndDeleteAsync(filter);
        });
    }

    public Task DeleteByIdAsync(string id)
    {
        return Task.Run(() =>
        {
            var objectId = new ObjectId(id);
            var filter = Builders<TDocument>.Filter.Eq(doc => doc!.Id, objectId);
            Collection.FindOneAndDeleteAsync(filter);
        });
    }

    public void DeleteMany(Expression<Func<TDocument, bool>> filterExpression)
    {
        Collection.DeleteMany(filterExpression);
    }

    public Task DeleteManyAsync(Expression<Func<TDocument, bool>> filterExpression)
    {
        return Task.Run(() => Collection.DeleteManyAsync(filterExpression));
    }
}