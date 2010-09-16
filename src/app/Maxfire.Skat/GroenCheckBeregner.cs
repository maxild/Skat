using Maxfire.Skat.Extensions;

namespace Maxfire.Skat
{
	// Den "gr�nne check" er ikke en egentlig check, men et bel�b, der bliver indregnet
	// i skatten. Bel�bet bliver indregnet f�rste gang p� forskudsopg�relsen for 2010 
	// og det bliver udbetalt l�bende i 2010 i kraft af et h�jere m�nedligt fradrag.
	//
	// Hver voksen (person over 18 �r) f�r 1.300 kr. For voksne med b�rn (under 18 �r) 
	// er der mulighed for en supplerende gr�n check p� 300 kr. pr. barn, 
	// dog maksimalt to b�rn pr. hustand. Som udgangspunkt indregnes den supplerende 
	// gr�nne check for b�rn i forskudsopg�relsen hos barnets mor, dog med f�lgende 
	// undtagelser:
	//  - Hos faderen, hvis det kun er ham der har for�ldremyndigheden over barnet
	//  - Hos faderen, hvis barnet bor hos ham - uafh�ngigt af, hvem der har for�ldremyndigheden
	//  - Hos plejefor�ldre jf. � 78 i lov om social service
	//
	// For personer med h�je indkomster sker der en aftrapning af bel�bet. Bel�bet 
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
		/// Beregn den skattefrie kompensation, der ogs� kaldes  "gr�n check", fremsat i L 198.
		/// </summary>
		public ValueTuple<decimal> BeregnKompensation(
			IValueTuple<ISkatteyder> skatteydere, 
			IValueTuple<IPersonligeIndkomster> indkomster, 
			int skatteAar)
		{
			decimal kompensationPrVoksen = _skattelovRegistry.GetGroenCheckPrVoksen(skatteAar);
			decimal kompensationPrBarn = _skattelovRegistry.GetGroenCheckPrBarn(skatteAar);
			
			// TODO: Maksimalt 2 b�rn pr husstand eller voksen???? men hvilke b�rn t�ller ikke hos hvilken person/voksen
			var fuldKompensation = kompensationPrVoksen.ToTupleOfSize(skatteydere.Size) 
				+ skatteydere.Map(p => p.AntalBoern * kompensationPrBarn);

			var fuldAftrapning = BeregnAftrapning(indkomster, skatteAar);

			var kompensation = +(fuldKompensation - fuldAftrapning);

			return kompensation;
		}

		public ValueTuple<decimal> BeregnAftrapning(IValueTuple<IPersonligeIndkomster> indkomster, int skatteAar)
		{
			decimal bundfradrag = _skattelovRegistry.GetGroenCheckBundfradrag(skatteAar);
			decimal aftrapningssats = _skattelovRegistry.GetGroenCheckAftrapningssats(skatteAar);
			decimal positivNettoKapitalIndkomstGrundbeloeb 
				= _skattelovRegistry.GetPositivNettoKapitalIndkomstGrundbeloeb(skatteAar);

			var topskatteGrundlag = GetTopskattegrundlag(indkomster, bundfradrag, positivNettoKapitalIndkomstGrundbeloeb);

			var aftrapning = aftrapningssats * topskatteGrundlag;

			return aftrapning;
		}

		protected virtual ValueTuple<decimal> GetTopskattegrundlag(IValueTuple<IPersonligeIndkomster> indkomster,
			decimal topskatBundfradrag, decimal positivNettoKapitalIndkomstGrundbeloeb)
		{
			var topskatBeregner = new TopskatBeregner(_skattelovRegistry);
			return topskatBeregner.BeregnGrundlagForGroenCheck(indkomster, topskatBundfradrag, 
			                                                   positivNettoKapitalIndkomstGrundbeloeb);
		}
	}
}