using System;
using System.Collections.Generic;

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
        private static readonly IDictionary<string, decimal[]> REGISTRY = new Dictionary<string, decimal[]>
        {
            {nameof(GetAktieIndkomstLavesteProgressionsgraense),  Values(48300) },
            {nameof(GetAktieIndkomstHoejesteProgressionsgraense), Values(106100, decimal.MaxValue) },   // Bortfalder fra og med 2010
			{nameof(GetAktieIndkomstLavesteSkattesats),           Values(0.28m, 0.28m, 0.28m, 0.27m) },
            {nameof(GetAktieIndkomstMellemsteSkattesats),         Values(0.43m, 0.42m) },
            {nameof(GetAktieIndkomstHoejesteSkattesats),          Values(0.45m, 0) },                   // Bortfalder fra og med 2010

			{nameof(GetAMBidragSkattesats),                       Values(0.08m) },

            {nameof(GetSundhedsbidragSkattesats),                 Values(0.08m,   0.08m,   0.08m,   0.07m,   0.06m,   0.05m,   0.04m,   0.03m,   0.02m,   0.01m,   0) },

            {nameof(GetBundSkattesats),                           Values(0.0504m, 0.0367m, 0.0367m, 0.0467m, 0.0567m, 0.0667m, 0.0767m, 0.0867m, 0.0967m, 0.1067m, 0.1167m) },

            {nameof(GetMellemSkattesats),                         Values(0.06m, 0) },                 // Bortfalder fra og med 2010
			{nameof(GetMellemskatBundfradrag),                    Values(347200, decimal.MaxValue) }, // Bortfalder fra og med 2010

			{nameof(GetTopSkattesats),                            Values(0.15m) },
            {nameof(GetTopskatBundfradrag),                       Values(347200, 389900, 409100) },
            {nameof(GetPositivNettoKapitalIndkomstGrundbeloeb),   Values(0, 40000) },                 // Indført fra og med 2010

			{nameof(GetSkatteloftSkattesats),                     Values(0.59m, 0.515m) },

            {nameof(GetPersonfradragForVoksne),                   Values(42900) },
            {nameof(GetPersonfradragForBoern),                    Values(32200) },

			// Beløb i kompensationsordningen (2012-2019) svarer til de beløbsgrænser, der ville være
			// gældende i 2010 uden Forårspakke 2.0 og med en opregulering fra 2009 til 2010 på 4,5 pct.
			{nameof(GetBundLettelseBundfradragForVoksne),         Values(decimal.MaxValue, decimal.MaxValue, decimal.MaxValue, 44800) },  // De 44.800 svarer til afrundet_til_100(42900 * 1,045)
			{nameof(GetBundLettelseBundfradragForBoern),          Values(decimal.MaxValue, decimal.MaxValue, decimal.MaxValue, 33600) },  // De 33.600 svarer til afrundet_til_100(32200 * 1,045)
			{nameof(GetMellemLettelseBundfradrag),                Values(decimal.MaxValue, decimal.MaxValue, decimal.MaxValue, 362800) }, // De 362.800 svarer til afrundet_til_100(347.200 * 1,045)
			{nameof(GetTopLettelseBundfradrag),                   Values(decimal.MaxValue, decimal.MaxValue, decimal.MaxValue, 362800) }, // De 362.800 svarer til afrundet_til_100(347.200 * 1,045)
			{nameof(GetPersonfradragSkaerpelseForVoksne),         Values(0, 0, 0, 1900) }, // De 1.900 svarer til afrundet_til_100(42900 * 0,045)
			{nameof(GetPersonfradragSkaerpelseForBoern),          Values(0, 0, 0, 1400) }, // De 1.400 svarer til afrundet_til_100(32200 * 0,045)

			// Nedslag for negativ nettokapitalindkomst (2012-2019)
			{nameof(GetNegativNettoKapitalIndkomstGrundbeloeb),   Values(0, 0, 0, 50000) }, // beløbsgrænsen reguleres ikke efter 2012
			{nameof(GetNegativNettoKapitalIndkomstSats),          Values(0, 0, 0, 0.01m, 0.02m, 0.03m, 0.04m, 0.05m, 0.06m, 0.07m, 0.08m) },

            {nameof(GetBeskaeftigelsesfradragGrundbeloeb),        Values(13600,   13600,   13600,   14100,  14400,  14900,   15400,  16000, 16600,  17300,  17900) },
            {nameof(GetBeskaeftigelsesfradragSats),               Values(0.0425m, 0.0425m, 0.0425m, 0.044m, 0.045m, 0.0465m, 0.048m, 0.05m, 0.052m, 0.054m, 0.056m) },

            {nameof(GetGroenCheckPrVoksen),                       Values(0, 1300) },
            {nameof(GetGroenCheckPrBarn),                         Values(0, 300) },
            {nameof(GetGroenCheckAftrapningssats),                Values(0, 0.075m) },
            {nameof(GetGroenCheckBundfradrag),                    Values(decimal.MaxValue, 362800) }
        };

        private static decimal[] Values(params decimal[] values)
        {
            const int SIZE = MAX_SKATTEAAR - MIN_SKATTEAAR + 1;
            var array = new decimal[SIZE];
            int i = 0;
            for (; i < Math.Min(SIZE, values.Length); i++)
            {
                array[i] = values[i];
            }
            decimal lastValue = values[values.Length - 1];
            for (; i < SIZE; i++)
            {
                array[i] = lastValue;
            }
            return array;
        }

        static decimal GetValue(string methodName, int skatteAar)
        {
            CheckSkatteaar(skatteAar);
            decimal[] valueArray;
            if (REGISTRY.TryGetValue(methodName, out valueArray) == false)
            {
                throw new InvalidOperationException(string.Format("{1} er konfigureret forkert, idet {0} er en ukendt metode --- check alle unit tests af {1}", methodName, typeof(DefaultSkattelovRegistry).Name));
            }
            return valueArray[skatteAar - 2009];
        }

        static void CheckSkatteaar(int skatteAar)
        {
            if (skatteAar < MIN_SKATTEAAR || skatteAar > MAX_SKATTEAAR)
                throw new ArgumentOutOfRangeException(nameof(skatteAar), skatteAar,
                    $"Beløbsgrænser eller skattesatser kan ikke aflæses for skatteår udenfor intervallet {MIN_SKATTEAAR}..{MAX_SKATTEAAR}.");
        }

        public decimal GetPersonfradrag(int skatteAar, int alder, bool gift)
        {
            return IsGiftOrVoksen(gift, alder) ? GetPersonfradragForVoksne(skatteAar) : GetPersonfradragForBoern(skatteAar);
        }

        public decimal GetBundLettelseBundfradrag(int skatteAar, int alder, bool gift)
        {
            return IsGiftOrVoksen(gift, alder) ? GetBundLettelseBundfradragForVoksne(skatteAar) : GetBundLettelseBundfradragForBoern(skatteAar);
        }

        public decimal GetPersonfradragSkaerpelse(int skatteAar, int alder, bool gift)
        {
            return IsGiftOrVoksen(gift, alder) ? GetPersonfradragSkaerpelseForVoksne(skatteAar) : GetPersonfradragSkaerpelseForBoern(skatteAar);
        }

        static bool IsGiftOrVoksen(bool gift, int alder)
        {
            return gift || alder >= 18;
        }

        public decimal GetAktieIndkomstLavesteProgressionsgraense(int skatteAar)
        {
            return GetValue(nameof(GetAktieIndkomstLavesteProgressionsgraense), skatteAar);
        }

        public decimal GetAktieIndkomstHoejesteProgressionsgraense(int skatteAar)
        {
            return GetValue(nameof(GetAktieIndkomstHoejesteProgressionsgraense), skatteAar);
        }

        public decimal GetAktieIndkomstLavesteSkattesats(int skatteAar)
        {
            return GetValue(nameof(GetAktieIndkomstLavesteSkattesats), skatteAar);
        }

        public decimal GetAktieIndkomstMellemsteSkattesats(int skatteAar)
        {
            return GetValue(nameof(GetAktieIndkomstMellemsteSkattesats), skatteAar);
        }

        public decimal GetAktieIndkomstHoejesteSkattesats(int skatteAar)
        {
            return GetValue(nameof(GetAktieIndkomstHoejesteSkattesats), skatteAar);
        }

        public decimal GetAMBidragSkattesats(int skatteAar)
        {
            return GetValue(nameof(GetAMBidragSkattesats), skatteAar);
        }

        public decimal GetSundhedsbidragSkattesats(int skatteAar)
        {
            return GetValue(nameof(GetSundhedsbidragSkattesats), skatteAar);
        }

        public decimal GetBundSkattesats(int skatteAar)
        {
            return GetValue(nameof(GetBundSkattesats), skatteAar);
        }

        public decimal GetMellemSkattesats(int skatteAar)
        {
            return GetValue(nameof(GetMellemSkattesats), skatteAar);
        }

        public decimal GetTopSkattesats(int skatteAar)
        {
            return GetValue(nameof(GetTopSkattesats), skatteAar);
        }

        public decimal GetSkatteloftSkattesats(int skatteAar)
        {
            return GetValue(nameof(GetSkatteloftSkattesats), skatteAar);
        }

        static decimal GetPersonfradragForVoksne(int skatteAar)
        {
            return GetValue(nameof(GetPersonfradragForVoksne), skatteAar);
        }

        static decimal GetPersonfradragForBoern(int skatteAar)
        {
            return GetValue(nameof(GetPersonfradragForBoern), skatteAar);
        }

        static decimal GetPersonfradragSkaerpelseForVoksne(int skatteAar)
        {
            return GetValue(nameof(GetPersonfradragSkaerpelseForVoksne), skatteAar);
        }

        static decimal GetPersonfradragSkaerpelseForBoern(int skatteAar)
        {
            return GetValue(nameof(GetPersonfradragSkaerpelseForBoern), skatteAar);
        }

        static decimal GetBundLettelseBundfradragForVoksne(int skatteAar)
        {
            return GetValue(nameof(GetBundLettelseBundfradragForVoksne), skatteAar);
        }

        static decimal GetBundLettelseBundfradragForBoern(int skatteAar)
        {
            return GetValue(nameof(GetBundLettelseBundfradragForBoern), skatteAar);
        }

        public decimal GetMellemLettelseBundfradrag(int skatteAar)
        {
            return GetValue(nameof(GetMellemLettelseBundfradrag), skatteAar);
        }

        public decimal GetTopLettelseBundfradrag(int skatteAar)
        {
            return GetValue(nameof(GetTopLettelseBundfradrag), skatteAar);
        }

        public decimal GetMellemskatBundfradrag(int skatteAar)
        {
            return GetValue(nameof(GetMellemskatBundfradrag), skatteAar);
        }

        public decimal GetTopskatBundfradrag(int skatteAar)
        {
            return GetValue(nameof(GetTopskatBundfradrag), skatteAar);
        }

        public decimal GetPositivNettoKapitalIndkomstGrundbeloeb(int skatteAar)
        {
            return GetValue(nameof(GetPositivNettoKapitalIndkomstGrundbeloeb), skatteAar);
        }

        public decimal GetNegativNettoKapitalIndkomstGrundbeloeb(int skatteAar)
        {
            return GetValue(nameof(GetNegativNettoKapitalIndkomstGrundbeloeb), skatteAar);
        }

        public decimal GetNegativNettoKapitalIndkomstSats(int skatteAar)
        {
            return GetValue(nameof(GetNegativNettoKapitalIndkomstSats), skatteAar);
        }

        public decimal GetBeskaeftigelsesfradragGrundbeloeb(int skatteAar)
        {
            return GetValue(nameof(GetBeskaeftigelsesfradragGrundbeloeb), skatteAar);
        }

        public decimal GetBeskaeftigelsesfradragSats(int skatteAar)
        {
            return GetValue(nameof(GetBeskaeftigelsesfradragSats), skatteAar);
        }

        public decimal GetGroenCheckPrVoksen(int skatteAar)
        {
            return GetValue(nameof(GetGroenCheckPrVoksen), skatteAar);
        }

        public decimal GetGroenCheckPrBarn(int skatteAar)
        {
            return GetValue(nameof(GetGroenCheckPrBarn), skatteAar);
        }

        public decimal GetGroenCheckAftrapningssats(int skatteAar)
        {
            return GetValue(nameof(GetGroenCheckAftrapningssats), skatteAar);
        }

        public decimal GetGroenCheckBundfradrag(int skatteAar)
        {
            return GetValue(nameof(GetGroenCheckBundfradrag), skatteAar);
        }
    }
}
