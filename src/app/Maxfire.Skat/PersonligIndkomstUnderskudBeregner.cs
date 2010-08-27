using System;
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
		public void ModregningAfUnderskud(IValueTuple<IPersonligeBeloeb> indkomster)
		{
			// I. �rets underskud

			Action<decimal, int> aaretsUnderskudNulstiller 
				= (value, index) => indkomster[index].ModregnetUnderskudPersonligIndkomst -= value;

			var personligeIndkomster = indkomster.Map(x => x.Skattegrundlag.PersonligIndkomst);
			var aaretsUnderskud = +(-personligeIndkomster);

			// Note: Vi modregner b�de i egen og �gtef�lles personlige indkomst, omend egen muligvis er b�de redundant (pga underskud)
			// og forkert idet det er uvist om egne indskud p� kapitalpension skal modregnes initial i dette trin.
			var aaretsRestunderskud = getRestunderskudEfterEgenOgOverfoertPersonligIndkomstModregning(indkomster,
				aaretsUnderskud, aaretsUnderskudNulstiller);

			// Modregning i �gtef�llers samlede nettokapitalindkomst
			aaretsRestunderskud = getRestunderskudEfterNettoKapitalIndkomstModregning(indkomster, 
				aaretsRestunderskud, aaretsUnderskudNulstiller);

			// restunderskud fremf�res
			aaretsRestunderskud.Each((underskud, index) =>
			{
				aaretsUnderskudNulstiller(underskud, index);
				indkomster[index].UnderskudPersonligIndkomstTilFremfoersel += underskud;
			});

			// II. Fremf�rt underskud

			var fremfoertUnderskud = indkomster.Map(x => x.FremfoertUnderskudPersonligIndkomst);

			Action<decimal, int> fremfoertUnderskudNulstiller 
				= (value, index) => indkomster[index].FremfoertUnderskudPersonligIndkomst -= value;

			// modregning i �gtef�llers samlede positive nettokapitalindkomst
			var fremfoertRestunderskud = getRestunderskudEfterNettoKapitalIndkomstModregning(indkomster,
				fremfoertUnderskud, fremfoertUnderskudNulstiller);

			// modregning i egen og �gtef�lles personlige indkomst
			fremfoertRestunderskud = getRestunderskudEfterEgenOgOverfoertPersonligIndkomstModregning(indkomster,
				fremfoertRestunderskud, fremfoertUnderskudNulstiller);

			// restunderskud fremf�res
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
			// Modregning i egen personlig indkomst med till�g af kapitalpensionsindskud
			var restunderskud = getRestunderskudEfterPersonligIndkomstModregning(indkomster, underskud);
			
			// Nulstil underskud med den udnyttede del af underskuddet
			var udnyttetUnderskud = underskud - restunderskud;
			udnyttetUnderskud.Each(nulstilHandler);

			// Modregning i �gtef�lles personlige indkomst med till�g af kapitalpensionsindskud
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
		/// Modregning i personlige indkomst med till�g af kapitalpensionsindskud
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

			// Modregn underskud i �gtef�lles (positive) nettokapitalindkomst
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