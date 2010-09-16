﻿using Maxfire.Skat.Extensions;

namespace Maxfire.Skat.Beregnere
{
	public class PersonfradragSkattevaerdiOmregner : ISkattevaerdiOmregner
	{
		private readonly Skatter _skattesatser;

		public PersonfradragSkattevaerdiOmregner(
			ISkatteyder skatteyder, 
			IKommunaleSatser kommunaleSatser, 
			ISkattepligtigIndkomstRegistry skattelovRegistry, 
			int skatteAar)
		{
			_skattesatser = new Skatter(
				bundskat: skattelovRegistry.GetBundSkattesats(skatteAar),
				sundhedsbidrag: skattelovRegistry.GetSundhedsbidragSkattesats(skatteAar),
				kommuneskat: kommunaleSatser.Kommuneskattesats,
				kirkeskat: kommunaleSatser.GetKirkeskattesatsFor(skatteyder));
		}

		public Skatter BeregnSkattevaerdier(decimal personfradrag)
		{
			var skattevaerdier = _skattesatser * personfradrag;
			return skattevaerdier.RoundMoney();
		}

		public decimal BeregnFradragsbeloeb(decimal skattevaerdi)
		{
			decimal fradragsbeloeb = skattevaerdi / _skattesatser.Sum();
			return fradragsbeloeb.RoundMoney();
		}
	}
}