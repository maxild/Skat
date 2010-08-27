using System;
using System.Collections.Generic;
using System.Reflection;

namespace Maxfire.Skat
{
	/// <summary>
	/// Beløbsgrænser og skattesatser som defineret i PSL.
	/// </summary>
	public class DefaultSkattelovRegistry : ISkattelovRegistry
	{
		const int MIN_SKATTEAAR = 2009;
		const int MAX_SKATTEAAR = 2019;
		
		// Note: This is the only interesting part of this class
		// (the remaining code is just 'butt ugly repeated code' due to lack of C# metadata programming facilities)
		private static readonly IDictionary<string, decimal[]> _registry = new Dictionary<string, decimal[]>
		{
			{"GetAktieIndkomstLavesteProgressionsgraense",  values(48300) },
			{"GetAktieIndkomstHoejesteProgressionsgraense", values(106100, decimal.MaxValue) },   // Bortfalder fra og med 2010
			{"GetAktieIndkomstLavesteSkattesats",           values(0.28m, 0.28m, 0.28m, 0.27m) },
			{"GetAktieIndkomstMellemsteSkattesats",         values(0.43m, 0.42m) },
			{"GetAktieIndkomstHoejesteSkattesats",          values(0.45m, 0) },                   // Bortfalder fra og med 2010
			
			{"GetAMBidragSkattesats",                       values(0.08m) },
			
			{"GetSundhedsbidragSkattesats",                 values(0.08m,   0.08m,   0.08m,   0.07m,   0.06m,   0.05m,   0.04m,   0.03m,   0.02m,   0.01m,   0) },
			
			{"GetBundSkattesats",                           values(0.0504m, 0.0367m, 0.0367m, 0.0467m, 0.0567m, 0.0667m, 0.0767m, 0.0867m, 0.0967m, 0.1067m, 0.1167m) },
			
			{"GetMellemSkattesats",                         values(0.06m, 0) },                 // Bortfalder fra og med 2010
			{"GetMellemskatBundfradrag",                    values(347200, decimal.MaxValue) }, // Bortfalder fra og med 2010
			
			{"GetTopSkattesats",                            values(0.15m) },
			{"GetTopskatBundfradrag",                       values(347200, 389900, 409100) },
			{"GetPositivNettoKapitalIndkomstGrundbeloeb",   values(0, 40000) },                 // Indført fra og med 2010
			
			{"GetSkatteloftSkattesats",                     values(0.59m, 0.515m) },
			
			{"getPersonfradragForVoksne",                   values(42900) },
			{"getPersonfradragForBoern",                    values(32200) },
			
			// Beløb i kompensationsordningen (2012-2019) svarer til de beløbsgrænser, der ville være
			// gældende i 2010 uden Forårspakke 2.0 og med en opregulering fra 2009 til 2010 på 4,5 pct.
			{"getBundLettelseBundfradragForVoksne",         values(decimal.MaxValue, decimal.MaxValue, decimal.MaxValue, 44800) },  // De 44.800 svarer til afrundet_til_100(42900 * 1,045)
			{"getBundLettelseBundfradragForBoern",          values(decimal.MaxValue, decimal.MaxValue, decimal.MaxValue, 33600) },  // De 33.600 svarer til afrundet_til_100(32200 * 1,045)
			{"GetMellemLettelseBundfradrag",                values(decimal.MaxValue, decimal.MaxValue, decimal.MaxValue, 362800) }, // De 362.800 svarer til afrundet_til_100(347.200 * 1,045)
			{"GetTopLettelseBundfradrag",                   values(decimal.MaxValue, decimal.MaxValue, decimal.MaxValue, 362800) }, // De 362.800 svarer til afrundet_til_100(347.200 * 1,045)
			{"getPersonfradragSkaerpelseForVoksne",         values(0, 0, 0, 1900) }, // De 1.900 svarer til afrundet_til_100(42900 * 0,045)
			{"getPersonfradragSkaerpelseForBoern",          values(0, 0, 0, 1400) }, // De 1.400 svarer til afrundet_til_100(32200 * 0,045)
			
			// Nedslag for negativ nettokapitalindkomst (2012-2019)
			{"GetNegativNettoKapitalIndkomstGrundbeloeb",   values(0, 0, 0, 50000) }, // beløbsgrænsen reguleres ikke efter 2012
			{"GetNegativNettoKapitalIndkomstSats",          values(0, 0, 0, 0.01m, 0.02m, 0.03m, 0.04m, 0.05m, 0.06m, 0.07m, 0.08m) },
			
			{"GetBeskaeftigelsesfradragGrundbeloeb",        values(13600,   13600,   13600,   14100,  14400,  14900,   15400,  16000, 16600,  17300,  17900) },
			{"GetBeskaeftigelsesfradragSats",               values(0.0425m, 0.0425m, 0.0425m, 0.044m, 0.045m, 0.0465m, 0.048m, 0.05m, 0.052m, 0.054m, 0.056m) },
			
			{"GetGroenCheckPrVoksen",                       values(0, 1300) },
			{"GetGroenCheckPrBarn",                         values(0, 300) },
			{"GetGroenCheckAftrapningssats",                values(0, 0.075m) },
			{"GetGroenCheckBundfradrag",                    values(decimal.MaxValue, 362800) }
		};

