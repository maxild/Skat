using Maxfire.Skat.Extensions;

namespace Maxfire.Skat
{
	public class SkatterAfSkattepligtigIndkomstBeregner
	{
		private readonly ISkattelovRegistry _skattelovRegistry;

		public SkatterAfSkattepligtigIndkomstBeregner(ISkattelovRegistry skattelovRegistry)
		{
			_skattelovRegistry = skattelovRegistry;
		}

		public ValueTuple<SkatterAfSkattepligtigIndkomst> BeregnSkat(IValueTuple<IPersonligeBeloeb> indkomster, IValueTuple<IKommunaleSatser> kommunaleSatser, int skatteAar)
		{
			var sundhedsbidragBeregner = new SundhedsbidragBeregner(_skattelovRegistry);
			var sundhedsbidrag = sundhedsbidragBeregner.BeregnSkat(indkomster, skatteAar);

			var kommuneskatBeregner = new KommuneskatBeregner();
			var kommuneskat = kommuneskatBeregner.BeregnSkat(indkomster, kommunaleSatser);

			var kirkeskatBeregner = new KirkeskatBeregner();
			var kirkeskat = kirkeskatBeregner.BeregnSkat(indkomster, kommunaleSatser);

			return sundhedsbidrag.Map(index => 
				new SkatterAfSkattepligtigIndkomst(sundhedsbidrag[index], kommuneskat[index], kirkeskat[index]));
		}
	}
}