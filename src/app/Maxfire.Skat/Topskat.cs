using System;
using Maxfire.Skat.Extensions;

namespace Maxfire.Skat
{
	//////////////////////////////////////////////////////////////////////////////////////////////////////////
	//
	// PSL § 7: Topskatteberegningen
	// =============================
	//
	// Stk. 1. Skatten efter § 5, nr. 2, udgør 15 pct. af den personlige indkomst med tillæg af heri fradragne
	// og ikke medregnede beløb omfattet af beløbsgrænsen i pensionsbeskatningslovens § 16, stk. 1, og med 
	// tillæg af positiv nettokapitalindkomst, der overstiger et grundbeløb på 40.000 kr. (2010-niveau), 
	// i det omfang det samlede beløb overstiger bundfradraget anført i stk. 2.
	// Stk. 2. For indkomståret 2010 udgør bundfradraget 389.900 kr. (2010-niveau). For indkomståret
	// 2011 og senere indkomstår udgør bundfradraget 409.100 kr. (2010-niveau).
	// Stk. 3. Er en gift persons nettokapitalindkomst lavere end grundbeløbet for positiv nettokapitalindkomst
	// i stk. 1, forhøjes den anden ægtefælles grundbeløb med forskelsbeløbet, hvis ægtefællerne er samlevende 
	// ved indkomstårets udløb. Er personens nettokapitalindkomst negativ, modregnes dette beløb i den anden
	// ægtefælles positive nettokapitalindkomst, inden ægtefællens grundbeløb forhøjes efter 1. pkt.
	// Stk. 4. For ægtefæller beregnes skatten efter stk. 5-10, når de er samlevende i hele indkomståret 
	// og dette udgør en periode af et helt år.
	// Stk. 5. For hver ægtefælle beregnes skat med 15 pct. af vedkommendes personlige indkomst med tillæg 
	// af heri fradragne og ikke medregnede beløb omfattet af beløbsgrænsen i pensionsbeskatningslovens § 16, stk. 1, 
	// i det omfang dette beregningsgrundlag overstiger bundfradraget i stk. 2.
	// Stk. 6. Der beregnes tillige skat af ægtefællernes samlede positive nettokapitalindkomst. Til dette 
	// formål beregnes en skat hos den af ægtefællerne, der har det højeste beregningsgrundlag efter stk. 5. 
	// Skatten beregnes med 15 pct. af denne ægtefælles beregningsgrundlag efter stk. 5 med tillæg af 
	// den del af ægtefællernes samlede positive nettokapitalindkomst, der overstiger det dobbelte af 
	// grundbeløbet i stk. 1. Skatten beregnes dog kun i det omfang, det samlede beløb overstiger bundfradraget i stk.2.
	// Stk. 7. Forskellen mellem skatten efter stk. 6 og skatten efter stk. 5 for den ægtefælle, der har 
	// det højeste beregningsgrundlag efter stk. 5, udgør skatten af ægtefællernes samlede positive nettokapitalindkomst.
	// Stk. 8. Har kun den ene ægtefælle positiv nettokapitalindkomst over grundbeløbet i stk. 1, påhviler den 
	// samlede skat af ægtefællernes nettokapitalindkomst efter stk. 7 denne ægtefælle.
	// Stk. 9. Hvis begge ægtefæller har positiv nettokapitalindkomst over grundbeløbet i stk. 1, fordeles 
	// skatten af ægtefællernes samlede nettokapitalindkomst imellem dem efter forholdet mellem hver enkelt 
	// ægtefælles positive nettokapitalindkomst over grundbeløbet i stk. 1.
	// Stk. 10. Hvis ægtefællernes beregningsgrundlag efter stk. 5 er lige store, anses den af ægtefællerne, 
	// som har de største udgifter af den art, der fradrages ved opgørelsen af den skattepligtige indkomst, 
	// men ikke ved opgørelsen af personlig indkomst og kapitalindkomst, for at have det højeste 
	// beregningsgrundlag efter stk. 5.
	// Stk. 11. Grundbeløb og bundfradrag i stk. 1 og 2 reguleres efter § 20.
	//
	// PBL § 16, stk. 1
	// ================
	//
	// Dette beløb omhandler indskud til kapitalpensionsordninger, kapitalforsikring og bidrag til supplerende 
	// engangsydelser fra pensionskasser.
	//
	// PSL § 19: Skatteloftet
	// ======================
	//
	// Hvis summen af skatteprocenterne efter §§ 6, 6 a og 7 og 8 tillagt den skattepligtiges kommunale 
	// indkomstskatteprocent henholdsvis skatteprocenten efter § 8 c overstiger 59 pct. (51,5 pct i 2010), 
	// beregnes et nedslag i statsskatten (topskatten helt præcist) svarende til, at skatteprocenten 
	// efter § 7 blev nedsat med de overskydende procenter.
	//
	//////////////////////////////////////////////////////////////////////////////////////////////////////////
	public class TopskatBeregner
	{
		private readonly ISkattelovRegistry _skattelovRegistry;

