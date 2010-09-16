using System.Collections.Generic;
using Maxfire.Core.Extensions;
using Maxfire.Skat.Extensions;

namespace Maxfire.Skat.Beregnere
{
	// Fuldt skattepligtige personer her i landet har et personfradrag på 42.900 kr. (2009 og 2010), 
	// jf. PSL § 10, stk. 1. Dog udgør personfradraget et beløb på 32.200 kr. (2009 og 2010) for 
	// personer, som ved indkomstårets udløb ikke er fyldt 18 år og ikke har indgået ægteskab, jf. 
	// PSL 10, stk. 2.
	//
	// Hvis skatteværdien af det statslige personfradrag ikke kan udnyttes ved beregningen af sundhedsbidrag, 
	// bundskat, (mellemskat i 2009), topskat eller skat af aktieindkomst over 48.300 kr (2009 og 2010), 
	// anvendes den ikke udnyttede del til nedsættelse af de øvrige skatter (dvs. de kommunale indkomstskatter 
	// og kirkeskatten, jf. min fortolkning), jf. PSL § 12, stk. 2.
	//
	// Hvis en gift person, der er samlevende med ægtefællen ved indkomstårets udløb, efter modregning af 
	// skatteværdier af personfradragene i egne indkomstskatter har uudnyttede skatteværdier, omregnes disse 
	// til fradragsbeløb og overføres til ægtefællen. Her beregnes skatteværdierne med ægtefællens egne 
	// skatteprocenter, og der foretages en lignende modregning som ovenfor anført. Et herefter uudnyttet 
	// personfradrag kan ikke fremføres til anvendelse i efterfølgende indkomstår.
	//
	// Indkomstskat til staten, dvs. sundhedsbidrag, bundskat, (mellemskat i 2009) og topskat, skal, efter 
	// at skattebeløbene er reguleret med evt. underskud, nedsættes med skatteværdien af personfradrag. 
	//
	// § 9 stk 1.: Bundskat, mellemskat, topskat og sundhedsbidrag og skat af aktieindkomst over 48.300 kr 
	// (2009 og 2010), til staten skal, efter at skattebeløbene er reguleret efter § 13, nedsættes med 
	// skatteværdien af personfradrag.
	//
	// § 12 Stk. 2. Hvis skatteværdien af personfradraget ikke kan udnyttes ved beregningen af en eller 
	// flere af de i § 9 nævnte skatter (dvs. følgende indkomstskatter til staten: bundskat, mellemskat, 
	// topskat, sundhedsbidrag, skat af aktieindkomst, der overstiger 48.300 kr. (2009 og 2010)), anvendes 
	// den ikke udnyttede del til nedsættelse af de øvrige skatter (dvs. kommunale indkomstskatter og kirkeskat).
	//
	// § 10 Stk. 3.: I det omfang en gift person, der er samlevende med ægtefællen ved indkomstårets udløb, 
	// ikke kan udnytte skatteværdien af personfradragene, benyttes den ikke udnyttede del af skatteværdien 
	// til nedsættelse efter § 9 af den anden ægtefælles skatter.
	public class PersonfradragBeregner
	{
		private readonly ISkattelovRegistry _skattelovRegistry;
		private readonly List<SkatteModregner<IndkomstSkatter>> _skatteModregnere;

