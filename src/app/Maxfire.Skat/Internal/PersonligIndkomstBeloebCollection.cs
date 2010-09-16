﻿using System.Collections.Generic;
using System.Linq;
using Maxfire.Core;
using Maxfire.Skat.Beregnere;

namespace Maxfire.Skat.Internal
{
	internal class PersonligIndkomstBeloebCollection : TextValuePairCollection<ITextValuePair<PersonligIndkomstValue>>, IPersonligIndkomstBeloebCollection
	{
		public static readonly PersonligIndkomstBeloebCollection Empty = new PersonligIndkomstBeloebCollection(Enumerable.Empty<ITextValuePair<PersonligIndkomstValue>>());

		public PersonligIndkomstBeloebCollection(IEnumerable<ITextValuePair<PersonligIndkomstValue>> items)
			: base(items, item => item.Value.EfterAMBidrag)
		{
		}
	}
}