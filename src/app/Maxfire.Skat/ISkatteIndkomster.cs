using System.Collections.Generic;
using Maxfire.Core;

namespace Maxfire.Skat
{
	public interface ISkatteIndkomster : IPersonligeIndkomster, ISkattepligtigeIndkomster
	{
	}

	public interface ISumableEnumerable<out TItem, out TValue>: IEnumerable<TItem>, ISumable<TValue>
	{
	}

	public interface IBeloebCollection : ISumableEnumerable<ITextValuePair<decimal>, decimal> {}

	public interface IPersonligIndkomstBeloebCollection : ISumableEnumerable<ITextValuePair<PersonligIndkomstValue>, decimal> {}

	public class PersonligIndkomstBeloebCollection : TextValuePairCollection<ITextValuePair<PersonligIndkomstValue>>, IPersonligIndkomstBeloebCollection
	{
		public PersonligIndkomstBeloebCollection(IEnumerable<ITextValuePair<PersonligIndkomstValue>> items)
			: base(items, item => item.Value.EfterAMBidrag)
		{
		}
	}

	/// <summary>
	/// Adapter that wraps collection of ITextValuePair<decimal> in an IBeloebCollection instance.
	/// </summary>
	public class BeloebCollection : TextValuePairCollection<ITextValuePair<decimal>>, IBeloebCollection
	{
		public BeloebCollection(IEnumerable<ITextValuePair<decimal>> items)
			: base(items, item => item.Value)
		{
		}
	}
}