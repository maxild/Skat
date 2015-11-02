using System;
using Maxfire.Skat.Extensions;

namespace Maxfire.Skat.Beregnere
{
	public abstract class SkattepligtigIndkomstSkatteberegner
	{
		protected virtual ValueTuple<decimal> BeregnSkatCore(
			IValueTuple<ISkattepligtigeIndkomster> indkomster, 
			Func<ValueTuple<decimal>> skattesatsProvider)
		{
			var skattepligtigIndkomst = indkomster.Map(x => x.SkattepligtigIndkomstSkattegrundlag);
			var skattesats = skattesatsProvider();
			return skattesats * (+skattepligtigIndkomst);
		}
	}
}