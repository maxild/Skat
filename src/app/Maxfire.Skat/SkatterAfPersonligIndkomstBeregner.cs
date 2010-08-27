using Maxfire.Skat.Extensions;

namespace Maxfire.Skat
{
	public class SkatterAfPersonligIndkomstBeregner
	{
		private readonly ISkattelovRegistry _skattelovRegistry;

		public SkatterAfPersonligIndkomstBeregner(ISkattelovRegistry skattelovRegistry)
		{
			_skattelovRegistry = skattelovRegistry;
		}

		public ValueTuple<SkatterAfPersonligIndkomst> BeregnSkat(IValueTuple<IPersonligeBeloeb> indkomster, IValueTuple<IKommunaleSatser> kommunaleSatser, int skatteAar)
		{
			var bundskatBeregner = new BundskatBeregner(_skattelovRegistry);
			var bundskat = bundskatBeregner.BeregnSkat(indkomster, skatteAar);

			var mellemskatBeregner = new MellemskatBeregner(_skattelovRegistry);
			var mellemskat = mellemskatBeregner.BeregnSkat(indkomster, skatteAar);

			var topskatBeregner = new TopskatBeregner(_skattelovRegistry);
			var topskat = topskatBeregner.BeregnSkat(indkomster, skatteAar, kommunaleSatser);

			var aktieindkomstskatLavesteTrinBeregner = new AktieindkomstskatLavesteTrinBeregner(_skattelovRegistry);
			var aktieindkomstskatLavesteTrin = aktieindkomstskatLavesteTrinBeregner.BeregnSkat(indkomster, skatteAar);

			var aktieindkomstskatMellemsteTrinBeregner = new AktieindkomstskatMellemsteTrinBeregner(_skattelovRegistry);
			var aktieindkomstskatMellemsteTrin = aktieindkomstskatMellemsteTrinBeregner.BeregnSkat(indkomster, skatteAar);

			var aktieindkomstskatHoejesteTrinBeregner = new AktieindkomstskatHoejesteTrinBeregner(_skattelovRegistry);
			var aktieindkomstskatHoejesteTrin = aktieindkomstskatHoejesteTrinBeregner.BeregnSkat(indkomster, skatteAar);

			return bundskat.Map(index =>
					new SkatterAfPersonligIndkomst(bundskat[index], mellemskat[index], topskat[index],
					                               aktieindkomstskatLavesteTrin[index],
					                               aktieindkomstskatMellemsteTrin[index] + aktieindkomstskatHoejesteTrin[index]));
		}
	}
}