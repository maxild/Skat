using Maxfire.Skat.Extensions;

namespace Maxfire.Skat.Beregnere
{
	public abstract class PersonligIndkomstSkatteberegner
	{
		/// <summary>
		/// Beregn bundskattegrundlaget under hensyn til evt. modregnet negativ negativ nettokapitalindkomst.
		/// </summary>
		public ValueTuple<decimal> BeregnBruttoGrundlag(IValueTuple<IPersonligeIndkomster> indkomster)
		{
			var personligIndkomst = indkomster.Map(x => x.PersonligIndkomstSkattegrundlag);
			var nettoKapitalIndkomst = indkomster.Map(x => x.NettoKapitalIndkomstSkattegrundlag);

			var nettoKapitalIndkomstEfterModregning = nettoKapitalIndkomst.NedbringPositivtMedEvtNegativt();

			return personligIndkomst + (+nettoKapitalIndkomstEfterModregning);
		}
	}
}