namespace Maxfire.Skat.Reflection
{
    public interface Getter<in TObject, out TPropertyValue>
    {
        TPropertyValue GetValue(TObject target);
        string PropertyName { get; }
    }
}
