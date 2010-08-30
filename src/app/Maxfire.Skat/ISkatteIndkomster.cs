using System.Collections.Generic;
using Maxfire.Core;

namespace Maxfire.Skat
{
	public interface ISkatteIndkomster
	{
		decimal PersonligIndkomstAMIndkomst { get; }
		decimal PersonligIndkomst { get; }

		decimal NettoKapitalIndkomst { get; }
		
		decimal LigningsmaessigtFradrag { get; }

		decimal SkattepligtigIndkomst { get; }
		
		decimal KapitalPensionsindskud { get; }
		decimal AktieIndkomst { get; }
	}

	public interface ISpecificeredeSkatteIndkomster : ISkatteIndkomster
	{
		IPersonligIndkomstBeloebCollection PersonligeIndkomsterAMIndkomster { get; }
		IPersonligIndkomstBeloebCollection PersonligeIndkomsterEjAMIndkomster { get; }
		IPersonligIndkomstBeloebCollection PersonligeIndkomster { get; }

		IBeloebCollection NettoKapitalIndkomster { get; }

		IBeloebCollection LigningsmaessigeFradrag { get; }

		IBeloebCollection SkattepligtigIndkomster { get; }
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

	public class BeloebCollection : TextValuePairCollection<ITextValuePair<decimal>>, IBeloebCollection
	{
		public BeloebCollection(IEnumerable<ITextValuePair<decimal>> items)
			: base(items, item => item.Value)
		{
		}
	}
}