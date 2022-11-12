using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Common.Mongo;

public abstract class IndexBuilder
{
    internal abstract Task BuildAsync(IRepository repository);
}

public abstract class IndexBuilder<TDocument> : IndexBuilder
        where TDocument : MongoDocument
{
    private readonly List<(IndexKeysDefinition<TDocument> definition, IndexOptions<TDocument> Options)> _definitions;

    protected IndexBuilder()
    {
        _definitions = new List<(IndexKeysDefinition<TDocument> definition, IndexOptions<TDocument> Options)>();
    }

    internal override Task BuildAsync(IRepository repository)
    {
        return BuildAsync((MongoRepository<TDocument>)repository);
    }

    private async Task BuildAsync(MongoRepository<TDocument> repository)
    {
        var cursor = await repository.Collection.Indexes.ListAsync();

        var indexes = await cursor.ToListAsync();

        foreach (var index in indexes)
        {
            repository.Logger.LogInformation("Found index {IndexDefinition}", index["name"].AsString);
        }

        var created = new List<(BsonDocument Document, CreateIndexModel<TDocument> Model)>();

        foreach (var (definition, options) in _definitions)
        {
            var createIndexOptions = options.Build();

            var document = definition.Render(repository.Collection.DocumentSerializer, new BsonSerializerRegistry());

            createIndexOptions.Name ??= GenerateIndexName(document);

            var i = indexes.FindIndex(x => x["name"].AsString == createIndexOptions.Name);
            if (i < 0)
            {
                created.Add((document, new CreateIndexModel<TDocument>(definition, createIndexOptions)));
            }
            else
            {
                indexes.RemoveAt(i);
            }
        }

        foreach (var index in indexes)
        {
            var name = index["name"].AsString;

            if (name != "_id_")
            {
                repository.Logger.LogInformation("Delete index {IndexDefinition}", index["key"]);

                await repository.Collection.Indexes.DropOneAsync(name);
            }
        }

        foreach (var tuple in created)
        {
            repository.Logger.LogInformation("Create index {IndexDefinition}", tuple.Document);

            await repository.Collection.Indexes.CreateOneAsync(tuple.Model);
        }

        _definitions.Clear();
    }

    protected IndexOptions<TDocument> Index(IndexKeysDefinition<TDocument> definition)
    {
        var options = new IndexOptions<TDocument>();

        _definitions.Add((definition, options));

        return options;
    }

    protected IndexKeysDefinition<TDocument> Ascending(Expression<Func<TDocument, object>> field)
    {
        return Builders<TDocument>.IndexKeys.Ascending(field);
    }

    protected IndexKeysDefinition<TDocument> Ascending(FieldDefinition<TDocument> field)
    {
        return Builders<TDocument>.IndexKeys.Ascending(field);
    }

    protected IndexKeysDefinition<TDocument> Descending(Expression<Func<TDocument, object>> field)
    {
        return Builders<TDocument>.IndexKeys.Descending(field);
    }

    protected IndexKeysDefinition<TDocument> Descending(FieldDefinition<TDocument> field)
    {
        return Builders<TDocument>.IndexKeys.Descending(field);
    }

    protected IndexKeysDefinition<TDocument> Text(Expression<Func<TDocument, object>> field)
    {
        return Builders<TDocument>.IndexKeys.Text(field);
    }

    protected IndexKeysDefinition<TDocument> Text(FieldDefinition<TDocument> field)
    {
        return Builders<TDocument>.IndexKeys.Text(field);
    }

    private string GenerateIndexName(BsonDocument document)
    {
        return string.Join("_", document.Elements.Select(x => $"{x.Name}.{x.Value}"));
    }
}

public class IndexOptions<TEntity>
        where TEntity : MongoDocument
{
    private readonly CreateIndexOptions<TEntity> _options;

    internal IndexOptions()
    {
        _options = new CreateIndexOptions<TEntity>();
    }

    public IndexOptions<TEntity> Name(string name)
    {
        _options.Name = name;
        return this;
    }

    public IndexOptions<TEntity> Expire(TimeSpan expiration)
    {
        _options.ExpireAfter = expiration;
        return this;
    }

    public IndexOptions<TEntity> Partial(FilterDefinition<TEntity> filter)
    {
        _options.PartialFilterExpression = filter;
        return this;
    }

    public IndexOptions<TEntity> Sparse()
    {
        _options.Sparse = true;
        return this;
    }

    public IndexOptions<TEntity> Unique()
    {
        _options.Unique = true;
        return this;
    }

    public CreateIndexOptions Build()
    {
        return _options;
    }
}
