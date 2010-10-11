using System;
using Maxfire.Skat.Beregnere;
using Maxfire.Skat.Extensions;
using Maxfire.TestCommons.AssertExtensions;
using Xunit;

namespace Maxfire.Skat.UnitTests
{
	/// <summary>
	/// Eksempler fra SKATs publikation "Beregning af personlige indkomstskatter mv. 2009"
	/// </summary>
	public class Eksempler
	{
		const int SKATTE_AAR = 2009;
		private readonly ISkattelovRegistry _skattelovRegistry;

		public Eksempler()
		{
			_skattelovRegistry = new DefaultSkattelovRegistry();
		}

		[Fact]
		public void Eksempel_2_Bundskattegrundlag_Gifte()
		{
			var indkomster = new ValueTuple<IPersonligeIndkomster>(
				new FakePersonligeIndkomster
				{
					PersonligIndkomst = 100000,
					NettoKapitalIndkomst = -50000,
				},
				new FakePersonligeIndkomster
				{
					PersonligIndkomst = 400000,
					NettoKapitalIndkomst = 60000
				});

			var bundskatGrundlagBeregner = new BundskatBeregner(_skattelovRegistry);
			var bundskatGrundlag = bundskatGrundlagBeregner.BeregnBruttoGrundlag(indkomster);

			bundskatGrundlag[0].ShouldEqual(100000);
			bundskatGrundlag[1].ShouldEqual(410000);
		}

		[Fact]
		public void Eksempel_3_Mellemskattegrundlag_Gifte()
		{
			var indkomster = new ValueTuple<IPersonligeIndkomster>(
				new FakePersonligeIndkomster
				{
					PersonligIndkomst = 100000,
					NettoKapitalIndkomst = -50000,
				},
				new FakePersonligeIndkomster
				{
					PersonligIndkomst = 600000,
					NettoKapitalIndkomst = 80000
				});

			var mellemskatGrundlagBeregner = new MellemskatBeregner(_skattelovRegistry);

			var grundlagFoerBundfradrag = mellemskatGrundlagBeregner.BeregnBruttoGrundlag(indkomster);
			var udnyttetBundfradrag = mellemskatGrundlagBeregner.BeregnSambeskattetBundfradrag(indkomster, SKATTE_AAR);
			var grundlag = mellemskatGrundlagBeregner.BeregnGrundlag(indkomster, SKATTE_AAR);

			grundlagFoerBundfradrag[0].ShouldEqual(100000);
			grundlagFoerBundfradrag[1].ShouldEqual(630000);
			udnyttetBundfradrag[0].ShouldEqual(100000);
			udnyttetBundfradrag[1].ShouldEqual(594400);
			grundlag[0].ShouldEqual(0);
			grundlag[1].ShouldEqual(35600);
		}

		[Fact]
		public void Eksempel_4_Topskattegrundlag_Gifte()
		{
			const decimal indskudPaaPrivatTegnetKapitalPension = 32000;

			var selvangivneBeloeb = new ValueTuple<ISelvangivneBeloeb>(
				new SelvangivneBeloeb
				{
					Bruttoloen = 400000,
					PrivatTegnetKapitalPensionsindskud = indskudPaaPrivatTegnetKapitalPension,
					NettoKapitalIndkomst = 13500 + 25000 - 10000,
					LigningsmaessigtFradrag = 1400 // idet beskæftigelsesfradrag 13.600
				},
				new SelvangivneBeloeb
				{
					Bruttoloen = 100000,
					NettoKapitalIndkomst = 23500 - 5000,
					LigningsmaessigtFradrag = 3750 // idet beskæftigelsesfradrag 4,25 pct af 100.000
				});

			var indkomstOpgoerelseBeregner = new IndkomstOpgoerelseBeregner(_skattelovRegistry);
			var indkomster = indkomstOpgoerelseBeregner.BeregnIndkomster(selvangivneBeloeb, SKATTE_AAR);

			indkomster[0].PersonligIndkomst.ShouldEqual(368000 - indskudPaaPrivatTegnetKapitalPension);
			indkomster[0].NettoKapitalIndkomst.ShouldEqual(28500);
			indkomster[0].LigningsmaessigtFradrag.ShouldEqual(15000); // inkl. beregnet beskæftigelsesfradrag

			indkomster[1].PersonligIndkomst.ShouldEqual(92000);
			indkomster[1].NettoKapitalIndkomst.ShouldEqual(18500);
			indkomster[1].LigningsmaessigtFradrag.ShouldEqual(8000);  // inkl. beregnet beskæftigelsesfradrag

			var topskatBeregner = new TopskatBeregner(_skattelovRegistry);
			var topskat = topskatBeregner.BeregnSkat(indkomster, SKATTE_AAR, getKommunaleSatserForGifte());

			topskat[0].Topskat.ShouldEqual(7395);
			topskat[1].Topskat.ShouldEqual(2775);
		}

		[Fact]
		public void Eksempel_5_SkatAfAktieindkomst()
		{
			// TODO
		}

		[Fact]
		public void Eksempel_6_IngenUnderskud_Ugift()
		{
			ValueTuple<IKommunaleSatser> kommunaleSatser = getKommunaleSatserForUgift();

			var skatteydere = getPersonerForUgift();

			// TODO: Lav den færdig
			var selvangivneBeloeb = new ValueTuple<ISelvangivneBeloeb>(
				new SelvangivneBeloeb
				{
					Bruttoloen = 463793,
					AtpEgetBidrag = 1080,
					PrivatTegnetRatePensionsindskud = 0, // eget bidrag til arbejdsgiver ordning
					PrivatTegnetKapitalPensionsindskud = 46000,
				});

			var indkomster = new ValueTuple<ISkatteIndkomster>(
				new FakeSkatteIndkomster
					{
						PersonligIndkomst = 358395,
						NettoKapitalIndkomst = 8500,
						LigningsmaessigtFradrag = 24600,
						KapitalPensionsindskud = 46000,
						SkattepligtigIndkomst = 342295
					});

			var skatterBeregner = new SkatBeregner(_skattelovRegistry);
			var skatter = skatterBeregner.BeregnSkat(skatteydere, indkomster, kommunaleSatser, SKATTE_AAR);
			
			// Skatter før modregning af underskud og personfradrag
			skatter[0].Bundskat
				.RoundMoney().ShouldEqual(18491.51m);
			skatter[0].Mellemskat
				.RoundMoney().ShouldEqual(1181.70m);
			skatter[0].Topskat
				.RoundMoney().ShouldEqual(9854.25m);
			skatter[0].Sundhedsbidrag
				.RoundMoney().ShouldEqual(27383.60m);
			skatter[0].KommunalIndkomstskatOgKirkeskat
				.RoundMoney().ShouldEqual(83143.46m);

			// Beregning af skatteværdier af personfradrag
			var personFradragBeregner = new PersonfradragBeregner(_skattelovRegistry);
			var skattevaerdier = personFradragBeregner.BeregnSkattevaerdierAfPersonfradrag(skatteydere, kommunaleSatser, SKATTE_AAR);

			skattevaerdier[0].Bundskat.ShouldEqual(2162.16m);
			skattevaerdier[0].Sundhedsbidrag.ShouldEqual(3432);
			skattevaerdier[0].KommunalIndkomstskatOgKirkeskat.ShouldEqual(10420.41m);
		}

		[Fact] 
		public void Eksempel_9_IngenUnderskud_Gifte()
		{
			var kommunaleSatser = getKommunaleSatserForGifte();
			var skatteydere = getPersonerForGifte();

			var indkomster = new ValueTuple<ISkatteIndkomster>(
				new FakeSkatteIndkomster
					{
						PersonligIndkomst = 388000,
						NettoKapitalIndkomst = 13500,
						LigningsmaessigtFradrag = 21600,
						KapitalPensionsindskud = 32000,
						SkattepligtigIndkomst = 379900
					},
				new FakeSkatteIndkomster
					{
						PersonligIndkomst = 150000,
						NettoKapitalIndkomst = 8500,
						LigningsmaessigtFradrag = 8180,
						SkattepligtigIndkomst = 150320
					});

			var skatterBeregner = new SkatBeregner(_skattelovRegistry);
			var skatter = skatterBeregner.BeregnSkat(skatteydere, indkomster, kommunaleSatser, SKATTE_AAR);

			// Skatter før modregning af underskud og personfradrag
			skatter[0].Bundskat
				.RoundMoney().ShouldEqual(20235.60m);
			skatter[0].Mellemskat
				.RoundMoney().ShouldEqual(0);
			skatter[0].Topskat
				.RoundMoney().ShouldEqual(10920 + 2025);
			skatter[0].Sundhedsbidrag
				.RoundMoney().ShouldEqual(30392);
			skatter[0].KommunalIndkomstskatOgKirkeskat
				.RoundMoney().ShouldEqual(92277.71m);

			skatter[1].Bundskat
				.RoundMoney().ShouldEqual(7988.40m);
			skatter[1].Mellemskat
				.RoundMoney().ShouldEqual(0);
			skatter[1].Topskat
				.RoundMoney().ShouldEqual(1275);
			skatter[1].Sundhedsbidrag
				.RoundMoney().ShouldEqual(12025.60m);
			skatter[1].KommunalIndkomstskatOgKirkeskat
				.RoundMoney().ShouldEqual(36512.73m);

			var personFradragBeregner = new PersonfradragBeregner(_skattelovRegistry);
			var skattevaerdier = personFradragBeregner.BeregnSkattevaerdierAfPersonfradrag(skatteydere, kommunaleSatser, SKATTE_AAR);

			skattevaerdier[0].Bundskat.ShouldEqual(2162.16m);
			skattevaerdier[1].Bundskat.ShouldEqual(2162.16m);
			skattevaerdier[0].Sundhedsbidrag.ShouldEqual(3432);
			skattevaerdier[1].Sundhedsbidrag.ShouldEqual(3432);
			skattevaerdier[0].KommunalIndkomstskatOgKirkeskat.ShouldEqual(10420.41m);
			skattevaerdier[1].KommunalIndkomstskatOgKirkeskat.ShouldEqual(10420.41m);
		}

		[Fact]
		public void Eksempel_10_GifteMedNegativNettoKapitalIndkomstOgUudnyttetBundfradrag()
		{
			const decimal indskudPaaPrivatTegnetKapitalPension = 32000;

			var kommunaleSatser = getKommunaleSatserForGifte();
			var skatteydere = getPersonerForGifte();

			var indkomster = new ValueTuple<ISkatteIndkomster>(
				new FakeSkatteIndkomster
				{
					PersonligIndkomst = 600000 - indskudPaaPrivatTegnetKapitalPension,
					NettoKapitalIndkomst = -11500,
					LigningsmaessigtFradrag = 21000,
					KapitalPensionsindskud = indskudPaaPrivatTegnetKapitalPension,
					SkattepligtigIndkomst = 535500
				},
				new FakeSkatteIndkomster
				{
					PersonligIndkomst = 150000,
					NettoKapitalIndkomst = 8500,
					LigningsmaessigtFradrag = 7772,
					SkattepligtigIndkomst = 150728
				});

			var skatterBeregner = new SkatBeregner(_skattelovRegistry);
			var skatter = skatterBeregner.BeregnSkat(skatteydere, indkomster, kommunaleSatser, SKATTE_AAR);

			// Skatter før modregning af underskud og personfradrag
			skatter[0].Bundskat
				.RoundMoney().ShouldEqual(28627.20m);
			skatter[0].Mellemskat
				.RoundMoney().ShouldEqual(1416);
			skatter[0].Topskat
				.RoundMoney().ShouldEqual(37920);
			skatter[0].Sundhedsbidrag
				.RoundMoney().ShouldEqual(42840);
			skatter[0].KommunalIndkomstskatOgKirkeskat
				.RoundMoney().ShouldEqual(130072.95m);

			skatter[1].Bundskat
				.RoundMoney().ShouldEqual(7560);
			skatter[1].Mellemskat
				.RoundMoney().ShouldEqual(0);
			skatter[1].Topskat
				.RoundMoney().ShouldEqual(0);
			skatter[1].Sundhedsbidrag
				.RoundMoney().ShouldEqual(12058.24m);
			skatter[1].KommunalIndkomstskatOgKirkeskat
				.RoundMoney().ShouldEqual(36611.83m);

			var personFradragBeregner = new PersonfradragBeregner(_skattelovRegistry);
			var skattevaerdier = personFradragBeregner.BeregnSkattevaerdierAfPersonfradrag(skatteydere, kommunaleSatser, SKATTE_AAR);

			skattevaerdier[0].Bundskat.ShouldEqual(2162.16m);
			skattevaerdier[1].Bundskat.ShouldEqual(2162.16m);
			skattevaerdier[0].Sundhedsbidrag.ShouldEqual(3432);
			skattevaerdier[1].Sundhedsbidrag.ShouldEqual(3432);
			skattevaerdier[0].KommunalIndkomstskatOgKirkeskat.ShouldEqual(10420.41m);
			skattevaerdier[1].KommunalIndkomstskatOgKirkeskat.ShouldEqual(10420.41m);
		}

		[Fact] 
		public void Eksempel_12_NegativSkattepligtigIndkomst_Ugift()
		{
			var skatteydere = getPersonerForUgift();

			var selvangivneBeloeb = new ValueTuple<ISelvangivneBeloeb>(
				new SelvangivneBeloeb
					{
						Pension = 360000,
						NettoKapitalIndkomst = -310000,
						LigningsmaessigtFradrag = 80000
					});

			var kommunaleSatser = getKommunaleSatserForUgift();

			var indkomstOpgoerelseBeregner = new IndkomstOpgoerelseBeregner(_skattelovRegistry);
			var indkomster = indkomstOpgoerelseBeregner.BeregnIndkomster(selvangivneBeloeb, SKATTE_AAR);

			// Skattepligtig indkomst før modregning
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(-30000);

			// OBS: PersonligIndkomstUnderskudBeregner ville blive eksekveret her, hvis P.I. var negativ

			var skatterAfPersonligIndkomstBeregner = new SkatterAfPersonligIndkomstBeregner(_skattelovRegistry);
			var skatterAfPersonligIndkomst = skatterAfPersonligIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser, SKATTE_AAR);
			
			// Skatter før modregning af underskud og personfradrag
			skatterAfPersonligIndkomst[0].Bundskat.ShouldEqual(18144);
			skatterAfPersonligIndkomst[0].Mellemskat.ShouldEqual(768);
			skatterAfPersonligIndkomst[0].Topskat.ShouldEqual(1920);
			
			var underskudBeregner = new SkattepligtigIndkomstUnderskudBeregner(_skattelovRegistry);
			var modregnResults = underskudBeregner.ModregningAfUnderskud(indkomster, skatterAfPersonligIndkomst, kommunaleSatser, SKATTE_AAR);
			var skatterAfPersonligIndkomstEfterModregningAfUnderskud = modregnResults.Map(x => x.ModregnedeSkatter);

			// Skattepligtig indkomst efter modregning
			indkomster[0].SkattepligtigIndkomstSkattegrundlag.ShouldEqual(0);
			
			// Ingen fremførsel af underskud idet hele skatteværdien kan rummes i bundskatten
			var underskudTilFremfoersel = modregnResults.Map(x => x.UnderskudTilFremfoersel);
			underskudTilFremfoersel[0].ShouldEqual(0);
			indkomster[0].SkattepligtigIndkomstUnderskudTilFremfoersel.ShouldEqual(0);

			// Behøver ikke reberegne, idet ændringer i ægtefælles skattepligtig indkomst, bliver medtaget i denne beregning
			var skatterAfSkattepligtigIndkomstBeregner = new SkatterAfSkattepligtigIndkomstBeregner(_skattelovRegistry);
			var skatterAfSkattepligtigIndkomst = skatterAfSkattepligtigIndkomstBeregner.BeregnSkat(skatteydere, indkomster, kommunaleSatser, SKATTE_AAR);

			var skatterFoerPersonfradrag = SkatteUtility.CombineSkat(skatterAfPersonligIndkomstEfterModregningAfUnderskud, skatterAfSkattepligtigIndkomst);

			// Skatter after modregning af underkud, men før modregning af personfradrag
			skatterFoerPersonfradrag[0].Bundskat.ShouldEqual(8457); // <--- modregning i bundskatten
			skatterFoerPersonfradrag[0].Mellemskat.ShouldEqual(768);
			skatterFoerPersonfradrag[0].Topskat.ShouldEqual(1920);

			var personfradragBeregner = new PersonfradragBeregner(_skattelovRegistry);
			var modregnPersonfradragResults = personfradragBeregner.ModregningAfPersonfradrag(skatteydere, skatterFoerPersonfradrag, kommunaleSatser, SKATTE_AAR);
			var skatterEfterPersonfradrag = modregnPersonfradragResults.Map(x => x.ModregnedeSkatter);

			skatterEfterPersonfradrag[0].ShouldEqual(IndkomstSkatter.Nul);

			var tabtVardiAfPersonfradrag = modregnPersonfradragResults.Map(x => x.IkkeUdnyttetSkattevaerdi);
			var tabtPersonfradrag = modregnPersonfradragResults.Map(x => x.IkkeUdnyttetFradrag);

