using System.Reflection;

namespace Maxfire.Skat.Reflection
{
	/// <summary>
	/// An anonymous type-safe accessor (getter and setter) for a property.
	/// </summary>
	/// <typeparam name="TObject">The type of object.</typeparam>
	/// <typeparam name="TPropertyValue">The type of property.</typeparam>
	///<remarks>
	/// Access restrictions are ignored for fully trusted code. That is, private properties can be accessed and mutated 
	/// via Reflection whenever the code is fully trusted.
	/// </remarks>
	public class PropertyAccessor<TObject, TPropertyValue> : Accessor<TObject, TPropertyValue>
	{
		private readonly PropertyInfo _propertyInfo;
		private const BindingFlags DEFAULT_BINDINGFLAGS = BindingFlags.Public | BindingFlags.Instance;

		public PropertyAccessor(PropertyInfo propertyInfo)
		{
			_propertyInfo = propertyInfo;
		}

		public void SetValue(TObject target, TPropertyValue propertyValue)
		{
			_propertyInfo.SetValue(target, propertyValue, DEFAULT_BINDINGFLAGS, null, null, null);
		}

		public TPropertyValue GetValue(TObject target)
		{
			return (TPropertyValue)_propertyInfo.GetValue(target, DEFAULT_BINDINGFLAGS, null, null, null);
		}

		public string PropertyName
		{
			get { return _propertyInfo.Name; }
		}
	}
}