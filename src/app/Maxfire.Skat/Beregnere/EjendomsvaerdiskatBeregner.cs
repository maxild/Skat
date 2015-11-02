using System;
using System.Collections.Generic;
using System.Linq;
using Maxfire.Skat.Extensions;

namespace Maxfire.Skat.Beregnere
{
	public class EjendomsvaerdiskatBeregner
	{
		private readonly IEjendomsskattelovRegistry _registry;

		public EjendomsvaerdiskatBeregner(IEjendomsskattelovRegistry registry)
		{
			_registry = registry;
		}

		// Princippet betyder, at for samlevende �gtef�ller vil ejendomsv�rdiskatten 
		// for en f�lles ejet bolig som udgangspunkt skulle henf�res med halvdelen til hver.

		// Overenskomst, der kan oprettes mellem flere k�bere, f.eks. ugifte samlevende 
		// ved k�b af fast ejendom, og som f.eks. fastl�gger fordelingen af omkostninger m.v., 
		// og fordeling af eventuelt provenu ved salg. 
		
		// 'Ugifte samlevende' vs �gtef�ller

		// Input:
		//
		// OBS: Hvordan ved vi om personerne er gift, idet ugifte godt kan eje bolig sammen????
		public ValueTuple<decimal> BeregnSkat(
			IEjendomsoplysninger ejendomsoplysninger, 
			IValueTuple<ISkatteyder> ejere, 
			ValueTuple<decimal> ejerandele, 
			bool gift, 
			IValueTuple<IPersonligeIndkomster> indkomster, 
			int skatteAar)
		{
			decimal lavsats  = _registry.GetSkattesatsForUnderProgressionsgraense(skatteAar);
			decimal hoejsats = _registry.GetSkattesatsForOverProgressionsgraense(skatteAar) - lavsats;
			decimal progressionsgraense = _registry.GetProgressionsgraense(skatteAar);
			decimal ejendomsvaerdi = ejendomsoplysninger.Ejendomsvaerdi;
			var fordelingsnoegle = ejerandele.NormalizeAndele();

			decimal ejendomsvaerdiskatAfLav = lavsats * ejendomsvaerdi;
			decimal ejendomsvaerdiskatAfHoej = hoejsats * ejendomsvaerdi.DifferenceGreaterThan(progressionsgraense);
			decimal ejendomsvaerdiskat = ejendomsvaerdiskatAfLav + ejendomsvaerdiskatAfHoej;

			// To overgangsordninger, der g�lder s�fremt b�de k�ber og s�lger har 
			// underskrevet en k�bsaftale senest denne dato.
			decimal nedslagVedKoebSenest01071998 = 0;
			if (ejendomsoplysninger.IsKoebtEfter01071998 == false)
			{
				decimal nedslagssatsP6 = _registry.GetNedslagssatsForParagraf6(skatteAar);
				decimal nedslagP6 = nedslagssatsP6 * ejendomsvaerdi;
				decimal nedslagP7 = 0;
				// Ejere af ejerlejligheder og ejere af fredede ejendomme, der foretager 
				// fradrag efter ligningslovens � 15 K, ikke er omfattet af P7-nedslag.
				if (ejendomsoplysninger.IsEjerlejlighed == false)
				{
					decimal nedslagssatsP7 = _registry.GetNedslagssatsForParagraf7(skatteAar);
					decimal maksNedslagP7 = _registry.GetMaksimaltNedslagForParagraf7(skatteAar);
					nedslagP7 = (nedslagssatsP7 * ejendomsvaerdi).Loft(maksNedslagP7);
				}
				nedslagVedKoebSenest01071998 = nedslagP6 + nedslagP7;
			}

			// S�rligt indkomstafh�ngigt nedslag for alderspensionister (..afh�nger af om ejere er gift eller ugift!!!)
			ValueTuple<decimal> pensionistNedslag;

			var personligIndkomst = indkomster.Map(x => x.PersonligIndkomstSkattegrundlag);
			var nettoKapitalIndkomst = indkomster.Map(x => x.NettoKapitalIndkomstSkattegrundlag);
			var aktieIndkomst = indkomster.Map(x => x.AktieIndkomst);

			decimal maksPensionstNedslag = ejendomsoplysninger.IsFritidsbolig ? 2000 : 6000;
			decimal pensionistNedslagFoerReduktion = (0.004m * ejendomsvaerdi).Loft(maksPensionstNedslag);
			
			if (gift)
			{
				pensionistNedslag = 0m.ToTupleOfSize(ejere.Size);
				if (skalHavePensionistNedslag(ejere, skatteAar))
				{
					decimal grundlag = personligIndkomst.Sum() + nettoKapitalIndkomst.Sum().NonNegative()
					                   + aktieIndkomst.Sum().DifferenceGreaterThan(10000);
					decimal reduktionAfPensionistNedslag = 0.05m * grundlag.DifferenceGreaterThan(268600);


					decimal samletPensionistNedslag = pensionistNedslagFoerReduktion.DifferenceGreaterThan(reduktionAfPensionistNedslag);
					pensionistNedslag = fordelingsnoegle * samletPensionistNedslag;
				}
			}
			else
			{
				// Ugifte samlevende/samejende pensionister opn�r kun nedslag for deres ejerandel!!!!
				// TODO: skalHave... filterer til nul i chain
				var nedslagPrEjer = new List<decimal>();
				for (int i = 0; i < ejere.Size; i++)
				{
					var ejer = ejere[i];
					if (skalHavePensionistNedslag(ejer, skatteAar))
					{
						decimal grundlag = personligIndkomst[i] + nettoKapitalIndkomst[i].NonNegative()
										   + aktieIndkomst[i].DifferenceGreaterThan(5000);
						decimal reduktionAfPensionistNedslag = 0.05m * grundlag.DifferenceGreaterThan(174600);

						nedslagPrEjer[i] = pensionistNedslagFoerReduktion.DifferenceGreaterThan(reduktionAfPensionistNedslag);
					}
				}
				pensionistNedslag = new ValueTuple<decimal>(nedslagPrEjer);
			}

			// TODO: Begr�nsningsreglerne � 9, � 9 a og � 9 b er ikke implementeret pga. skattestoppet
			
			return fordelingsnoegle * (ejendomsvaerdiskat - nedslagVedKoebSenest01071998) - pensionistNedslag;
		}

