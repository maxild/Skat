using System;
using Maxfire.Core.Extensions;
using Maxfire.Skat.Extensions;

namespace Maxfire.Skat.Beregnere
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////
	// § 13. stk. 1: Ugifte
	//
	// Pkt. 1: Hvis den skattepligtige indkomst udviser underskud, beregnes skatteværdien af underskuddet 
	// med beskatningsprocenten for sundhedsbidrag, jf. § 8, og beskatningsprocenterne for kommunal 
	// indkomstskat og kirkeskat henholdsvis med beskatningsprocenten efter § 8 c. 
	// Pkt. 2: Skatteværdien af underskuddet modregnes i den nævnte rækkefølge i skatterne efter §§ 6, 
	// 6 a og 7 og § 8 a, stk. 2. 
	// Pkt. 3: Et herefter resterende underskud fremføres til fradrag i den skattepligtige indkomst 
	// for de følgende indkomstår. 
	// Pkt. 4: Fradraget for underskud i skattepligtig indkomst kan kun fremføres til et senere indkomstår, 
	// hvis det ikke kan rummes i skattepligtig indkomst eller modregnes med skatteværdien i skat efter 
	// §§ 6, 6 a og 7 og § 8 a, stk. 2, for et tidligere indkomstår.
	//
	// § 13. stk. 2: Gifte
	//
	// Pkt. 1: Hvis en gift persons skattepligtige indkomst udviser underskud, og ægtefællerne er samlevende 
	// ved indkomstårets udløb, skal underskud, der ikke er modregnet efter stk. 1, 2. pkt., i størst 
	// muligt omfang fradrages i den anden ægtefælles skattepligtige indkomst. 
	// Pkt. 2: Derefter modregnes skatteværdien af uudnyttet underskud i ægtefællens beregnede skatter 
	// efter §§ 6, 6 a, 7 og 8 a, stk. 2. 
	// Pkt. 3: Modregning sker, før ægtefællens egne uudnyttede underskud fra tidligere indkomstår fremføres 
	// efter 4.-6. pkt. 
	// Pkt. 4: Et herefter overskydende beløb fremføres til fradrag i følgende indkomstår efter 
	// stk. 1, 4. pkt.
	// Pkt. 5: Hvert år fradrages underskud først i den skattepligtiges indkomst og i øvrigt efter samme
	// regler, som gælder for underskudsåret.
	// Pkt. 6: Hvis ægtefællen ligeledes har uudnyttet underskud vedrørende tidligere indkomstår, skal 
	// ægtefællens egne underskud modregnes først.
	//
	//////////////////////////////////////////////////////////////////////////////////////////////////////
	public class SkattepligtigIndkomstUnderskudBeregner
	{
		private readonly ISkattelovRegistry _skattelovRegistry;

		public SkattepligtigIndkomstUnderskudBeregner(ISkattelovRegistry skattelovRegistry)
		{
			_skattelovRegistry = skattelovRegistry;
		}

		/// <summary>
		/// Modregner først årets (eventuelle) underskud i egen og derefter ægtefælles skattepligtige indkomst og skatter, dernæst bliver
		/// et (eventuelt) fremført underskud modregnet på samme måde, og endelig bliver et eventuelt restunderskud fremført til næste 
		/// indkomstår.
		/// </summary>
		/// <param name="indkomster">De indkomster, hvor underskuddet bliver modregnet</param>
		/// <param name="skatter">De skatter, hvor underskud bliver modregnet</param>
		/// <param name="kommunaleSatser">Kommunale skattesatser</param>
		/// <param name="skatteAar">Skatteåret</param>
		/// <returns>Resultatet</returns>
		public ValueTuple<ModregnUnderskudResult> ModregningAfUnderskud(
			IValueTuple<ISkattepligtigeIndkomsterModregning> indkomster, 
			ValueTuple<SkatterAfPersonligIndkomst> skatter, 
			IValueTuple<IKommunaleSatser> kommunaleSatser, 
			int skatteAar)
		{
			// I. Årets underskud
			var aaretsUnderskud = indkomster.Map(x => x.SkattepligtigIndkomstAaretsUnderskud);

			Action<string, decimal, int> aaretsUnderskudNulstilHandler = 
				(text, value, index) => indkomster[index].NedbringUnderskudForSkattepligtigIndkomst(
					string.Format("Overførsel af årets underskud til {0} skattepligtige indkomst og skatter", text), value);

			var modregnAaretsUnderskudResult 
				= modregnEgenOgAegtefaelleUnderskudOgUnderskudsvaerdi(indkomster, 
						skatter, aaretsUnderskud, kommunaleSatser, skatteAar,
							aaretsUnderskudNulstilHandler);

			// II. Fremført underskud
			var fremfoertUnderskud = indkomster.Map(x => x.SkattepligtigIndkomstFremfoertUnderskud);
			var modregningSkatter = modregnAaretsUnderskudResult.Map(x => x.ModregningSkatter);

			Action<string, decimal, int> fremfoertUnderskudNulstilHandler = 
				(text, value, index) => indkomster[index].NedbringFremfoertUnderskudForSkattepligtigIndkomst(
					string.Format("Overførsel af fremført underskud til {0} skattepligtige indkomst og skatter", text), value);

			var modregnFremfoertUnderskudResult
				= modregnEgenOgAegtefaelleUnderskudOgUnderskudsvaerdi(indkomster,
						skatter - modregningSkatter, fremfoertUnderskud, kommunaleSatser, skatteAar,
							fremfoertUnderskudNulstilHandler);

			return (modregnAaretsUnderskudResult + modregnFremfoertUnderskudResult)
				.ToModregnResult(skatter, aaretsUnderskud + fremfoertUnderskud);
		}

		private ValueTuple<BeregnModregningerResult> modregnEgenOgAegtefaelleUnderskudOgUnderskudsvaerdi(
			IValueTuple<ISkattepligtigeIndkomsterModregning> indkomster, 
			ValueTuple<SkatterAfPersonligIndkomst> skatter, 
			ValueTuple<decimal> underskud, 
			IValueTuple<IKommunaleSatser> kommunaleSatser,
			int skatteAar,
			Action<string, decimal, int> nulstilHandler)
		{
			var skattepligtigeIndkomster = indkomster.Map(x => x.SkattepligtigIndkomstSkattegrundlag);

			// Modregn i egen skattepligtig indkomst og skatter
			var modregnEgetUnderskudResult = modregnUnderskudOgUnderskudsvaerdi(skattepligtigeIndkomster, skatter, underskud, kommunaleSatser, skatteAar);

			var restunderskud = modregnEgetUnderskudResult.Map((x, index) => x.GetRestunderskud(underskud[index]));
			var modregningIndkomster = modregnEgetUnderskudResult.Map(x => x.ModregningUnderskudSkattepligtigIndkomst);

			modregningIndkomster.Each((modregningIndkomst, index) => 
				indkomster[index].NedbringSkattepligtigIndkomst("", modregningIndkomst));

			if (skatter.Size == 1)
			{
				modregningIndkomster.Each((modregningIndkomst, index) =>
				{
					// Nulstil underskuddet
					nulstilHandler("egen", underskud[index], index);
					// Fremførsel af resterende underskud
					indkomster[index].FremfoerUnderskudForSkattepligtigIndkomst("", restunderskud[index]);
				});

				return modregnEgetUnderskudResult;
			}

			// Modregn i ægtefælles skattepligtig indkomst og skatter
			var modregningSkatter = modregnEgetUnderskudResult.Map(x => x.ModregningSkatter);
			var skattepligtigeIndkomsterEfterEgenModregning = indkomster.Map(x => x.SkattepligtigIndkomstSkattegrundlag);
			var overfoertUnderskud = restunderskud.Swap();

			var modregnOverfoertUnderskudResult = modregnUnderskudOgUnderskudsvaerdi(skattepligtigeIndkomsterEfterEgenModregning,
			                                             skatter - modregningSkatter, overfoertUnderskud, kommunaleSatser, skatteAar);

			var overfoertRestunderskud = modregnOverfoertUnderskudResult.Map((x, index) => x.GetRestunderskud(overfoertUnderskud[index]));
			var tilbagefoertUnderskud = overfoertRestunderskud.Swap();

			var modregningIndkomsterEgetUnderskud = modregnEgetUnderskudResult.Map(x => x.ModregningUnderskudSkattepligtigIndkomst);
			var modregningIndkomsterOverfoertUnderskud = modregnOverfoertUnderskudResult.Map(x => x.ModregningUnderskudSkattepligtigIndkomst);

			modregningIndkomsterOverfoertUnderskud.Each((modregningIndkomst, index) => 
				indkomster[index].NedbringSkattepligtigIndkomst("", modregningIndkomst));

			(modregningIndkomsterEgetUnderskud + modregningIndkomsterOverfoertUnderskud).Each((modregningIndkomst, index) =>
			{
				nulstilHandler("ægtefælles", underskud[index], index);
				indkomster[index].FremfoerUnderskudForSkattepligtigIndkomst("", tilbagefoertUnderskud[index]);
			});

			return modregnEgetUnderskudResult + modregnOverfoertUnderskudResult.SwapUnderskud();
		}

		private ValueTuple<BeregnModregningerResult> modregnUnderskudOgUnderskudsvaerdi(
			ValueTuple<decimal> skattepligtigeIndkomster, 
			ValueTuple<SkatterAfPersonligIndkomst> skatter, 
			ValueTuple<decimal> underskud, 
			IValueTuple<IKommunaleSatser> kommunaleSatser, 
			int skatteAar) 
		{
			var skattevaerdiOmregnere = getUnderskudSkattevaerdiBeregnere(kommunaleSatser, skatteAar);

			// Modregn underskud i positiv skattepligtige indkomst
			var modregnIndkomstResults = skattepligtigeIndkomster.ModregnUnderskud(underskud);
			var modregningSkattepligtigIndkomst = modregnIndkomstResults.Map(x => x.UdnyttetUnderskud);
			var restunderskud = modregnIndkomstResults.Map(x => x.IkkeUdnyttetUnderskud);

			// Modregn underskudsværdi i skatter
			var modregningSkatterResult = modregnUnderskudsvaerdi(skatter, restunderskud, skattevaerdiOmregnere);
			var modregningSkatter = modregningSkatterResult.Map(x => x.UdnyttedeSkattevaerdier);
			restunderskud = modregningSkatterResult.Map(x => x.IkkeUdnyttetFradrag);
			
			return restunderskud.MapByIndex(index => 
				new BeregnModregningerResult(modregningSkattepligtigIndkomst[index], modregningSkatter[index], 
					underskud[index] - restunderskud[index]));
		}

		private static SkatteModregner<SkatterAfPersonligIndkomst> getSkattepligtigIndkomstUnderskudModregner()
		{
			return new SkatteModregner<SkatterAfPersonligIndkomst>(
				Modregning<SkatterAfPersonligIndkomst>.Af(x => x.Bundskat),
				Modregning<SkatterAfPersonligIndkomst>.Af(x => x.Mellemskat),
				Modregning<SkatterAfPersonligIndkomst>.Af(x => x.Topskat),
				Modregning<SkatterAfPersonligIndkomst>.Af(x => x.AktieindkomstskatOverGrundbeloebet)
			);
		}

		private static ValueTuple<ModregnSkatterResultEx<SkatterAfPersonligIndkomst>> modregnUnderskudsvaerdi(
			ValueTuple<SkatterAfPersonligIndkomst> skatter,
			ValueTuple<decimal> underskud,
			IValueTuple<SkattevaerdiOmregner> skattevaerdiOmregnere)
		{
			var skatteModregner = getSkattepligtigIndkomstUnderskudModregner();
			var underskudsvaerdier = underskud.Map((beloeb, index) => skattevaerdiOmregnere[index].BeregnSkattevaerdi(beloeb));
			var modregnResult = skatteModregner.Modregn(skatter, underskudsvaerdier);
			return modregnResult.ToModregnSkatterResultEx(skattevaerdiOmregnere);
		}

		private ValueTuple<SkattevaerdiOmregner> getUnderskudSkattevaerdiBeregnere(IValueTuple<IKommunaleSatser> kommunaleSatser, int skatteAar)
		{
			return kommunaleSatser.Map(satser => getUnderskudSkattevaerdiBeregner(satser, skatteAar));
		}

		private SkattevaerdiOmregner getUnderskudSkattevaerdiBeregner(IKommunaleSatser kommunaleSatser, int skatteAar)
		{
			return new SkattevaerdiOmregner(kommunaleSatser.GetKommuneOgKirkeskattesats() + _skattelovRegistry.GetSundhedsbidragSkattesats(skatteAar));
		}
	}
}