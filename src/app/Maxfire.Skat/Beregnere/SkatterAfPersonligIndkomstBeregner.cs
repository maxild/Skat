﻿using Maxfire.Skat.Extensions;

namespace Maxfire.Skat.Beregnere
{
	public class SkatterAfPersonligIndkomstBeregner
	{
		private readonly ISkattelovRegistry _skattelovRegistry;

		public SkatterAfPersonligIndkomstBeregner(ISkattelovRegistry skattelovRegistry)
		{
			_skattelovRegistry = skattelovRegistry;
		}

		public ValueTuple<IndkomstSkatterAfPersonligIndkomst> BeregnSkat(
			IValueTuple<IPersonligeIndkomster> indkomster, 
			IValueTuple<IKommunaleSatser> kommunaleSatser, 
			int skatteAar)
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

			return bundskat.MapByIndex(index =>
					new IndkomstSkatterAfPersonligIndkomst(bundskat[index], mellemskat[index], topskat[index],
					                               aktieindkomstskatLavesteTrin[index],
					                               aktieindkomstskatMellemsteTrin[index] + aktieindkomstskatHoejesteTrin[index]));
		}
	}
}