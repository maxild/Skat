using Maxfire.Skat.Extensions;

namespace Maxfire.Skat
{
	public class KirkeskatBeregner : SkattepligtigIndkomstSkatteberegner
	{
		public ValueTuple<decimal> BeregnSkat(IValueTuple<IPersonligeBeloeb> indkomster, IValueTuple<IKommunaleSatser> kommunaleSatser)
		{
			return BeregnSkatCore(indkomster, () => kommunaleSatser.Map(x => x.Kirkeskattesats));
		}
	}
}