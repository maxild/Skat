using System;
using Maxfire.Core;
using Maxfire.Core.Extensions;
using Maxfire.Skat.Extensions;

namespace Maxfire.Skat
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////
	//
	// � 13, Stk. 3.
	//
	// Pkt. 1: Hvis den personlige indkomst er negativ, modregnes den inden opg�relsen af beregningsgrundlaget 
	// efter �� 6, 6 a og 7 i indkomst�rets positive kapitalindkomst.
	// Pkt. 2: Et resterende negativt bel�b fremf�res til modregning f�rst i kapitalindkomst og derefter i 
	// personlig indkomst med till�g af heri fradragne og ikke medregnede bel�b omfattet af bel�bsgr�nsen 
	// i pensionsbeskatningslovens � 16, stk. 1, for de f�lgende indkomst�r inden opg�relsen af 
	// beregningsgrundlagene efter �� 6, 6 a og 7. 
	// Pkt. 3: Negativ personlig indkomst kan kun fremf�res, i det omfang den ikke kan modregnes efter 
	// 1. eller 2. pkt. for et tidligere indkomst�r.
	//
	// � 13, Stk. 4.
	//
	// Pkt. 1: Hvis en gift persons personlige indkomst er negativ og �gtef�llerne er samlevende ved 
	// indkomst�rets udl�b, skal det negative bel�b inden opg�relse af beregningsgrundlagene efter 
	// �� 6, 6 a og 7 modregnes i den anden �gtef�lles personlige indkomst med till�g af heri 
	// fradragne og ikke medregnede bel�b omfattet af bel�bsgr�nsen i pensionsbeskatningslovens � 16, stk. 1. 
	// Pkt. 2: Et overskydende negativt bel�b modregnes i �gtef�llernes positive kapitalindkomst opgjort 
	// under �t.
	// Pkt. 3: Har begge �gtef�ller positiv kapitalindkomst, modregnes det negative bel�b fortrinsvis 
	// i den skattepligtiges kapitalindkomst og derefter i �gtef�llens kapitalindkomst.
	// Pkt. 4: Modregning sker, f�r �gtef�llens egne uudnyttede underskud fra tidligere indkomst�r 
	// fremf�res efter 5.-8. pkt.
	// Pkt. 5: Et negativt bel�b, der herefter ikke er modregnet, fremf�res inden for de f�lgende 
	// indkomst�r, i det omfang bel�bet ikke kan modregnes for et tidligere indkomst�r, til modregning 
	// inden opg�relsen af beregningsgrundlagene efter �� 6, 6 a og 7.
	// Pkt. 6: Hvert �r modregnes negativ personlig indkomst f�rst i �gtef�llernes positive kapitalindkomst 
	// opgjort under �t, derefter i den skattepligtiges personlige indkomst med till�g af heri fradragne 
	// og ikke medregnede bel�b omfattet af bel�bsgr�nsen i pensionsbeskatningslovens � 16, stk. 1, og 
	// endelig i �gtef�llens personlige indkomst med till�g af heri fradragne og ikke medregnede bel�b 
	// omfattet af bel�bsgr�nsen i pensionsbeskatningslovens � 16, stk. 1. 3. pkt. finder tilsvarende 
	// anvendelse. 
	// Pkt. 7: Hvis �gtef�llen ligeledes har uudnyttet underskud vedr�rende tidligere indkomst�r, skal 
	// �gtef�llens egne underskud modregnes f�rst. 
	//
	//////////////////////////////////////////////////////////////////////////////////////////////////////////
	public class PersonligIndkomstUnderskudBeregner
	{
// ReSharper disable MemberCanBeMadeStatic.Global
		public void ModregningAfUnderskud(IValueTuple<IPersonligeIndkomsterModregning> indkomster)
// ReSharper restore MemberCanBeMadeStatic.Global
		{
			// I. �rets underskud

			// Vi skal b�de holde styr p� (rest-)underskud hos kilden og modregninger i hhv. 
			// personligindkomst, kapitalpensionsindskud og nettokapitalindkomst.
			// N�r den ene �gtef�lle overf�rer negativ personlig indkomst til den anden �gtef�lle
			// s� skal ogs� den der overf�res _fra_ have sin personlige indkomst justeret med det
			// udnyttede underskud. Ogs� til sidst hvis underskuddet ikke kan udnyttes fuldt ud
			// skal overf�rsel af negativ personlig indkomst til 'underskud_til_fremf�rsel'.
			// Den personlige indkomst kan alts� ikke v�re negativ efter modregning er gennemf�rt, 
			// idet der sker fuld overf�rsel til enten �gtef�lle eller 'underskud_til_fremf�rsel' 
			// nulstiller den.
			
			var aaretsUnderskud = indkomster.Map(x => x.PersonligIndkomstAaretsUnderskud);

			// Note: Vi modregner b�de i egen og �gtef�lles personlige indkomst, omend egen muligvis er b�de redundant (pga underskud)
			// og forkert idet det er uvist om egne indskud p� kapitalpension skal modregnes initialt i dette trin.

			// modregning i egen og �gtef�lles positive personlige indkomst
			var aaretsRestunderskud = getRestunderskudEfterEgenOgOverfoertPersonligIndkomstModregning(indkomster,
				aaretsUnderskud, (text, udnyttetUnderskud, index) =>
					indkomster[index].NedbringUnderskudForPersonligIndkomst(
						String.Format("Overf�rsel af �rets underskud til {0} personlige indkomst med till�g af kapitalpensionsindskud", text),
						udnyttetUnderskud));

			// modregning i egen samlede positive nettokapitalindkomst
			aaretsRestunderskud = getRestunderskudEfterNettoKapitalIndkomstModregning(indkomster, 
				aaretsRestunderskud, (text, udnyttetUnderskud, index) => 
					indkomster[index].NedbringUnderskudForPersonligIndkomst(text, udnyttetUnderskud));

			// restunderskud fremf�res
			aaretsRestunderskud.Each((underskud, index) =>
			{
				indkomster[index].NedbringUnderskudForPersonligIndkomst(
					"Overf�rsel af �rets uudnyttede underskud til n�ste skatte�r", underskud);
				indkomster[index].TilfoejUnderskudTilFremfoerselForPersonligIndkomst(
					"�rets uudnyttede underskud", underskud);
			});

			// II. Fremf�rt underskud

			var fremfoertUnderskud = indkomster.Map(x => x.PersonligIndkomstFremfoertUnderskud);

			// modregning i �gtef�lles samlede positive nettokapitalindkomst
			var fremfoertRestunderskud = getRestunderskudEfterNettoKapitalIndkomstModregning(indkomster,
				fremfoertUnderskud, (text, udnyttetUnderskud, index) => 
					indkomster[index].NedbringFremfoertUnderskudForPersonligIndkomst(text, udnyttetUnderskud));

			// modregning i egen og �gtef�lles positive personlige indkomst
			fremfoertRestunderskud = getRestunderskudEfterEgenOgOverfoertPersonligIndkomstModregning(indkomster,
				fremfoertRestunderskud, (text, udnyttetUnderskud, index) =>
					indkomster[index].NedbringFremfoertUnderskudForPersonligIndkomst(
						String.Format("Overf�rsel af fremf�rt underskud til {0} personlige indkomst med till�g af kapitalpensionsindskud", text), 
						udnyttetUnderskud));

			// restunderskud fremf�res
			fremfoertRestunderskud.Each((underskud, index) =>
			{
				indkomster[index].NedbringFremfoertUnderskudForPersonligIndkomst(
					"Overf�rsel af tidligere skatte�rs uudnyttede underskud til n�ste skatte�r", underskud);
				indkomster[index].TilfoejUnderskudTilFremfoerselForPersonligIndkomst(
					"Tidligere skatte�rs uudnyttede underskud", underskud);
			});
		}

		private static ValueTuple<decimal> getRestunderskudEfterEgenOgOverfoertPersonligIndkomstModregning(
			IValueTuple<IPersonligeIndkomsterModregning> indkomster, 
			ValueTuple<decimal> underskud,
			Action<string, decimal, int> nedbringUnderskud)
		{
			// Modregning i egen personlig indkomst med till�g af kapitalpensionsindskud
			var restunderskud = getRestunderskudEfterPersonligIndkomstModregning(indkomster, underskud);
			
			// Nulstil underskud med den udnyttede del af underskuddet
			var udnyttetUnderskud = underskud - restunderskud;
			udnyttetUnderskud.Each((value, index) => nedbringUnderskud("egen", value, index));

			// Modregning i �gtef�lles personlige indkomst med till�g af kapitalpensionsindskud
			if (indkomster.Size == 2)
			{
				underskud = restunderskud;
				var overfoertUnderskud = restunderskud.Swap();
				var overfoertRestunderskud = getRestunderskudEfterPersonligIndkomstModregning(indkomster, overfoertUnderskud);
				restunderskud = overfoertRestunderskud.Swap();
				udnyttetUnderskud = underskud - restunderskud;

				// Nulstil underskud med den udnyttede del af underskuddet
				udnyttetUnderskud.Each((value, index) => nedbringUnderskud("�gtef�lles", value, index));
			}

			return restunderskud;
		}

		/// <summary>
		/// Modregning i personlige indkomst med till�g af kapitalpensionsindskud
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
				indkomster[index].NedbringUnderskudForPersonligIndkomst("Overf�rsel fra �gtef�lle", -modregnIndkomstResult.UdnyttetUnderskud));

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
					"Overf�rsel til egen nettokapitalindkomst", udnyttetUnderskud, index);
				indkomster[index].NedbringNettoKapitalIndkomst("", udnyttetUnderskud);
			});

			nettokapitalindkomst -= modregningEgenNettoKapitalIndkomst;
			underskud -= modregningEgenNettoKapitalIndkomst;

			// Modregn underskud i �gtef�lles (positive) nettokapitalindkomst
			if (indkomster.Size == 2)
			{
				var overfoertUnderskud = underskud.Swap();
				var modregningPartnersNettoKapitalIndkomst = getMuligModregning(nettokapitalindkomst, overfoertUnderskud);

				modregningPartnersNettoKapitalIndkomst.Each((modregning, index) => 
					indkomster[index].NedbringNettoKapitalIndkomst("", modregning));

				var udnyttetUnderskud = modregningPartnersNettoKapitalIndkomst.Swap();

				udnyttetUnderskud.Each((modregning, index) =>
					nedbringUnderskudForPersonligIndkomst(
						"Overf�rsel til �gtef�lles nettokapitalindkomst", modregning, index));
				
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