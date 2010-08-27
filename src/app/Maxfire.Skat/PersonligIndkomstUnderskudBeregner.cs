using System;
using Maxfire.Core.Extensions;
using Maxfire.Skat.Extensions;

namespace Maxfire.Skat
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
		public void ModregningAfUnderskud(IValueTuple<IPersonligeBeloeb> indkomster)
		{
			// I. Årets underskud

			Action<decimal, int> aaretsUnderskudNulstiller 
				= (value, index) => indkomster[index].ModregnetUnderskudPersonligIndkomst -= value;

			var personligeIndkomster = indkomster.Map(x => x.Skattegrundlag.PersonligIndkomst);
			var aaretsUnderskud = +(-personligeIndkomster);

			// Note: Vi modregner både i egen og ægtefælles personlige indkomst, omend egen muligvis er både redundant (pga underskud)
			// og forkert idet det er uvist om egne indskud på kapitalpension skal modregnes initial i dette trin.
			var aaretsRestunderskud = getRestunderskudEfterEgenOgOverfoertPersonligIndkomstModregning(indkomster,
				aaretsUnderskud, aaretsUnderskudNulstiller);

			// Modregning i ægtefællers samlede nettokapitalindkomst
			aaretsRestunderskud = getRestunderskudEfterNettoKapitalIndkomstModregning(indkomster, 
				aaretsRestunderskud, aaretsUnderskudNulstiller);

			// restunderskud fremføres
			aaretsRestunderskud.Each((underskud, index) =>
			{
				aaretsUnderskudNulstiller(underskud, index);
				indkomster[index].UnderskudPersonligIndkomstTilFremfoersel += underskud;
			});

			// II. Fremført underskud

			var fremfoertUnderskud = indkomster.Map(x => x.FremfoertUnderskudPersonligIndkomst);

			Action<decimal, int> fremfoertUnderskudNulstiller 
				= (value, index) => indkomster[index].FremfoertUnderskudPersonligIndkomst -= value;

			// modregning i ægtefællers samlede positive nettokapitalindkomst
			var fremfoertRestunderskud = getRestunderskudEfterNettoKapitalIndkomstModregning(indkomster,
				fremfoertUnderskud, fremfoertUnderskudNulstiller);

			// modregning i egen og ægtefælles personlige indkomst
			fremfoertRestunderskud = getRestunderskudEfterEgenOgOverfoertPersonligIndkomstModregning(indkomster,
				fremfoertRestunderskud, fremfoertUnderskudNulstiller);

			// restunderskud fremføres
			fremfoertRestunderskud.Each((underskud, index) =>
			{
				fremfoertUnderskudNulstiller(underskud, index);
				indkomster[index].UnderskudPersonligIndkomstTilFremfoersel += underskud;
			});
		}

		private static ValueTuple<decimal> getRestunderskudEfterEgenOgOverfoertPersonligIndkomstModregning(
			IValueTuple<IPersonligeBeloeb> indkomster, 
			ValueTuple<decimal> underskud, 
			Action<decimal, int> nulstilHandler)
		{
			// Modregning i egen personlig indkomst med tillæg af kapitalpensionsindskud
			var restunderskud = getRestunderskudEfterPersonligIndkomstModregning(indkomster, underskud);
			
			// Nulstil underskud med den udnyttede del af underskuddet
			var udnyttetUnderskud = underskud - restunderskud;
			udnyttetUnderskud.Each(nulstilHandler);

			// Modregning i ægtefælles personlige indkomst med tillæg af kapitalpensionsindskud
			if (indkomster.Size == 2)
			{
				underskud = restunderskud;
				var overfoertUnderskud = restunderskud.Swap();
				var overfoertRestunderskud = getRestunderskudEfterPersonligIndkomstModregning(indkomster, overfoertUnderskud);
				restunderskud = overfoertRestunderskud.Swap();
				udnyttetUnderskud = underskud - restunderskud;

				// Nulstil underskud med den udnyttede del af underskuddet
				udnyttetUnderskud.Each(nulstilHandler);
			}

			return restunderskud;
		}

		/// <summary>
		/// Modregning i personlige indkomst med tillæg af kapitalpensionsindskud
		/// </summary>
		private static ValueTuple<decimal> getRestunderskudEfterPersonligIndkomstModregning(
			IValueTuple<IPersonligeBeloeb> indkomster, 
			ValueTuple<decimal> underskud)
		{
			var personligeIndkomster = indkomster.Map(x => x.Skattegrundlag.PersonligIndkomst);
			var kapitalPensionsindskud = indkomster.Map(x => x.Skattegrundlag.KapitalPensionsindskud);

			// Modregning i personlig indkomst
			var modregnIndkomstResults = personligeIndkomster.ModregnUnderskud(underskud);

			// TODO: Side-effekt
			modregnIndkomstResults.Each((modregnIndkomstResult, index) =>
			{
				indkomster[index].ModregnetUnderskudPersonligIndkomst += modregnIndkomstResult.UdnyttetUnderskud;
			});

			var restunderskud = modregnIndkomstResults.Map(x => x.IkkeUdnyttetUnderskud);

			// Modregning i kapitalpensionsindskud
			var modregnKapitalPensionsindskudResults = kapitalPensionsindskud.ModregnUnderskud(restunderskud);
			
			// TODO: Side-effekt
			modregnKapitalPensionsindskudResults.Each((modregnIndkomstResult, index) =>
			{
				indkomster[index].ModregnetUnderskudKapitalPensionsindskud += modregnIndkomstResult.UdnyttetUnderskud;
			});

			restunderskud = modregnKapitalPensionsindskudResults.Map(x => x.IkkeUdnyttetUnderskud);

			return restunderskud;
		}

		private static ValueTuple<decimal> getRestunderskudEfterNettoKapitalIndkomstModregning(
			IValueTuple<IPersonligeBeloeb> indkomster, 
			ValueTuple<decimal> underskud, 
			Action<decimal, int> nulstilHandler)
		{
			// Modregn underskud i egen (positive) nettokapitalindkomst
			var nettokapitalindkomst = indkomster.Map(x => x.Skattegrundlag.NettoKapitalIndkomst);
			var modregningEgenNettoKapitalIndkomst = getMuligModregning(nettokapitalindkomst, underskud);
			
			// TODO: Side-effekt
			modregningEgenNettoKapitalIndkomst.Each((modregning, index) => 
			{
				nulstilHandler(modregning, index);
				indkomster[index].ModregnetUnderskudNettoKapitalIndkomst += modregning;
			});

			nettokapitalindkomst -= modregningEgenNettoKapitalIndkomst;
			underskud -= modregningEgenNettoKapitalIndkomst;

			// Modregn underskud i ægtefælles (positive) nettokapitalindkomst
			if (indkomster.Size == 2)
			{
				var overfoertUnderskud = underskud.Swap();
				var modregningPartnersNettoKapitalIndkomst = getMuligModregning(nettokapitalindkomst, overfoertUnderskud);

				// TODO: Side-effekt
				modregningPartnersNettoKapitalIndkomst.Each((modregning, index) =>
				{
					indkomster[index].ModregnetUnderskudNettoKapitalIndkomst += modregning;
				});

				var udnyttetUnderskud = modregningPartnersNettoKapitalIndkomst.Swap();
				udnyttetUnderskud.Each(nulstilHandler);
				
				underskud -= udnyttetUnderskud;
			}

			return underskud;
		}

		private static ValueTuple<decimal> getMuligModregning(
			ValueTuple<decimal> nettokapitalindkomst, 
			ValueTuple<decimal> underskud)
		{
			var samletkapitalindkomst = nettokapitalindkomst.Sum();
			// TODO: Make overloads of Min, Max, Path.Combine in Maxfire.Core
			return nettokapitalindkomst.Map((kapitalindkomst, index) => 
				Math.Min(Math.Min(kapitalindkomst, samletkapitalindkomst), underskud[index]).NonNegative());
		}
	}
}