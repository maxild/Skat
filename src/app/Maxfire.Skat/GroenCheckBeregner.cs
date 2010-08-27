using Maxfire.Skat.Extensions;

namespace Maxfire.Skat
{
	// Den "grønne check" er ikke en egentlig check, men et beløb, der bliver indregnet
	// i skatten. Beløbet bliver indregnet første gang på forskudsopgørelsen for 2010 
	// og det bliver udbetalt løbende i 2010 i kraft af et højere månedligt fradrag.
	//
	// Hver voksen (person over 18 år) får 1.300 kr. For voksne med børn (under 18 år) 
	// er der mulighed for en supplerende grøn check på 300 kr. pr. barn, 
	// dog maksimalt to børn pr. hustand. Som udgangspunkt indregnes den supplerende 
	// grønne check for børn i forskudsopgørelsen hos barnets mor, dog med følgende 
	// undtagelser:
	//  - Hos faderen, hvis det kun er ham der har forældremyndigheden over barnet
	//  - Hos faderen, hvis barnet bor hos ham - uafhængigt af, hvem der har forældremyndigheden
	//  - Hos plejeforældre jf. § 78 i lov om social service
	//
	// For personer med høje indkomster sker der en aftrapning af beløbet. Beløbet 
	// aftrappes med 7,5 procent af den del af topskattegrundlaget, der overstiger 
	// 362.800 kr (2010-niveau).
	public class GroenCheckBeregner
	{
		private readonly ISkattelovRegistry _skattelovRegistry;

		public GroenCheckBeregner(ISkattelovRegistry skattelovRegistry)
		{
			_skattelovRegistry = skattelovRegistry;
		}

		/// <summary>
		/// Beregn den skattefrie kompensation, der også kaldes  "grøn check", fremsat i L 198.
		/// </summary>
		public ValueTuple<decimal> BeregnKompensation(
			IValueTuple<IPerson> personer, 
			IValueTuple<IPersonligeBeloeb> indkomster, 
			int skatteAar)
		{
			decimal kompensationPrVoksen = _skattelovRegistry.GetGroenCheckPrVoksen(skatteAar);
			decimal kompensationPrBarn = _skattelovRegistry.GetGroenCheckPrBarn(skatteAar);
			
			// TODO: Maksimalt 2 børn pr husstand eller voksen???? men hvilke børn tæller ikke hos hvilken person/voksen
			var fuldKompensation = kompensationPrVoksen.ToTupleOfSize(personer.Size) 
				+ personer.Map(p => p.AntalBoern * kompensationPrBarn);

			var fuldAftrapning = BeregnAftrapning(indkomster, skatteAar);

			var kompensation = +(fuldKompensation - fuldAftrapning);

			return kompensation;
		}

		public ValueTuple<decimal> BeregnAftrapning(IValueTuple<IPersonligeBeloeb> indkomster, int skatteAar)
		{
			decimal bundfradrag = _skattelovRegistry.GetGroenCheckBundfradrag(skatteAar);
			decimal aftrapningssats = _skattelovRegistry.GetGroenCheckAftrapningssats(skatteAar);
			decimal positivNettoKapitalIndkomstGrundbeloeb 
				= _skattelovRegistry.GetPositivNettoKapitalIndkomstGrundbeloeb(skatteAar);

			var topskatteGrundlag = GetTopskattegrundlag(indkomster, bundfradrag, positivNettoKapitalIndkomstGrundbeloeb);

			var aftrapning = aftrapningssats * topskatteGrundlag;

			return aftrapning;
		}

		protected virtual ValueTuple<decimal> GetTopskattegrundlag(IValueTuple<IPersonligeBeloeb> indkomster,
			decimal topskatBundfradrag, decimal positivNettoKapitalIndkomstGrundbeloeb)
		{
			var topskatBeregner = new TopskatBeregner(_skattelovRegistry);
			return topskatBeregner.BeregnGrundlagForGroenCheck(indkomster, topskatBundfradrag, 
			                                                   positivNettoKapitalIndkomstGrundbeloeb);
		}
	}
}