			tabtVardiAfPersonfradrag[0].ShouldEqual(4869.57m);
			tabtPersonfradrag[0].ShouldEqual(13044.66m);
		}

		[Fact]
		public void Eksempel_13_HeleUnderskuddetFremfoeres_Ugift()
		{
			var selvangivneBeloeb = new ValueTuple<ISelvangivneBeloeb>(
				new SelvangivneBeloeb
				{
					NettoKapitalIndkomst = -55000,
					LigningsmaessigtFradrag = 4000
				});

			var skatteydere = getPersonerForUgift();
			var kommunaleSatser = getKommunaleSatserForUgift();

			var indkomstOpgoerelseBeregner = new IndkomstOpgoerelseBeregner(_skattelovRegistry);
			var indkomster = indkomstOpgoerelseBeregner.BeregnIndkomster(selvangivneBeloeb, SKATTE_AAR);

			// Skattepligtig indkomst før modregning og fremførsel
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(-59000);

			var skatAfPersonligIndkomstBeregner = new SkatterAfPersonligIndkomstBeregner(_skattelovRegistry);
			var skatterAfPersonligIndkomst = skatAfPersonligIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser, SKATTE_AAR);
			
			// Skatter før modregning af underskud og personfradrag er alle nul
			skatterAfPersonligIndkomst[0].ShouldEqual(IndkomstSkatterAfPersonligIndkomst.Nul);
			
			var underskudBeregner = new SkattepligtigIndkomstUnderskudBeregner(_skattelovRegistry);
			var modregnResults = underskudBeregner.ModregningAfUnderskud(indkomster, skatterAfPersonligIndkomst, kommunaleSatser, SKATTE_AAR);
			skatterAfPersonligIndkomst = modregnResults.Map(x => x.ModregnedeSkatter);
			
			// Skatter efter modregning af underskud i bundskat, mellemskat og topskat er alle nul
			skatterAfPersonligIndkomst[0].ShouldEqual(IndkomstSkatterAfPersonligIndkomst.Nul);

			// Skattepligtig indkomst efter modregning og fremførsel
			indkomster[0].SkattepligtigIndkomstSkattegrundlag.ShouldEqual(0);
			indkomster[0].SkattepligtigIndkomstUnderskudTilFremfoersel.ShouldEqual(59000);

			// Hele årets underskud i S.I fremføres
			var underskudTilFremfoersel = modregnResults.Map(x => x.UnderskudTilFremfoersel);
			underskudTilFremfoersel[0].ShouldEqual(59000);

			var skatAfSkattepligtigIndkomstBeregner = new SkatterAfSkattepligtigIndkomstBeregner(_skattelovRegistry);
			var skatterAfSkattepligtigIndkomst = skatAfSkattepligtigIndkomstBeregner.BeregnSkat(skatteydere, indkomster, kommunaleSatser, SKATTE_AAR);
			
