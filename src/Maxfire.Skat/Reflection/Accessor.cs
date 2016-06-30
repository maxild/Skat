namespace Maxfire.Skat.Reflection
{
    /// <summary>
    /// The accessor (of a property) contains the executable statements associated with
    /// getting (reading or computing) or setting (writing) the property.
    /// </summary>
    public interface Accessor<in TObject, TPropertyValue> : Getter<TObject, TPropertyValue>, Setter<TObject, TPropertyValue>
    {
    }
}