		private static bool skalHavePensionistNedslag(IEnumerable<ISkatteyder> ejere, int skatteAar)
		{
			return ejere.Any(ejer => skalHavePensionistNedslag(ejer, skatteAar));
		}

		private static bool skalHavePensionistNedslag(ISkatteyder skatteyder, int skatteAar)
		{
			// Pensionsalderen neds�ttes til 65 �r for personer, der fylder 60 �r den 1. juli 1999, eller senere.
			int aldersgraense = skatteyder.GetAlder(new DateTime(1999, 7, 1)) <= 60 ? 65 : 67;
			return skatteyder.GetAlder(skatteAar) >= aldersgraense;
		}
	}

	// Grundv�rdi, s�fremt grundskyld skal kunne beregnes. Grundskyld betales ikke over selvangivelsen men opkr�ves via indbetalingskort
	public interface IEjendomsoplysninger
	{
		/// <summary>
		/// Grundlaget for ejendomsv�rdiskatten
		/// </summary>
		decimal Ejendomsvaerdi { get; }

		/// <summary>
		/// Ejendomme hvor k�bsaftale er underskrevet af b�de k�ber og s�lger d. 1/7-1998, 
		/// eller tidligere, opn�r nedslag i ejendomsv�rdiskatten.
		/// </summary>
		bool IsKoebtEfter01071998 { get; }

		/// <summary>
		/// Pensionister opn�r nedslag p� fritidsboliger
		/// </summary>
		bool IsFritidsbolig { get; }
		
		/// <summary>
		/// Ejendomme hvor k�bsaftale er underskrevet af b�de k�ber og s�lger d. 1/7-1998, 
		/// eller tidligere, opn�r et yderligere nedslag i ejendomsv�rdiskatten, hvis der 
		/// ikke er tale om ejerlejligheder.
		/// </summary>
		bool IsEjerlejlighed { get; }
	}

	public interface IEjendomsskattelovRegistry
	{
		decimal GetSkattesatsForUnderProgressionsgraense(decimal skatteAar);
		decimal GetSkattesatsForOverProgressionsgraense(decimal skatteAar);

		/// <summary>
		/// Afl�s progressionsgr�nsen for beregning af ejendomsv�rdiskat.
		/// (Ejendomsv�rdiskatteloven � 5, stk. 1)
		/// </summary>
		decimal GetProgressionsgraense(decimal skatteAar);

		/// <summary>
		/// Afl�s det procentuelle nedslag i ejendomsv�rdiskatten for ejerboliger erhvervet den 1. juli 1998 eller tidligere.
		/// (Ejendomsv�rdiskatteloven � 6, stk. 1)
		/// </summary>
		decimal GetNedslagssatsForParagraf6(decimal skatteAar);

		/// <summary>
		/// Afl�s det maksimale nedslag i ejendomsv�rdiskatten for ejerboliger erhvervet den 1. juli 1998 eller tidligere.
		/// (Ejendomsv�rdiskatteloven � 7, stk. 1)
		/// </summary>
		decimal GetMaksimaltNedslagForParagraf7(decimal skatteAar);

		/// <summary>
		/// Afl�s det procentuelle nedslag i ejendomsv�rdiskatten for ejerboliger erhvervet den 1. juli 1998 eller tidligere.
		/// (Ejendomsv�rdiskatteloven � 7, stk. 1)
		/// </summary>
		decimal GetNedslagssatsForParagraf7(decimal skatteAar);
	}
}