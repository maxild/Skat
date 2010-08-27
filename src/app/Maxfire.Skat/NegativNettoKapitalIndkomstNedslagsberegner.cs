using Maxfire.Skat.Extensions;

namespace Maxfire.Skat
{
	// Note: Skal ses i sammenhæng med udfasningen af sundhedsbidraget (sammenlægning med bundskatten) 
	// Note: og den deraf forringelse af værdien af rentefradraget
	// Note: Det vil sige at renteudgifter på op til 50.000 (100.000 for ægtepar) ikke berøreres af nedsættelsen af værdien af rentefradraget
	///////////////////////////////////////////////////////////////////////////////////////////
	// PSL § 11: 
	//
	// Stk. 1. For den del af den skattepligtiges negative nettokapitalindkomst, der ikke 
	// overstiger et beløb på 50.000 kr., beregnes et nedslag i skatten med den procent, 
	// der anføres i stk. 2. Nedslaget modregnes i skatterne efter §§ 6, 7, 8 og 8 a, stk. 2,
	// indkomstskat til kommunen og kirkeskat i den nævnte rækkefølge.
	// 
	// (Stk 2 viser blot nedslagsprocenten fra 2012-2019)
	//
	// Stk. 3. For en gift person modregnes den anden ægtefælles positive nettokapitalindkomst 
	// ved opgørelsen af negativ nettokapitalindkomst efter stk. 1, hvis ægtefællerne er 
	// samlevende ved indkomstårets udløb.
	//
	// Stk. 4. Hvis en gift person ikke kan udnytte nedslaget beregnet efter stk. 1 og 2, 
	// modregnes den ikke udnyttede del i den anden ægtefælles skatter, hvis ægtefællerne 
	// er samlevende ved indkomstårets udløb.
	//
	// TODO: (Fra mail): Med hensyn til skattenedslaget for negativ nettokapitalindkomst under 50.000 
	// henholdsvis 100.000 kr. for ægtepar, sker beregningen ligeledes for ægtefællernes 
	// negative nettokapitalindkomst under ét. I dit eksempel vil der blive beregnet nedslag 
	// af 100.000 kr., og nedslaget vil efterfølgende blive delt mellem ægtefællerne i forhold 
	// til deres andel af den samlede negative nettokapitalindkomst.
	//
	///////////////////////////////////////////////////////////////////////////////////////////
	public class NegativNettoKapitalIndkomstNedslagBeregner
	{
		private readonly ISkattelovRegistry _skattelovRegistry;

		public NegativNettoKapitalIndkomstNedslagBeregner(ISkattelovRegistry skattelovRegistry)
		{
			_skattelovRegistry = skattelovRegistry;
		}

		public ValueTuple<ModregnSkatterResult<Skatter>> ModregnMedNedslag(
			IValueTuple<IPersonligeBeloeb> indkomster, 
			ValueTuple<Skatter> skatter, 
			int skatteAar)
		{
			var nedslag = BeregnNedslag(indkomster, skatteAar);
			return ModregnMedNedslag(skatter, nedslag);
		}

		public ValueTuple<decimal> BeregnNedslag(IValueTuple<IPersonligeBeloeb> indkomster, int skatteAar)
		{
			var nettoKapitalIndkomst = indkomster.Map(x => x.Skattegrundlag.NettoKapitalIndkomst);
			var nettoKapitalIndkomstEfterModregning = nettoKapitalIndkomst.NedbringPositivtMedEvtNegativt();
			var grundbeloeb = _skattelovRegistry.GetNegativNettoKapitalIndkomstGrundbeloeb(skatteAar);
			var negativNettoKapitalIndkomstEfterModregningDerIkkeOverstigerGrundbeloeb
				= (+(-nettoKapitalIndkomstEfterModregning)).Loft(grundbeloeb);

			var sats = _skattelovRegistry.GetNegativNettoKapitalIndkomstSats(skatteAar);
			var nedslag = sats * negativNettoKapitalIndkomstEfterModregningDerIkkeOverstigerGrundbeloeb;

			return nedslag;
		}

// ReSharper disable MemberCanBeMadeStatic.Global
		public ValueTuple<ModregnSkatterResult<Skatter>> ModregnMedNedslag(
			ValueTuple<Skatter> skatter, 
			ValueTuple<decimal> nedslag)
// ReSharper restore MemberCanBeMadeStatic.Global
		{
			var skatteModregner = getSkatteModregner();

			// Modregn nedslag i egne skatter
			var modregningerFraEgetNedslag = skatteModregner.BeregnModregninger(skatter, nedslag);

			if (skatter.Size == 1)
			{
				return new ModregnSkatterResult<Skatter>(skatter[0], nedslag[0], modregningerFraEgetNedslag[0]).ToTuple();
			}

			// Modregn evt. uudnyttet nedslag i ægtefælles skatter
			var udnyttetNedslag = modregningerFraEgetNedslag.Map(x => x.Sum());
			var ikkeUdnyttetNedslag = nedslag - udnyttetNedslag;
			var overfoertNedslag = ikkeUdnyttetNedslag.Swap();

			var modregningerFraOverfoertNedslag
				= skatteModregner.BeregnModregninger(skatter - modregningerFraEgetNedslag, overfoertNedslag);
			var udnyttetOverfortNedslag = modregningerFraOverfoertNedslag.Map(x => x.Sum());

			// Denne tuple repræsenter overførslen af udnyttet nedslag (skatteværdi) mellem ægtefællerne...
			var overfortUdnyttetNedslag = udnyttetOverfortNedslag - udnyttetOverfortNedslag.Swap();

			//...sådan at IkkeUdnyttetSkattevaerdi er korrekt på den returnerede værdi
			return skatter.Map((skat, index) =>
				new ModregnSkatterResult<Skatter>(skat, nedslag[index] + overfortUdnyttetNedslag[index],
					modregningerFraEgetNedslag[index] + modregningerFraOverfoertNedslag[index]));
		}

		private static SkatteModregner<Skatter> getSkatteModregner()
		{
			return new SkatteModregner<Skatter>(
				Modregning<Skatter>.Af(x => x.Bundskat),
				Modregning<Skatter>.Af(x => x.Topskat),
				Modregning<Skatter>.Af(x => x.Sundhedsbidrag),
				Modregning<Skatter>.Af(x => x.AktieindkomstskatOverGrundbeloebet),
				Modregning<Skatter>.Af(x => x.Kommuneskat),
				Modregning<Skatter>.Af(x => x.Kirkeskat)
			);
		}
	}
}