		static decimal[] values(params decimal[] values)
		{
			const int size = MAX_SKATTEAAR - MIN_SKATTEAAR + 1;
			var array = new decimal[size];
			int i = 0;
			for (; i < Math.Min(size, values.Length); i++)
			{
				array[i] = values[i];
			}
			decimal lastValue = values[values.Length - 1];
			for (; i < size; i++)
			{
				array[i] = lastValue;
			}
			return array;
		}

		static decimal getValue(string methodName, int skatteAar)
		{
			checkSkatteaar(skatteAar);
			decimal[] valueArray;
			if (_registry.TryGetValue(methodName, out valueArray) == false)
			{
				throw new InvalidOperationException(string.Format("{1} er konfigureret forkert, idet {0} er en ukendt metode --- check alle unit tests af {1}", methodName, typeof(DefaultSkattelovRegistry).Name));
			}
			return valueArray[skatteAar - 2009];
		}

		static void checkSkatteaar(int skatteAar)
		{
			if (skatteAar < MIN_SKATTEAAR|| skatteAar > MAX_SKATTEAAR)
				throw new ArgumentOutOfRangeException("skatteAar", skatteAar, 
					string.Format("Beløbsgrænser eller skattesatser kan ikke aflæses for skatteår udenfor intervallet {0}..{1}.", MIN_SKATTEAAR, MAX_SKATTEAAR));
		}

		public decimal GetPersonfradrag(int skatteAar, int alder, bool gift)
		{
			return isGiftOrVoksen(gift, alder) ? getPersonfradragForVoksne(skatteAar) : getPersonfradragForBoern(skatteAar);
		}

		public decimal GetBundLettelseBundfradrag(int skatteAar, int alder, bool gift)
		{
			return isGiftOrVoksen(gift, alder) ? getBundLettelseBundfradragForVoksne(skatteAar) : getBundLettelseBundfradragForBoern(skatteAar);
		}

		public decimal GetPersonfradragSkaerpelse(int skatteAar, int alder, bool gift)
		{
			return isGiftOrVoksen(gift, alder) ? getPersonfradragSkaerpelseForVoksne(skatteAar) : getPersonfradragSkaerpelseForBoern(skatteAar);
		}

		static bool isGiftOrVoksen(bool gift, int alder)
		{
			return gift || alder >= 18;
		}

		public decimal GetAktieIndkomstLavesteProgressionsgraense(int skatteAar)
		{
			return getValue(MethodBase.GetCurrentMethod().Name, skatteAar);
		}

		public decimal GetAktieIndkomstHoejesteProgressionsgraense(int skatteAar)
		{
			return getValue(MethodBase.GetCurrentMethod().Name, skatteAar);
		}

		public decimal GetAktieIndkomstLavesteSkattesats(int skatteAar)
		{
			return getValue(MethodBase.GetCurrentMethod().Name, skatteAar);
		}

		public decimal GetAktieIndkomstMellemsteSkattesats(int skatteAar)
		{
			return getValue(MethodBase.GetCurrentMethod().Name, skatteAar);
		}

		public decimal GetAktieIndkomstHoejesteSkattesats(int skatteAar)
		{
			return getValue(MethodBase.GetCurrentMethod().Name, skatteAar);
		}