		public TopskatBeregner(ISkattelovRegistry skattelovRegistry)
		{
			_skattelovRegistry = skattelovRegistry;
		}

		public ValueTuple<decimal> BeregnSkat(
			IValueTuple<IPersonligeBeloeb> indkomster, 
			int skatteAar, 
			IValueTuple<IKommunaleSatser> kommunaleSatser = null)
		{
			decimal topskatBundfradrag = _skattelovRegistry.GetTopskatBundfradrag(skatteAar);
			decimal positivNettoKapitalIndkomstGrundbeloeb = _skattelovRegistry.GetPositivNettoKapitalIndkomstGrundbeloeb(skatteAar);

			var topskatteGrundlag = BeregnGrundlag(indkomster, topskatBundfradrag, positivNettoKapitalIndkomstGrundbeloeb);

			return beregnSkatUnderSkatteloft(topskatteGrundlag, kommunaleSatser, skatteAar);
		}


// ReSharper disable MemberCanBeMadeStatic.Global
		public ValueTuple<decimal> BeregnGrundlag(
			IValueTuple<IPersonligeBeloeb> indkomster, 
			decimal topskatBundfradrag, 
			decimal positivNettoKapitalIndkomstGrundbeloeb)
// ReSharper restore MemberCanBeMadeStatic.Global
		{
			var grundlagUdenPositivNettoKapitalIndkomst = BeregnGrundlagUdenPositivNettoKapitalIndkomst(indkomster, topskatBundfradrag);
			var grundlagAfPositivNettoKapitalIndkomst = BeregnGrundlagAfPositivNettoKapitalIndkomst(indkomster, topskatBundfradrag, positivNettoKapitalIndkomstGrundbeloeb);
			return grundlagUdenPositivNettoKapitalIndkomst + grundlagAfPositivNettoKapitalIndkomst;
		}


		// ReSharper disable MemberCanBeMadeStatic.Global
		public ValueTuple<decimal> BeregnGrundlagForGroenCheck(
			IValueTuple<IPersonligeBeloeb> indkomster, 
			decimal topskatBundfradrag, 
			decimal positivNettoKapitalIndkomstGrundbeloeb)
		// ReSharper restore MemberCanBeMadeStatic.Global
		{
			var grundlagUdenPositivNettoKapitalIndkomst = BeregnGrundlagUdenPositivNettoKapitalIndkomst(indkomster, topskatBundfradrag);
			// Ved beregning af topskattegrundlaget for "grøn check" bliver ægtefællers samlede grundlag 
			// opgjort sådan at den samlede positiv nettokapitalindkomst til beskatning, bliver lagt 
			// medregnet hos den ægtefælle med det største grundlag.
			var grundlagAfPositivNettoKapitalIndkomst 
				= beregnGrundlagAfPositivNettoKapitalIndkomst(indkomster, topskatBundfradrag, 
					positivNettoKapitalIndkomstGrundbeloeb,
					(indexOfMaxGrundlag, nettoKapitalIndkomstTilBeskatning) => indexOfMaxGrundlag.ToUnitTuple());
			return grundlagUdenPositivNettoKapitalIndkomst + grundlagAfPositivNettoKapitalIndkomst;
		}

		static ValueTuple<decimal> beregnGrundlagUdenPositivNettoKapitalIndkomst(
			IValueTuple<IPersonligeBeloeb> indkomster, 
			decimal topskatBundfradrag)
		{
			var personligIndkomst = indkomster.Map(x => x.Skattegrundlag.PersonligIndkomst);
			var kapitalPensionsindskud = indkomster.Map(x => x.Skattegrundlag.KapitalPensionsindskud);
			return personligIndkomst + kapitalPensionsindskud - topskatBundfradrag;
		}

// ReSharper disable MemberCanBeMadeStatic.Global
		public ValueTuple<decimal> BeregnGrundlagUdenPositivNettoKapitalIndkomst(
			IValueTuple<IPersonligeBeloeb> indkomster, 
			decimal topskatBundfradrag)
// ReSharper restore MemberCanBeMadeStatic.Global
		{
			return +beregnGrundlagUdenPositivNettoKapitalIndkomst(indkomster, topskatBundfradrag);
		}

// ReSharper disable MemberCanBeMadeStatic.Global
		public ValueTuple<decimal> BeregnGrundlagAfPositivNettoKapitalIndkomst(
			IValueTuple<IPersonligeBeloeb> indkomster, 
			decimal topskatBundfradrag, 
			decimal positivNettoKapitalIndkomstGrundbeloeb)
// ReSharper restore MemberCanBeMadeStatic.Global
		{
			// Ved beregning af topskattegrundlaget fordeles skat af samlet positiv nettokapitalindkomst 
			// over bundfradraget ift. fordelingen af ægtefællernes positive nettokapitalindkomst.
			return beregnGrundlagAfPositivNettoKapitalIndkomst(indkomster, topskatBundfradrag,
				positivNettoKapitalIndkomstGrundbeloeb,
				(indexOfMaxGrundlag, nettoKapitalIndkomstTilBeskatning) =>
				{
					var positivNettoKapitalIndkomstTilBeskatning 
						= nettoKapitalIndkomstTilBeskatning.DifferenceGreaterThan(positivNettoKapitalIndkomstGrundbeloeb);
					return positivNettoKapitalIndkomstTilBeskatning / positivNettoKapitalIndkomstTilBeskatning.Sum();
				});
		}

