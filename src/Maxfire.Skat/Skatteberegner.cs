using Maxfire.Skat.Beregnere;
using Maxfire.Skat.Extensions;
using Maxfire.Skat.Internal;

namespace Maxfire.Skat
{
    public class Skatteberegner
    {
        private readonly ISkattelovRegistry _skattelovRegistry;

        public Skatteberegner(ISkattelovRegistry skattelovRegistry)
        {
            _skattelovRegistry = skattelovRegistry;
        }

        public ValueTuple<SkatteberegningResult> Beregn(
            IValueTuple<ISkatteyder> skatteydere,
            IValueTuple<ISelvangivneBeloeb> selvangivneBeloeb,
            IValueTuple<IKommunaleSatser> kommunaleSatser,
            int skatteAar
            )
        {
            ValueTuple<ISpecficeredeKommunaleSatser> specificeredeKommunaleSatser =
                kommunaleSatser.Map(MakeSpecificerede).ToValueTuple();
            ValueTuple<ISpecificeredeSkatteyder> specificeredeSkatteydere =
                skatteydere.Map(
                    (skatteyder, index) =>
                        MakeSpecificerede(skatteyder, specificeredeKommunaleSatser[index],
                            gift: skatteydere.Size > 1)).ToValueTuple();

            var indkomstOpgoerelseBeregner = new IndkomstOpgoerelseBeregner(_skattelovRegistry);
            var indkomster = indkomstOpgoerelseBeregner.BeregnIndkomster(selvangivneBeloeb, skatteAar);

            //
            // TODO: Ejendomsværdiskat (springes over, da den kan insættes hvorsomhelst)
            //

            // Modregning af negativ personlig indkomst
            var personligIndkomstUnderskudBeregner = new PersonligIndkomstUnderskudBeregner();
            personligIndkomstUnderskudBeregner.ModregningAfUnderskud(indkomster);

            // Beregn bundskat, mellemskat og topskat samt aktieskat
            var skatterAfPersonligIndkomstBeregner = new SkatterAfPersonligIndkomstBeregner(_skattelovRegistry);
            var skatterAfPersonligIndkomstResults = skatterAfPersonligIndkomstBeregner.BeregnSkatResult(indkomster,
                kommunaleSatser, skatteAar);
            var skatterAfPersonligIndkomst = skatterAfPersonligIndkomstResults.Map(x => x.SkatterAfPersonligIndkomst).ToValueTuple();
            var topskatResults = skatterAfPersonligIndkomstResults.Map(x => x.TopskatResult).ToValueTuple();

            // Modregning af negativ skattepligtig indkomst
            var skattepligtigIndkomstUnderskudBeregner = new SkattepligtigIndkomstUnderskudBeregner(_skattelovRegistry);
            var modregnResults = skattepligtigIndkomstUnderskudBeregner.ModregningAfUnderskud(indkomster, skatterAfPersonligIndkomst, kommunaleSatser, skatteAar);
            var skatterAfPersonligIndkomstEfterModregningAfUnderskud = modregnResults.Map(x => x.ModregnedeSkatter).ToValueTuple();
            var underskudSkattepligtigIndkomst = -modregnResults.Map(x => x.ModregningSkatter).ToValueTuple();

            // Beregn sundhedsbidrag samt kommuneskat og kirkeskat
            var skatterAfSkattepligtigIndkomstBeregner = new SkatterAfSkattepligtigIndkomstBeregner(_skattelovRegistry);
            var skatterAfSkattepligtigIndkomst = skatterAfSkattepligtigIndkomstBeregner.BeregnSkat(specificeredeSkatteydere, indkomster, kommunaleSatser, skatteAar);

            var skatterFoerNedslag = SkatteUtility.CombineSkat(skatterAfPersonligIndkomst, skatterAfSkattepligtigIndkomst);
            var skatterEfterModregningAfUnderskud = SkatteUtility.CombineSkat(skatterAfPersonligIndkomstEfterModregningAfUnderskud,
                                                                    skatterAfSkattepligtigIndkomst);


            // Nedbring skatter med værdien af personfradraget
            var personfradragBeregner = new PersonfradragBeregner(_skattelovRegistry);
            var modregnPersonfradragResults = personfradragBeregner.ModregningAfPersonfradrag(
                                specificeredeSkatteydere, skatterEfterModregningAfUnderskud, kommunaleSatser, skatteAar);
            var personfradrag = -modregnPersonfradragResults.Map(x => x.UdnyttedeSkattevaerdier).ToValueTuple();

            // Nedbring topskat med skatteloft
            var skatteloftNedslag = -topskatResults.Map(x => x.SkatteloftNedslag).ToValueTuple();

            //
            // 9: Bestem nedslaget i (forårspakke 2.0)
            //

            //
            // 10: Beregn kompensation (forårspakke 2.0) for skatteår 2012, 2013, etc.
            //

            //
            // 11: Beregn grøn check
            //

            var indkomstSkatter = skatterFoerNedslag.Map((skatter, index) =>
                new SpecificeredeIndkomstSkatter(skatter, skatteloftNedslag[index],
                    underskudSkattepligtigIndkomst[index], personfradrag[index])).ToValueTuple();

            return specificeredeSkatteydere.Map((skatteyder, index) =>
                new SkatteberegningResult(skatteAar, skatteyder, indkomster[index], indkomstSkatter[index])).ToValueTuple();
        }

        private static ISpecficeredeKommunaleSatser MakeSpecificerede(IKommunaleSatser kommunaleSatser)
        {
            var specficeredeKommunaleSatser = kommunaleSatser as ISpecficeredeKommunaleSatser;
            return specficeredeKommunaleSatser ?? new DefaultSpecificeredeKommunaleSatser(kommunaleSatser, "Ukendt");
        }

        private static ISpecificeredeSkatteyder MakeSpecificerede(ISkatteyder skatteyder, ISpecficeredeKommunaleSatser kommunaleSatser, bool gift)
        {
            return new DefaultSpecificeredeSkatteyder(skatteyder, kommunaleSatser, gift);
        }
    }
}
