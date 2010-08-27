namespace Maxfire.Skat.Reflection
{
	public interface Accessor<in TObject, TPropertyValue> : Getter<TObject, TPropertyValue>, Setter<TObject, TPropertyValue>
	{
	}
}