		private static ValueTuple<decimal> beregnGrundlagAfPositivNettoKapitalIndkomst(
			IValueTuple<IPersonligeBeloeb> indkomster, 
			decimal topskatBundfradrag, 
			decimal positivNettoKapitalIndkomstGrundbeloeb, 
			Func<int, ValueTuple<decimal>, 
			ValueTuple<decimal>> fordelingsnoegleProvider)
		{
			var nettoKapitalIndkomst = indkomster.Map(x => x.Skattegrundlag.NettoKapitalIndkomst);
			var nettoKapitalIndkomstTilBeskatning = nettoKapitalIndkomst.NedbringPositivtMedEvtNegativt();

			var samletNettoKapitalIndkomstTilBeskatning = nettoKapitalIndkomstTilBeskatning.Sum() - indkomster.Size * positivNettoKapitalIndkomstGrundbeloeb;

			if (samletNettoKapitalIndkomstTilBeskatning <= 0)
			{
				return 0m.ToTupleOfSize(indkomster.Size);
			}

			var grundlagUdenPositivNettoKapitalIndkomst = beregnGrundlagUdenPositivNettoKapitalIndkomst(indkomster, topskatBundfradrag);
			
			int indexOfMaxGrundlag = getIndexOfMaxGrundlag(indkomster, grundlagUdenPositivNettoKapitalIndkomst);

			decimal grundlagInklSamletPositivNettoKapitalIndkomst
				= (grundlagUdenPositivNettoKapitalIndkomst[indexOfMaxGrundlag] + samletNettoKapitalIndkomstTilBeskatning).NonNegative();

			decimal samletGrundlagAfPositivNettoKapitalIndkomst
				= grundlagInklSamletPositivNettoKapitalIndkomst - grundlagUdenPositivNettoKapitalIndkomst[indexOfMaxGrundlag].NonNegative();

			ValueTuple<decimal> fordelingsnoegle = fordelingsnoegleProvider(indexOfMaxGrundlag, nettoKapitalIndkomstTilBeskatning);

			return fordelingsnoegle * samletGrundlagAfPositivNettoKapitalIndkomst;
		}

		private static int getIndexOfMaxGrundlag(
			IValueTuple<IPersonligeBeloeb> indkomster, 
			ValueTuple<decimal> grundlagUdenPositivNettoKapitalIndkomst)
		{
			if (grundlagUdenPositivNettoKapitalIndkomst.Size == 1)
			{
				return 0;
			}
			int indexOfMaxGrundlag;
			if (grundlagUdenPositivNettoKapitalIndkomst[0] > grundlagUdenPositivNettoKapitalIndkomst[1])
			{
				indexOfMaxGrundlag = 0;
			}
			else if (grundlagUdenPositivNettoKapitalIndkomst[0] < grundlagUdenPositivNettoKapitalIndkomst[1])
			{
				indexOfMaxGrundlag = 1;
			}
			else
			{
				indexOfMaxGrundlag = indkomster[0].Skattegrundlag.LigningsmaessigeFradrag > indkomster[1].Skattegrundlag.LigningsmaessigeFradrag ? 0 : 1;
			}
			return indexOfMaxGrundlag;
		}

		private ValueTuple<decimal> beregnSkatUnderSkatteloft(
			ValueTuple<decimal> grundlag, 
			IValueTuple<IKommunaleSatser> kommunaleSatser, 
			int skatteAar)
		{
			decimal bundskattesats = _skattelovRegistry.GetBundSkattesats(skatteAar);
			decimal mellemskattesats = _skattelovRegistry.GetMellemSkattesats(skatteAar);
			decimal topskattesats = _skattelovRegistry.GetTopSkattesats(skatteAar);
			decimal sundhedsbidragsats = _skattelovRegistry.GetSundhedsbidragSkattesats(skatteAar);
			var kommunaleskattesatser = kommunaleSatser != null ? 
				kommunaleSatser.Map(x => x.Kommuneskattesats) : 
				0.0m.ToTupleOfSize(grundlag.Size);

			decimal fastsats = bundskattesats + mellemskattesats + topskattesats + sundhedsbidragsats;
			var skattesatser = fastsats + kommunaleskattesatser;

			decimal skatteloftsats = _skattelovRegistry.GetSkatteloftSkattesats(skatteAar);
			var nedslagssatsen = +(skattesatser - skatteloftsats);

			var topskattesatsen = +(topskattesats - nedslagssatsen);

			var topskat = topskattesatsen * grundlag;

			return topskat.RoundMoney();
		}
	}
}