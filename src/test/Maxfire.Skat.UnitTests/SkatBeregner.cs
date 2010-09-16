using Maxfire.Skat.Beregnere;

namespace Maxfire.Skat.UnitTests
{
	public class SkatBeregner
	{
		private readonly ISkattelovRegistry _skattelovRegistry;

		public SkatBeregner(ISkattelovRegistry skattelovRegistry)
		{
			_skattelovRegistry = skattelovRegistry;
		}

		public ValueTuple<IndkomstSkatter> BeregnSkat(
			IValueTuple<ISkatteyder> skatteydere,
			IValueTuple<ISkatteIndkomster> indkomster, 
			IValueTuple<IKommunaleSatser> kommunaleSatser, 
			int skatteAar)
		{
			var skatAfPersonligIndkomstBeregner = new SkatterAfPersonligIndkomstBeregner(_skattelovRegistry);
			var skatterAfPersonligIndkomst = skatAfPersonligIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser, skatteAar);

			var skatAfSkattepligtigIndkomstBeregner = new SkatterAfSkattepligtigIndkomstBeregner(_skattelovRegistry);
			var skatterAfSkattepligtigIndkomst = skatAfSkattepligtigIndkomstBeregner.BeregnSkat(skatteydere, indkomster, kommunaleSatser, skatteAar);

			return SkatteUtility.CombineSkat(skatterAfPersonligIndkomst, skatterAfSkattepligtigIndkomst);
		}
	}
}