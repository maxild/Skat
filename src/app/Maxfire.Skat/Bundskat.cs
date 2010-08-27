using Maxfire.Skat.Extensions;

namespace Maxfire.Skat
{
	///////////////////////////////////////////////////////////////////////////////////////////////////////
	//
	// PSL § 6.
	//
	// Stk. 1. Skatten efter § 5, nr. 1, beregnes med den procent, der anføres i stk. 2, af den personlige
	// indkomst med tillæg af positiv nettokapitalindkomst.
	// (Stk. 2. angiver bundskattesatsen 2010-19)
	// Stk. 3. Hvis en gift person har negativ nettokapitalindkomst, modregnes dette beløb i den anden
	// ægtefælles positive nettokapitalindkomst, inden skatten efter stk. 1 og 2 beregnes. Det er en forudsætning,
	// at ægtefællerne er samlevende ved indkomstårets udløb.
	//
	///////////////////////////////////////////////////////////////////////////////////////////////////////
	public class BundskatBeregner
	{
		private readonly ISkattelovRegistry _skattelovRegistry;

		public BundskatBeregner(ISkattelovRegistry skattelovRegistry)
		{
			_skattelovRegistry = skattelovRegistry;
		}

		public ValueTuple<decimal> BeregnSkat(IValueTuple<IPersonligeBeloeb> indkomster, int skatteAar)
		{
			decimal bundSkattesats = _skattelovRegistry.GetBundSkattesats(skatteAar);
			var grundlag = BeregnBruttoGrundlag(indkomster);
			return bundSkattesats * grundlag;
		}

		/// <summary>
		/// Beregn bundskattegrundlaget under hensyn til evt. modregnet negativ negativ nettokapitalindkomst.
		/// </summary>
// ReSharper disable MemberCanBeMadeStatic.Global
		public ValueTuple<decimal> BeregnBruttoGrundlag(IValueTuple<IPersonligeBeloeb> input)
// ReSharper restore MemberCanBeMadeStatic.Global
		{
			// TODO: Findes også i Mellemskatberegner
			var personligIndkomst = input.Map(x => x.Skattegrundlag.PersonligIndkomst);
			var nettoKapitalIndkomst = input.Map(x => x.Skattegrundlag.NettoKapitalIndkomst);

			var nettoKapitalIndkomstEfterModregning = nettoKapitalIndkomst.NedbringPositivtMedEvtNegativt();

			return personligIndkomst + (+nettoKapitalIndkomstEfterModregning);
		}
	}
}