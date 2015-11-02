using Maxfire.Skat.Extensions;

namespace Maxfire.Skat.Beregnere
{
	////////////////////////////////////////////////////////////////////////////////
	// Man kan kun opnå beskæftigelsesfradrag hvis man betaler AM-bidrag, dvs. hvis
	// ens AM-indkomst er større end nul.
	//
	// Beskæftigelsesfradraget er på 4,25 pct. (2010) af AM-bidragspligtig indkomst 
	// minus eventuelt bidrag og præmier til privattegnede pensionsordninger.
	// Fradraget bliver beregnet automatisk af SKAT og er højst 13.600 kr. (2010)
	////////////////////////////////////////////////////////////////////////////////
	public class BeskaeftigelsesfradragBeregner : IAMIndkomstSkatteberegner
	{
		private readonly ISkattelovRegistry _skattelovRegistry;

		public BeskaeftigelsesfradragBeregner(ISkattelovRegistry skattelovRegistry)
		{
			_skattelovRegistry = skattelovRegistry;
		}

		public ValueTuple<decimal> BeregnFradrag(IValueTuple<IArbejdsmarkedIndkomster> indkomster, int skatteAar)
		{
			dynamic x = GetSatsAndGrundbeloeb(skatteAar);
			return BeregnFradrag(indkomster, x.Sats, x.Grundbeloeb);
		}

		public decimal BeregnFradrag(IArbejdsmarkedIndkomster indkomster, int skatteAar)
		{
			dynamic x = GetSatsAndGrundbeloeb(skatteAar);
			return BeregnFradrag(indkomster, x.Sats, x.Grundbeloeb);
		}

		private object GetSatsAndGrundbeloeb(int skatteAar)
		{
			return new
			{
				Sats = _skattelovRegistry.GetBeskaeftigelsesfradragSats(skatteAar), 
				Grundbeloeb = _skattelovRegistry.GetBeskaeftigelsesfradragGrundbeloeb(skatteAar)
			};
		}

// ReSharper disable MemberCanBeMadeStatic.Global
		public ValueTuple<decimal> BeregnFradrag(
			IValueTuple<IArbejdsmarkedIndkomster> indkomster, 
			decimal sats, 
			decimal grundbeloeb)
// ReSharper restore MemberCanBeMadeStatic.Global
		{
			return indkomster.Map(indkomst => BeregnFradrag(indkomst, sats, grundbeloeb));
		}

// ReSharper disable MemberCanBeMadeStatic.Global
		public decimal BeregnFradrag(IArbejdsmarkedIndkomster indkomster, decimal sats, decimal grundbeloeb)
// ReSharper restore MemberCanBeMadeStatic.Global
		{
			return BeregnFradrag(indkomster.Arbejdsindkomst.NonNegative(), sats, grundbeloeb);
		}

		private static decimal BeregnFradrag(decimal grundlag, decimal sats, decimal grundbeloeb)
		{
			return (sats * grundlag.NonNegative()).Loft(grundbeloeb);
		}

		decimal IAMIndkomstSkatteberegner.Beregn(decimal grundlag, int skatteAar)
		{
			dynamic x = GetSatsAndGrundbeloeb(skatteAar);
			return BeregnFradrag(grundlag, x.Sats, x.Grundbeloeb);
		}

		decimal IAMIndkomstSkatteberegner.Beregn(IArbejdsmarkedIndkomster indkomster, int skatteAar)
		{
			return BeregnFradrag(indkomster, skatteAar);
		}
	}
}