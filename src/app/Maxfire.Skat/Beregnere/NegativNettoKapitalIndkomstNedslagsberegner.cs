using Maxfire.Skat.Extensions;

namespace Maxfire.Skat.Beregnere
{
	// Note: Skal ses i sammenh�ng med udfasningen af sundhedsbidraget (sammenl�gning med bundskatten) 
	// Note: og den deraf forringelse af v�rdien af rentefradraget
	// Note: Det vil sige at renteudgifter p� op til 50.000 (100.000 for �gtepar) ikke ber�reres af neds�ttelsen af v�rdien af rentefradraget
	///////////////////////////////////////////////////////////////////////////////////////////
	// PSL � 11: 
	//
	// Stk. 1. For den del af den skattepligtiges negative nettokapitalindkomst, der ikke 
	// overstiger et bel�b p� 50.000 kr., beregnes et nedslag i skatten med den procent, 
	// der anf�res i stk. 2. Nedslaget modregnes i skatterne efter �� 6, 7, 8 og 8 a, stk. 2,
	// indkomstskat til kommunen og kirkeskat i den n�vnte r�kkef�lge.
	// 
	// (Stk 2 viser blot nedslagsprocenten fra 2012-2019)
	//
	// Stk. 3. For en gift person modregnes den anden �gtef�lles positive nettokapitalindkomst 
	// ved opg�relsen af negativ nettokapitalindkomst efter stk. 1, hvis �gtef�llerne er 
	// samlevende ved indkomst�rets udl�b.
	//
	// Stk. 4. Hvis en gift person ikke kan udnytte nedslaget beregnet efter stk. 1 og 2, 
	// modregnes den ikke udnyttede del i den anden �gtef�lles skatter, hvis �gtef�llerne 
	// er samlevende ved indkomst�rets udl�b.
	//
	// TODO: (Fra mail): Med hensyn til skattenedslaget for negativ nettokapitalindkomst under 50.000 
	// henholdsvis 100.000 kr. for �gtepar, sker beregningen ligeledes for �gtef�llernes 
	// negative nettokapitalindkomst under �t. I dit eksempel vil der blive beregnet nedslag 
	// af 100.000 kr., og nedslaget vil efterf�lgende blive delt mellem �gtef�llerne i forhold 
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

		public ValueTuple<ModregnSkatterResult<IndkomstSkatter>> ModregnMedNedslag(
			IValueTuple<IPersonligeIndkomster> indkomster, 
			ValueTuple<IndkomstSkatter> skatter, 
			int skatteAar)
		{
			var nedslag = BeregnNedslag(indkomster, skatteAar);
			return ModregnMedNedslag(skatter, nedslag);
		}

		public ValueTuple<decimal> BeregnNedslag(IValueTuple<IPersonligeIndkomster> indkomster, int skatteAar)
		{
			var nettoKapitalIndkomst = indkomster.Map(x => x.NettoKapitalIndkomstSkattegrundlag);
			var nettoKapitalIndkomstEfterModregning = nettoKapitalIndkomst.NedbringPositivtMedEvtNegativt();
			var grundbeloeb = _skattelovRegistry.GetNegativNettoKapitalIndkomstGrundbeloeb(skatteAar);
			var negativNettoKapitalIndkomstEfterModregningDerIkkeOverstigerGrundbeloeb
				= (+(-nettoKapitalIndkomstEfterModregning)).Loft(grundbeloeb);

			var sats = _skattelovRegistry.GetNegativNettoKapitalIndkomstSats(skatteAar);
			var nedslag = sats * negativNettoKapitalIndkomstEfterModregningDerIkkeOverstigerGrundbeloeb;

			return nedslag;
		}

// ReSharper disable MemberCanBeMadeStatic.Global
		public ValueTuple<ModregnSkatterResult<IndkomstSkatter>> ModregnMedNedslag(
			ValueTuple<IndkomstSkatter> skatter, 
			ValueTuple<decimal> nedslag)
// ReSharper restore MemberCanBeMadeStatic.Global
		{
			var skatteModregner = getSkatteModregner();

			// Modregn nedslag i egne skatter
			var modregningerFraEgetNedslag = skatteModregner.BeregnModregninger(skatter, nedslag);

			if (skatter.Size == 1)
			{
				return new ModregnSkatterResult<IndkomstSkatter>(skatter[0], nedslag[0], modregningerFraEgetNedslag[0]).ToTuple();
			}

			// Modregn evt. uudnyttet nedslag i �gtef�lles skatter
			var udnyttetNedslag = modregningerFraEgetNedslag.Map(x => x.Sum());
			var ikkeUdnyttetNedslag = nedslag - udnyttetNedslag;
			var overfoertNedslag = ikkeUdnyttetNedslag.Swap();

			var modregningerFraOverfoertNedslag
				= skatteModregner.BeregnModregninger(skatter - modregningerFraEgetNedslag, overfoertNedslag);
			var udnyttetOverfortNedslag = modregningerFraOverfoertNedslag.Map(x => x.Sum());

			// Denne tuple repr�senter overf�rslen af udnyttet nedslag (skattev�rdi) mellem �gtef�llerne...
			var overfortUdnyttetNedslag = udnyttetOverfortNedslag - udnyttetOverfortNedslag.Swap();

			//...s�dan at IkkeUdnyttetSkattevaerdi er korrekt p� den returnerede v�rdi
			return skatter.Map((skat, index) =>
				new ModregnSkatterResult<IndkomstSkatter>(skat, nedslag[index] + overfortUdnyttetNedslag[index],
					modregningerFraEgetNedslag[index] + modregningerFraOverfoertNedslag[index]));
		}

		private static SkatteModregner<IndkomstSkatter> getSkatteModregner()
		{
			return new SkatteModregner<IndkomstSkatter>(
				Modregning<IndkomstSkatter>.Af(x => x.Bundskat),
				Modregning<IndkomstSkatter>.Af(x => x.Topskat),
				Modregning<IndkomstSkatter>.Af(x => x.Sundhedsbidrag),
				Modregning<IndkomstSkatter>.Af(x => x.AktieindkomstskatOverGrundbeloebet),
				Modregning<IndkomstSkatter>.Af(x => x.Kommuneskat),
				Modregning<IndkomstSkatter>.Af(x => x.Kirkeskat)
			);
		}
	}
}