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

		//public ValueTuple<decimal> BeregnFradrag(ValueTuple<SelvangivneBeloeb> selvangivneBeloeb, int skatteAar)
		//{
		//    return BeregnFradrag(selvangivneBeloeb.Map(x => x.PersonligIndkomstAMIndkomst), skatteAar);
		//}

		//public ValueTuple<decimal> BeregnFradrag(ValueTuple<PersonligeBeloeb> indkomster, int skatteAar)
		//{
		//    return BeregnFradrag(indkomster.Map(x => x.AMIndkomst), skatteAar);
		//}

		public ValueTuple<decimal> BeregnFradrag(ValueTuple<decimal > amIndkomster, int skatteAar)
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