namespace Common.Mongo.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class IndexAttribute : Attribute
{
    public Type IndexesType { get; }

    public IndexAttribute(Type indexesType)
    {
        IndexesType = indexesType;

        if (indexesType == null)
        {
            throw new ArgumentNullException(nameof(indexesType));
        }
        if (!indexesType.IsSubclassOf(typeof(IndexBuilder)))
        {
            throw new ArgumentException($"{nameof(indexesType)} must be extended of {typeof(IndexBuilder<>)}");
        }
        if (indexesType.GetConstructors().All(c => c.GetParameters().Length != 0))
        {
            throw new ArgumentNullException($"{nameof(indexesType)} must be had default constructor");
        }
    }
}
