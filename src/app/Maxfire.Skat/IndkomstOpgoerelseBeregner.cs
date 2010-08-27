using Maxfire.Skat.Extensions;

namespace Maxfire.Skat
{
	// TODO: Benyt denne i eksempler
	public class IndkomstOpgoerelseBeregner
	{
		private readonly ISkattelovRegistry _skattelovRegistry;

		public IndkomstOpgoerelseBeregner(ISkattelovRegistry skattelovRegistry)
		{
			_skattelovRegistry = skattelovRegistry;
		}

		// TODO: Mangler aktieindkomst, restskat, fremførte underskud i selvangivne beløb
		public ValueTuple<IPersonligeBeloeb> BeregnIndkomster(
			IValueTuple<ISelvangivneBeloeb> selvangivneBeloeb, 
			int skatteAar)
		{
			// Beregn de endogene beløb for AM-bidrag og beskæftigelsesfradrag
			var amBidragBeregner = new AMBidragBeregner(_skattelovRegistry);
			var amIndkomster = selvangivneBeloeb.Map(x => x.PersonligIndkomstAMIndkomst);
			var amBidrag = amBidragBeregner.BeregnSkat(amIndkomster, skatteAar);
			var beskaeftigelsesfradragBeregner = new BeskaeftigelsesfradragBeregner(_skattelovRegistry);
			var beskaeftigelsesfradrag = beskaeftigelsesfradragBeregner.BeregnFradrag(amIndkomster, skatteAar);
			
			// TODO: make PersonligeBeloeb immutable, men gør det sideløbende med et beregnet eksempel med alle slags modregninger af underskud
			return amBidrag.Map(index => 
				(IPersonligeBeloeb)new PersonligeBeloeb(selvangivneBeloeb[index], amBidrag[index], beskaeftigelsesfradrag[index]));
		}
	}
}