			// Kommuneskat, kirkeskat og sundhedsbidrag er alle nul
			skatterAfSkattepligtigIndkomst[0].ShouldEqual(SkatterAfSkattepligtigIndkomst.Nul);
		}

		[Fact]
		public void Eksempel_14_DelvisModregningDelvisFremfoerselAfUnderskud_Ugift()
		{
			var skatteydere = getPersonerForUgift();

			var selvangivneBeloeb = new ValueTuple<ISelvangivneBeloeb>(
				new SelvangivneBeloeb
				{
					Pension = 360000,
					NettoKapitalIndkomst = -400000,
					LigningsmaessigtFradrag = 50000
				});

			var kommunaleSatser = getKommunaleSatserForUgift();

			var indkomstOpgoerelseBeregner = new IndkomstOpgoerelseBeregner(_skattelovRegistry);
			var indkomster = indkomstOpgoerelseBeregner.BeregnIndkomster(selvangivneBeloeb, SKATTE_AAR);

			// Skattepligtig indkomst før modregning og fremførsel
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(-90000);

			var skatterAfPersonligIndkomstBeregner = new SkatterAfPersonligIndkomstBeregner(_skattelovRegistry);
			var skatterAfPersonligIndkomst = skatterAfPersonligIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser, SKATTE_AAR);

			// Skatter før modregning af underskud og personfradrag
			skatterAfPersonligIndkomst[0].Bundskat.ShouldEqual(18144);
			skatterAfPersonligIndkomst[0].Mellemskat.ShouldEqual(768);
			skatterAfPersonligIndkomst[0].Topskat.ShouldEqual(1920);
			
			var underskudBeregner = new SkattepligtigIndkomstUnderskudBeregner(_skattelovRegistry);
			var modregnSkatteplitigIndkomstResults = underskudBeregner.ModregningAfUnderskud(indkomster, skatterAfPersonligIndkomst, kommunaleSatser, SKATTE_AAR);
			var underskudTilFremfoersel = modregnSkatteplitigIndkomstResults.Map(x => x.UnderskudTilFremfoersel);
			var modregningUnderskud = modregnSkatteplitigIndkomstResults.Map(x => x.ModregningUnderskud);
			
			// Skattepligtig indkomst efter modregning og fremførsel
			indkomster[0].SkattepligtigIndkomstSkattegrundlag.ShouldEqual(0);
			indkomster[0].SkattepligtigIndkomstUnderskudTilFremfoersel.ShouldEqual(25484.67m);
			underskudTilFremfoersel[0].ShouldEqual(25484.67m);
			modregningUnderskud[0].ShouldEqual(64515.33m);
			
			// Dette er skatter efter modregning af underskud i skattepligtig indkomst
			skatterAfPersonligIndkomst = modregnSkatteplitigIndkomstResults.Map(x => x.ModregnedeSkatter);

			var skatAfSkattepligtigIndkomstBeregner = new SkatterAfSkattepligtigIndkomstBeregner(_skattelovRegistry);
			var skatterAfSkattepligtigIndkomst = skatAfSkattepligtigIndkomstBeregner.BeregnSkat(skatteydere, indkomster, kommunaleSatser, SKATTE_AAR);

			var skatterFoerPersonfradrag = new IndkomstSkatter(skatterAfPersonligIndkomst[0], skatterAfSkattepligtigIndkomst[0]).ToTuple();

			skatterFoerPersonfradrag[0].ShouldEqual(IndkomstSkatter.Nul);

			var personfradragBeregner = new PersonfradragBeregner(_skattelovRegistry);
			var modregnPersonfradragResults = personfradragBeregner.ModregningAfPersonfradrag(skatteydere, skatterFoerPersonfradrag, kommunaleSatser, SKATTE_AAR);
			var skatterEfterPersonfradrag = modregnPersonfradragResults.Map(x => x.ModregnedeSkatter);
			var tabtPersonfradrag = modregnPersonfradragResults.Map(x => x.IkkeUdnyttetFradrag);

			// Hele personfradraget fortabes
			tabtPersonfradrag[0].ShouldEqual(_skattelovRegistry.GetPersonfradrag(SKATTE_AAR, skatteydere[0].GetAlder(SKATTE_AAR), false));
			skatterEfterPersonfradrag.ShouldEqual(skatterFoerPersonfradrag);
		}

		[Fact]
		public void Eksempel_15_FuldModregningAfFremfoertUnderskudOgIntetUnderskudTilFremfoersel_Ugift()
		{
			var skatteydere = getPersonerForUgift();
			
			var selvangivneBeloeb = new ValueTuple<ISelvangivneBeloeb>(
				new SelvangivneBeloeb
				{
					SkattepligtigIndkomstFremfoertUnderskud = 12000,
					Pension = 170000,
					NettoKapitalIndkomst = -30000,
					LigningsmaessigtFradrag = 10000
				});

			var kommunaleSatser = getKommunaleSatserForUgift();

			var indkomstOpgoerelseBeregner = new IndkomstOpgoerelseBeregner(_skattelovRegistry);
			var indkomster = indkomstOpgoerelseBeregner.BeregnIndkomster(selvangivneBeloeb, SKATTE_AAR);

			// Skattepligtig indkomst før modregning og fremførsel af underskud
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(130000);
			indkomster[0].SkattepligtigIndkomstFremfoertUnderskud.ShouldEqual(12000);

			var skatterAfPersonligIndkomstBeregner = new SkatterAfPersonligIndkomstBeregner(_skattelovRegistry);
			var skatterAfPersonligIndkomst = skatterAfPersonligIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser, SKATTE_AAR);

			var underskudBeregner = new SkattepligtigIndkomstUnderskudBeregner(_skattelovRegistry);
			var modregnResults = underskudBeregner.ModregningAfUnderskud(indkomster, skatterAfPersonligIndkomst, kommunaleSatser, SKATTE_AAR);
			var underskudTilFremfoersel = modregnResults.Map(x => x.UnderskudTilFremfoersel);
			var modregningUnderskud = modregnResults.Map(x => x.ModregningUnderskud);
			var modregningSkatter = modregnResults.Map(x => x.ModregningSkatter);
			
			// Skattepligtig indkomst efter modregning og fremførsel af underskud
			indkomster[0].SkattepligtigIndkomstSkattegrundlag.ShouldEqual(118000);
			indkomster[0].SkattepligtigIndkomstFremfoertUnderskud.ShouldEqual(0);
			modregningUnderskud[0].ShouldEqual(12000);
			underskudTilFremfoersel[0].ShouldEqual(0);
			indkomster[0].SkattepligtigIndkomstUnderskudTilFremfoersel.ShouldEqual(0);
			indkomster[0].SkattepligtigIndkomstModregninger.ShouldEqual(-12000);
			// Hele modregningen sker i indkomst
			modregningSkatter[0].ShouldEqual(IndkomstSkatterAfPersonligIndkomst.Nul);
			
			var skatterAfSkattepligtigIndkomstBeregner = new SkatterAfSkattepligtigIndkomstBeregner(_skattelovRegistry);
			var skatterAfSkattepligtigIndkomst = skatterAfSkattepligtigIndkomstBeregner.BeregnSkat(skatteydere, indkomster, kommunaleSatser, SKATTE_AAR);

			var skatterFoerPersonfradrag = new IndkomstSkatter(skatterAfPersonligIndkomst[0], skatterAfSkattepligtigIndkomst[0]).ToTuple();

			skatterFoerPersonfradrag[0].Bundskat.ShouldEqual(8568);
			skatterFoerPersonfradrag[0].Mellemskat.ShouldEqual(0);
			skatterFoerPersonfradrag[0].Topskat.ShouldEqual(0);
			skatterFoerPersonfradrag[0].Sundhedsbidrag.ShouldEqual(9440);
			skatterFoerPersonfradrag[0].KommunalIndkomstskatOgKirkeskat.ShouldEqual(28662.20m);

			var personfradragBeregner = new PersonfradragBeregner(_skattelovRegistry);
			var modregnPersonfradragResults = personfradragBeregner.ModregningAfPersonfradrag(skatteydere, skatterFoerPersonfradrag, kommunaleSatser, SKATTE_AAR);
			var skatterEfterPersonfradrag = modregnPersonfradragResults.Map(x => x.ModregnedeSkatter);

			skatterEfterPersonfradrag[0].Bundskat.ShouldEqual(8568 - 2162.16m);
			skatterEfterPersonfradrag[0].Mellemskat.ShouldEqual(0);
			skatterEfterPersonfradrag[0].Topskat.ShouldEqual(0);
			skatterEfterPersonfradrag[0].Sundhedsbidrag.ShouldEqual(9440 - 3432);
			skatterEfterPersonfradrag[0].KommunalIndkomstskatOgKirkeskat.ShouldEqual(28662.20m - 10420.41m);
		}

		[Fact]
		public void Eksempel_16_ModregningAfFremfoertUnderskudOgUnderskudTilFremfoersel_Ugift()
		{
			var skatteydere = getPersonerForUgift();

			var selvangivneBeloeb = new ValueTuple<ISelvangivneBeloeb>(
				new SelvangivneBeloeb
				{
					SkattepligtigIndkomstFremfoertUnderskud = 500000,
					Pension = 360000,
					NettoKapitalIndkomst = -10000,
					LigningsmaessigtFradrag = 50000
				});

			var kommunaleSatser = getKommunaleSatserForUgift();

			var indkomstOpgoerelseBeregner = new IndkomstOpgoerelseBeregner(_skattelovRegistry);
			var indkomster = indkomstOpgoerelseBeregner.BeregnIndkomster(selvangivneBeloeb, SKATTE_AAR);

			// Skattepligtig indkomst før modregning og fremførsel af underskud
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(300000);
			indkomster[0].SkattepligtigIndkomstFremfoertUnderskud.ShouldEqual(500000);
			indkomster[0].SkattepligtigIndkomstUnderskudTilFremfoersel.ShouldEqual(0);

			var skatterAfPersonligIndkomstBeregner = new SkatterAfPersonligIndkomstBeregner(_skattelovRegistry);
			var skatterAfPersonligIndkomst = skatterAfPersonligIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser, SKATTE_AAR);

			// Skatter før modregning af underskud og personfradrag
			skatterAfPersonligIndkomst[0].Bundskat.ShouldEqual(18144);
			skatterAfPersonligIndkomst[0].Mellemskat.ShouldEqual(768);
			skatterAfPersonligIndkomst[0].Topskat.ShouldEqual(1920);
			skatterAfPersonligIndkomst[0].Sum().ShouldEqual(20832);

			var underskudBeregner = new SkattepligtigIndkomstUnderskudBeregner(_skattelovRegistry);
			var modregnSkattepligtigIndkomstUnderskudResults 
				= underskudBeregner.ModregningAfUnderskud(indkomster, skatterAfPersonligIndkomst, kommunaleSatser, SKATTE_AAR);
			skatterAfPersonligIndkomst = modregnSkattepligtigIndkomstUnderskudResults.Map(x => x.ModregnedeSkatter);
			var underskudTilFremfoersel = modregnSkattepligtigIndkomstUnderskudResults.Map(x => x.UnderskudTilFremfoersel);
			var modregningUnderskud = modregnSkattepligtigIndkomstUnderskudResults.Map(x => x.ModregningUnderskud);
			var modregningSkatter = modregnSkattepligtigIndkomstUnderskudResults.Map(x => x.ModregningSkatter);
			var modregningUnderskudSkatter = modregnSkattepligtigIndkomstUnderskudResults.Map(x => x.ModregningUnderskudSkatter);
			var modregningSkattepligtigIndkomst = modregnSkattepligtigIndkomstUnderskudResults.Map(x => x.ModregningUnderskudSkattepligtigIndkomst);

			// Skattepligtig indkomst efter modregning og fremførsel af underskud
			indkomster[0].SkattepligtigIndkomstSkattegrundlag.ShouldEqual(0);
			indkomster[0].SkattepligtigIndkomstFremfoertUnderskud.ShouldEqual(0);
			skatterAfPersonligIndkomst[0].ShouldEqual(IndkomstSkatterAfPersonligIndkomst.Nul);
			// Og dette fremføres til efterfølgende skatteår
			indkomster[0].SkattepligtigIndkomstUnderskudTilFremfoersel.ShouldEqual(135484.67m);
			underskudTilFremfoersel[0].ShouldEqual(135484.67m);
			modregningSkatter[0].Sum().ShouldEqual(20832); // svarende til et underskud på 64515.33m
			modregningUnderskudSkatter[0].ShouldEqual(64515.33m);
			modregningSkattepligtigIndkomst[0].ShouldEqual(300000);
			modregningUnderskud[0].ShouldEqual(300000 + 64515.33m);
			
			var skatAfSkattepligtigIndkomstBeregner = new SkatterAfSkattepligtigIndkomstBeregner(_skattelovRegistry);
			var skatterAfSkattepligtigIndkomst = skatAfSkattepligtigIndkomstBeregner.BeregnSkat(skatteydere, indkomster, kommunaleSatser, SKATTE_AAR);

			skatterAfSkattepligtigIndkomst[0].Sundhedsbidrag.ShouldEqual(0);
			skatterAfSkattepligtigIndkomst[0].KommunalIndkomstskatOgKirkeskat.ShouldEqual(0);

			var skatterFoerPersonfradrag =
				new IndkomstSkatter(skatterAfPersonligIndkomst[0], skatterAfSkattepligtigIndkomst[0]).ToTuple();

			var personfradragBeregner = new PersonfradragBeregner(_skattelovRegistry);
			var modregnPersonfradragResults = personfradragBeregner.ModregningAfPersonfradrag(skatteydere, skatterFoerPersonfradrag, kommunaleSatser, SKATTE_AAR);
			var skatterEfterPersonfradrag = modregnPersonfradragResults.Map(x => x.ModregnedeSkatter);
			var tabtPersonfradrag = modregnPersonfradragResults.Map(x => x.IkkeUdnyttetFradrag);

			tabtPersonfradrag[0].ShouldEqual(_skattelovRegistry.GetPersonfradrag(SKATTE_AAR, skatteydere[0].GetAlder(SKATTE_AAR), false));
			skatterEfterPersonfradrag.ShouldEqual(skatterFoerPersonfradrag);
		}

		[Fact]
		public void Eksempel_17_ModregningAfAaretsOgFremfoertUnderskudOgResterendeUnderskudFremfoeres_Ugift()
		{
			var skatteydere = getPersonerForUgift();

			var selvangivneBeloeb = new ValueTuple<ISelvangivneBeloeb>(
				new SelvangivneBeloeb
				{
					SkattepligtigIndkomstFremfoertUnderskud = 210000,
					Pension = 20000,
					NettoKapitalIndkomst = -80000,
					AktieIndkomst = 200000
				});

			var kommunaleSatser = getKommunaleSatserForUgift();

			var indkomstOpgoerelseBeregner = new IndkomstOpgoerelseBeregner(_skattelovRegistry);
			var indkomster = indkomstOpgoerelseBeregner.BeregnIndkomster(selvangivneBeloeb, SKATTE_AAR);

			// Skattepligtig indkomst inden modregning og fremførsel
			indkomster[0].PersonligIndkomst.ShouldEqual(20000);
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(-60000);
			indkomster[0].SkattepligtigIndkomstFremfoertUnderskud.ShouldEqual(210000);
			indkomster[0].SkattepligtigIndkomstUnderskudTilFremfoersel.ShouldEqual(0);

			var skatAfPersonligIndkomstBeregner = new SkatterAfPersonligIndkomstBeregner(_skattelovRegistry);
			var skatterAfPersonligIndkomst = skatAfPersonligIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser, SKATTE_AAR);

			// Skatter før modregning af underskud og personfradrag
			skatterAfPersonligIndkomst[0].Bundskat.ShouldEqual(1008);
			skatterAfPersonligIndkomst[0].Mellemskat.ShouldEqual(0);
			skatterAfPersonligIndkomst[0].Topskat.ShouldEqual(0);
			skatterAfPersonligIndkomst[0].AktieindkomstskatUnderGrundbeloebet.ShouldEqual(13524);
			skatterAfPersonligIndkomst[0].AktieindkomstskatOverGrundbeloebet.ShouldEqual(24854 + 42255);

			var underskudBeregner = new SkattepligtigIndkomstUnderskudBeregner(_skattelovRegistry );
			var modregnResults = underskudBeregner.ModregningAfUnderskud(indkomster, skatterAfPersonligIndkomst, kommunaleSatser, SKATTE_AAR);
			var skatterAfPersonligIndkomstEfterModregningAfUnderskud = modregnResults.Map(x => x.ModregnedeSkatter);
			var underskudTilFremfoersel = modregnResults.Map(x => x.UnderskudTilFremfoersel);

			// Skattepligtig indkomst efter modregning og fremførsel
			indkomster[0].SkattepligtigIndkomstSkattegrundlag.ShouldEqual(0);
			indkomster[0].SkattepligtigIndkomstFremfoertUnderskud.ShouldEqual(0);

			// Dette fremføres til efterfølgende skatteår
			indkomster[0].SkattepligtigIndkomstUnderskudTilFremfoersel.ShouldEqual(59046.14m);
			underskudTilFremfoersel[0].ShouldEqual(59046.14m);
			
			var skatAfSkattepligtigIndkomstBeregner = new SkatterAfSkattepligtigIndkomstBeregner(_skattelovRegistry);
			var skatterAfSkattepligtigIndkomst = skatAfSkattepligtigIndkomstBeregner.BeregnSkat(skatteydere, indkomster, kommunaleSatser, SKATTE_AAR);

			var skatterFoerPersonfradrag 
				= new IndkomstSkatter(skatterAfPersonligIndkomstEfterModregningAfUnderskud[0], skatterAfSkattepligtigIndkomst[0]).ToTuple();

			skatterFoerPersonfradrag[0].Bundskat.ShouldEqual(0);
			skatterFoerPersonfradrag[0].Mellemskat.ShouldEqual(0);
			skatterFoerPersonfradrag[0].Topskat.ShouldEqual(0);
			skatterFoerPersonfradrag[0].Sundhedsbidrag.ShouldEqual(0);
			skatterFoerPersonfradrag[0].KommunalIndkomstskatOgKirkeskat.ShouldEqual(0);
			skatterFoerPersonfradrag[0].AktieindkomstskatUnderGrundbeloebet.ShouldEqual(13524);
			skatterFoerPersonfradrag[0].AktieindkomstskatOverGrundbeloebet.ShouldEqual(0);

			var personfradragBeregner = new PersonfradragBeregner(_skattelovRegistry);
			var modregnPersonfradragResults = personfradragBeregner.ModregningAfPersonfradrag(skatteydere, skatterFoerPersonfradrag, kommunaleSatser, SKATTE_AAR);
			var skatterEfterPersonfradrag = modregnPersonfradragResults.Map(x => x.ModregnedeSkatter);
			var tabtPersonfradrag = modregnPersonfradragResults.Map(x => x.IkkeUdnyttetFradrag);

			tabtPersonfradrag[0].ShouldEqual(_skattelovRegistry.GetPersonfradrag(SKATTE_AAR, skatteydere[0].GetAlder(SKATTE_AAR), false));
			skatterEfterPersonfradrag.ShouldEqual(skatterFoerPersonfradrag);
		}

		[Fact]
		public void Eksempel_18_ModregningFuldtUdViaNedbringelseAfPartnersSkattepligtigeIndkomst()
		{
			var skatteydere = getPersonerForGifte();

			var selvangivneBeloeb = new ValueTuple<ISelvangivneBeloeb>(
				new SelvangivneBeloeb
				{
					NettoKapitalIndkomst = -52000,
					LigningsmaessigtFradrag = 1000
				},
				new SelvangivneBeloeb
				{
					Pension = 260000,
					LigningsmaessigtFradrag = 8000
				});

			var kommunaleSatser = getKommunaleSatserForGifte();

			var indkomstOpgoerelseBeregner = new IndkomstOpgoerelseBeregner(_skattelovRegistry);
			var indkomster = indkomstOpgoerelseBeregner.BeregnIndkomster(selvangivneBeloeb, SKATTE_AAR);

			// Skattepligtig indkomst før modregning og fremførsel af underskud
			indkomster[0].SkattepligtigIndkomstUnderskudTilFremfoersel.ShouldEqual(0);
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(-53000);
			indkomster[1].SkattepligtigIndkomstUnderskudTilFremfoersel.ShouldEqual(0);
			indkomster[1].SkattepligtigIndkomst.ShouldEqual(252000);

			var skatterAfPersonligIndkomstBeregner = new SkatterAfPersonligIndkomstBeregner(_skattelovRegistry);
			var skatterAfPersonligIndkomst = skatterAfPersonligIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser, SKATTE_AAR);

			// Æ1 har ingen egne skatter at modregne underskud i
			skatterAfPersonligIndkomst[0].ShouldEqual(IndkomstSkatterAfPersonligIndkomst.Nul);
			
			// Og hele underskuddet overføres til Æ2, hvor det kan indeholdes i Æ2s skattepligtige indkomst
			var underskudBeregner = new SkattepligtigIndkomstUnderskudBeregner(_skattelovRegistry);
			var modregnResults = underskudBeregner.ModregningAfUnderskud(indkomster, skatterAfPersonligIndkomst, kommunaleSatser, SKATTE_AAR);
			var skatterAfPersonligIndkomstEfterModregningAfUnderskud = modregnResults.Map(x => x.ModregnedeSkatter);

			// Skattepligtig indkomst efter modregning og fremførsel af underskud
			indkomster[0].SkattepligtigIndkomstModregninger.ShouldEqual(53000);
			indkomster[0].SkattepligtigIndkomstSkattegrundlag.ShouldEqual(0);
			indkomster[0].SkattepligtigIndkomstUnderskudTilFremfoersel.ShouldEqual(0);
			indkomster[1].SkattepligtigIndkomstModregninger.ShouldEqual(-53000);
			indkomster[1].SkattepligtigIndkomstSkattegrundlag.ShouldEqual(199000);
			indkomster[1].SkattepligtigIndkomstUnderskudTilFremfoersel.ShouldEqual(0);

			var modregningUnderskudSkattepligtigIndkomst = modregnResults.Map(x => x.ModregningUnderskudSkattepligtigIndkomst);
			var modregningUnderskudSkatter = modregnResults.Map(x => x.ModregningUnderskudSkatter);
			var modregningUnderskud = modregnResults.Map(x => x.ModregningUnderskud);
			var underskudTilFremfoersel = modregnResults.Map(x => x.UnderskudTilFremfoersel);

			modregningUnderskudSkattepligtigIndkomst[0].ShouldEqual(53000);
			modregningUnderskudSkatter[0].ShouldEqual(0);
			modregningUnderskud[0].ShouldEqual(53000);
			underskudTilFremfoersel[0].ShouldEqual(0);
			
			modregningUnderskudSkattepligtigIndkomst[1].ShouldEqual(0);
			modregningUnderskudSkatter[1].ShouldEqual(0);
			modregningUnderskud[1].ShouldEqual(0);
			underskudTilFremfoersel[1].ShouldEqual(0);

			var skatterAfSkattepligtigIndkomstBeregner = new SkatterAfSkattepligtigIndkomstBeregner(_skattelovRegistry );
			var skatterAfSkattepligtigIndkomst = skatterAfSkattepligtigIndkomstBeregner.BeregnSkat(skatteydere, indkomster, kommunaleSatser, SKATTE_AAR);

			var skatterFoerPersonfradrag = new ValueTuple<IndkomstSkatter>(
				new IndkomstSkatter(skatterAfPersonligIndkomstEfterModregningAfUnderskud[0], skatterAfSkattepligtigIndkomst[0]),
				new IndkomstSkatter(skatterAfPersonligIndkomstEfterModregningAfUnderskud[1], skatterAfSkattepligtigIndkomst[1]));

			skatterFoerPersonfradrag[0].ShouldEqual(IndkomstSkatter.Nul);

			skatterFoerPersonfradrag[1].Bundskat.ShouldEqual(13104);
			skatterFoerPersonfradrag[1].Mellemskat.ShouldEqual(0);
			skatterFoerPersonfradrag[1].Topskat.ShouldEqual(0);
			skatterFoerPersonfradrag[1].AktieindkomstskatUnderGrundbeloebet.ShouldEqual(0);
			skatterFoerPersonfradrag[1].AktieindkomstskatOverGrundbeloebet.ShouldEqual(0);
			skatterFoerPersonfradrag[1].Sundhedsbidrag.ShouldEqual(15920);
			skatterFoerPersonfradrag[1].KommunalIndkomstskatOgKirkeskat.ShouldEqual(48337.10m);

			var personfradragBeregner = new PersonfradragBeregner(_skattelovRegistry);
			var modregnPersonfradragResults = personfradragBeregner.ModregningAfPersonfradrag(skatteydere, skatterFoerPersonfradrag, kommunaleSatser, SKATTE_AAR);
			var skatterEfterPersonfradrag = modregnPersonfradragResults.Map(x => x.ModregnedeSkatter);

			skatterEfterPersonfradrag[0].ShouldEqual(IndkomstSkatter.Nul);

			skatterEfterPersonfradrag[1].Bundskat.ShouldEqual(8779.68m);
			skatterEfterPersonfradrag[1].Mellemskat.ShouldEqual(0);
			skatterEfterPersonfradrag[1].Topskat.ShouldEqual(0);
			skatterEfterPersonfradrag[1].AktieindkomstskatUnderGrundbeloebet.ShouldEqual(0);
			skatterEfterPersonfradrag[1].AktieindkomstskatOverGrundbeloebet.ShouldEqual(0);
			skatterEfterPersonfradrag[1].Sundhedsbidrag.ShouldEqual(9056);
			skatterEfterPersonfradrag[1].KommunalIndkomstskatOgKirkeskat.ShouldEqual(27496.28m);
			skatterEfterPersonfradrag[1].Sum().ShouldEqual(45331.96m);
		}

		[Fact]
		public void Eksempel_19_ModregningEgenSkatOgPartnersSkattepligtigeIndkomst()
		{
			var skatteydere = getPersonerForGifte();

			var selvangivneBeloeb = new ValueTuple<ISelvangivneBeloeb>(
				new SelvangivneBeloeb
				{
					Pension = 50000,
					NettoKapitalIndkomst = -58000,
					LigningsmaessigtFradrag = 13000
				},
				new SelvangivneBeloeb
				{
					Pension = 260000,
					NettoKapitalIndkomst = -4000,
					LigningsmaessigtFradrag = 8000
				});

			var kommunaleSatser = getKommunaleSatserForGifte();

			var indkomstOpgoerelseBeregner = new IndkomstOpgoerelseBeregner(_skattelovRegistry);
			var indkomster = indkomstOpgoerelseBeregner.BeregnIndkomster(selvangivneBeloeb, SKATTE_AAR);

			// Skattepligtig indkomst før modregning og fremførsel af underskud
			indkomster[0].SkattepligtigIndkomstFremfoertUnderskud.ShouldEqual(0);
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(-21000);
			indkomster[1].SkattepligtigIndkomstFremfoertUnderskud.ShouldEqual(0);
			indkomster[1].SkattepligtigIndkomst.ShouldEqual(248000);

			var skatterAfPersonligIndkomstBeregner = new SkatterAfPersonligIndkomstBeregner(_skattelovRegistry);
			var skatterAfPersonligIndkomst = skatterAfPersonligIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser, SKATTE_AAR);

			// Æ1 har egne skatter at modregne underskud i
			skatterAfPersonligIndkomst[0].Bundskat.ShouldEqual(2520);
			skatterAfPersonligIndkomst[0].Mellemskat.ShouldEqual(0);
			skatterAfPersonligIndkomst[0].Topskat.ShouldEqual(0);
			skatterAfPersonligIndkomst[0].AktieindkomstskatOverGrundbeloebet.ShouldEqual(0);

			var underskudBeregner = new SkattepligtigIndkomstUnderskudBeregner(_skattelovRegistry );
			var modregnResults = underskudBeregner.ModregningAfUnderskud(indkomster, skatterAfPersonligIndkomst, kommunaleSatser, SKATTE_AAR);
			var skatterAfPersonligIndkomstEfterModregningAfUnderskud = modregnResults.Map(x => x.ModregnedeSkatter);
			
			// Skattepligtig indkomst efter modregning og fremførsel af underskud
			indkomster[0].SkattepligtigIndkomstSkattegrundlag.ShouldEqual(0);
			indkomster[0].SkattepligtigIndkomstUnderskudTilFremfoersel.ShouldEqual(0);
			indkomster[0].SkattepligtigIndkomstModregninger.ShouldEqual(21000); // ikke muligt at dekomponere i modregning i hhv indkomst og skatter
			indkomster[1].SkattepligtigIndkomstSkattegrundlag.ShouldEqual(248000 - 13195.73m);
			indkomster[1].SkattepligtigIndkomstUnderskudTilFremfoersel.ShouldEqual(0);
			indkomster[1].SkattepligtigIndkomstModregninger.ShouldEqual(-13195.73m);

			var modregningUnderskudSkattepligtigIndkomst = modregnResults.Map(x => x.ModregningUnderskudSkattepligtigIndkomst);
			var modregningUnderskudSkatter = modregnResults.Map(x => x.ModregningUnderskudSkatter);
			var modregningUnderskud = modregnResults.Map(x => x.ModregningUnderskud);
			var underskudTilFremfoersel = modregnResults.Map(x => x.UnderskudTilFremfoersel);

			// Selvom en del af modregningen sker i egne skatter (7804.27) og den resterende del 
			// i ægtefælles indkomst, så er modregningen af underskud opgjort hos kilden
			modregningUnderskudSkattepligtigIndkomst[0].ShouldEqual(13195.73m);
			modregningUnderskudSkatter[0].ShouldEqual(7804.27m);
			modregningUnderskud[0].ShouldEqual(21000);
			underskudTilFremfoersel[0].ShouldEqual(0);
			modregningUnderskudSkattepligtigIndkomst[1].ShouldEqual(0);
			modregningUnderskudSkatter[1].ShouldEqual(0);
			modregningUnderskud[1].ShouldEqual(0);
			underskudTilFremfoersel[1].ShouldEqual(0);

			var skatAfSkattepligtigIndkomstBeregner = new SkatterAfSkattepligtigIndkomstBeregner(_skattelovRegistry);
			var skatterAfSkattepligtigIndkomst = skatAfSkattepligtigIndkomstBeregner.BeregnSkat(skatteydere, indkomster, kommunaleSatser, SKATTE_AAR);

			var skatterFoerPersonfradrag = new ValueTuple<IndkomstSkatter>(
				new IndkomstSkatter(skatterAfPersonligIndkomstEfterModregningAfUnderskud[0], skatterAfSkattepligtigIndkomst[0]),
				new IndkomstSkatter(skatterAfPersonligIndkomstEfterModregningAfUnderskud[1], skatterAfSkattepligtigIndkomst[1]));

			skatterFoerPersonfradrag[0].ShouldEqual(IndkomstSkatter.Nul);

			skatterFoerPersonfradrag[1].Bundskat.ShouldEqual(13104);
			skatterFoerPersonfradrag[1].Mellemskat.ShouldEqual(0);
			skatterFoerPersonfradrag[1].Topskat.ShouldEqual(0);
			skatterFoerPersonfradrag[1].AktieindkomstskatUnderGrundbeloebet.ShouldEqual(0);
			skatterFoerPersonfradrag[1].AktieindkomstskatOverGrundbeloebet.ShouldEqual(0);
			skatterFoerPersonfradrag[1].Sundhedsbidrag.RoundMoney().ShouldEqual(18784.34m);
			skatterFoerPersonfradrag[1].KommunalIndkomstskatOgKirkeskat.RoundMoney().ShouldEqual(57033.96m);

			var personfradragBeregner = new PersonfradragBeregner(_skattelovRegistry);
			var modregnPersonfradragResults = personfradragBeregner.ModregningAfPersonfradrag(skatteydere, skatterFoerPersonfradrag, kommunaleSatser, SKATTE_AAR);
			var skatterEfterPersonfradrag = modregnPersonfradragResults.Map(x => x.ModregnedeSkatter);

			skatterEfterPersonfradrag[0].ShouldEqual(IndkomstSkatter.Nul);

			skatterEfterPersonfradrag[1].Bundskat.ShouldEqual(8779.68m);
			skatterEfterPersonfradrag[1].Mellemskat.ShouldEqual(0);
			skatterEfterPersonfradrag[1].Topskat.ShouldEqual(0);
			skatterEfterPersonfradrag[1].AktieindkomstskatUnderGrundbeloebet.ShouldEqual(0);
			skatterEfterPersonfradrag[1].AktieindkomstskatOverGrundbeloebet.ShouldEqual(0);
			skatterEfterPersonfradrag[1].Sundhedsbidrag.RoundMoney().ShouldEqual(11920.34m);
			skatterEfterPersonfradrag[1].KommunalIndkomstskatOgKirkeskat.RoundMoney().ShouldEqual(36193.14m);
			skatterEfterPersonfradrag[1].Sum().RoundMoney().ShouldEqual(56893.16m);
		}

		[Fact]
		public void Eksempel_20_ModregningPartnersIndkomstOgSkat()
		{
			var skatteydere = getPersonerForGifte();

			var selvangivneBeloeb = new ValueTuple<ISelvangivneBeloeb>(
				new SelvangivneBeloeb
				{
					NettoKapitalIndkomst = -20000,
					LigningsmaessigtFradrag = 5000
				},
				new SelvangivneBeloeb
				{
					Pension = 160000,
					NettoKapitalIndkomst = -128000,
					LigningsmaessigtFradrag = 20000
				});

			var kommunaleSatser = getKommunaleSatserForGifte();

			var indkomstOpgoerelseBeregner = new IndkomstOpgoerelseBeregner(_skattelovRegistry);
			var indkomster = indkomstOpgoerelseBeregner.BeregnIndkomster(selvangivneBeloeb, SKATTE_AAR);

			// Skattepligtig indkomst før modregning af underskud mellem ægtefæller
			indkomster[0].SkattepligtigIndkomstFremfoertUnderskud.ShouldEqual(0);
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(-25000);
			indkomster[1].SkattepligtigIndkomstFremfoertUnderskud.ShouldEqual(0);
			indkomster[1].SkattepligtigIndkomst.ShouldEqual(12000);

			var skatAfPersonligIndkomstBeregner = new SkatterAfPersonligIndkomstBeregner(_skattelovRegistry);
			var skatterAfPersonligIndkomst = skatAfPersonligIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser, SKATTE_AAR);
			
			// Æ1 har ingen egne skatter at modregne underskud i
			skatterAfPersonligIndkomst[0].ShouldEqual(IndkomstSkatterAfPersonligIndkomst.Nul);

			var underskudBeregner = new SkattepligtigIndkomstUnderskudBeregner(_skattelovRegistry);
			var modregnResults = underskudBeregner.ModregningAfUnderskud(indkomster, skatterAfPersonligIndkomst, kommunaleSatser, SKATTE_AAR);
			var skatterAfPersonligIndkomstEfterModregningAfUnderskud = modregnResults.Map(x => x.ModregnedeSkatter);
			var modregningUnderskudSkattepligtigIndkomst = modregnResults.Map(x => x.ModregningUnderskudSkattepligtigIndkomst);
			var modregningUnderskudSkatter = modregnResults.Map(x => x.ModregningUnderskudSkatter);
			var modregningUnderskud = modregnResults.Map(x => x.ModregningUnderskud);
			var underskudTilFremfoersel = modregnResults.Map(x => x.UnderskudTilFremfoersel);

			// Skattepligtig indkomst efter modregning og fremførsel af underskud
			indkomster[0].SkattepligtigIndkomstSkattegrundlag.ShouldEqual(0);
			indkomster[0].SkattepligtigIndkomstUnderskudTilFremfoersel.ShouldEqual(0);
			indkomster[0].SkattepligtigIndkomstModregninger.ShouldEqual(25000);
			indkomster[1].SkattepligtigIndkomstSkattegrundlag.ShouldEqual(0);
			indkomster[1].SkattepligtigIndkomstUnderskudTilFremfoersel.ShouldEqual(0);
			indkomster[1].SkattepligtigIndkomstModregninger.ShouldEqual(-12000); 

			// Der er ingen underskud til fremførsel
			modregningUnderskudSkattepligtigIndkomst[0].ShouldEqual(12000);
			modregningUnderskudSkatter[0].ShouldEqual(13000);
			modregningUnderskud[0].ShouldEqual(25000);
			underskudTilFremfoersel[0].ShouldEqual(0);
			modregningUnderskudSkattepligtigIndkomst[1].ShouldEqual(0);
			modregningUnderskudSkatter[1].ShouldEqual(0);
			modregningUnderskud[1].ShouldEqual(0);
			underskudTilFremfoersel[1].ShouldEqual(0);

			var skatAfSkattepligtigIndkomstBeregner = new SkatterAfSkattepligtigIndkomstBeregner(_skattelovRegistry);
			var skatterAfSkattepligtigIndkomst = skatAfSkattepligtigIndkomstBeregner.BeregnSkat(skatteydere, indkomster, kommunaleSatser, SKATTE_AAR);

			var skatterFoerPersonfradrag = new ValueTuple<IndkomstSkatter>(
				new IndkomstSkatter(skatterAfPersonligIndkomstEfterModregningAfUnderskud[0], skatterAfSkattepligtigIndkomst[0]),
				new IndkomstSkatter(skatterAfPersonligIndkomstEfterModregningAfUnderskud[1], skatterAfSkattepligtigIndkomst[1]));

			skatterFoerPersonfradrag[0].ShouldEqual(IndkomstSkatter.Nul);

			skatterFoerPersonfradrag[1].Bundskat.ShouldEqual(3866.30m);
			skatterFoerPersonfradrag[1].Mellemskat.ShouldEqual(0);
			skatterFoerPersonfradrag[1].Topskat.ShouldEqual(0);
			skatterFoerPersonfradrag[1].AktieindkomstskatUnderGrundbeloebet.ShouldEqual(0);
			skatterFoerPersonfradrag[1].AktieindkomstskatOverGrundbeloebet.ShouldEqual(0);
			skatterFoerPersonfradrag[1].Sundhedsbidrag.ShouldEqual(0);
			skatterFoerPersonfradrag[1].KommunalIndkomstskatOgKirkeskat.ShouldEqual(0);

			var personfradragBeregner = new PersonfradragBeregner(_skattelovRegistry);
			var modregnPersonfradragResults = personfradragBeregner.ModregningAfPersonfradrag(skatteydere, skatterFoerPersonfradrag, kommunaleSatser, SKATTE_AAR);
			var skatterEfterPersonfradrag = modregnPersonfradragResults.Map(x => x.ModregnedeSkatter);

			skatterEfterPersonfradrag[0].ShouldEqual(IndkomstSkatter.Nul);
			skatterEfterPersonfradrag[1].ShouldEqual(IndkomstSkatter.Nul);
		}

		[Fact]
		public void Eksempel_21_ModregningEgenSkatOgPartnersIndkomstOgSkat()
		{
			var skatteydere = getPersonerForGifte();

			var selvangivneBeloeb = new ValueTuple<ISelvangivneBeloeb>(
				new SelvangivneBeloeb
				{
					Pension = 110000,
					NettoKapitalIndkomst = -200000,
					LigningsmaessigtFradrag = 4000
				},
				new SelvangivneBeloeb
				{
					Pension = 375000,
					NettoKapitalIndkomst = -320000,
					LigningsmaessigtFradrag = 31000
				});

			var kommunaleSatser = getKommunaleSatserForGifte();

			var indkomstOpgoerelseBeregner = new IndkomstOpgoerelseBeregner(_skattelovRegistry);
			var indkomster = indkomstOpgoerelseBeregner.BeregnIndkomster(selvangivneBeloeb, SKATTE_AAR);

			// Skattepligtig indkomst før modregning af underskud mellem ægtefæller
			indkomster[0].SkattepligtigIndkomstFremfoertUnderskud.ShouldEqual(0);
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(-94000);
			indkomster[1].SkattepligtigIndkomstFremfoertUnderskud.ShouldEqual(0);
			indkomster[1].SkattepligtigIndkomst.ShouldEqual(24000);

			var skatAfPersonligIndkomstBeregner = new SkatterAfPersonligIndkomstBeregner(_skattelovRegistry);
			var skatterAfPersonligIndkomst = skatAfPersonligIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser, SKATTE_AAR);

			// Æ1 har egne skatter at modregne i
			skatterAfPersonligIndkomst[0].Bundskat.ShouldEqual(5544);
			skatterAfPersonligIndkomst[0].Mellemskat.ShouldEqual(0);
			skatterAfPersonligIndkomst[0].Topskat.ShouldEqual(0);
			skatterAfPersonligIndkomst[0].AktieindkomstskatOverGrundbeloebet.ShouldEqual(0);

			skatterAfPersonligIndkomst[1].Bundskat.ShouldEqual(18900);
			skatterAfPersonligIndkomst[1].Mellemskat.ShouldEqual(0);
			skatterAfPersonligIndkomst[1].Topskat.ShouldEqual(4170);
			skatterAfPersonligIndkomst[1].AktieindkomstskatOverGrundbeloebet.ShouldEqual(0);

			var underskudBeregner = new SkattepligtigIndkomstUnderskudBeregner(_skattelovRegistry);
			var modregnResults = underskudBeregner.ModregningAfUnderskud(indkomster, skatterAfPersonligIndkomst, kommunaleSatser, SKATTE_AAR);
			var skatterAfPersonligIndkomstEfterModregningAfUnderskud = modregnResults.Map(x => x.ModregnedeSkatter);
			
			// Skattepligtig indkomst efter modregning af underskud mellem ægtefæller
			indkomster[0].SkattepligtigIndkomstSkattegrundlag.ShouldEqual(0);
			indkomster[0].SkattepligtigIndkomstModregninger.ShouldEqual(94000);
			indkomster[0].SkattepligtigIndkomstUnderskudTilFremfoersel.ShouldEqual(0);
			indkomster[1].SkattepligtigIndkomstSkattegrundlag.ShouldEqual(0);
			indkomster[1].SkattepligtigIndkomstModregninger.ShouldEqual(-24000);
			indkomster[1].SkattepligtigIndkomstUnderskudTilFremfoersel.ShouldEqual(0);
			
			var modregningUnderskudSkattepligtigIndkomst = modregnResults.Map(x => x.ModregningUnderskudSkattepligtigIndkomst);
			var modregningUnderskudSkatter = modregnResults.Map(x => x.ModregningUnderskudSkatter);
			var modregningUnderskud = modregnResults.Map(x => x.ModregningUnderskud);
			var underskudTilFremfoersel = modregnResults.Map(x => x.UnderskudTilFremfoersel);

			modregningUnderskudSkattepligtigIndkomst[0].ShouldEqual(24000);
			modregningUnderskudSkatter[0].ShouldEqual(17169.40m + 52830.60m);
			modregningUnderskud[0].ShouldEqual(94000);
			underskudTilFremfoersel[0].ShouldEqual(0);
			
			modregningUnderskudSkattepligtigIndkomst[1].ShouldEqual(0);
			modregningUnderskudSkatter[1].ShouldEqual(0);
			modregningUnderskud[1].ShouldEqual(0);
			underskudTilFremfoersel[1].ShouldEqual(0);

			var skatAfSkattepligtigIndkomstBeregner = new SkatterAfSkattepligtigIndkomstBeregner(_skattelovRegistry);
			var skatterAfSkattepligtigIndkomst = skatAfSkattepligtigIndkomstBeregner.BeregnSkat(skatteydere, indkomster, kommunaleSatser, SKATTE_AAR);

			var skatterFoerPersonfradrag = new ValueTuple<IndkomstSkatter>(
				new IndkomstSkatter(skatterAfPersonligIndkomstEfterModregningAfUnderskud[0], skatterAfSkattepligtigIndkomst[0]),
				new IndkomstSkatter(skatterAfPersonligIndkomstEfterModregningAfUnderskud[1], skatterAfSkattepligtigIndkomst[1]));

			skatterFoerPersonfradrag[0].ShouldEqual(IndkomstSkatter.Nul);

			skatterFoerPersonfradrag[1].Bundskat.ShouldEqual(1841);
			skatterFoerPersonfradrag[1].Mellemskat.ShouldEqual(0);
			skatterFoerPersonfradrag[1].Topskat.ShouldEqual(4170);
			skatterFoerPersonfradrag[1].AktieindkomstskatUnderGrundbeloebet.ShouldEqual(0);
			skatterFoerPersonfradrag[1].AktieindkomstskatOverGrundbeloebet.ShouldEqual(0);
			skatterFoerPersonfradrag[1].Sundhedsbidrag.ShouldEqual(0);
			skatterFoerPersonfradrag[1].KommunalIndkomstskatOgKirkeskat.ShouldEqual(0);

			var personfradragBeregner = new PersonfradragBeregner(_skattelovRegistry);
			var modregnPersonfradragResults = personfradragBeregner.ModregningAfPersonfradrag(skatteydere, skatterFoerPersonfradrag, kommunaleSatser, SKATTE_AAR);
			var skatterEfterPersonfradrag = modregnPersonfradragResults.Map(x => x.ModregnedeSkatter);

			skatterEfterPersonfradrag[0].ShouldEqual(IndkomstSkatter.Nul);
			skatterEfterPersonfradrag[1].ShouldEqual(IndkomstSkatter.Nul);
		}

		[Fact]
		public void Eksempel_22_ModregningFuldtUdPartnersSkat()
		{
			var skatteydere = getPersonerForGifte();

			var selvangivneBeloeb = new ValueTuple<ISelvangivneBeloeb>(
				new SelvangivneBeloeb
				{
					LigningsmaessigtFradrag = 2000
				},
				new SelvangivneBeloeb
				{
					Pension = 23000,
					NettoKapitalIndkomst = -4000,
					LigningsmaessigtFradrag = 20000
				});

			var kommunaleSatser = getKommunaleSatserForGifte();

			var indkomstOpgoerelseBeregner = new IndkomstOpgoerelseBeregner(_skattelovRegistry);
			var indkomster = indkomstOpgoerelseBeregner.BeregnIndkomster(selvangivneBeloeb, SKATTE_AAR);

			// Skattepligtig indkomst før modregning af underskud mellem ægtefæller
			indkomster[0].SkattepligtigIndkomstFremfoertUnderskud.ShouldEqual(0);
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(-2000);
			indkomster[1].SkattepligtigIndkomstFremfoertUnderskud.ShouldEqual(0);
			indkomster[1].SkattepligtigIndkomst.ShouldEqual(-1000);

			var skatAfPersonligIndkomstBeregner = new SkatterAfPersonligIndkomstBeregner(_skattelovRegistry);
			var skatterAfPersonligIndkomst = skatAfPersonligIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser, SKATTE_AAR);

			// Æ1 har egne skatter at modregne i
			skatterAfPersonligIndkomst[0].Bundskat.ShouldEqual(0);
			skatterAfPersonligIndkomst[0].Mellemskat.ShouldEqual(0);
			skatterAfPersonligIndkomst[0].Topskat.ShouldEqual(0);
			skatterAfPersonligIndkomst[0].AktieindkomstskatOverGrundbeloebet.ShouldEqual(0);

			skatterAfPersonligIndkomst[1].Bundskat.ShouldEqual(1159.20m); // <-- denne skat kan indeholde begge ægtefællers underskud
			skatterAfPersonligIndkomst[1].Mellemskat.ShouldEqual(0);
			skatterAfPersonligIndkomst[1].Topskat.ShouldEqual(0);
			skatterAfPersonligIndkomst[1].AktieindkomstskatOverGrundbeloebet.ShouldEqual(0);

			var underskudBeregner = new SkattepligtigIndkomstUnderskudBeregner(_skattelovRegistry);
			var modregnResults = underskudBeregner.ModregningAfUnderskud(indkomster, skatterAfPersonligIndkomst, kommunaleSatser, SKATTE_AAR);
			
			// Skattepligtig indkomst efter modregning af underskud mellem ægtefæller
			indkomster[0].SkattepligtigIndkomstSkattegrundlag.ShouldEqual(0);
			indkomster[0].SkattepligtigIndkomstModregninger.ShouldEqual(2000);
			indkomster[0].SkattepligtigIndkomstUnderskudTilFremfoersel.ShouldEqual(0);
			indkomster[1].SkattepligtigIndkomstSkattegrundlag.ShouldEqual(0);
			indkomster[1].SkattepligtigIndkomstModregninger.ShouldEqual(1000);
			indkomster[1].SkattepligtigIndkomstUnderskudTilFremfoersel.ShouldEqual(0);

			var modregningUnderskudSkattepligtigIndkomst = modregnResults.Map(x => x.ModregningUnderskudSkattepligtigIndkomst);
			var modregningUnderskudSkatter = modregnResults.Map(x => x.ModregningUnderskudSkatter);
			var modregningUnderskud = modregnResults.Map(x => x.ModregningUnderskud);
			var underskudTilFremfoersel = modregnResults.Map(x => x.UnderskudTilFremfoersel);

			modregningUnderskudSkattepligtigIndkomst[0].ShouldEqual(0);
			modregningUnderskudSkatter[0].ShouldEqual(2000);
			modregningUnderskud[0].ShouldEqual(2000);
			underskudTilFremfoersel[0].ShouldEqual(0);

			modregningUnderskudSkattepligtigIndkomst[1].ShouldEqual(0);
			modregningUnderskudSkatter[1].ShouldEqual(1000);
			modregningUnderskud[1].ShouldEqual(1000);
			underskudTilFremfoersel[1].ShouldEqual(0);

			var skatterAfPersonligIndkomstEfterModregningAfUnderskud = modregnResults.Map(x => x.ModregnedeSkatter);

			var skatAfSkattepligtigIndkomstBeregner = new SkatterAfSkattepligtigIndkomstBeregner(_skattelovRegistry);
			var skatterAfSkattepligtigIndkomst = skatAfSkattepligtigIndkomstBeregner.BeregnSkat(skatteydere, indkomster, kommunaleSatser, SKATTE_AAR);

			var skatterFoerPersonfradrag = new ValueTuple<IndkomstSkatter>(
				new IndkomstSkatter(skatterAfPersonligIndkomstEfterModregningAfUnderskud[0], skatterAfSkattepligtigIndkomst[0]),
				new IndkomstSkatter(skatterAfPersonligIndkomstEfterModregningAfUnderskud[1], skatterAfSkattepligtigIndkomst[1]));

			skatterFoerPersonfradrag[0].ShouldEqual(IndkomstSkatter.Nul);
			
			// Så meget bundskat er der tilbage efter modregning af underskud
			skatterFoerPersonfradrag[1].Bundskat.ShouldEqual(190.50m);
			skatterFoerPersonfradrag[1].Mellemskat.ShouldEqual(0);
			skatterFoerPersonfradrag[1].Topskat.ShouldEqual(0);
			skatterFoerPersonfradrag[1].AktieindkomstskatUnderGrundbeloebet.ShouldEqual(0);
			skatterFoerPersonfradrag[1].AktieindkomstskatOverGrundbeloebet.ShouldEqual(0);
			skatterFoerPersonfradrag[1].Sundhedsbidrag.ShouldEqual(0);
			skatterFoerPersonfradrag[1].KommunalIndkomstskatOgKirkeskat.ShouldEqual(0);

			var personfradragBeregner = new PersonfradragBeregner(_skattelovRegistry);
			var modregnPersonfradragResults = personfradragBeregner.ModregningAfPersonfradrag(skatteydere, skatterFoerPersonfradrag, kommunaleSatser, SKATTE_AAR);
			var skatterEfterPersonfradrag = modregnPersonfradragResults.Map(x => x.ModregnedeSkatter);

			skatterEfterPersonfradrag[0].ShouldEqual(IndkomstSkatter.Nul);
			skatterEfterPersonfradrag[1].ShouldEqual(IndkomstSkatter.Nul);
		}

		[Fact]
		public void Eksempel_23_DenEnePartnerHarUnderskudFraTidligereAar()
		{
			var skatteydere = getPersonerForGifte();

			var selvangivneBeloeb = new ValueTuple<ISelvangivneBeloeb>(
				new SelvangivneBeloeb
				{
					SkattepligtigIndkomstFremfoertUnderskud = 500000,
					Pension = 200000,
					NettoKapitalIndkomst = -20000,
					LigningsmaessigtFradrag = 7000
				},
				new SelvangivneBeloeb
				{
					Pension = 100000,
					NettoKapitalIndkomst = -30000,
					LigningsmaessigtFradrag = 3000
				});

			var kommunaleSatser = getKommunaleSatserForGifte();

			var indkomstOpgoerelseBeregner = new IndkomstOpgoerelseBeregner(_skattelovRegistry);
			var indkomster = indkomstOpgoerelseBeregner.BeregnIndkomster(selvangivneBeloeb, SKATTE_AAR);

			// Skattepligtig indkomst før modregning af underskud mellem ægtefæller
			indkomster[0].SkattepligtigIndkomstFremfoertUnderskud.ShouldEqual(500000);   // fremført underskud
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(173000);                     // årets underskud
			indkomster[1].SkattepligtigIndkomstFremfoertUnderskud.ShouldEqual(0);
			indkomster[1].SkattepligtigIndkomst.ShouldEqual(67000);

			var skatAfPersonligIndkomstBeregner = new SkatterAfPersonligIndkomstBeregner(_skattelovRegistry);
			var skatterAfPersonligIndkomst = skatAfPersonligIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser, SKATTE_AAR);

			skatterAfPersonligIndkomst[0].Bundskat.ShouldEqual(10080);
			skatterAfPersonligIndkomst[0].Mellemskat.ShouldEqual(0);
			skatterAfPersonligIndkomst[0].Topskat.ShouldEqual(0);
			skatterAfPersonligIndkomst[0].AktieindkomstskatOverGrundbeloebet.ShouldEqual(0);

			skatterAfPersonligIndkomst[1].Bundskat.ShouldEqual(5040);
			skatterAfPersonligIndkomst[1].Mellemskat.ShouldEqual(0);
			skatterAfPersonligIndkomst[1].Topskat.ShouldEqual(0);
			skatterAfPersonligIndkomst[1].AktieindkomstskatOverGrundbeloebet.ShouldEqual(0);

			var underskudBeregner = new SkattepligtigIndkomstUnderskudBeregner(_skattelovRegistry);
			var modregnResults = underskudBeregner.ModregningAfUnderskud(indkomster, skatterAfPersonligIndkomst, kommunaleSatser, SKATTE_AAR);

			// Skattepligtig indkomst efter modregning af underskud mellem ægtefæller
			indkomster[0].SkattepligtigIndkomstSkattegrundlag.ShouldEqual(0);
			indkomster[0].SkattepligtigIndkomstModregninger.ShouldEqual(-173000);
			indkomster[0].SkattepligtigIndkomstUnderskudTilFremfoersel.ShouldEqual(213174.35m);
			indkomster[1].SkattepligtigIndkomstSkattegrundlag.ShouldEqual(0);
			indkomster[1].SkattepligtigIndkomstModregninger.ShouldEqual(-67000);
			indkomster[1].SkattepligtigIndkomstUnderskudTilFremfoersel.ShouldEqual(0);

			var summenAfAaretsOgFremfoertUnderskud = modregnResults.Map(x => x.Underskud);
			var modregningUnderskudSkattepligtigIndkomst = modregnResults.Map(x => x.ModregningUnderskudSkattepligtigIndkomst);
			var modregningUnderskudSkatter = modregnResults.Map(x => x.ModregningUnderskudSkatter);
			var modregningUnderskud = modregnResults.Map(x => x.ModregningUnderskud);
			var underskudTilFremfoersel = modregnResults.Map(x => x.UnderskudTilFremfoersel);

			summenAfAaretsOgFremfoertUnderskud[0].ShouldEqual(500000);
			modregningUnderskudSkattepligtigIndkomst[0].ShouldEqual(173000 + 67000);
			modregningUnderskudSkatter[0].ShouldEqual(31217.10m + 15608.55m);
			modregningUnderskud[0].ShouldEqual(173000 + 67000 + 31217.10m + 15608.55m);
			underskudTilFremfoersel[0].ShouldEqual(213174.35m);

			summenAfAaretsOgFremfoertUnderskud[1].ShouldEqual(0); 
			modregningUnderskudSkattepligtigIndkomst[1].ShouldEqual(0);
			modregningUnderskudSkatter[1].ShouldEqual(0);
			modregningUnderskud[1].ShouldEqual(0);
			underskudTilFremfoersel[1].ShouldEqual(0);

			var skatterAfPersonligIndkomstEfterModregningAfUnderskud = modregnResults.Map(x => x.ModregnedeSkatter);

			var skatAfSkattepligtigIndkomstBeregner = new SkatterAfSkattepligtigIndkomstBeregner(_skattelovRegistry);
			var skatterAfSkattepligtigIndkomst = skatAfSkattepligtigIndkomstBeregner.BeregnSkat(skatteydere, indkomster, kommunaleSatser, SKATTE_AAR);

			var skatterFoerPersonfradrag = SkatteUtility.CombineSkat(skatterAfPersonligIndkomstEfterModregningAfUnderskud,
			                                                        skatterAfSkattepligtigIndkomst);
			
			skatterFoerPersonfradrag[0].ShouldEqual(IndkomstSkatter.Nul);
			skatterFoerPersonfradrag[1].ShouldEqual(IndkomstSkatter.Nul);

			var personfradragBeregner = new PersonfradragBeregner(_skattelovRegistry);
			var modregnPersonfradragResults = personfradragBeregner.ModregningAfPersonfradrag(skatteydere, skatterFoerPersonfradrag, kommunaleSatser, SKATTE_AAR);
			var skatterEfterPersonfradrag = modregnPersonfradragResults.Map(x => x.ModregnedeSkatter);

			skatterEfterPersonfradrag[0].ShouldEqual(IndkomstSkatter.Nul);
			skatterEfterPersonfradrag[1].ShouldEqual(IndkomstSkatter.Nul);
		}

		[Fact]
		public void Eksempel_24_DenEnePartnerHarNegativSkattepligtigIndkomstDenAndenHarEtFremfoertUnderskud()
		{
			var skatteydere = getPersonerForGifte();

			var selvangivneBeloeb = new ValueTuple<ISelvangivneBeloeb>(
				new SelvangivneBeloeb
				{
					Pension = 24000,
					NettoKapitalIndkomst = -16000,
					LigningsmaessigtFradrag = 10000
				},
				new SelvangivneBeloeb
				{
					SkattepligtigIndkomstFremfoertUnderskud = 290000,
					Pension = 250000
				});

			var kommunaleSatser = getKommunaleSatserForGifte();

			var indkomstOpgoerelseBeregner = new IndkomstOpgoerelseBeregner(_skattelovRegistry);
			var indkomster = indkomstOpgoerelseBeregner.BeregnIndkomster(selvangivneBeloeb, SKATTE_AAR);

			// Skattepligtig indkomst før modregning af underskud mellem ægtefæller
			indkomster[0].SkattepligtigIndkomstFremfoertUnderskud.ShouldEqual(0);
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(-2000);                      // årets underskud
			indkomster[1].SkattepligtigIndkomstFremfoertUnderskud.ShouldEqual(290000);   // fremført underskud
			indkomster[1].SkattepligtigIndkomst.ShouldEqual(250000);

			var skatAfPersonligIndkomstBeregner = new SkatterAfPersonligIndkomstBeregner(_skattelovRegistry);
			var skatterAfPersonligIndkomst = skatAfPersonligIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser, SKATTE_AAR);

			// Skatter før modregning
			skatterAfPersonligIndkomst[0].Bundskat.ShouldEqual(1209.60m);
			skatterAfPersonligIndkomst[0].Mellemskat.ShouldEqual(0);
			skatterAfPersonligIndkomst[0].Topskat.ShouldEqual(0);
			skatterAfPersonligIndkomst[0].AktieindkomstskatOverGrundbeloebet.ShouldEqual(0);

			skatterAfPersonligIndkomst[1].Bundskat.ShouldEqual(12600);
			skatterAfPersonligIndkomst[1].Mellemskat.ShouldEqual(0);
			skatterAfPersonligIndkomst[1].Topskat.ShouldEqual(0);
			skatterAfPersonligIndkomst[1].AktieindkomstskatOverGrundbeloebet.ShouldEqual(0);

			var underskudBeregner = new SkattepligtigIndkomstUnderskudBeregner(_skattelovRegistry);
			var modregnResults = underskudBeregner.ModregningAfUnderskud(indkomster, skatterAfPersonligIndkomst, kommunaleSatser, SKATTE_AAR);

			// Skattepligtig indkomst efter modregning af underskud mellem ægtefæller
			indkomster[0].SkattepligtigIndkomstSkattegrundlag.ShouldEqual(0);
			indkomster[0].SkattepligtigIndkomstModregninger.ShouldEqual(2000);
			indkomster[0].SkattepligtigIndkomstUnderskudTilFremfoersel.ShouldEqual(0);
			indkomster[1].SkattepligtigIndkomstSkattegrundlag.ShouldEqual(0);
			indkomster[1].SkattepligtigIndkomstModregninger.ShouldEqual(-250000);
			indkomster[1].SkattepligtigIndkomstUnderskudTilFremfoersel.ShouldEqual(0);

			var summenAfAaretsOgFremfoertUnderskud = modregnResults.Map(x => x.Underskud);
			var modregningUnderskudSkattepligtigIndkomst = modregnResults.Map(x => x.ModregningUnderskudSkattepligtigIndkomst);
			var modregningUnderskudSkatter = modregnResults.Map(x => x.ModregningUnderskudSkatter);
			var modregningUnderskud = modregnResults.Map(x => x.ModregningUnderskud);
			var underskudTilFremfoersel = modregnResults.Map(x => x.UnderskudTilFremfoersel);

			summenAfAaretsOgFremfoertUnderskud[0].ShouldEqual(2000);
			modregningUnderskudSkattepligtigIndkomst[0].ShouldEqual(0);
			modregningUnderskudSkatter[0].ShouldEqual(2000);
			modregningUnderskud[0].ShouldEqual(2000);
			underskudTilFremfoersel[0].ShouldEqual(0);

			summenAfAaretsOgFremfoertUnderskud[1].ShouldEqual(290000);
			modregningUnderskudSkattepligtigIndkomst[1].ShouldEqual(250000);
			modregningUnderskudSkatter[1].ShouldEqual(39021.37m + 978.63m); // egne skatter, derefter ægetfælles skatter
			modregningUnderskud[1].ShouldEqual(290000);
			underskudTilFremfoersel[1].ShouldEqual(0);

			var skatterAfPersonligIndkomstEfterModregningAfUnderskud = modregnResults.Map(x => x.ModregnedeSkatter);

			var skatAfSkattepligtigIndkomstBeregner = new SkatterAfSkattepligtigIndkomstBeregner(_skattelovRegistry);
			var skatterAfSkattepligtigIndkomst = skatAfSkattepligtigIndkomstBeregner.BeregnSkat(skatteydere, indkomster, kommunaleSatser, SKATTE_AAR);

			var skatterFoerPersonfradrag = SkatteUtility.CombineSkat(skatterAfPersonligIndkomstEfterModregningAfUnderskud,
																	skatterAfSkattepligtigIndkomst);

			// Skatter efter modregning
			skatterFoerPersonfradrag[0].Bundskat.ShouldEqual(247.80m);
			skatterFoerPersonfradrag[0].Mellemskat.ShouldEqual(0);
			skatterFoerPersonfradrag[0].Topskat.ShouldEqual(0);
			skatterFoerPersonfradrag[0].Sundhedsbidrag.ShouldEqual(0);
			skatterFoerPersonfradrag[0].KommunalIndkomstskatOgKirkeskat.ShouldEqual(0);
			skatterFoerPersonfradrag[1].ShouldEqual(IndkomstSkatter.Nul);

			var personfradragBeregner = new PersonfradragBeregner(_skattelovRegistry);
			var modregnPersonfradragResults = personfradragBeregner.ModregningAfPersonfradrag(skatteydere, skatterFoerPersonfradrag, kommunaleSatser, SKATTE_AAR);
			var skatterEfterPersonfradrag = modregnPersonfradragResults.Map(x => x.ModregnedeSkatter);

			skatterEfterPersonfradrag[0].ShouldEqual(IndkomstSkatter.Nul);
			skatterEfterPersonfradrag[1].ShouldEqual(IndkomstSkatter.Nul);
		}

		[Fact]
		public void Eksempel_25_BeggeHarUnderskudFraTidligereAar()
		{
			var skatteydere = getPersonerForGifte();

			var selvangivneBeloeb = new ValueTuple<ISelvangivneBeloeb>(
				new SelvangivneBeloeb
				{
					SkattepligtigIndkomstFremfoertUnderskud = 120000 + 970000,
					Pension = 800000,
					NettoKapitalIndkomst = 10000,
					LigningsmaessigtFradrag = 15000
				},
				new SelvangivneBeloeb
				{
					SkattepligtigIndkomstFremfoertUnderskud = 100000 + 80000,
					Pension = 9000,
					NettoKapitalIndkomst = -64000,
					LigningsmaessigtFradrag = 15000
				});

			var kommunaleSatser = getKommunaleSatserForGifte();

			var indkomstOpgoerelseBeregner = new IndkomstOpgoerelseBeregner(_skattelovRegistry);
			var indkomster = indkomstOpgoerelseBeregner.BeregnIndkomster(selvangivneBeloeb, SKATTE_AAR);

			// Skattepligtig indkomst før modregning af underskud mellem ægtefæller
			indkomster[0].SkattepligtigIndkomstFremfoertUnderskud.ShouldEqual(1090000);  // fremført underskud
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(795000);
			indkomster[1].SkattepligtigIndkomstFremfoertUnderskud.ShouldEqual(180000);   // fremført underskud
			indkomster[1].SkattepligtigIndkomst.ShouldEqual(-70000);                     // årets underskud

			var skatAfPersonligIndkomstBeregner = new SkatterAfPersonligIndkomstBeregner(_skattelovRegistry);
			var skatterAfPersonligIndkomst = skatAfPersonligIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser, SKATTE_AAR);

			// Skatter før modregning
			skatterAfPersonligIndkomst[0].Bundskat.ShouldEqual(40320);
			skatterAfPersonligIndkomst[0].Mellemskat.ShouldEqual(6876);
			skatterAfPersonligIndkomst[0].Topskat.ShouldEqual(67920);
			skatterAfPersonligIndkomst[0].AktieindkomstskatOverGrundbeloebet.ShouldEqual(0);
			skatterAfPersonligIndkomst[0].Sum().ShouldEqual(115116);

			skatterAfPersonligIndkomst[1].Bundskat.ShouldEqual(453.60m);
			skatterAfPersonligIndkomst[1].Mellemskat.ShouldEqual(0);
			skatterAfPersonligIndkomst[1].Topskat.ShouldEqual(0);
			skatterAfPersonligIndkomst[1].AktieindkomstskatOverGrundbeloebet.ShouldEqual(0);

			var underskudBeregner = new SkattepligtigIndkomstUnderskudBeregner(_skattelovRegistry);
			var modregnResults = underskudBeregner.ModregningAfUnderskud(indkomster, skatterAfPersonligIndkomst, kommunaleSatser, SKATTE_AAR);

			// Skattepligtig indkomst efter modregning af underskud mellem ægtefæller
			indkomster[0].SkattepligtigIndkomstSkattegrundlag.ShouldEqual(0);
			indkomster[0].SkattepligtigIndkomstModregninger.ShouldEqual(-(68595.23m + 726404.77m));
			indkomster[0].SkattepligtigIndkomstUnderskudTilFremfoersel.ShouldEqual(7088.57m);
			indkomster[1].SkattepligtigIndkomstSkattegrundlag.ShouldEqual(0);
			indkomster[1].SkattepligtigIndkomstModregninger.ShouldEqual(70000);
			indkomster[1].SkattepligtigIndkomstUnderskudTilFremfoersel.ShouldEqual(180000);
			
			var summenAfAaretsOgFremfoertUnderskud = modregnResults.Map(x => x.Underskud);
			var modregningUnderskudSkattepligtigIndkomst = modregnResults.Map(x => x.ModregningUnderskudSkattepligtigIndkomst);
			var modregningUnderskudSkatter = modregnResults.Map(x => x.ModregningUnderskudSkatter);
			var modregningUnderskud = modregnResults.Map(x => x.ModregningUnderskud);
			var underskudTilFremfoersel = modregnResults.Map(x => x.UnderskudTilFremfoersel);

			summenAfAaretsOgFremfoertUnderskud[0].ShouldEqual(1090000);
			modregningUnderskudSkattepligtigIndkomst[0].ShouldEqual(726404.77m);
			modregningUnderskudSkatter[0].ShouldEqual(356506.66m);
			modregningUnderskud[0].ShouldEqual(726404.77m + 356506.66m);
			underskudTilFremfoersel[0].ShouldEqual(1090000 - (726404.77m + 356506.66m));

			summenAfAaretsOgFremfoertUnderskud[1].ShouldEqual(250000);
			modregningUnderskudSkattepligtigIndkomst[1].ShouldEqual(68595.23m); // dette beløb er modregnet i ægtefælles indkomst
			modregningUnderskudSkatter[1].ShouldEqual(1404.77m);
			modregningUnderskud[1].ShouldEqual(70000);
			underskudTilFremfoersel[1].ShouldEqual(180000);

			var skatterAfPersonligIndkomstEfterModregningAfUnderskud = modregnResults.Map(x => x.ModregnedeSkatter);

			var skatAfSkattepligtigIndkomstBeregner = new SkatterAfSkattepligtigIndkomstBeregner(_skattelovRegistry);
			var skatterAfSkattepligtigIndkomst = skatAfSkattepligtigIndkomstBeregner.BeregnSkat(skatteydere, indkomster, kommunaleSatser, SKATTE_AAR);

			var skatterFoerPersonfradrag = SkatteUtility.CombineSkat(skatterAfPersonligIndkomstEfterModregningAfUnderskud,
																	skatterAfSkattepligtigIndkomst);

			// Skatter efter modregning
			skatterFoerPersonfradrag[0].ShouldEqual(IndkomstSkatter.Nul);
			skatterFoerPersonfradrag[1].ShouldEqual(IndkomstSkatter.Nul);

			var personfradragBeregner = new PersonfradragBeregner(_skattelovRegistry);
			var modregnPersonfradragResults = personfradragBeregner.ModregningAfPersonfradrag(skatteydere, skatterFoerPersonfradrag, kommunaleSatser, SKATTE_AAR);
			var skatterEfterPersonfradrag = modregnPersonfradragResults.Map(x => x.ModregnedeSkatter);

			skatterEfterPersonfradrag[0].ShouldEqual(IndkomstSkatter.Nul);
			skatterEfterPersonfradrag[1].ShouldEqual(IndkomstSkatter.Nul);
		}

		[Fact]
		public void Eksempel_33_ModregningFuldtUdINettokapitalindkomst_Ugift()
		{
			var skatteydere = getPersonerForUgift();

			var selvangivneBeloeb = new ValueTuple<ISelvangivneBeloeb>(
				new SelvangivneBeloeb
					{
						Bruttoloen = -10000,
						NettoKapitalIndkomst = 370000,
						LigningsmaessigtFradrag = 3000
					});

			var indkomstOpgoerelseBeregner = new IndkomstOpgoerelseBeregner(_skattelovRegistry);
			var indkomster = indkomstOpgoerelseBeregner.BeregnIndkomster(selvangivneBeloeb, SKATTE_AAR);

			indkomster[0].SkattepligtigIndkomst.ShouldEqual(357000);

			// Personlig indkomst og nettokapitalindkomst før modregning og fremførsel

			indkomster[0].PersonligIndkomstSkattegrundlag.ShouldEqual(-10000);
			indkomster[0].NettoKapitalIndkomstSkattegrundlag.ShouldEqual(370000);

			var personligIndkomstUnderskudBeregner = new PersonligIndkomstUnderskudBeregner();
			personligIndkomstUnderskudBeregner.ModregningAfUnderskud(indkomster);

			// Personlig indkomst og nettokapitalindkomst efter modregning og fremførsel
			indkomster[0].PersonligIndkomstSkattegrundlag.ShouldEqual(0);
			indkomster[0].NettoKapitalIndkomstSkattegrundlag.ShouldEqual(360000);

			var kommunaleSatser = getKommunaleSatserForUgift();

			var skatterAfPersonligIndkomstBeregner = new SkatterAfPersonligIndkomstBeregner(_skattelovRegistry);
			var skatterAfPersonligIndkomst = skatterAfPersonligIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser, SKATTE_AAR);

			skatterAfPersonligIndkomst[0].Bundskat.ShouldEqual(18144);
			skatterAfPersonligIndkomst[0].Mellemskat.ShouldEqual(768);
			skatterAfPersonligIndkomst[0].Topskat.ShouldEqual(1920);

			var skatterAfSkattepligtigIndkomstBeregner = new SkatterAfSkattepligtigIndkomstBeregner(_skattelovRegistry);
			var skatterAfSkattepligtigIndkomst = skatterAfSkattepligtigIndkomstBeregner.BeregnSkat(skatteydere, indkomster, kommunaleSatser, SKATTE_AAR);

			skatterAfSkattepligtigIndkomst[0].Sum().ShouldEqual(115275.30m);

			var skatterFoerPersonfradrag = SkatteUtility.CombineSkat(skatterAfPersonligIndkomst, skatterAfSkattepligtigIndkomst);

			var personfradragBeregner = new PersonfradragBeregner(_skattelovRegistry);
			var modregnPersonfradragResults = personfradragBeregner.ModregningAfPersonfradrag(skatteydere, skatterFoerPersonfradrag, kommunaleSatser, SKATTE_AAR);
			var skatterEfterPersonfradrag = modregnPersonfradragResults.Map(x => x.ModregnedeSkatter);
			var skattevaerdiAfPersonfradrag = modregnPersonfradragResults.Map(x => x.UdnyttetSkattevaerdi);

			skattevaerdiAfPersonfradrag[0].ShouldEqual(16014.57m);
			skatterEfterPersonfradrag[0].Sum().ShouldEqual(120092.73m);
		}

		[Fact]
		public void Eksempel_34_ModregningNettoKapitalIndkomstOgFremfoersel_Ugift()
		{
			var skatteydere = getPersonerForUgift();

			var selvangivneBeloeb = new ValueTuple<ISelvangivneBeloeb>(
				new SelvangivneBeloeb
				{
					Bruttoloen = -310000,
					NettoKapitalIndkomst = 300000,
					LigningsmaessigtFradrag = 12000
				});

			var indkomstOpgoerelseBeregner = new IndkomstOpgoerelseBeregner(_skattelovRegistry);
			var indkomster = indkomstOpgoerelseBeregner.BeregnIndkomster(selvangivneBeloeb, SKATTE_AAR);

			// Personlig indkomst og nettokapitalindkomst før modregning og fremførsel
			indkomster[0].PersonligIndkomstFoerAMBidrag.ShouldEqual(-310000);
			indkomster[0].NettoKapitalIndkomst.ShouldEqual(300000);

			var personligIndkomstUnderskudBeregner = new PersonligIndkomstUnderskudBeregner();
			personligIndkomstUnderskudBeregner.ModregningAfUnderskud(indkomster);

			// Personlig indkomst og nettokapitalindkomst efter modregning og fremførsel
			indkomster[0].PersonligIndkomstSkattegrundlag.ShouldEqual(0);
			indkomster[0].NettoKapitalIndkomstSkattegrundlag.ShouldEqual(0);
			indkomster[0].PersonligIndkomstUnderskudTilFremfoersel.ShouldEqual(10000);

			var kommunaleSatser = getKommunaleSatserForUgift();

			var skatterAfPersonligIndkomstBeregner = new SkatterAfPersonligIndkomstBeregner(_skattelovRegistry);
			var skatterAfPersonligIndkomst = skatterAfPersonligIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser, SKATTE_AAR);

			skatterAfPersonligIndkomst[0].ShouldEqual(IndkomstSkatterAfPersonligIndkomst.Nul);

			// Skattepligtig indkomst før modregning og fremførsel
			indkomster[0].SkattepligtigIndkomstSkattegrundlag.ShouldEqual(-22000);

			var skattepligtigIndkomstUnderskudBeregner = new SkattepligtigIndkomstUnderskudBeregner(_skattelovRegistry);
			skattepligtigIndkomstUnderskudBeregner.ModregningAfUnderskud(indkomster, skatterAfPersonligIndkomst, kommunaleSatser, SKATTE_AAR);

			// Skattepligtig indkomst efter modregning og fremførsel
			indkomster[0].SkattepligtigIndkomstSkattegrundlag.ShouldEqual(0);
			indkomster[0].SkattepligtigIndkomstUnderskudTilFremfoersel.ShouldEqual(22000);

			var skatterAfSkattepligtigIndkomstBeregner = new SkatterAfSkattepligtigIndkomstBeregner(_skattelovRegistry);
			var skatterAfSkattepligtigIndkomst = skatterAfSkattepligtigIndkomstBeregner.BeregnSkat(skatteydere, indkomster, kommunaleSatser, SKATTE_AAR);

			skatterAfSkattepligtigIndkomst[0].ShouldEqual(SkatterAfSkattepligtigIndkomst.Nul);

			var skatterFoerPersonfradrag = SkatteUtility.CombineSkat(skatterAfPersonligIndkomst, skatterAfSkattepligtigIndkomst);

			var personfradragBeregner = new PersonfradragBeregner(_skattelovRegistry);
			var modregnPersonfradragResults = personfradragBeregner.ModregningAfPersonfradrag(skatteydere, skatterFoerPersonfradrag, kommunaleSatser, SKATTE_AAR);
			var skatterEfterPersonfradrag = modregnPersonfradragResults.Map(x => x.ModregnedeSkatter);
			var ikkeUdnyttetFradrag = modregnPersonfradragResults.Map(x => x.IkkeUdnyttetFradrag);

			ikkeUdnyttetFradrag[0].ShouldEqual(_skattelovRegistry.GetPersonfradrag(SKATTE_AAR, skatteydere[0].GetAlder(SKATTE_AAR), false));
			skatterEfterPersonfradrag[0].ShouldEqual(IndkomstSkatter.Nul);
		}

		[Fact]
		public void Eksempel_35_ModregningFuldtUdAfFremfoertUnderskudINettoKapitalIndkomstOgPersonligIndkomst()
		{
			var skatteydere = getPersonerForUgift();

			var selvangivneBeloeb = new ValueTuple<ISelvangivneBeloeb>(
				new SelvangivneBeloeb
				{
					PersonligIndkomstFremfoertUnderskud = 215000,
					Pension = 230000,
					PrivatTegnetKapitalPensionsindskud = 30000,
					NettoKapitalIndkomst = 10000,
					LigningsmaessigtFradrag = 12000
				});

			var indkomstOpgoerelseBeregner = new IndkomstOpgoerelseBeregner(_skattelovRegistry);
			var indkomster = indkomstOpgoerelseBeregner.BeregnIndkomster(selvangivneBeloeb, SKATTE_AAR);

			// Personlig indkomst og nettokapitalindkomst før modregning og fremførsel
			indkomster[0].PersonligIndkomstSkattegrundlag.ShouldEqual(200000);
			indkomster[0].NettoKapitalIndkomstSkattegrundlag.ShouldEqual(10000);

			var personligIndkomstUnderskudBeregner = new PersonligIndkomstUnderskudBeregner();
			personligIndkomstUnderskudBeregner.ModregningAfUnderskud(indkomster);

			// Personlig indkomst og nettokapitalindkomst efter modregning og fremførsel
			indkomster[0].PersonligIndkomstSkattegrundlag.ShouldEqual(0);
			indkomster[0].NettoKapitalIndkomstSkattegrundlag.ShouldEqual(0);
			indkomster[0].KapitalPensionsindskudSkattegrundlag.ShouldEqual(25000);
			indkomster[0].PersonligIndkomstUnderskudTilFremfoersel.ShouldEqual(0);

			var kommunaleSatser = getKommunaleSatserForUgift();

			var skatterAfPersonligIndkomstBeregner = new SkatterAfPersonligIndkomstBeregner(_skattelovRegistry);
			var skatterAfPersonligIndkomst = skatterAfPersonligIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser, SKATTE_AAR);

			skatterAfPersonligIndkomst[0].ShouldEqual(IndkomstSkatterAfPersonligIndkomst.Nul);

			// Skattepligtig indkomst før modregning og fremførsel
			indkomster[0].SkattepligtigIndkomstSkattegrundlag.ShouldEqual(198000);

			var skattepligtigIndkomstUnderskudBeregner = new SkattepligtigIndkomstUnderskudBeregner(_skattelovRegistry);
			skattepligtigIndkomstUnderskudBeregner.ModregningAfUnderskud(indkomster, skatterAfPersonligIndkomst, kommunaleSatser, SKATTE_AAR);

			// Skattepligtig indkomst efter modregning og fremførsel
			indkomster[0].SkattepligtigIndkomstSkattegrundlag.ShouldEqual(198000);
			indkomster[0].SkattepligtigIndkomstUnderskudTilFremfoersel.ShouldEqual(0);

			var skatterAfSkattepligtigIndkomstBeregner = new SkatterAfSkattepligtigIndkomstBeregner(_skattelovRegistry);
			var skatterAfSkattepligtigIndkomst = skatterAfSkattepligtigIndkomstBeregner.BeregnSkat(skatteydere, indkomster, kommunaleSatser, SKATTE_AAR);

			skatterAfSkattepligtigIndkomst[0].Sum().ShouldEqual(63934.20m);

			var skatterFoerPersonfradrag = SkatteUtility.CombineSkat(skatterAfPersonligIndkomst, skatterAfSkattepligtigIndkomst);

			var personfradragBeregner = new PersonfradragBeregner(_skattelovRegistry);
			var modregnPersonfradragResults = personfradragBeregner.ModregningAfPersonfradrag(skatteydere, skatterFoerPersonfradrag, kommunaleSatser, SKATTE_AAR);
			var skatterEfterPersonfradrag = modregnPersonfradragResults.Map(x => x.ModregnedeSkatter);
			var ikkeUdnyttetFradrag = modregnPersonfradragResults.Map(x => x.IkkeUdnyttetFradrag);

			ikkeUdnyttetFradrag[0].ShouldEqual(0);
			skatterEfterPersonfradrag[0].Sum().ShouldEqual(47919.63m);
		}

		[Fact]
		public void Eksempel_36_KombineretFremfoertUnderskudPersonligIndkomstOgSkattepligtigIndkomst_Ugift()
		{
			var skatteydere = getPersonerForUgift();

			var selvangivneBeloeb = new ValueTuple<ISelvangivneBeloeb>(
				new SelvangivneBeloeb
				{
					PersonligIndkomstFremfoertUnderskud = 30000,
					SkattepligtigIndkomstFremfoertUnderskud = 30000,
					Pension = 200000,
					NettoKapitalIndkomst = 10000
				});

			var indkomstOpgoerelseBeregner = new IndkomstOpgoerelseBeregner(_skattelovRegistry);
			var indkomster = indkomstOpgoerelseBeregner.BeregnIndkomster(selvangivneBeloeb, SKATTE_AAR);

			// Personlig indkomst og nettokapitalindkomst før modregning og fremførsel
			indkomster[0].PersonligIndkomstSkattegrundlag.ShouldEqual(200000);
			indkomster[0].NettoKapitalIndkomstSkattegrundlag.ShouldEqual(10000);

			var personligIndkomstUnderskudBeregner = new PersonligIndkomstUnderskudBeregner();
			personligIndkomstUnderskudBeregner.ModregningAfUnderskud(indkomster);

			// Personlig indkomst og nettokapitalindkomst efter modregning og fremførsel
			indkomster[0].PersonligIndkomstSkattegrundlag.ShouldEqual(180000);
			indkomster[0].NettoKapitalIndkomstSkattegrundlag.ShouldEqual(0);
			indkomster[0].KapitalPensionsindskudSkattegrundlag.ShouldEqual(0);
			indkomster[0].PersonligIndkomstUnderskudTilFremfoersel.ShouldEqual(0);

			var kommunaleSatser = getKommunaleSatserForUgift();

			var skatterAfPersonligIndkomstBeregner = new SkatterAfPersonligIndkomstBeregner(_skattelovRegistry);
			var skatterAfPersonligIndkomst = skatterAfPersonligIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser, SKATTE_AAR);

			skatterAfPersonligIndkomst[0].Bundskat.ShouldEqual(9072);
			skatterAfPersonligIndkomst[0].Mellemskat.ShouldEqual(0);
			skatterAfPersonligIndkomst[0].Topskat.ShouldEqual(0);

			// Skattepligtig indkomst før modregning og fremførsel
			indkomster[0].SkattepligtigIndkomstSkattegrundlag.ShouldEqual(210000);

			var skattepligtigIndkomstUnderskudBeregner = new SkattepligtigIndkomstUnderskudBeregner(_skattelovRegistry);
			skattepligtigIndkomstUnderskudBeregner.ModregningAfUnderskud(indkomster, skatterAfPersonligIndkomst, kommunaleSatser, SKATTE_AAR);

			// Skattepligtig indkomst efter modregning og fremførsel
			indkomster[0].SkattepligtigIndkomstSkattegrundlag.ShouldEqual(180000);
			indkomster[0].SkattepligtigIndkomstUnderskudTilFremfoersel.ShouldEqual(0);

			var skatterAfSkattepligtigIndkomstBeregner = new SkatterAfSkattepligtigIndkomstBeregner(_skattelovRegistry);
			var skatterAfSkattepligtigIndkomst = skatterAfSkattepligtigIndkomstBeregner.BeregnSkat(skatteydere, indkomster, kommunaleSatser, SKATTE_AAR);

			skatterAfSkattepligtigIndkomst[0].Sum().ShouldEqual(58122);

			var skatterFoerPersonfradrag = SkatteUtility.CombineSkat(skatterAfPersonligIndkomst, skatterAfSkattepligtigIndkomst);

			var personfradragBeregner = new PersonfradragBeregner(_skattelovRegistry);
			var modregnPersonfradragResults = personfradragBeregner.ModregningAfPersonfradrag(skatteydere, skatterFoerPersonfradrag, kommunaleSatser, SKATTE_AAR);
			var skatterEfterPersonfradrag = modregnPersonfradragResults.Map(x => x.ModregnedeSkatter);
			var ikkeUdnyttetFradrag = modregnPersonfradragResults.Map(x => x.IkkeUdnyttetFradrag);

			ikkeUdnyttetFradrag[0].ShouldEqual(0);
			skatterEfterPersonfradrag[0].Sum().ShouldEqual(51179.43m);
		}

		[Fact]
		public void Eksempel_37_ModregningNettoKapitalIndkomstOgPersonligIndkomstOgFremfoerselAfRestunderskud_Ugift()
		{
			var skatteydere = getPersonerForUgift();

			var selvangivneBeloeb = new ValueTuple<ISelvangivneBeloeb>(
				new SelvangivneBeloeb
				{
					PersonligIndkomstFremfoertUnderskud = 300000,
					Pension = 200000,
					ArbejdsgiverAdminKapitalPensionsindskud = 30000,
					NettoKapitalIndkomst = 10000,
					LigningsmaessigtFradrag = 7000
				});

			var indkomstOpgoerelseBeregner = new IndkomstOpgoerelseBeregner(_skattelovRegistry);
			var indkomster = indkomstOpgoerelseBeregner.BeregnIndkomster(selvangivneBeloeb, SKATTE_AAR);

			// Personlig indkomst og nettokapitalindkomst før modregning og fremførsel
			indkomster[0].PersonligIndkomstSkattegrundlag.ShouldEqual(200000);
			indkomster[0].NettoKapitalIndkomstSkattegrundlag.ShouldEqual(10000);

			var personligIndkomstUnderskudBeregner = new PersonligIndkomstUnderskudBeregner();
			personligIndkomstUnderskudBeregner.ModregningAfUnderskud(indkomster);

			// Personlig indkomst og nettokapitalindkomst efter modregning og fremførsel
			indkomster[0].PersonligIndkomstSkattegrundlag.ShouldEqual(0);
			indkomster[0].NettoKapitalIndkomstSkattegrundlag.ShouldEqual(0);
			indkomster[0].KapitalPensionsindskudSkattegrundlag.ShouldEqual(0);
			indkomster[0].PersonligIndkomstUnderskudTilFremfoersel.ShouldEqual(60000);

			var kommunaleSatser = getKommunaleSatserForUgift();

			var skatterAfPersonligIndkomstBeregner = new SkatterAfPersonligIndkomstBeregner(_skattelovRegistry);
			var skatterAfPersonligIndkomst = skatterAfPersonligIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser, SKATTE_AAR);

			skatterAfPersonligIndkomst[0].ShouldEqual(IndkomstSkatterAfPersonligIndkomst.Nul);

			// Skattepligtig indkomst før modregning og fremførsel
			indkomster[0].SkattepligtigIndkomstSkattegrundlag.ShouldEqual(203000);

			var skattepligtigIndkomstUnderskudBeregner = new SkattepligtigIndkomstUnderskudBeregner(_skattelovRegistry);
			skattepligtigIndkomstUnderskudBeregner.ModregningAfUnderskud(indkomster, skatterAfPersonligIndkomst, kommunaleSatser, SKATTE_AAR);

			// Skattepligtig indkomst efter modregning og fremførsel
			indkomster[0].SkattepligtigIndkomstSkattegrundlag.ShouldEqual(203000);
			indkomster[0].SkattepligtigIndkomstUnderskudTilFremfoersel.ShouldEqual(0);

			var skatterAfSkattepligtigIndkomstBeregner = new SkatterAfSkattepligtigIndkomstBeregner(_skattelovRegistry);
			var skatterAfSkattepligtigIndkomst = skatterAfSkattepligtigIndkomstBeregner.BeregnSkat(skatteydere, indkomster, kommunaleSatser, SKATTE_AAR);

			skatterAfSkattepligtigIndkomst[0].Sum().ShouldEqual(65548.70m);

			var skatterFoerPersonfradrag = SkatteUtility.CombineSkat(skatterAfPersonligIndkomst, skatterAfSkattepligtigIndkomst);

			var personfradragBeregner = new PersonfradragBeregner(_skattelovRegistry);
			var modregnPersonfradragResults = personfradragBeregner.ModregningAfPersonfradrag(skatteydere, skatterFoerPersonfradrag, kommunaleSatser, SKATTE_AAR);
			var skatterEfterPersonfradrag = modregnPersonfradragResults.Map(x => x.ModregnedeSkatter);
			var ikkeUdnyttetFradrag = modregnPersonfradragResults.Map(x => x.IkkeUdnyttetFradrag);

			ikkeUdnyttetFradrag[0].ShouldEqual(0);
			skatterEfterPersonfradrag[0].Sum().ShouldEqual(49534.13m);
		}

		[Fact]
		public void Eksempel_38_UnderskudPersonligIndkomstBaadeIndevaerendeAarOgFremfoertUnderskud_Ugift()
		{
			var skatteydere = getPersonerForUgift();

			var selvangivneBeloeb = new ValueTuple<ISelvangivneBeloeb>(
				new SelvangivneBeloeb
				{
					PersonligIndkomstFremfoertUnderskud = 30000,
					SkattepligtigIndkomstFremfoertUnderskud = 30000,
					Bruttoloen = -60000,
					NettoKapitalIndkomst = 11000,
					LigningsmaessigtFradrag = 1000
				});

			var indkomstOpgoerelseBeregner = new IndkomstOpgoerelseBeregner(_skattelovRegistry);
			var indkomster = indkomstOpgoerelseBeregner.BeregnIndkomster(selvangivneBeloeb, SKATTE_AAR);

			// Personlig indkomst og nettokapitalindkomst før modregning og fremførsel
			indkomster[0].PersonligIndkomstSkattegrundlag.ShouldEqual(-60000);
			indkomster[0].NettoKapitalIndkomstSkattegrundlag.ShouldEqual(11000);

			var personligIndkomstUnderskudBeregner = new PersonligIndkomstUnderskudBeregner();
			personligIndkomstUnderskudBeregner.ModregningAfUnderskud(indkomster);

			// Personlig indkomst og nettokapitalindkomst efter modregning og fremførsel
			indkomster[0].PersonligIndkomstSkattegrundlag.ShouldEqual(0);
			indkomster[0].NettoKapitalIndkomstSkattegrundlag.ShouldEqual(0);
			indkomster[0].KapitalPensionsindskudSkattegrundlag.ShouldEqual(0);
			indkomster[0].PersonligIndkomstUnderskudTilFremfoersel.ShouldEqual(79000);

			var kommunaleSatser = getKommunaleSatserForUgift();

			var skatterAfPersonligIndkomstBeregner = new SkatterAfPersonligIndkomstBeregner(_skattelovRegistry);
			var skatterAfPersonligIndkomst = skatterAfPersonligIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser, SKATTE_AAR);

			skatterAfPersonligIndkomst[0].ShouldEqual(IndkomstSkatterAfPersonligIndkomst.Nul);

			// Skattepligtig indkomst før modregning og fremførsel
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(-50000);

			var skattepligtigIndkomstUnderskudBeregner = new SkattepligtigIndkomstUnderskudBeregner(_skattelovRegistry);
			skattepligtigIndkomstUnderskudBeregner.ModregningAfUnderskud(indkomster, skatterAfPersonligIndkomst, kommunaleSatser, SKATTE_AAR);

			// Skattepligtig indkomst efter modregning og fremførsel
			indkomster[0].SkattepligtigIndkomstSkattegrundlag.ShouldEqual(0);
			indkomster[0].SkattepligtigIndkomstUnderskudTilFremfoersel.ShouldEqual(80000);

			var skatterAfSkattepligtigIndkomstBeregner = new SkatterAfSkattepligtigIndkomstBeregner(_skattelovRegistry);
			var skatterAfSkattepligtigIndkomst = skatterAfSkattepligtigIndkomstBeregner.BeregnSkat(skatteydere, indkomster, kommunaleSatser, SKATTE_AAR);

			skatterAfSkattepligtigIndkomst[0].ShouldEqual(SkatterAfSkattepligtigIndkomst.Nul);

			var skatterFoerPersonfradrag = SkatteUtility.CombineSkat(skatterAfPersonligIndkomst, skatterAfSkattepligtigIndkomst);

			var personfradragBeregner = new PersonfradragBeregner(_skattelovRegistry);
			var modregnPersonfradragResults = personfradragBeregner.ModregningAfPersonfradrag(skatteydere, skatterFoerPersonfradrag, kommunaleSatser, SKATTE_AAR);
			var skatterEfterPersonfradrag = modregnPersonfradragResults.Map(x => x.ModregnedeSkatter);
			var ikkeUdnyttetFradrag = modregnPersonfradragResults.Map(x => x.IkkeUdnyttetFradrag);

			ikkeUdnyttetFradrag[0].ShouldEqual(_skattelovRegistry.GetPersonfradrag(SKATTE_AAR, skatteydere[0].GetAlder(SKATTE_AAR), false));
			skatterEfterPersonfradrag[0].Sum().ShouldEqual(0);
		}

		[Fact]
		public void Eksempel_39_ModregningIkkeMuligtOgHeleUnderskudFremfoeres_Ugift()
		{
			var skatteydere = getPersonerForUgift();

			var selvangivneBeloeb = new ValueTuple<ISelvangivneBeloeb>(
				new SelvangivneBeloeb
				{
					Bruttoloen = -100000,
					LigningsmaessigtFradrag = 10000
				});

			var indkomstOpgoerelseBeregner = new IndkomstOpgoerelseBeregner(_skattelovRegistry);
			var indkomster = indkomstOpgoerelseBeregner.BeregnIndkomster(selvangivneBeloeb, SKATTE_AAR);

			// Personlig indkomst og nettokapitalindkomst før modregning og fremførsel
			indkomster[0].PersonligIndkomstSkattegrundlag.ShouldEqual(-100000);
			indkomster[0].NettoKapitalIndkomstSkattegrundlag.ShouldEqual(0);

			var personligIndkomstUnderskudBeregner = new PersonligIndkomstUnderskudBeregner();
			personligIndkomstUnderskudBeregner.ModregningAfUnderskud(indkomster);

			// Personlig indkomst og nettokapitalindkomst efter modregning og fremførsel
			indkomster[0].PersonligIndkomstSkattegrundlag.ShouldEqual(0);
			indkomster[0].NettoKapitalIndkomstSkattegrundlag.ShouldEqual(0);
			indkomster[0].KapitalPensionsindskudSkattegrundlag.ShouldEqual(0);
			indkomster[0].PersonligIndkomstUnderskudTilFremfoersel.ShouldEqual(100000);

			var kommunaleSatser = getKommunaleSatserForUgift();

			var skatterAfPersonligIndkomstBeregner = new SkatterAfPersonligIndkomstBeregner(_skattelovRegistry);
			var skatterAfPersonligIndkomst = skatterAfPersonligIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser, SKATTE_AAR);

			skatterAfPersonligIndkomst[0].ShouldEqual(IndkomstSkatterAfPersonligIndkomst.Nul);

			// Skattepligtig indkomst før modregning og fremførsel
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(-110000);

			var skattepligtigIndkomstUnderskudBeregner = new SkattepligtigIndkomstUnderskudBeregner(_skattelovRegistry);
			skattepligtigIndkomstUnderskudBeregner.ModregningAfUnderskud(indkomster, skatterAfPersonligIndkomst, kommunaleSatser, SKATTE_AAR);

			// Skattepligtig indkomst efter modregning og fremførsel
			indkomster[0].SkattepligtigIndkomstSkattegrundlag.ShouldEqual(0);
			indkomster[0].SkattepligtigIndkomstUnderskudTilFremfoersel.ShouldEqual(110000);

			var skatterAfSkattepligtigIndkomstBeregner = new SkatterAfSkattepligtigIndkomstBeregner(_skattelovRegistry);
			var skatterAfSkattepligtigIndkomst = skatterAfSkattepligtigIndkomstBeregner.BeregnSkat(skatteydere, indkomster, kommunaleSatser, SKATTE_AAR);

			skatterAfSkattepligtigIndkomst[0].ShouldEqual(SkatterAfSkattepligtigIndkomst.Nul);

			var skatterFoerPersonfradrag = SkatteUtility.CombineSkat(skatterAfPersonligIndkomst, skatterAfSkattepligtigIndkomst);

			var personfradragBeregner = new PersonfradragBeregner(_skattelovRegistry);
			var modregnPersonfradragResults = personfradragBeregner.ModregningAfPersonfradrag(skatteydere, skatterFoerPersonfradrag, kommunaleSatser, SKATTE_AAR);
			var skatterEfterPersonfradrag = modregnPersonfradragResults.Map(x => x.ModregnedeSkatter);
			var ikkeUdnyttetFradrag = modregnPersonfradragResults.Map(x => x.IkkeUdnyttetFradrag);

			ikkeUdnyttetFradrag[0].ShouldEqual(_skattelovRegistry.GetPersonfradrag(SKATTE_AAR, skatteydere[0].GetAlder(SKATTE_AAR), false));
			skatterEfterPersonfradrag[0].Sum().ShouldEqual(0);
		}

		[Fact]
		public void Eksempel_40_ModregningFuldtUdHosPartnersPersonligeIndkomst_Gifte()
		{
			var skatteydere = getPersonerForGifte();

			var selvangivneBeloeb = new ValueTuple<ISelvangivneBeloeb>(
				new SelvangivneBeloeb
				{
					Bruttoloen = -10000,
					NettoKapitalIndkomst = 50000
				},
				new SelvangivneBeloeb 
				{
					Pension = 360000,
					LigningsmaessigtFradrag = 9000
				});

			var indkomstOpgoerelseBeregner = new IndkomstOpgoerelseBeregner(_skattelovRegistry);
			var indkomster = indkomstOpgoerelseBeregner.BeregnIndkomster(selvangivneBeloeb, SKATTE_AAR);

			// Personlig indkomst og nettokapitalindkomst før modregning og fremførsel
			indkomster[0].PersonligIndkomstSkattegrundlag.ShouldEqual(-10000);
			indkomster[0].NettoKapitalIndkomstSkattegrundlag.ShouldEqual(50000);
			indkomster[1].PersonligIndkomstSkattegrundlag.ShouldEqual(360000);
			indkomster[1].NettoKapitalIndkomstSkattegrundlag.ShouldEqual(0);

			var personligIndkomstUnderskudBeregner = new PersonligIndkomstUnderskudBeregner();
			personligIndkomstUnderskudBeregner.ModregningAfUnderskud(indkomster);

			// Personlig indkomst og nettokapitalindkomst efter modregning og fremførsel
			indkomster[0].PersonligIndkomstSkattegrundlag.ShouldEqual(0);
			indkomster[0].NettoKapitalIndkomstSkattegrundlag.ShouldEqual(50000);
			indkomster[0].PersonligIndkomstUnderskudTilFremfoersel.ShouldEqual(0);
			indkomster[1].PersonligIndkomstSkattegrundlag.ShouldEqual(350000);
			indkomster[1].NettoKapitalIndkomstSkattegrundlag.ShouldEqual(0);
			indkomster[1].PersonligIndkomstUnderskudTilFremfoersel.ShouldEqual(0);

			var kommunaleSatser = getKommunaleSatserForGifte();
			
			// Beregning af skatter af personlig indkomst
			var skatterAfPersonligIndkomstBeregner = new SkatterAfPersonligIndkomstBeregner(_skattelovRegistry);
			var skatterAfPersonligIndkomst = skatterAfPersonligIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser, SKATTE_AAR);

			skatterAfPersonligIndkomst[0].Bundskat.ShouldEqual(2520);
			skatterAfPersonligIndkomst[0].Mellemskat.ShouldEqual(0);
			skatterAfPersonligIndkomst[0].Topskat.ShouldEqual(7500);
			skatterAfPersonligIndkomst[1].Bundskat.ShouldEqual(17640);
			skatterAfPersonligIndkomst[1].Mellemskat.ShouldEqual(0);
			skatterAfPersonligIndkomst[1].Topskat.ShouldEqual(420);
			
			// Skattepligtig indkomst før modregning og fremførsel
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(40000);
			indkomster[1].SkattepligtigIndkomst.ShouldEqual(351000);

			var skattepligtigIndkomstUnderskudBeregner = new SkattepligtigIndkomstUnderskudBeregner(_skattelovRegistry);
			skattepligtigIndkomstUnderskudBeregner.ModregningAfUnderskud(indkomster, skatterAfPersonligIndkomst, kommunaleSatser, SKATTE_AAR);

			// Skattepligtig indkomst efter modregning og fremførsel
			indkomster[0].SkattepligtigIndkomstSkattegrundlag.ShouldEqual(40000);
			indkomster[1].SkattepligtigIndkomstSkattegrundlag.ShouldEqual(351000);

			// Beregning af skatter af skattepligtig indkomst
			var skatterAfSkattepligtigIndkomstBeregner = new SkatterAfSkattepligtigIndkomstBeregner(_skattelovRegistry);
			var skatterAfSkattepligtigIndkomst = skatterAfSkattepligtigIndkomstBeregner.BeregnSkat(skatteydere, indkomster, kommunaleSatser, SKATTE_AAR);

			skatterAfSkattepligtigIndkomst[0].Sum().ShouldEqual(12916);
			skatterAfSkattepligtigIndkomst[1].Sum().ShouldEqual(113337.90m);

			var skatterFoerPersonfradrag = SkatteUtility.CombineSkat(skatterAfPersonligIndkomst, skatterAfSkattepligtigIndkomst);

			var personfradragBeregner = new PersonfradragBeregner(_skattelovRegistry);
			var modregnPersonfradragResults = personfradragBeregner.ModregningAfPersonfradrag(skatteydere, skatterFoerPersonfradrag, kommunaleSatser, SKATTE_AAR);

			var skatterEfterPersonfradrag = modregnPersonfradragResults.Map(x => x.ModregnedeSkatter);

			skatterEfterPersonfradrag[0].Sum().ShouldEqual(6921.43m);
			skatterEfterPersonfradrag[1].Sum().ShouldEqual(115383.33m);
		}

		[Fact]
		public void Eksempel_41_ModregningAfPersonligIndkomstUnderskudHosPartnersPersonligeIndkomstOgSamletNettoKapitalIndkomst_Gifte()
		{
			var skatteydere = getPersonerForGifte();

			var selvangivneBeloeb = new ValueTuple<ISelvangivneBeloeb>(
				new SelvangivneBeloeb
				{
					Bruttoloen = -105000,
					NettoKapitalIndkomst = 20000
				},
				new SelvangivneBeloeb
				{
					Pension = 40000,
					PrivatTegnetKapitalPensionsindskud = 30000,
					NettoKapitalIndkomst = 110000
				});

			var indkomstOpgoerelseBeregner = new IndkomstOpgoerelseBeregner(_skattelovRegistry);
			var indkomster = indkomstOpgoerelseBeregner.BeregnIndkomster(selvangivneBeloeb, SKATTE_AAR);

			// Personlig indkomst og nettokapitalindkomst før modregning og fremførsel
			indkomster[0].PersonligIndkomstSkattegrundlag.ShouldEqual(-105000);
			indkomster[0].NettoKapitalIndkomstSkattegrundlag.ShouldEqual(20000);
			indkomster[1].PersonligIndkomstSkattegrundlag.ShouldEqual(10000);
			indkomster[1].NettoKapitalIndkomstSkattegrundlag.ShouldEqual(110000);

			var personligIndkomstUnderskudBeregner = new PersonligIndkomstUnderskudBeregner();
			personligIndkomstUnderskudBeregner.ModregningAfUnderskud(indkomster);

			// Personlig indkomst og nettokapitalindkomst efter modregning og fremførsel
			indkomster[0].PersonligIndkomstSkattegrundlag.ShouldEqual(0);
			indkomster[0].NettoKapitalIndkomstSkattegrundlag.ShouldEqual(0);
			indkomster[0].PersonligIndkomstUnderskudTilFremfoersel.ShouldEqual(0);
			indkomster[1].PersonligIndkomstSkattegrundlag.ShouldEqual(0);
			indkomster[1].KapitalPensionsindskudSkattegrundlag.ShouldEqual(0);
			indkomster[1].NettoKapitalIndkomstSkattegrundlag.ShouldEqual(110000 - 45000);
			indkomster[1].PersonligIndkomstUnderskudTilFremfoersel.ShouldEqual(0);

			var kommunaleSatser = getKommunaleSatserForGifte();

			// Beregning af skatter af personlig indkomst
			var skatterAfPersonligIndkomstBeregner = new SkatterAfPersonligIndkomstBeregner(_skattelovRegistry);
			var skatterAfPersonligIndkomst = skatterAfPersonligIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser, SKATTE_AAR);

			skatterAfPersonligIndkomst[0].Bundskat.ShouldEqual(0);
			skatterAfPersonligIndkomst[0].Mellemskat.ShouldEqual(0);
			skatterAfPersonligIndkomst[0].Topskat.ShouldEqual(0);
			skatterAfPersonligIndkomst[1].Bundskat.ShouldEqual(3276);
			skatterAfPersonligIndkomst[1].Mellemskat.ShouldEqual(0);
			skatterAfPersonligIndkomst[1].Topskat.ShouldEqual(0);

			// Skattepligtig indkomst før modregning og fremførsel
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(-85000);
			indkomster[1].SkattepligtigIndkomst.ShouldEqual(120000);

			var skattepligtigIndkomstUnderskudBeregner = new SkattepligtigIndkomstUnderskudBeregner(_skattelovRegistry);
			skattepligtigIndkomstUnderskudBeregner.ModregningAfUnderskud(indkomster, skatterAfPersonligIndkomst, kommunaleSatser, SKATTE_AAR);

			// Skattepligtig indkomst efter modregning og fremførsel
			indkomster[0].SkattepligtigIndkomstSkattegrundlag.ShouldEqual(0);
			indkomster[1].SkattepligtigIndkomstSkattegrundlag.ShouldEqual(35000);

			// Beregning af skatter af skattepligtig indkomst
			var skatterAfSkattepligtigIndkomstBeregner = new SkatterAfSkattepligtigIndkomstBeregner(_skattelovRegistry);
			var skatterAfSkattepligtigIndkomst = skatterAfSkattepligtigIndkomstBeregner.BeregnSkat(skatteydere, indkomster, kommunaleSatser, SKATTE_AAR);

			skatterAfSkattepligtigIndkomst[0].Sum().ShouldEqual(0);
			skatterAfSkattepligtigIndkomst[1].Sum().ShouldEqual(11301.50m);

			var skatterFoerPersonfradrag = SkatteUtility.CombineSkat(skatterAfPersonligIndkomst, skatterAfSkattepligtigIndkomst);

			var personfradragBeregner = new PersonfradragBeregner(_skattelovRegistry);
			var modregnPersonfradragResults = personfradragBeregner.ModregningAfPersonfradrag(skatteydere, skatterFoerPersonfradrag, kommunaleSatser, SKATTE_AAR);

			var skatterEfterPersonfradrag = modregnPersonfradragResults.Map(x => x.ModregnedeSkatter);

			skatterEfterPersonfradrag[0].ShouldEqual(IndkomstSkatter.Nul);
			skatterEfterPersonfradrag[1].ShouldEqual(IndkomstSkatter.Nul);
		}

		[Fact]
		public void Eksempel_42_()
		{
			var skatteydere = getPersonerForGifte();

			var selvangivneBeloeb = new ValueTuple<ISelvangivneBeloeb>(
				new SelvangivneBeloeb
				{
					PersonligIndkomstFremfoertUnderskud = 18000,
					Pension = 610000,
					NettoKapitalIndkomst = -30000,
					LigningsmaessigtFradrag = 8200
				},
				new SelvangivneBeloeb
				{
					PersonligIndkomstFremfoertUnderskud = 29000,
					Bruttoloen = -15000,
					NettoKapitalIndkomst = -20000
				});

			var indkomstOpgoerelseBeregner = new IndkomstOpgoerelseBeregner(_skattelovRegistry);
			var indkomster = indkomstOpgoerelseBeregner.BeregnIndkomster(selvangivneBeloeb, SKATTE_AAR);

			// Personlig indkomst og nettokapitalindkomst før modregning og fremførsel
			indkomster[0].PersonligIndkomstSkattegrundlag.ShouldEqual(610000);
			indkomster[0].NettoKapitalIndkomstSkattegrundlag.ShouldEqual(-30000);
			indkomster[1].PersonligIndkomstSkattegrundlag.ShouldEqual(-15000);
			indkomster[1].NettoKapitalIndkomstSkattegrundlag.ShouldEqual(-20000);

			var personligIndkomstUnderskudBeregner = new PersonligIndkomstUnderskudBeregner();
			personligIndkomstUnderskudBeregner.ModregningAfUnderskud(indkomster);

			// Personlig indkomst og nettokapitalindkomst efter modregning og fremførsel
			indkomster[0].PersonligIndkomstSkattegrundlag.ShouldEqual(548000);
			indkomster[0].NettoKapitalIndkomstSkattegrundlag.ShouldEqual(-30000);
			indkomster[0].PersonligIndkomstUnderskudTilFremfoersel.ShouldEqual(0);

			indkomster[1].PersonligIndkomstSkattegrundlag.ShouldEqual(0);
			indkomster[1].NettoKapitalIndkomstSkattegrundlag.ShouldEqual(-20000);
			indkomster[1].PersonligIndkomstUnderskudTilFremfoersel.ShouldEqual(0);

			var kommunaleSatser = getKommunaleSatserForGifte();

			// Beregning af skatter af personlig indkomst
			var skatterAfPersonligIndkomstBeregner = new SkatterAfPersonligIndkomstBeregner(_skattelovRegistry);
			var skatterAfPersonligIndkomst = skatterAfPersonligIndkomstBeregner.BeregnSkat(indkomster, kommunaleSatser, SKATTE_AAR);

			skatterAfPersonligIndkomst[0].Bundskat.ShouldEqual(27619.20m);
			skatterAfPersonligIndkomst[0].Mellemskat.ShouldEqual(0);
			skatterAfPersonligIndkomst[0].Topskat.ShouldEqual(30120);
			skatterAfPersonligIndkomst[1].Bundskat.ShouldEqual(0);
			skatterAfPersonligIndkomst[1].Mellemskat.ShouldEqual(0);
			skatterAfPersonligIndkomst[1].Topskat.ShouldEqual(0);

			// Skattepligtig indkomst før modregning og fremførsel
			indkomster[0].SkattepligtigIndkomst.ShouldEqual(571800);
			indkomster[1].SkattepligtigIndkomst.ShouldEqual(-35000);

			var skattepligtigIndkomstUnderskudBeregner = new SkattepligtigIndkomstUnderskudBeregner(_skattelovRegistry);
			skattepligtigIndkomstUnderskudBeregner.ModregningAfUnderskud(indkomster, skatterAfPersonligIndkomst, kommunaleSatser, SKATTE_AAR);

			// Skattepligtig indkomst efter modregning og fremførsel
			indkomster[0].SkattepligtigIndkomstSkattegrundlag.ShouldEqual(536800);
			indkomster[1].SkattepligtigIndkomstSkattegrundlag.ShouldEqual(0);

			// Beregning af skatter af skattepligtig indkomst
			var skatterAfSkattepligtigIndkomstBeregner = new SkatterAfSkattepligtigIndkomstBeregner(_skattelovRegistry);
			var skatterAfSkattepligtigIndkomst = skatterAfSkattepligtigIndkomstBeregner.BeregnSkat(skatteydere, indkomster, kommunaleSatser, SKATTE_AAR);

			skatterAfSkattepligtigIndkomst[0].Sum().ShouldEqual(173332.72m);
			skatterAfSkattepligtigIndkomst[1].ShouldEqual(SkatterAfSkattepligtigIndkomst.Nul);

			var skatterFoerPersonfradrag = SkatteUtility.CombineSkat(skatterAfPersonligIndkomst, skatterAfSkattepligtigIndkomst);

			var personfradragBeregner = new PersonfradragBeregner(_skattelovRegistry);
			var modregnPersonfradragResults = personfradragBeregner.ModregningAfPersonfradrag(skatteydere, skatterFoerPersonfradrag, kommunaleSatser, SKATTE_AAR);

			var skatterEfterPersonfradrag = modregnPersonfradragResults.Map(x => x.ModregnedeSkatter);

			skatterEfterPersonfradrag[0].Sum().ShouldEqual(199042.78m);
			skatterEfterPersonfradrag[1].ShouldEqual(IndkomstSkatter.Nul);
		}

		private static IKommunaleSatser getKommunaleSatser()
		{
			return new KommunaleSatser
			{
				Kommuneskattesats = 0.237m,
				Kirkeskattesats = 0.0059m
			};
		}

		private static ValueTuple<IKommunaleSatser> getKommunaleSatserForUgift()
		{
			return getKommunaleSatser().ToTuple();
		}

		
		private static ValueTuple<IKommunaleSatser> getKommunaleSatserForGifte()
		{
			return getKommunaleSatser().ToTupleOfSize(2);
		}

		private static ISkatteyder getPerson()
		{
			return new Skatteyder(new DateTime(1970, 6, 3), MedlemAfFolkekirken.Ja);
		}

		private static ValueTuple<ISkatteyder> getPersonerForUgift()
		{
			return getPerson().ToTuple();
		}

		private static ValueTuple<ISkatteyder> getPersonerForGifte()
		{
			return getPerson().ToTupleOfSize(2);
		}
	}
}