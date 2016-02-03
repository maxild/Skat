using System.Collections.Generic;
using Maxfire.Core;
using Maxfire.Skat.Internal;

namespace Maxfire.Skat
{
	public static class SelvangivneBeloeb
	{
		public static IBeloebCollection Create(params ITextValuePair<decimal>[] selvangivneBeloeb)
		{
			return new BeloebCollection(selvangivneBeloeb);
		}
		public static IBeloebCollection Create(IEnumerable<ITextValuePair<decimal>> selvangivneBeloeb)
		{
			return new BeloebCollection(selvangivneBeloeb);
		}
	}
}