		public PersonfradragBeregner(ISkattelovRegistry skattelovRegistry)
		{
			_skattelovRegistry = skattelovRegistry;
			_skatteModregnere = new List<SkatteModregner<IndkomstSkatter>>
			{
				// I det omfang skatteværdien af personfradraget mht. sundhedsbidrag ikke kan fradrages i selve 
				// sundhedsbidraget, fragår den i nævnte rækkefølge i følgende skatter: bundskat, mellemskat, 
				// topskat og skat af aktieindkomst, der overstiger 48.300 kr. (2009 og 2010). 
				new SkatteModregner<IndkomstSkatter>(
					Modregning<IndkomstSkatter>.Af(x => x.Sundhedsbidrag),
					Modregning<IndkomstSkatter>.Af(x => x.Bundskat),
					Modregning<IndkomstSkatter>.Af(x => x.Mellemskat),
					Modregning<IndkomstSkatter>.Af(x => x.Topskat),
					Modregning<IndkomstSkatter>.Af(x => x.AktieindkomstskatOverGrundbeloebet),
					Modregning<IndkomstSkatter>.Af(x => x.Kommuneskat),
					Modregning<IndkomstSkatter>.Af(x => x.Kirkeskat)
				),
				// Tilsvarende, hvis skatteværdien af personfradraget mht. bundskat ikke kan fradrages i selve bundskatten, 
				// fragår den i nævnte rækkefølge i følgende skatter: sundhedsbidrag, mellemskat og topskat og skat af 
				// aktieindkomst, der overstiger 48.300 kr. (2009 og 2010). 
				new SkatteModregner<IndkomstSkatter>(
					Modregning<IndkomstSkatter>.Af(x => x.Bundskat),
					Modregning<IndkomstSkatter>.Af(x => x.Sundhedsbidrag),
					Modregning<IndkomstSkatter>.Af(x => x.Mellemskat),
					Modregning<IndkomstSkatter>.Af(x => x.Topskat),
					Modregning<IndkomstSkatter>.Af(x => x.AktieindkomstskatOverGrundbeloebet),
					Modregning<IndkomstSkatter>.Af(x => x.Kommuneskat),
					Modregning<IndkomstSkatter>.Af(x => x.Kirkeskat)
				),
				// Skatteværdi af kommuneskat
				new SkatteModregner<IndkomstSkatter>(
					Modregning<IndkomstSkatter>.Af(x => x.Kommuneskat),
					Modregning<IndkomstSkatter>.Af(x => x.Sundhedsbidrag),
					Modregning<IndkomstSkatter>.Af(x => x.Bundskat),
					Modregning<IndkomstSkatter>.Af(x => x.Mellemskat),
					Modregning<IndkomstSkatter>.Af(x => x.Topskat),
					Modregning<IndkomstSkatter>.Af(x => x.AktieindkomstskatOverGrundbeloebet),
					Modregning<IndkomstSkatter>.Af(x => x.Kirkeskat)
				),
				// Skatteværdi af kirkeskat
				new SkatteModregner<IndkomstSkatter>(
					Modregning<IndkomstSkatter>.Af(x => x.Kirkeskat),
					Modregning<IndkomstSkatter>.Af(x => x.Sundhedsbidrag),
					Modregning<IndkomstSkatter>.Af(x => x.Bundskat),
					Modregning<IndkomstSkatter>.Af(x => x.Mellemskat),
					Modregning<IndkomstSkatter>.Af(x => x.Topskat),
					Modregning<IndkomstSkatter>.Af(x => x.AktieindkomstskatOverGrundbeloebet),
					Modregning<IndkomstSkatter>.Af(x => x.Kommuneskat)
				)
			};
		}

		/// <summary>
		/// De beregnede indkomstskatter til stat, kommune samt kirke nedsættes hver for sig 
		/// med en procentdel af personfradraget.
		/// </summary>
		/// <remarks>
		/// For gifte personer kan uudnyttet personfradrag overføres til den anden ægtefælle.
		/// Et slutteligt ikke udnyttet personfradrag kan ikke overføres til det efterfølgende skatteår.
		/// </remarks>
		public ValueTuple<ModregnSkatterResultEx<IndkomstSkatter>> ModregningAfPersonfradrag(
			IValueTuple<ISkatteyder> skatteydere, 
			ValueTuple<IndkomstSkatter> skatter, 
			IValueTuple<IKommunaleSatser> kommunaleSatser, 
			int skatteAar)
		{
			// Modregning af skatteværdier af personfradraget i egne indkomstskatter.
			var modregnEgneSkatterResults = ModregningAfPersonfradragEgneSkatter(skatteydere, skatter, kommunaleSatser, skatteAar);

			// Modregning af ....
			return ModregningAfOverfoertPersonfradragTilPartner(modregnEgneSkatterResults, skatteydere, kommunaleSatser, skatteAar);
		}

		/// <summary>
		/// Beregn skatter efter modregning af skatteværdier af personfradraget i egne indkomstskatter.
		/// </summary>
		public ValueTuple<ModregnSkatterResultEx<IndkomstSkatter>> ModregningAfPersonfradragEgneSkatter(
			IValueTuple<ISkatteyder> skatteydere, 
			ValueTuple<IndkomstSkatter> skatter, 
			IValueTuple<IKommunaleSatser> kommunaleSatser, 
			int skatteAar)
		{
			var skattevaerdier = BeregnSkattevaerdierAfPersonfradrag(skatteydere, kommunaleSatser, skatteAar);
			return skatter.Map((skat, index) => ModregningAfSkattevaerdier(skat, skattevaerdier[index], skatteydere[index], kommunaleSatser[index], skatteAar));
		}

