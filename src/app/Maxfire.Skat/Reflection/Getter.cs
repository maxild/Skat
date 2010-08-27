namespace Maxfire.Skat.Reflection
{
	/// <summary>
	/// The accessor (of a property) contains the executable statements associated with 
	/// getting (reading or computing) or setting (writing) the property.
	/// </summary>
	public interface Getter<in TObject, out TPropertyValue>
	{
		TPropertyValue GetValue(TObject target);
		string PropertyName { get; }
	}
}