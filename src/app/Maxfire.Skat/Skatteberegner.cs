using Maxfire.Skat.Extensions;

namespace Maxfire.Skat
{
	public class Skatteberegner
	{
		private readonly ISkattelovRegistry _skattelovRegistry;

		public Skatteberegner(ISkattelovRegistry skattelovRegistry)
		{
			_skattelovRegistry = skattelovRegistry;
		}

		public SkatteberegningResult Beregn(
			IValueTuple<IPerson> personer, 
			IValueTuple<ISelvangivneBeloeb> selvangivneBeloeb,
			IValueTuple<IKommunaleSatser> kommunaleSatser,
			int skatteAar
			)
		{
			var indkomstOpgoerelseBeregner = new IndkomstOpgoerelseBeregner(_skattelovRegistry);
			ValueTuple<IPersonligeBeloeb> indkomster = indkomstOpgoerelseBeregner.BeregnIndkomster(selvangivneBeloeb, skatteAar);

			//
			// TODO: Ejendomsv�rdiskat (springes over, da den kan ins�ttes hvorsomhelst)
			// 

			// Modregning af negativ personlig indkomst
			var personligIndkomstUnderskudBeregner = new PersonligIndkomstUnderskudBeregner();
			personligIndkomstUnderskudBeregner.ModregningAfUnderskud(indkomster);

			// Beregn bundskat, mellemskat og topskat samt aktieskat
			var skatterAfPersonligIndkomstBeregner = new SkatterAfPersonligIndkomstBeregner(_skattelovRegistry);
			var skatterAfPersonligIndkomst = skatterAfPersonligIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser, skatteAar);

			// Modregning af negativ skattepligtig indkomst
			var skattepligtigIndkomstUnderskudBeregner = new SkattepligtigIndkomstUnderskudBeregner(_skattelovRegistry);
			var modregnResults = skattepligtigIndkomstUnderskudBeregner.ModregningAfUnderskud(indkomster, skatterAfPersonligIndkomst, kommunaleSatser, skatteAar);
			var skatterAfPersonligIndkomstEfterModregningAfUnderskud = modregnResults.Map(x => x.ModregnedeSkatter);

			// Beregn sundhedsbidrag samt kommuneskat og kirkeskat
			var skatterAfSkattepligtigIndkomstBeregner = new SkatterAfSkattepligtigIndkomstBeregner(_skattelovRegistry);
			var skatterAfSkattepligtigIndkomst = skatterAfSkattepligtigIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser, skatteAar);

			var skatterFoerPersonfradrag = SkatteUtility.CombineSkat(skatterAfPersonligIndkomstEfterModregningAfUnderskud,
																	skatterAfSkattepligtigIndkomst);
			

			// Nedbring skatter med v�rdien af personfradraget
			var personfradragBeregner = new PersonfradragBeregner(_skattelovRegistry);
			var modregnPersonfradragResults = personfradragBeregner.ModregningAfPersonfradrag(personer, skatterFoerPersonfradrag, kommunaleSatser, skatteAar);
			var skatterEfterPersonfradrag = modregnPersonfradragResults.Map(x => x.ModregnedeSkatter);

			//
			// 9: Bestem nedslaget i (for�rspakke 2.0)
			//

			//
			// 10: Beregn kompensation (for�rspakke 2.0) for skatte�r 2012, 2013, etc.
			//

			//
			// 11: Beregn gr�n check
			//

			return new SkatteberegningResult(indkomster, skatterEfterPersonfradrag);
		}
	}
}