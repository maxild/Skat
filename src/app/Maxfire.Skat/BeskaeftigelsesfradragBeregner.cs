using Maxfire.Skat.Extensions;

namespace Maxfire.Skat
{
	// TODO: grundlaget er forkert her
	public class BeskaeftigelsesfradragBeregner : IAMIndkomstSkatteberegner
	{
		private readonly ISkattelovRegistry _skattelovRegistry;

		public BeskaeftigelsesfradragBeregner(ISkattelovRegistry skattelovRegistry)
		{
			_skattelovRegistry = skattelovRegistry;
		}

		public ValueTuple<decimal> BeregnFradrag(ValueTuple<decimal> amIndkomster, int skatteAar)
		{
			decimal sats = _skattelovRegistry.GetBeskaeftigelsesfradragSats(skatteAar);
			decimal grundbeloeb = _skattelovRegistry.GetBeskaeftigelsesfradragGrundbeloeb(skatteAar);
			return BeregnFradrag(amIndkomster, sats, grundbeloeb);
		}

		public decimal BeregnFradrag(decimal amIndkomst, int skatteAar)
		{
			decimal sats = _skattelovRegistry.GetBeskaeftigelsesfradragSats(skatteAar);
			decimal grundbeloeb = _skattelovRegistry.GetBeskaeftigelsesfradragGrundbeloeb(skatteAar);
			return BeregnFradrag(amIndkomst, sats, grundbeloeb);
		}

// ReSharper disable MemberCanBeMadeStatic.Global
		public ValueTuple<decimal> BeregnFradrag(ValueTuple<decimal> amIndkomster, decimal sats, decimal grundbeloeb)
// ReSharper restore MemberCanBeMadeStatic.Global
		{
			return amIndkomster.Map((decimal amIndkomst) => sats * amIndkomst.NonNegative()).Loft(grundbeloeb);
		}

// ReSharper disable MemberCanBeMadeStatic.Global
		public decimal BeregnFradrag(decimal amIndkomst, decimal sats, decimal grundbeloeb)
// ReSharper restore MemberCanBeMadeStatic.Global
		{
			return (sats * amIndkomst.NonNegative()).Loft(grundbeloeb);
		}

		decimal IAMIndkomstSkatteberegner.Beregn(decimal amIndkomst, int skatteAr)
		{
			return BeregnFradrag(amIndkomst, skatteAr);
		}
	}
}