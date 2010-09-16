using System.Collections.Generic;
using System.Linq;
using Maxfire.Core;

namespace Maxfire.Skat.Beregnere
{
	public class PersonligIndkomstBeloebCollection : TextValuePairCollection<ITextValuePair<PersonligIndkomstValue>>, IPersonligIndkomstBeloebCollection
	{
		public static readonly PersonligIndkomstBeloebCollection Empty = new PersonligIndkomstBeloebCollection(Enumerable.Empty<ITextValuePair<PersonligIndkomstValue>>());

		public PersonligIndkomstBeloebCollection(IEnumerable<ITextValuePair<PersonligIndkomstValue>> items)
			: base(items, item => item.Value.EfterAMBidrag)
		{
		}
	}
}