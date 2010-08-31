using Maxfire.Skat.Extensions;

namespace Maxfire.Skat
{
	public class BeskaeftigelsesfradragBeregner
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

// ReSharper disable MemberCanBeMadeStatic.Global
		public ValueTuple<decimal> BeregnFradrag(ValueTuple<decimal> amIndkomster, decimal sats, decimal grundbeloeb)
// ReSharper restore MemberCanBeMadeStatic.Global
		{
			return amIndkomster.Map((decimal amIndkomst) => sats * amIndkomst).Loft(grundbeloeb);
		}
	}
}