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

		// TODO: Mangler aktieindkomst, restskat, fremf�rte underskud i selvangivne bel�b
		public ValueTuple<IPersonligeBeloeb> BeregnIndkomster(
			IValueTuple<ISelvangivneBeloeb> selvangivneBeloeb, 
			int skatteAar)
		{
			// Beregn de endogene bel�b for AM-bidrag og besk�ftigelsesfradrag
			var amBidragBeregner = new AMBidragBeregner(_skattelovRegistry);
			var amIndkomster = selvangivneBeloeb.Map(x => x.PersonligIndkomstAMIndkomst);
			var amBidrag = amBidragBeregner.BeregnSkat(amIndkomster, skatteAar);
			var beskaeftigelsesfradragBeregner = new BeskaeftigelsesfradragBeregner(_skattelovRegistry);
			var beskaeftigelsesfradrag = beskaeftigelsesfradragBeregner.BeregnFradrag(amIndkomster, skatteAar);
			
			// TODO: make PersonligeBeloeb immutable, men g�r det sidel�bende med et beregnet eksempel med alle slags modregninger af underskud
			return amBidrag.Map(index => 
				(IPersonligeBeloeb)new PersonligeBeloeb(selvangivneBeloeb[index], amBidrag[index], beskaeftigelsesfradrag[index]));
		}
	}
}