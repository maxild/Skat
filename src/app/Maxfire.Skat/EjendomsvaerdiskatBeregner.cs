using System;
using System.Collections.Generic;
using System.Linq;
using Maxfire.Skat.Extensions;

namespace Maxfire.Skat
{
	public class EjendomsvaerdiskatBeregner
	{
		private readonly IEjendomsskattelovRegistry _registry;

		public EjendomsvaerdiskatBeregner(IEjendomsskattelovRegistry registry)
		{
			_registry = registry;
		}

		// Princippet betyder, at for samlevende ægtefæller vil ejendomsværdiskatten 
		// for en fælles ejet bolig som udgangspunkt skulle henføres med halvdelen til hver.

		// Overenskomst, der kan oprettes mellem flere købere, f.eks. ugifte samlevende 
		// ved køb af fast ejendom, og som f.eks. fastlægger fordelingen af omkostninger m.v., 
		// og fordeling af eventuelt provenu ved salg. 
		
		// 'Ugifte samlevende' vs ægtefæller

		// Input:
		//
		// OBS: Hvordan ved vi om personerne er gift, idet ugifte godt kan eje bolig sammen????
		public ValueTuple<decimal> BeregnSkat(
			IEjendomsoplysninger ejendomsoplysninger, 
			IValueTuple<IPerson> ejere, 
			ValueTuple<decimal> ejerandele, 
			bool gift, 
			IValueTuple<IPersonligeBeloeb> indkomster, 
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

			// To overgangsordninger, der gælder såfremt både køber og sælger har 
			// underskrevet en købsaftale senest denne dato.
			decimal nedslagVedKoebSenest01071998 = 0;
			if (ejendomsoplysninger.IsKoebtEfter01071998 == false)
			{
				decimal nedslagssatsP6 = _registry.GetNedslagssatsForParagraf6(skatteAar);
				decimal nedslagP6 = nedslagssatsP6 * ejendomsvaerdi;
				decimal nedslagP7 = 0;
				// Ejere af ejerlejligheder og ejere af fredede ejendomme, der foretager 
				// fradrag efter ligningslovens § 15 K, ikke er omfattet af P7-nedslag.
				if (ejendomsoplysninger.IsEjerlejlighed == false)
				{
					decimal nedslagssatsP7 = _registry.GetNedslagssatsForParagraf7(skatteAar);
					decimal maksNedslagP7 = _registry.GetMaksimaltNedslagForParagraf7(skatteAar);
					nedslagP7 = (nedslagssatsP7 * ejendomsvaerdi).Loft(maksNedslagP7);
				}
				nedslagVedKoebSenest01071998 = nedslagP6 + nedslagP7;
			}

			// Særligt indkomstafhængigt nedslag for alderspensionister (..afhænger af om ejere er gift eller ugift!!!)
			ValueTuple<decimal> pensionistNedslag;

			var personligIndkomst = indkomster.Map(x => x.Skattegrundlag.PersonligIndkomst);
			var nettoKapitalIndkomst = indkomster.Map(x => x.Skattegrundlag.NettoKapitalIndkomst);
			var aktieIndkomst = indkomster.Map(x => x.Skattegrundlag.AktieIndkomst);

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
				// Ugifte samlevende/samejende pensionister opnår kun nedslag for deres ejerandel!!!!
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

			// TODO: Begrænsningsreglerne § 9, § 9 a og § 9 b er ikke implementeret pga. skattestoppet
			
			return fordelingsnoegle * (ejendomsvaerdiskat - nedslagVedKoebSenest01071998) - pensionistNedslag;
		}

		private static bool skalHavePensionistNedslag(IValueTuple<IPerson> ejere, int skatteAar)
		{
			return ejere.Any(ejer => skalHavePensionistNedslag(ejer, skatteAar));
		}

		private static bool skalHavePensionistNedslag(IPerson person, int skatteAar)
		{
			// Pensionsalderen nedsættes til 65 år for personer, der fylder 60 år den 1. juli 1999, eller senere.
			int aldersgraense = person.GetAlder(new DateTime(1999, 7, 1)) <= 60 ? 65 : 67;
			return person.GetAlder(skatteAar) >= aldersgraense;
		}
	}

	// Grundværdi, såfremt grundskyld skal kunne beregnes. Grundskyld betales ikke over selvangivelsen men opkræves via indbetalingskort
	public interface IEjendomsoplysninger
	{
		/// <summary>
		/// Grundlaget for ejendomsværdiskatten
		/// </summary>
		decimal Ejendomsvaerdi { get; }

		/// <summary>
		/// Ejendomme hvor købsaftale er underskrevet af både køber og sælger d. 1/7-1998, 
		/// eller tidligere, opnår nedslag i ejendomsværdiskatten.
		/// </summary>
		bool IsKoebtEfter01071998 { get; }

		/// <summary>
		/// Pensionister opnår nedslag på fritidsboliger
		/// </summary>
		bool IsFritidsbolig { get; }
		
		/// <summary>
		/// Ejendomme hvor købsaftale er underskrevet af både køber og sælger d. 1/7-1998, 
		/// eller tidligere, opnår et yderligere nedslag i ejendomsværdiskatten, hvis der 
		/// ikke er tale om ejerlejligheder.
		/// </summary>
		bool IsEjerlejlighed { get; }
	}

	public interface IEjendomsskattelovRegistry
	{
		decimal GetSkattesatsForUnderProgressionsgraense(decimal skatteAar);
		decimal GetSkattesatsForOverProgressionsgraense(decimal skatteAar);

		/// <summary>
		/// Aflæs progressionsgrænsen for beregning af ejendomsværdiskat.
		/// (Ejendomsværdiskatteloven § 5, stk. 1)
		/// </summary>
		decimal GetProgressionsgraense(decimal skatteAar);

		/// <summary>
		/// Aflæs det procentuelle nedslag i ejendomsværdiskatten for ejerboliger erhvervet den 1. juli 1998 eller tidligere.
		/// (Ejendomsværdiskatteloven § 6, stk. 1)
		/// </summary>
		decimal GetNedslagssatsForParagraf6(decimal skatteAar);

		/// <summary>
		/// Aflæs det maksimale nedslag i ejendomsværdiskatten for ejerboliger erhvervet den 1. juli 1998 eller tidligere.
		/// (Ejendomsværdiskatteloven § 7, stk. 1)
		/// </summary>
		decimal GetMaksimaltNedslagForParagraf7(decimal skatteAar);

		/// <summary>
		/// Aflæs det procentuelle nedslag i ejendomsværdiskatten for ejerboliger erhvervet den 1. juli 1998 eller tidligere.
		/// (Ejendomsværdiskatteloven § 7, stk. 1)
		/// </summary>
		decimal GetNedslagssatsForParagraf7(decimal skatteAar);
	}
}