		/// <summary>
		/// Beregn skatter efter modregning af skatteværdier af personfradraget i egne indkomstskatter.
		/// </summary>
		public ModregnSkatterResultEx<IndkomstSkatter> ModregningAfSkattevaerdier(
			IndkomstSkatter indkomstSkatter, 
			IndkomstSkatter skattevaerdier,
			ISkatteyder skatteyder,
			IKommunaleSatser kommunaleSatser, 
			int skatteAar)
		{
			var modregninger = new IndkomstSkatter();
			
			// Hver af skatteværdierne modregnes i lovens nævnte rækkefølge
			_skatteModregnere.Each(skatteModregner =>
			{
				var accessor = skatteModregner.FirstAccessor();
				decimal vaerdiAfSkat = accessor.GetValue(skattevaerdier);
				IndkomstSkatter modregningerAfSkat = skatteModregner.BeregnModregninger(indkomstSkatter - modregninger, vaerdiAfSkat);
				modregninger += modregningerAfSkat;
			});

			var omregner = new PersonfradragSkattevaerdiOmregner(skatteyder, kommunaleSatser, _skattelovRegistry, skatteAar);

			var skattevaerdi = skattevaerdier.Sum();
			//var fradrag = omregner.BeregnFradragsbeloeb(skattevaerdi);
			//var udnyttetFradrag = omregner.BeregnFradragsbeloeb(modregninger.Sum());

			return new ModregnSkatterResult<IndkomstSkatter>(indkomstSkatter, skattevaerdi, modregninger).ToModregnSkatterResultEx(omregner);
		}

		// Er dette summen eller den ekstra overførsel? Lige nu er det summen
		public ValueTuple<ModregnSkatterResultEx<IndkomstSkatter>> ModregningAfOverfoertPersonfradragTilPartner(
			ValueTuple<ModregnSkatterResultEx<IndkomstSkatter>> modregnEgneSkatterResults,
			IValueTuple<ISkatteyder> skatteydere,
			IValueTuple<IKommunaleSatser> kommunaleSatser, 
			int skatteAar)
		{
			if (modregnEgneSkatterResults.Size == 1)
			{
				return modregnEgneSkatterResults;
			}

			var ikkeUdnyttetFradragEgneSkatter = modregnEgneSkatterResults.Map(x => x.IkkeUdnyttetFradrag);
			int i = ikkeUdnyttetFradragEgneSkatter.IndexOf(skattevaerdi => skattevaerdi > 0);
			if (i == -1 || ikkeUdnyttetFradragEgneSkatter.PartnerOf(i) > 0)
			{
				return modregnEgneSkatterResults;
			}

			// index i har et resterende ikke udnyttet personfradrag og ægtefælle har fuld udnyttelse af sit personfradrag
			var skatter = modregnEgneSkatterResults.Map(x => x.Skatter);

			var skattevaerdiEgneSkatter = modregnEgneSkatterResults.Map(x => x.Skattevaerdi);
			var udnyttedeSkattevaerdierEgneSkatter = modregnEgneSkatterResults.Map(x => x.UdnyttedeSkattevaerdier); // modregninger
			var fradragEgneSkatter = modregnEgneSkatterResults.Map(x => x.Fradrag);
			var udnyttetFradragEgneSkatter = modregnEgneSkatterResults.Map(x => x.UdnyttetFradrag);

			var partner = skatteydere.PartnerOf(i);
			var kommunaleSatserOfPartner = kommunaleSatser.PartnerOf(i);
			var skatterOfPartner = skatter.PartnerOf(i);
			
			// Skatteværdierne af det overførte fradragsbeløb bliver beregnet med ægtefællens egne skatteprocenter.
			var overfoertFradragTilPartner = ikkeUdnyttetFradragEgneSkatter[i];
			var omregner = new PersonfradragSkattevaerdiOmregner(partner, kommunaleSatserOfPartner, _skattelovRegistry, skatteAar);
			var overfoerteSkattevaerdierTilPartner = omregner.BeregnSkattevaerdier(overfoertFradragTilPartner);
			
			// Modregning af skatteværdierne i ægtefællens egne indkomstskatter.
			var modregnPartnersEgneSkatterResult = ModregningAfSkattevaerdier(skatterOfPartner,
																				overfoerteSkattevaerdierTilPartner,
																				partner,
																				kommunaleSatserOfPartner,
																				skatteAar);
			
			// Beregn evt. resterende ikke-udnyttet personfradrag, der 'tabes på gulvet'
			var udnyttedeSkattevaerdierOfPartner = modregnPartnersEgneSkatterResult.UdnyttedeSkattevaerdier;
			var udnyttetFradragOfPartner = modregnPartnersEgneSkatterResult.UdnyttetFradrag;

			// TODO: Refactor this shit
			ModregnSkatterResultEx<IndkomstSkatter> first, second;
			if (i == 0)
			{
				// TODO: Hvad med reduktion af denne
				first = new ModregnSkatterResultEx<IndkomstSkatter>(skatter[0], 
										   skattevaerdiEgneSkatter[0],
				                           udnyttedeSkattevaerdierEgneSkatter[0], 
										   fradragEgneSkatter[0],
				                           udnyttetFradragEgneSkatter[0]);
				second = new ModregnSkatterResultEx<IndkomstSkatter>(skatter[1],
											skattevaerdiEgneSkatter[1],
											udnyttedeSkattevaerdierEgneSkatter[1] + udnyttedeSkattevaerdierOfPartner, 
											fradragEgneSkatter[1] + overfoertFradragTilPartner,
											udnyttetFradragEgneSkatter[1] + udnyttetFradragOfPartner);
			}
			else
			{
				// TODO: Hvad med reduktion af denne
				first = new ModregnSkatterResultEx<IndkomstSkatter>(skatter[0], 
										   skattevaerdiEgneSkatter[0],
										   udnyttedeSkattevaerdierEgneSkatter[0] + udnyttedeSkattevaerdierOfPartner,
										   fradragEgneSkatter[0] + overfoertFradragTilPartner,
										   udnyttetFradragEgneSkatter[0] + udnyttetFradragOfPartner);
				second = new ModregnSkatterResultEx<IndkomstSkatter>(skatter[1],
											skattevaerdiEgneSkatter[1],
											udnyttedeSkattevaerdierEgneSkatter[1], 
											fradragEgneSkatter[1],
											udnyttetFradragEgneSkatter[1]);
			}

			return new ValueTuple<ModregnSkatterResultEx<IndkomstSkatter>>(first, second);
		}

