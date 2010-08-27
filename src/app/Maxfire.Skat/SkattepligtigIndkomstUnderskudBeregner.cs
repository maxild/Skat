using System;
using Maxfire.Core.Extensions;
using Maxfire.Skat.Extensions;

namespace Maxfire.Skat
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////
	// � 13. stk. 1: Ugifte
	//
	// Pkt. 1: Hvis den skattepligtige indkomst udviser underskud, beregnes skattev�rdien af underskuddet 
	// med beskatningsprocenten for sundhedsbidrag, jf. � 8, og beskatningsprocenterne for kommunal 
	// indkomstskat og kirkeskat henholdsvis med beskatningsprocenten efter � 8 c. 
	// Pkt. 2: Skattev�rdien af underskuddet modregnes i den n�vnte r�kkef�lge i skatterne efter �� 6, 
	// 6 a og 7 og � 8 a, stk. 2. 
	// Pkt. 3: Et herefter resterende underskud fremf�res til fradrag i den skattepligtige indkomst 
	// for de f�lgende indkomst�r. 
	// Pkt. 4: Fradraget for underskud i skattepligtig indkomst kan kun fremf�res til et senere indkomst�r, 
	// hvis det ikke kan rummes i skattepligtig indkomst eller modregnes med skattev�rdien i skat efter 
	// �� 6, 6 a og 7 og � 8 a, stk. 2, for et tidligere indkomst�r.
	//
	// � 13. stk. 2: Gifte
	//
	// Pkt. 1: Hvis en gift persons skattepligtige indkomst udviser underskud, og �gtef�llerne er samlevende 
	// ved indkomst�rets udl�b, skal underskud, der ikke er modregnet efter stk. 1, 2. pkt., i st�rst 
	// muligt omfang fradrages i den anden �gtef�lles skattepligtige indkomst. 
	// Pkt. 2: Derefter modregnes skattev�rdien af uudnyttet underskud i �gtef�llens beregnede skatter 
	// efter �� 6, 6 a, 7 og 8 a, stk. 2. 
	// Pkt. 3: Modregning sker, f�r �gtef�llens egne uudnyttede underskud fra tidligere indkomst�r fremf�res 
	// efter 4.-6. pkt. 
	// Pkt. 4: Et herefter overskydende bel�b fremf�res til fradrag i f�lgende indkomst�r efter 
	// stk. 1, 4. pkt.
	// Pkt. 5: Hvert �r fradrages underskud f�rst i den skattepligtiges indkomst og i �vrigt efter samme
	// regler, som g�lder for underskuds�ret.
	// Pkt. 6: Hvis �gtef�llen ligeledes har uudnyttet underskud vedr�rende tidligere indkomst�r, skal 
	// �gtef�llens egne underskud modregnes f�rst.
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
		/// Modregner f�rst �rets (eventuelle) underskud i egen og derefter �gtef�lles skattepligtige indkomst og skatter, dern�st bliver
		/// et (eventuelt) fremf�rt underskud modregnet p� samme m�de, og endelig bliver et eventuelt restunderskud fremf�rt til n�ste 
		/// indkomst�r.
		/// </summary>
		/// <param name="indkomster">De indkomster, hvor underskuddet bliver modregnet</param>
		/// <param name="skatter">De skatter, hvor underskud bliver modregnet</param>
		/// <param name="kommunaleSatser">Kommunale skattesatser</param>
		/// <param name="skatteAar">Skatte�ret</param>
		/// <returns>Resultatet</returns>
		public ValueTuple<ModregnUnderskudResult> ModregningAfUnderskud(
			IValueTuple<IPersonligeBeloeb> indkomster, 
			ValueTuple<SkatterAfPersonligIndkomst> skatter, 
			IValueTuple<IKommunaleSatser> kommunaleSatser, 
			int skatteAar)
		{
			var skattepligtigeIndkomster = indkomster.Map(x => x.Skattegrundlag.SkattepligtigIndkomst);
			
			// I. �rets underskud
			var aaretsUnderskud = +(-skattepligtigeIndkomster);

			var modregnAaretsUnderskudResult 
				= modregnEgenOgAegtefaelleUnderskudOgUnderskudsvaerdi(indkomster, skattepligtigeIndkomster,
						skatter, aaretsUnderskud, kommunaleSatser, skatteAar,
							(value, index) => indkomster[index].ModregnetUnderskudSkattepligtigIndkomst -= value);

			
			// II. Fremf�rt underskud
			var fremfoertUnderskud = indkomster.Map(x => x.FremfoertUnderskudSkattepligtigIndkomst);
			var skattepligtigeIndkomsterEfterModregningAfAaretsUnderskud = indkomster.Map(x => x.Skattegrundlag.SkattepligtigIndkomst);
			var modregningSkatter = modregnAaretsUnderskudResult.Map(x => x.ModregningSkatter);
			
			var modregnFremfoertUnderskudResult
				= modregnEgenOgAegtefaelleUnderskudOgUnderskudsvaerdi(indkomster, skattepligtigeIndkomsterEfterModregningAfAaretsUnderskud, 
						skatter - modregningSkatter, fremfoertUnderskud, kommunaleSatser, skatteAar,
							(value, index) => indkomster[index].FremfoertUnderskudSkattepligtigIndkomst -= value);

			return (modregnAaretsUnderskudResult + modregnFremfoertUnderskudResult)
				.ToModregnResult(skatter, aaretsUnderskud + fremfoertUnderskud);
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

		private ValueTuple<BeregnModregningerResult> modregnEgenOgAegtefaelleUnderskudOgUnderskudsvaerdi(
			IValueTuple<IPersonligeBeloeb> indkomster, 
			ValueTuple<decimal> skattepligtigeIndkomster, 
			ValueTuple<SkatterAfPersonligIndkomst> skatter, 
			ValueTuple<decimal> underskud, 
			IValueTuple<IKommunaleSatser> kommunaleSatser,
			int skatteAar,
			Action<decimal, int> nulstilHandler)
		{
			// Modregn i egen skattepligtig indkomst og skatter
			var modregnEgetUnderskudResult = modregnUnderskudOgUnderskudsvaerdi(skattepligtigeIndkomster, skatter, underskud, kommunaleSatser, skatteAar);
			var restunderskud = modregnEgetUnderskudResult.Map((x, index) => x.GetRestunderskud(underskud[index]));

			var modregningIndkomster = modregnEgetUnderskudResult.Map(x => x.ModregningUnderskudSkattepligtigIndkomst);

			// Note: Side-effekter...skal fjernes
			modregningIndkomster.Each((modregningIndkomst, index) =>
			{
				// Modregn i den skattepligtige indkomst
				indkomster[index].ModregnetUnderskudSkattepligtigIndkomst += modregningIndkomst;
			});

			if (skatter.Size == 1)
			{
				// Note: Side-effekter...skal fjernes
				modregningIndkomster.Each((modregningIndkomst, index) =>
				{
					// Nulstil underskuddet
					nulstilHandler(underskud[index], index);
					// Fremf�rsel af resterende underskud
					indkomster[index].UnderskudSkattepligtigIndkomstTilFremfoersel += restunderskud[index];
				});

				return modregnEgetUnderskudResult;
			}

			// Modregn i �gtef�lles skattepligtig indkomst og skatter
			var modregningSkatter = modregnEgetUnderskudResult.Map(x => x.ModregningSkatter);
			var skattepligtigeIndkomsterEfterEgenModregning = indkomster.Map(x => x.Skattegrundlag.SkattepligtigIndkomst);
			var overfoertUnderskud = restunderskud.Swap();
			var modregnOverfoertUnderskudResult = modregnUnderskudOgUnderskudsvaerdi(skattepligtigeIndkomsterEfterEgenModregning,
			                                             skatter - modregningSkatter, overfoertUnderskud, kommunaleSatser, skatteAar);
			var overfoertRestunderskud = modregnOverfoertUnderskudResult.Map((x, index) => x.GetRestunderskud(overfoertUnderskud[index]));
			var tilbagefoertUnderskud = overfoertRestunderskud.Swap();

			var modregningIndkomsterEgetUnderskud = modregnEgetUnderskudResult.Map(x => x.ModregningUnderskudSkattepligtigIndkomst);
			var modregningIndkomsterOverfoertUnderskud = modregnOverfoertUnderskudResult.Map(x => x.ModregningUnderskudSkattepligtigIndkomst);

			// Note: Side-effekter...skal fjernes
			modregningIndkomsterOverfoertUnderskud.Each((modregningIndkomst, index) =>
			{
				// Modregn i den skattepligtige indkomst
				indkomster[index].ModregnetUnderskudSkattepligtigIndkomst += modregningIndkomst;
			});

			// Note: Side-effekter...skal fjernes
			(modregningIndkomsterEgetUnderskud + modregningIndkomsterOverfoertUnderskud).Each((modregningIndkomst, index) =>
			{
				// Nulstil underskuddet
				nulstilHandler(underskud[index], index);
				// Fremf�rsel af resterende underskud
				indkomster[index].UnderskudSkattepligtigIndkomstTilFremfoersel += tilbagefoertUnderskud[index];
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

			// Modregn underskudsv�rdi i skatter
			var modregningSkatterResult = modregnUnderskudsvaerdi(skatter, restunderskud, skattevaerdiOmregnere);
			var modregningSkatter = modregningSkatterResult.Map(x => x.UdnyttedeSkattevaerdier);
			restunderskud = modregningSkatterResult.Map(x => x.IkkeUdnyttetFradrag);
			
			return skattepligtigeIndkomster.Map(index => 
				new BeregnModregningerResult(modregningSkattepligtigIndkomst[index], modregningSkatter[index], 
					underskud[index] - restunderskud[index]));
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