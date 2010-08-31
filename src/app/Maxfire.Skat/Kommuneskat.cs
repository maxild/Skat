using Maxfire.Skat.Extensions;

namespace Maxfire.Skat
{	
	public class KommuneskatBeregner : SkattepligtigIndkomstSkatteberegner
	{
		public ValueTuple<decimal> BeregnSkat(
			IValueTuple<ISkattepligtigeIndkomster> indkomster, 
			IValueTuple<IKommunaleSatser> kommunaleSatser)
		{
			return BeregnSkatCore(indkomster, () => kommunaleSatser.Map(x => x.Kommuneskattesats));
		}
	}
}