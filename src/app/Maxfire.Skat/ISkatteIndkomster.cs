using System.Collections.Generic;
using System.Linq;
using Maxfire.Core;

namespace Maxfire.Skat
{
	public interface ISkatteIndkomster : IPersonligeIndkomster, ISkattepligtigeIndkomster
	{
	}

	public interface ISkatteIndkomsterModregning : ISkatteIndkomster, IPersonligeIndkomsterModregning, ISkattepligtigeIndkomsterModregning
	{
	}

	public interface ISumableEnumerable<out TItem, out TValue>: IEnumerable<TItem>, ISumable<TValue>
	{
	}

	public interface IBeloebCollection : ISumableEnumerable<ITextValuePair<decimal>, decimal> {}

	public interface IPersonligIndkomstBeloebCollection : ISumableEnumerable<ITextValuePair<PersonligIndkomstValue>, decimal> {}

	public class PersonligIndkomstBeloebCollection : TextValuePairCollection<ITextValuePair<PersonligIndkomstValue>>, IPersonligIndkomstBeloebCollection
	{
		public static readonly PersonligIndkomstBeloebCollection Empty = new PersonligIndkomstBeloebCollection(Enumerable.Empty<ITextValuePair<PersonligIndkomstValue>>());

		public PersonligIndkomstBeloebCollection(IEnumerable<ITextValuePair<PersonligIndkomstValue>> items)
			: base(items, item => item.Value.EfterAMBidrag)
		{
		}
	}

	/// <summary>
	/// Adapter that wraps collection of ITextValuePair&lt;decimal&gt; in an IBeloebCollection instance.
	/// </summary>
	public class BeloebCollection : TextValuePairCollection<ITextValuePair<decimal>>, IBeloebCollection
	{
		public static readonly BeloebCollection Empty = new BeloebCollection(Enumerable.Empty<ITextValuePair<decimal>>());

		public BeloebCollection(IEnumerable<ITextValuePair<decimal>> items)
			: base(items, item => item.Value)
		{
		}
	}
}