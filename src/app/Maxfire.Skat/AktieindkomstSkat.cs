using System;
using Maxfire.Skat.Extensions;

namespace Maxfire.Skat
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////
	//
	// PSL § 8 a.
	//
	// Stk. 1. Skat af aktieindkomst, der ikke overstiger et grundbeløb på 48.300 kr. (2010-niveau), 
	// beregnes som en endelig skat, der for indkomstårene 2010 og 2011 udgør 28 pct., og for 
	// indkomståret 2012 og efterfølgende indkomstår udgør 27 pct. Indeholdt udbytteskat efter 
	// kildeskattelovens § 65 af aktieindkomst, der ikke overstiger grundbeløbet, er endelig betaling 
	// af skatten, og udbytteskatten modregnes ikke i slutskatten efter kildeskattelovens § 67.

	// GAMMEL!!!!
	// Stk. 2. Skat af aktieindkomst, der overstiger et grundbeløb på 26.400 kr., beregnes med 43 pct.
	// Skat af aktieindkomst, der overstiger et grundbeløb på 58.050 kr., beregnes med 45 pct. 2. pkt.
	// finder dog ikke anvendelse, i det omfang aktieindkomsten kan rummes i positiv overgangssaldo
	// opgjort i henhold til aktieavancebeskatningslovens § 45 A. Hvis en gift persons aktieindkomst
	// ikke kan rummes i vedkommende ægtefælles egen overgangssaldo, finder 2. pkt. ikke anvendelse, 
	// i det omfang det overskydende beløb kan rummes i den anden ægtefælles eventuelle
	// overgangssaldo. Ved anvendelsen af 4. pkt. skal den anden ægtefælles overgangssaldo først nedskrives
	// med vedkommendes egen aktieindkomst i det pågældende indkomstår. 4. pkt. finder kun
	// anvendelse, hvis ægtefællerne er samlevende ved indkomstårets udløb. Skat af aktieindkomst,
	// der overstiger grundbeløbene, indgår i slutskatten, og den udbytteskat, der er indeholdt i denne
	// del af udbyttet efter kildeskattelovens § 65, modregnes i slutskatten efter kildeskattelovens § 67.

	//NY!!!!!
	// Stk. 2. Skat af aktieindkomst, der overstiger et grundbeløb på 48.300 kr. (2010-niveau), beregnes
	// med 42 pct. Skatten indgår i slutskatten, og den udbytteskat, der er indeholdt i denne del af
	// udbyttet efter kildeskattelovens § 65, modregnes i slutskatten efter kildeskattelovens § 67.

	// Stk. 3. Overstiger den indeholdte udbytteskat efter kildeskattelovens § 65 den i stk. 1 nævnte 
	// skatteprocent af den samlede aktieindkomst, modregnes det overskydende beløb i slutskatten. 
	// Er aktieindkomsten negativ, modregnes hele den indeholdte udbytteskat i slutskatten.
	// Stk. 4. Er en gift persons aktieindkomst lavere end det i stk. 1 og 2 nævnte grundbeløb, forhøjes 
	// den anden ægtefælles grundbeløb med forskelsbeløbet, dog højst med grundbeløbet. Det er en 
	// forudsætning, at ægtefællerne er samlevende ved indkomstårets udløb. 
	// Stk. 5. Er aktieindkomsten negativ, beregnes negativ skat med den i stk. 1 nævnte procentsats 
	// af beløb, der ikke overstiger grundbeløbet, og med skatteprocenten nævnt i stk. 2 af beløb,
	// der overstiger grundbeløbet. Den negative skat modregnes i den skattepligtiges slutskat, og et 
	// eventuelt resterende beløb fremføres til modregning i slutskatten for de følgende indkomstår. 
	// Stk. 6. Er en gift persons aktieindkomst negativ, modregnes beløbet i den anden ægtefælles 
	// positive aktieindkomst. Det er en forudsætning, at ægtefællerne er samlevende ved indkomstårets 
	// udløb. Af et eventuelt resterende negativt beløb beregnes negativ skat efter stk. 5 med 
	// anvendelse af dobbelt grundbeløb. Har begge ægtefæller negativ aktieindkomst, fordeles 
	// det dobbelte grundbeløb forholdsmæssigt mellem ægtefællerne. Negativ skat, der ikke kan 
	// modregnes i den skattepligtiges slutskat, modregnes i ægtefællens slutskat. 
	// Stk. 7. Grundbeløb i stk. 1-2 reguleres efter § 20. 
	//
	//////////////////////////////////////////////////////////////////////////////////////////////////////
	// TODO: Mangler sambeskatningseffekter og skattefradrag for negativ aktieindkomst

	/// <summary>
	/// Det laveste trin beskattes med 28 pct i 2009 og 2010.
	/// </summary>
	public class AktieindkomstskatLavesteTrinBeregner
	{
		private readonly ISkattelovRegistry _skattelovRegistry;

		public AktieindkomstskatLavesteTrinBeregner(ISkattelovRegistry skattelovRegistry)
		{
			_skattelovRegistry = skattelovRegistry;
		}

		public ValueTuple<decimal> BeregnSkat(IValueTuple<IPersonligeBeloeb> indkomster, int skatteAar)
		{
			return indkomster.Map(indkomst => BeregnSkat(indkomst, skatteAar));
		}

		public decimal BeregnSkat(IPersonligeBeloeb indkomst, int skatteAar)
		{
			decimal beloebUnderLavesteProgressionsgraense
				= Math.Min(_skattelovRegistry.GetAktieIndkomstLavesteProgressionsgraense(skatteAar), indkomst.Skattegrundlag.AktieIndkomst).NonNegative();
			return _skattelovRegistry.GetAktieIndkomstLavesteSkattesats(skatteAar) * beloebUnderLavesteProgressionsgraense;
		}
	}

	/// <summary>
	/// Det mellemste trin beskattes med 43 pct i 2009, og 42 pct i 2010.
	/// </summary>
	public class AktieindkomstskatMellemsteTrinBeregner
	{
		private readonly ISkattelovRegistry _skattelovRegistry;

		public AktieindkomstskatMellemsteTrinBeregner(ISkattelovRegistry skattelovRegistry)
		{
			_skattelovRegistry = skattelovRegistry;
		}

		public ValueTuple<decimal> BeregnSkat(IValueTuple<IPersonligeBeloeb> indkomster, int skatteAar)
		{
			return indkomster.Map(indkomst => BeregnSkat(indkomst, skatteAar));
		}

		public decimal BeregnSkat(IPersonligeBeloeb indkomst, int skatteAar)
		{
			decimal beloebUnderProgressionsgraense 
				= Math.Min(_skattelovRegistry.GetAktieIndkomstHoejesteProgressionsgraense(skatteAar),
															  indkomst.Skattegrundlag.AktieIndkomst);
			decimal beloebOverLavesteProgressionsgraenseOgUnderHoejesteProgressionsgraense 
				= (beloebUnderProgressionsgraense - _skattelovRegistry.GetAktieIndkomstLavesteProgressionsgraense(skatteAar)).NonNegative();
			return _skattelovRegistry.GetAktieIndkomstMellemsteSkattesats(skatteAar) * beloebOverLavesteProgressionsgraenseOgUnderHoejesteProgressionsgraense;
		}
	}

	/// <summary>
	/// Det højeste trin er beskattes med 45 pct. i 2009, og dette trin bortfalder i 2010.
	/// </summary>
	public class AktieindkomstskatHoejesteTrinBeregner
	{
		private readonly ISkattelovRegistry _skattelovRegistry;

		public AktieindkomstskatHoejesteTrinBeregner(ISkattelovRegistry skattelovRegistry)
		{
			_skattelovRegistry = skattelovRegistry;
		}

		public ValueTuple<decimal> BeregnSkat(IValueTuple<IPersonligeBeloeb> indkomster, int skatteAar)
		{
			return indkomster.Map(indkomst => BeregnSkat(indkomst, skatteAar));
		}

		public decimal BeregnSkat(IPersonligeBeloeb indkomst, int skatteAar)
		{
			decimal beloebOverHoejesteProgressionsgraense
				= (indkomst.Skattegrundlag.AktieIndkomst - _skattelovRegistry.GetAktieIndkomstHoejesteProgressionsgraense(skatteAar)).NonNegative();
			return _skattelovRegistry.GetAktieIndkomstHoejesteSkattesats(skatteAar) * beloebOverHoejesteProgressionsgraense;
		}
	}
}