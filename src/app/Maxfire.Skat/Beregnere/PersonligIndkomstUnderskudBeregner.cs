using System;
using Maxfire.Core;
using Maxfire.Core.Extensions;
using Maxfire.Skat.Extensions;

namespace Maxfire.Skat.Beregnere
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////
	//
	// § 13, Stk. 3.
	//
	// Pkt. 1: Hvis den personlige indkomst er negativ, modregnes den inden opgørelsen af beregningsgrundlaget 
	// efter §§ 6, 6 a og 7 i indkomstårets positive kapitalindkomst.
	// Pkt. 2: Et resterende negativt beløb fremføres til modregning først i kapitalindkomst og derefter i 
	// personlig indkomst med tillæg af heri fradragne og ikke medregnede beløb omfattet af beløbsgrænsen 
	// i pensionsbeskatningslovens § 16, stk. 1, for de følgende indkomstår inden opgørelsen af 
	// beregningsgrundlagene efter §§ 6, 6 a og 7. 
	// Pkt. 3: Negativ personlig indkomst kan kun fremføres, i det omfang den ikke kan modregnes efter 
	// 1. eller 2. pkt. for et tidligere indkomstår.
	//
	// § 13, Stk. 4.
	//
	// Pkt. 1: Hvis en gift persons personlige indkomst er negativ og ægtefællerne er samlevende ved 
	// indkomstårets udløb, skal det negative beløb inden opgørelse af beregningsgrundlagene efter 
	// §§ 6, 6 a og 7 modregnes i den anden ægtefælles personlige indkomst med tillæg af heri 
	// fradragne og ikke medregnede beløb omfattet af beløbsgrænsen i pensionsbeskatningslovens § 16, stk. 1. 
	// Pkt. 2: Et overskydende negativt beløb modregnes i ægtefællernes positive kapitalindkomst opgjort 
	// under ét.
	// Pkt. 3: Har begge ægtefæller positiv kapitalindkomst, modregnes det negative beløb fortrinsvis 
	// i den skattepligtiges kapitalindkomst og derefter i ægtefællens kapitalindkomst.
	// Pkt. 4: Modregning sker, før ægtefællens egne uudnyttede underskud fra tidligere indkomstår 
	// fremføres efter 5.-8. pkt.
	// Pkt. 5: Et negativt beløb, der herefter ikke er modregnet, fremføres inden for de følgende 
	// indkomstår, i det omfang beløbet ikke kan modregnes for et tidligere indkomstår, til modregning 
	// inden opgørelsen af beregningsgrundlagene efter §§ 6, 6 a og 7.
	// Pkt. 6: Hvert år modregnes negativ personlig indkomst først i ægtefællernes positive kapitalindkomst 
	// opgjort under ét, derefter i den skattepligtiges personlige indkomst med tillæg af heri fradragne 
	// og ikke medregnede beløb omfattet af beløbsgrænsen i pensionsbeskatningslovens § 16, stk. 1, og 
	// endelig i ægtefællens personlige indkomst med tillæg af heri fradragne og ikke medregnede beløb 
	// omfattet af beløbsgrænsen i pensionsbeskatningslovens § 16, stk. 1. 3. pkt. finder tilsvarende 
	// anvendelse. 
	// Pkt. 7: Hvis ægtefællen ligeledes har uudnyttet underskud vedrørende tidligere indkomstår, skal 
	// ægtefællens egne underskud modregnes først. 
	//
	//////////////////////////////////////////////////////////////////////////////////////////////////////////
	public class PersonligIndkomstUnderskudBeregner
	{
// ReSharper disable MemberCanBeMadeStatic.Global
		public void ModregningAfUnderskud(IValueTuple<IPersonligeIndkomsterModregning> indkomster)
// ReSharper restore MemberCanBeMadeStatic.Global
		{
			// I. Årets underskud

			// Vi skal både holde styr på (rest-)underskud hos kilden og modregninger i hhv. 
			// personligindkomst, kapitalpensionsindskud og nettokapitalindkomst.
			// Når den ene ægtefælle overfører negativ personlig indkomst til den anden ægtefælle
			// så skal også den der overføres _fra_ have sin personlige indkomst justeret med det
			// udnyttede underskud. Også til sidst hvis underskuddet ikke kan udnyttes fuldt ud
			// skal overførsel af negativ personlig indkomst til 'underskud_til_fremførsel'.
			// Den personlige indkomst kan altså ikke være negativ efter modregning er gennemført, 
			// idet der sker fuld overførsel til enten ægtefælle eller 'underskud_til_fremførsel' 
			// nulstiller den.
			
			var aaretsUnderskud = indkomster.Map(x => x.PersonligIndkomstAaretsUnderskud);

			// Note: Vi modregner både i egen og ægtefælles personlige indkomst, omend egen muligvis er både redundant (pga underskud)
			// og forkert idet det er uvist om egne indskud på kapitalpension skal modregnes initialt i dette trin.

			// modregning i egen og ægtefælles positive personlige indkomst
			var aaretsRestunderskud = getRestunderskudEfterEgenOgOverfoertPersonligIndkomstModregning(indkomster,
				aaretsUnderskud, (text, udnyttetUnderskud, index) =>
					indkomster[index].NedbringUnderskudForPersonligIndkomst(
						String.Format("Overførsel af årets underskud til {0} personlige indkomst med tillæg af kapitalpensionsindskud", text),
						udnyttetUnderskud));

			// modregning i egen samlede positive nettokapitalindkomst
			aaretsRestunderskud = getRestunderskudEfterNettoKapitalIndkomstModregning(indkomster, 
				aaretsRestunderskud, (text, udnyttetUnderskud, index) => 
					indkomster[index].NedbringUnderskudForPersonligIndkomst(text, udnyttetUnderskud));

			// restunderskud fremføres
			aaretsRestunderskud.Each((underskud, index) =>
			{
				indkomster[index].NedbringUnderskudForPersonligIndkomst(
					"Overførsel af årets uudnyttede underskud til næste skatteår", underskud);
				indkomster[index].FremfoerUnderskudForPersonligIndkomst(
					"Årets uudnyttede underskud", underskud);
			});

			// II. Fremført underskud

			var fremfoertUnderskud = indkomster.Map(x => x.PersonligIndkomstFremfoertUnderskud);

			// modregning i ægtefælles samlede positive nettokapitalindkomst
			var fremfoertRestunderskud = getRestunderskudEfterNettoKapitalIndkomstModregning(indkomster,
				fremfoertUnderskud, (text, udnyttetUnderskud, index) => 
					indkomster[index].NedbringFremfoertUnderskudForPersonligIndkomst(text, udnyttetUnderskud));

			// modregning i egen og ægtefælles positive personlige indkomst
			fremfoertRestunderskud = getRestunderskudEfterEgenOgOverfoertPersonligIndkomstModregning(indkomster,
				fremfoertRestunderskud, (text, udnyttetUnderskud, index) =>
					indkomster[index].NedbringFremfoertUnderskudForPersonligIndkomst(
						String.Format("Overførsel af fremført underskud til {0} personlige indkomst med tillæg af kapitalpensionsindskud", text), 
						udnyttetUnderskud));

			// restunderskud fremføres
			fremfoertRestunderskud.Each((underskud, index) =>
			{
				indkomster[index].NedbringFremfoertUnderskudForPersonligIndkomst(
					"Overførsel af tidligere skatteårs uudnyttede underskud til næste skatteår", underskud);
				indkomster[index].FremfoerUnderskudForPersonligIndkomst(
					"Tidligere skatteårs uudnyttede underskud", underskud);
			});
		}

		private static ValueTuple<decimal> getRestunderskudEfterEgenOgOverfoertPersonligIndkomstModregning(
			IValueTuple<IPersonligeIndkomsterModregning> indkomster, 
			ValueTuple<decimal> underskud,
			Action<string, decimal, int> nedbringUnderskud)
		{
			// Modregning i egen personlig indkomst med tillæg af kapitalpensionsindskud
			var restunderskud = getRestunderskudEfterPersonligIndkomstModregning(indkomster, underskud);
			
			// Nulstil underskud med den udnyttede del af underskuddet
			var udnyttetUnderskud = underskud - restunderskud;
			udnyttetUnderskud.Each((value, index) => nedbringUnderskud("egen", value, index));

			// Modregning i ægtefælles personlige indkomst med tillæg af kapitalpensionsindskud
			if (indkomster.Size == 2)
			{
				underskud = restunderskud;
				var overfoertUnderskud = restunderskud.Swap();
				var overfoertRestunderskud = getRestunderskudEfterPersonligIndkomstModregning(indkomster, overfoertUnderskud);
				restunderskud = overfoertRestunderskud.Swap();
				udnyttetUnderskud = underskud - restunderskud;

				// Nulstil underskud med den udnyttede del af underskuddet
				udnyttetUnderskud.Each((value, index) => nedbringUnderskud("ægtefælles", value, index));
			}

			return restunderskud;
		}

		/// <summary>
		/// Modregning i personlige indkomst med tillæg af kapitalpensionsindskud
		/// </summary>
		private static ValueTuple<decimal> getRestunderskudEfterPersonligIndkomstModregning(
			IValueTuple<IPersonligeIndkomsterModregning> indkomster, 
			ValueTuple<decimal> underskud)
		{
			var personligeIndkomster = indkomster.Map(x => x.PersonligIndkomstSkattegrundlag);
			var kapitalPensionsindskud = indkomster.Map(x => x.KapitalPensionsindskudSkattegrundlag);

			// Modregning i personlig indkomst
			var modregnIndkomstResults = personligeIndkomster.ModregnUnderskud(underskud);

			modregnIndkomstResults.Each((modregnIndkomstResult, index) => 
				indkomster[index].NedbringPersonligIndkomst("Overførsel fra ægtefælle", modregnIndkomstResult.UdnyttetUnderskud));

			var restunderskud = modregnIndkomstResults.Map(x => x.IkkeUdnyttetUnderskud);

			// Modregning i kapitalpensionsindskud
			var modregnKapitalPensionsindskudResults = kapitalPensionsindskud.ModregnUnderskud(restunderskud);
			
			modregnKapitalPensionsindskudResults.Each((modregnIndkomstResult, index) => 
				indkomster[index].NedbringKapitalPensionsindskud("", modregnIndkomstResult.UdnyttetUnderskud));

			restunderskud = modregnKapitalPensionsindskudResults.Map(x => x.IkkeUdnyttetUnderskud);

			return restunderskud;
		}

		private static ValueTuple<decimal> getRestunderskudEfterNettoKapitalIndkomstModregning(
			IValueTuple<IPersonligeIndkomsterModregning> indkomster, 
			ValueTuple<decimal> underskud,
			Action<string, decimal, int> nedbringUnderskudForPersonligIndkomst)
		{
			// Modregn underskud i egen (positive) nettokapitalindkomst
			var nettokapitalindkomst = indkomster.Map(x => x.NettoKapitalIndkomstSkattegrundlag);
			var modregningEgenNettoKapitalIndkomst = getMuligModregning(nettokapitalindkomst, underskud);
			
			modregningEgenNettoKapitalIndkomst.Each((udnyttetUnderskud, index) => 
			{
				nedbringUnderskudForPersonligIndkomst(
					"Overførsel til egen nettokapitalindkomst", udnyttetUnderskud, index);
				indkomster[index].NedbringNettoKapitalIndkomst("", udnyttetUnderskud);
			});

			nettokapitalindkomst -= modregningEgenNettoKapitalIndkomst;
			underskud -= modregningEgenNettoKapitalIndkomst;

			// Modregn underskud i ægtefælles (positive) nettokapitalindkomst
			if (indkomster.Size == 2)
			{
				var overfoertUnderskud = underskud.Swap();
				var modregningPartnersNettoKapitalIndkomst = getMuligModregning(nettokapitalindkomst, overfoertUnderskud);

				modregningPartnersNettoKapitalIndkomst.Each((modregning, index) => 
					indkomster[index].NedbringNettoKapitalIndkomst("", modregning));

				var udnyttetUnderskud = modregningPartnersNettoKapitalIndkomst.Swap();

				udnyttetUnderskud.Each((modregning, index) =>
					nedbringUnderskudForPersonligIndkomst(
						"Overførsel til ægtefælles nettokapitalindkomst", modregning, index));
				
				underskud -= udnyttetUnderskud;
			}

			return underskud;
		}

		private static ValueTuple<decimal> getMuligModregning(
			ValueTuple<decimal> nettokapitalindkomst, 
			ValueTuple<decimal> underskud)
		{
			var samletkapitalindkomst = nettokapitalindkomst.Sum();
			return nettokapitalindkomst.Map((kapitalindkomst, index) => 
				Numbers.Min(kapitalindkomst, samletkapitalindkomst, underskud[index]).NonNegative());
		}
	}
}