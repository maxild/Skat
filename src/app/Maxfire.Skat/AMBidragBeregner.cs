using Maxfire.Skat.Extensions;

namespace Maxfire.Skat
{
	public class AMBidragBeregner : IAMIndkomstSkatteberegner
	{
		private readonly ISkattelovRegistry _skattelovRegistry;

		public AMBidragBeregner(ISkattelovRegistry skattelovRegistry)
		{
			_skattelovRegistry = skattelovRegistry;
		}

		public ValueTuple<decimal> BeregnSkat(ValueTuple<decimal> amIndkomster, int skatteAar)
		{
			decimal amBidragSkattesats = _skattelovRegistry.GetAMBidragSkattesats(skatteAar);
			return amBidragSkattesats * (+amIndkomster);
		}

		public decimal BeregnSkat(decimal amIndkomst, int skatteAar)
		{
			decimal amBidragSkattesats = _skattelovRegistry.GetAMBidragSkattesats(skatteAar);
			return amBidragSkattesats * amIndkomst.NonNegative();
		}

		decimal IAMIndkomstSkatteberegner.Beregn(decimal amIndkomst, int skatteAr)
		{
			return BeregnSkat(amIndkomst, skatteAr);
		}
	}
}