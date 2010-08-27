using Maxfire.Skat.Extensions;

namespace Maxfire.Skat
{
	public class MellemskatBeregner
	{
		private readonly ISkattelovRegistry _skattelovRegistry;

		public MellemskatBeregner(ISkattelovRegistry skattelovRegistry)
		{
			_skattelovRegistry = skattelovRegistry;
		}

		public ValueTuple<decimal> BeregnSkat(IValueTuple<IPersonligeBeloeb> indkomster, int skatteAar)
		{
			decimal mellemSkattesats = _skattelovRegistry.GetMellemSkattesats(skatteAar);
			decimal mellemskatBundfradrag = _skattelovRegistry.GetMellemskatBundfradrag(skatteAar);
			
			var grundlag = BeregnGrundlag(indkomster, mellemskatBundfradrag);
			return mellemSkattesats * grundlag;
		}

		/// <summary>
		/// Beregn mellemskattegrundlaget før bundfradrag.
		/// </summary>
		// ReSharper disable MemberCanBeMadeStatic.Global
		public ValueTuple<decimal> BeregnBruttoGrundlag(IValueTuple<IPersonligeBeloeb> indkomster)
		// ReSharper restore MemberCanBeMadeStatic.Global
		{
			// TODO: Findes også i BundskatBeregner
			var personligIndkomst = indkomster.Map(x => x.Skattegrundlag.PersonligIndkomst);
			var nettoKapitalIndkomst = indkomster.Map(x => x.Skattegrundlag.NettoKapitalIndkomst);

			var nettoKapitalIndkomstEfterModregning = nettoKapitalIndkomst.NedbringPositivtMedEvtNegativt();

			return (personligIndkomst + (+nettoKapitalIndkomstEfterModregning));
		}

		/// <summary>
		/// Beregn mellemskattegrundlaget efter bundfradrag.
		/// </summary>
		public ValueTuple<decimal> BeregnGrundlag(IValueTuple<IPersonligeBeloeb> indkomster, decimal mellemskatBundfradrag)
		{
			var bruttoGrundlag = BeregnBruttoGrundlag(indkomster);
			var udnyttetBundfradrag = bruttoGrundlag.BeregnSambeskattetBundfradrag(mellemskatBundfradrag);
			return +(bruttoGrundlag - udnyttetBundfradrag);
		}

		/// <summary>
		/// Beregn mellemskattegrundlaget efter bundfradrag.
		/// </summary>
		public ValueTuple<decimal> BeregnGrundlag(IValueTuple<IPersonligeBeloeb> indkomster, int skatteAar)
		{
			decimal mellemskatBundfradrag = _skattelovRegistry.GetMellemskatBundfradrag(skatteAar);
			return BeregnGrundlag(indkomster, mellemskatBundfradrag);
		}

		/// <summary>
		/// Beregn det udnyttede bundfradrag efter at der er sket overførsel af bundfradrag mellem ægtefæller.
		/// </summary>
		public ValueTuple<decimal> BeregnSambeskattetBundfradrag(IValueTuple<IPersonligeBeloeb> indkomster, decimal mellemskatBundfradrag)
		{
			var bruttoGrundlag = BeregnBruttoGrundlag(indkomster);
			return bruttoGrundlag.BeregnSambeskattetBundfradrag(mellemskatBundfradrag);
		}

		/// <summary>
		/// Beregn det udnyttede bundfradrag efter at der er sket overførsel af bundfradrag mellem ægtefæller.
		/// </summary>
		public ValueTuple<decimal> BeregnSambeskattetBundfradrag(IValueTuple<IPersonligeBeloeb> indkomster, int skatteAar)
		{
			decimal mellemskatBundfradrag = _skattelovRegistry.GetMellemskatBundfradrag(skatteAar);
			return BeregnSambeskattetBundfradrag(indkomster, mellemskatBundfradrag);
		}
	}
}