		public decimal GetAMBidragSkattesats(int skatteAar)
		{
			return getValue(MethodBase.GetCurrentMethod().Name, skatteAar);
		}

		public decimal GetSundhedsbidragSkattesats(int skatteAar)
		{
			return getValue(MethodBase.GetCurrentMethod().Name, skatteAar);
		}

		public decimal GetBundSkattesats(int skatteAar)
		{
			return getValue(MethodBase.GetCurrentMethod().Name, skatteAar);
		}

		public decimal GetMellemSkattesats(int skatteAar)
		{
			return getValue(MethodBase.GetCurrentMethod().Name, skatteAar);
		}

		public decimal GetTopSkattesats(int skatteAar)
		{
			return getValue(MethodBase.GetCurrentMethod().Name, skatteAar);
		}

		public decimal GetSkatteloftSkattesats(int skatteAar)
		{
			return getValue(MethodBase.GetCurrentMethod().Name, skatteAar);
		}

		static decimal getPersonfradragForVoksne(int skatteAar)
		{
			return getValue(MethodBase.GetCurrentMethod().Name, skatteAar);
		}

		static decimal getPersonfradragForBoern(int skatteAar)
		{
			return getValue(MethodBase.GetCurrentMethod().Name, skatteAar);
		}

		static decimal getPersonfradragSkaerpelseForVoksne(int skatteAar)
		{
			return getValue(MethodBase.GetCurrentMethod().Name, skatteAar);
		}

		static decimal getPersonfradragSkaerpelseForBoern(int skatteAar)
		{
			return getValue(MethodBase.GetCurrentMethod().Name, skatteAar);
		}

		static decimal getBundLettelseBundfradragForVoksne(int skatteAar)
		{
			return getValue(MethodBase.GetCurrentMethod().Name, skatteAar);
		}

		static decimal getBundLettelseBundfradragForBoern(int skatteAar)
		{
			return getValue(MethodBase.GetCurrentMethod().Name, skatteAar);
		}

		public decimal GetMellemLettelseBundfradrag(int skatteAar)
		{
			return getValue(MethodBase.GetCurrentMethod().Name, skatteAar);
		}

		public decimal GetTopLettelseBundfradrag(int skatteAar)
		{
			return getValue(MethodBase.GetCurrentMethod().Name, skatteAar);
		}

		public decimal GetMellemskatBundfradrag(int skatteAar)
		{
			return getValue(MethodBase.GetCurrentMethod().Name, skatteAar);
		}

		public decimal GetTopskatBundfradrag(int skatteAar)
		{
			return getValue(MethodBase.GetCurrentMethod().Name, skatteAar);
		}

		public decimal GetPositivNettoKapitalIndkomstGrundbeloeb(int skatteAar)
		{
			return getValue(MethodBase.GetCurrentMethod().Name, skatteAar);
		}

		public decimal GetNegativNettoKapitalIndkomstGrundbeloeb(int skatteAar)
		{
			return getValue(MethodBase.GetCurrentMethod().Name, skatteAar);
		}

		public decimal GetNegativNettoKapitalIndkomstSats(int skatteAar)
		{
			return getValue(MethodBase.GetCurrentMethod().Name, skatteAar);
		}

		public decimal GetBeskaeftigelsesfradragGrundbeloeb(int skatteAar)
		{
			return getValue(MethodBase.GetCurrentMethod().Name, skatteAar);
		}

		public decimal GetBeskaeftigelsesfradragSats(int skatteAar)
		{
			return getValue(MethodBase.GetCurrentMethod().Name, skatteAar);
		}

		public decimal GetGroenCheckPrVoksen(int skatteAar)
		{
			return getValue(MethodBase.GetCurrentMethod().Name, skatteAar);
		}

		public decimal GetGroenCheckPrBarn(int skatteAar)
		{
			return getValue(MethodBase.GetCurrentMethod().Name, skatteAar);
		}

		public decimal GetGroenCheckAftrapningssats(int skatteAar)
		{
			return getValue(MethodBase.GetCurrentMethod().Name, skatteAar);
		}

		public decimal GetGroenCheckBundfradrag(int skatteAar)
		{
			return getValue(MethodBase.GetCurrentMethod().Name, skatteAar);
		}
	}
}