		/// <summary>
		/// Beregn skatteværdier af personfradraget for kommuneskat, kirkeskat, bundskat og sundhedsbidrag.
		/// </summary>
		public ValueTuple<IndkomstSkatter> BeregnSkattevaerdierAfPersonfradrag(
			IValueTuple<ISkatteyder> skatteydere, 
			IValueTuple<IKommunaleSatser> kommunaleSatser, 
			int skatteAar)
		{
			return kommunaleSatser.Map((kommunaleSats, index) => BeregnSkattevaerdierAfPersonfradrag(skatteydere[index], kommunaleSats, skatteAar, skatteydere.Size > 1));
		}

		public ValueTuple<IndkomstSkatter> BeregnSkattevaerdierAfPersonfradrag(
			IValueTuple<ISkatteyder> skatteydere, 
			IValueTuple<IKommunaleSatser> kommunaleSatser, 
			int skatteAar, 
			ValueTuple<decimal> personfradrag)
		{
			return kommunaleSatser.Map((kommunaleSats, index) => BeregnSkattevaerdierAfPersonfradrag(skatteydere[index], kommunaleSats, skatteAar, personfradrag[index]));
		}

		public IndkomstSkatter BeregnSkattevaerdierAfPersonfradrag(
			ISkatteyder skatteyder, 
			IKommunaleSatser kommunaleSatser, 
			int skatteAar, 
			bool gift)
		{
			// TODO: Personfradrag er individuelt bestemt af alder og civilstand
			// Personfradrag opgøres efter § 10 og skatteværdien heraf efter § 12.

			// § 12 stk. 1: Skatteværdien af de i § 10 nævnte personfradrag opgøres som en procentdel af fradragene. 
			// Ved opgørelsen anvendes samme procent som ved beregningen af indkomstskat til kommunen samt kirkeskat. 
			// Ved beregningen af indkomstskat til staten beregnes skatteværdien af personfradraget med beskatnings-
			// procenterne for bundskat efter § 5, nr. 1, og for sundhedsbidrag efter § 5, nr. 4.
			decimal personfradrag = _skattelovRegistry.GetPersonfradrag(skatteAar, skatteyder.GetAlder(skatteAar), gift);
			return BeregnSkattevaerdierAfPersonfradrag(skatteyder, kommunaleSatser, skatteAar, personfradrag);
		}

		public IndkomstSkatter BeregnSkattevaerdierAfPersonfradrag(
			ISkatteyder skatteyder,
			IKommunaleSatser kommunaleSatser, 
			int skatteAar, 
			decimal personfradrag)
		{
			var omregner = new PersonfradragSkattevaerdiOmregner(skatteyder, kommunaleSatser, _skattelovRegistry, skatteAar);
			return omregner.BeregnSkattevaerdier(personfradrag);
		}
	}
}