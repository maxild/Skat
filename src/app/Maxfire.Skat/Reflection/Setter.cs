namespace Maxfire.Skat.Reflection
{
	public interface Setter<in TObject, in TPropertyValue>
	{
		void SetValue(TObject target, TPropertyValue propertyValue);
		string PropertyName { get; }
	}
}