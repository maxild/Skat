using System;
using Maxfire.Skat.Extensions;

namespace Maxfire.Skat
{
	public abstract class SkattepligtigIndkomstSkatteberegner
	{
		protected virtual ValueTuple<decimal> BeregnSkatCore(
			IValueTuple<IPersonligeBeloeb> indkomster, 
			Func<ValueTuple<decimal>> skattesatsProvider)
		{
			var skattepligtigIndkomst = indkomster.Map(x => x.Skattegrundlag.SkattepligtigIndkomst);
			var skattesats = skattesatsProvider();
			return skattesats * (+skattepligtigIndkomst);
		}
	}
}