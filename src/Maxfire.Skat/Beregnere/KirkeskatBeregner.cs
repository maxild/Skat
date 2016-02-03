using Maxfire.Skat.Extensions;

namespace Maxfire.Skat.Beregnere
{
	public class KirkeskatBeregner : SkattepligtigIndkomstSkatteberegner
	{
		public ValueTuple<decimal> BeregnSkat(
			IValueTuple<ISkatteyder> skatteydere,
			IValueTuple<ISkattepligtigeIndkomster> indkomster, 
			IValueTuple<IKommunaleSatser> kommunaleSatser)
		{
			return BeregnSkatCore(indkomster, () => 
				kommunaleSatser.Map((satser, index) => satser.GetKirkeskattesatsFor(skatteydere[index])));
		}
	}
}