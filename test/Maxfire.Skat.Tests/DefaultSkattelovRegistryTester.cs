using System;
using Shouldly;
using Xunit;

namespace Maxfire.Skat.UnitTests
{
	public class DefaultSkattelovRegistryTester
	{
		private readonly DefaultSkattelovRegistry _registry;

		public DefaultSkattelovRegistryTester()
		{
			_registry = new DefaultSkattelovRegistry();
		}

		[Fact]
		public void Vaerdier2008Throws()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => _registry.GetBundSkattesats(2008));
		}

		[Fact]
		public void Vaerdier2009()
		{
			const int AAR = 2009;

			_registry.GetAktieIndkomstLavesteProgressionsgraense(AAR).ShouldBe(48300);
			_registry.GetAktieIndkomstHoejesteProgressionsgraense(AAR).ShouldBe(106100);
			_registry.GetAktieIndkomstLavesteSkattesats(AAR).ShouldBe(0.28m);
			_registry.GetAktieIndkomstMellemsteSkattesats(AAR).ShouldBe(0.43m);
			_registry.GetAktieIndkomstHoejesteSkattesats(AAR).ShouldBe(0.45m);
			_registry.GetAMBidragSkattesats(AAR).ShouldBe(0.08m);
			_registry.GetSundhedsbidragSkattesats(AAR).ShouldBe(0.08m);
			_registry.GetBundSkattesats(AAR).ShouldBe(0.0504m);
			_registry.GetMellemSkattesats(AAR).ShouldBe(0.06m);
			_registry.GetTopSkattesats(AAR).ShouldBe(0.15m);
			_registry.GetSkatteloftSkattesats(AAR).ShouldBe(0.59m);
			_registry.GetPersonfradrag(AAR, 20, false).ShouldBe(42900);
			_registry.GetPersonfradrag(AAR, 17, true).ShouldBe(42900);
			_registry.GetPersonfradrag(AAR, 17, false).ShouldBe(32200);
			_registry.GetBundLettelseBundfradrag(AAR, 20, false).ShouldBe(decimal.MaxValue);
			_registry.GetBundLettelseBundfradrag(AAR, 17, true).ShouldBe(decimal.MaxValue);
			_registry.GetBundLettelseBundfradrag(AAR, 17, false).ShouldBe(decimal.MaxValue);
			_registry.GetMellemLettelseBundfradrag(AAR).ShouldBe(decimal.MaxValue);
			_registry.GetTopLettelseBundfradrag(AAR).ShouldBe(decimal.MaxValue);
			_registry.GetMellemskatBundfradrag(AAR).ShouldBe(347200);
			_registry.GetTopskatBundfradrag(AAR).ShouldBe(347200);
			_registry.GetPositivNettoKapitalIndkomstGrundbeloeb(AAR).ShouldBe(0);
			_registry.GetNegativNettoKapitalIndkomstGrundbeloeb(AAR).ShouldBe(0);
			_registry.GetNegativNettoKapitalIndkomstSats(AAR).ShouldBe(0);
			_registry.GetBeskaeftigelsesfradragGrundbeloeb(AAR).ShouldBe(13600);
			_registry.GetBeskaeftigelsesfradragSats(AAR).ShouldBe(0.0425m);
			_registry.GetGroenCheckPrVoksen(AAR).ShouldBe(0);
			_registry.GetGroenCheckPrBarn(AAR).ShouldBe(0);
			_registry.GetGroenCheckBundfradrag(AAR).ShouldBe(decimal.MaxValue);
			_registry.GetGroenCheckAftrapningssats(AAR).ShouldBe(0);
		}

		[Fact]
		public void Vaerdier2010()
		{
			const int AAR = 2010;

			_registry.GetAktieIndkomstLavesteProgressionsgraense(AAR).ShouldBe(48300);
			_registry.GetAktieIndkomstHoejesteProgressionsgraense(AAR).ShouldBe(decimal.MaxValue);
			_registry.GetAktieIndkomstLavesteSkattesats(AAR).ShouldBe(0.28m);
			_registry.GetAktieIndkomstMellemsteSkattesats(AAR).ShouldBe(0.42m);
			_registry.GetAktieIndkomstHoejesteSkattesats(AAR).ShouldBe(0);
			_registry.GetAMBidragSkattesats(AAR).ShouldBe(0.08m);
			_registry.GetSundhedsbidragSkattesats(AAR).ShouldBe(0.08m);
			_registry.GetBundSkattesats(AAR).ShouldBe(0.0367m);
			_registry.GetMellemSkattesats(AAR).ShouldBe(0);
			_registry.GetTopSkattesats(AAR).ShouldBe(0.15m);
			_registry.GetSkatteloftSkattesats(AAR).ShouldBe(0.515m);
			_registry.GetPersonfradrag(AAR, 20, false).ShouldBe(42900);
			_registry.GetPersonfradrag(AAR, 17, true).ShouldBe(42900);
			_registry.GetPersonfradrag(AAR, 17, false).ShouldBe(32200);
			_registry.GetBundLettelseBundfradrag(AAR, 20, false).ShouldBe(decimal.MaxValue);
			_registry.GetBundLettelseBundfradrag(AAR, 17, true).ShouldBe(decimal.MaxValue);
			_registry.GetBundLettelseBundfradrag(AAR, 17, false).ShouldBe(decimal.MaxValue);
			_registry.GetMellemLettelseBundfradrag(AAR).ShouldBe(decimal.MaxValue);
			_registry.GetTopLettelseBundfradrag(AAR).ShouldBe(decimal.MaxValue);
			_registry.GetMellemskatBundfradrag(AAR).ShouldBe(decimal.MaxValue);
			_registry.GetTopskatBundfradrag(AAR).ShouldBe(389900);
			_registry.GetPositivNettoKapitalIndkomstGrundbeloeb(AAR).ShouldBe(40000);
			_registry.GetNegativNettoKapitalIndkomstGrundbeloeb(AAR).ShouldBe(0);
			_registry.GetNegativNettoKapitalIndkomstSats(AAR).ShouldBe(0);
			_registry.GetBeskaeftigelsesfradragGrundbeloeb(AAR).ShouldBe(13600);
			_registry.GetBeskaeftigelsesfradragSats(AAR).ShouldBe(0.0425m);
			_registry.GetGroenCheckPrVoksen(AAR).ShouldBe(1300);
			_registry.GetGroenCheckPrBarn(AAR).ShouldBe(300);
			_registry.GetGroenCheckBundfradrag(AAR).ShouldBe(362800);
			_registry.GetGroenCheckAftrapningssats(AAR).ShouldBe(0.075m);
		}

		[Fact]
		public void Vaerdier2011()
		{
			const int AAR = 2011;

			_registry.GetAktieIndkomstLavesteProgressionsgraense(AAR).ShouldBe(48300);
			_registry.GetAktieIndkomstHoejesteProgressionsgraense(AAR).ShouldBe(decimal.MaxValue);
			_registry.GetAktieIndkomstLavesteSkattesats(AAR).ShouldBe(0.28m);
			_registry.GetAktieIndkomstMellemsteSkattesats(AAR).ShouldBe(0.42m);
			_registry.GetAktieIndkomstHoejesteSkattesats(AAR).ShouldBe(0);
			_registry.GetAMBidragSkattesats(AAR).ShouldBe(0.08m);
			_registry.GetSundhedsbidragSkattesats(AAR).ShouldBe(0.08m);
			_registry.GetBundSkattesats(AAR).ShouldBe(0.0367m);
			_registry.GetMellemSkattesats(AAR).ShouldBe(0);
			_registry.GetTopSkattesats(AAR).ShouldBe(0.15m);
			_registry.GetSkatteloftSkattesats(AAR).ShouldBe(0.515m);
			_registry.GetPersonfradrag(AAR, 20, false).ShouldBe(42900);
			_registry.GetPersonfradrag(AAR, 17, true).ShouldBe(42900);
			_registry.GetPersonfradrag(AAR, 17, false).ShouldBe(32200);
			_registry.GetBundLettelseBundfradrag(AAR, 20, false).ShouldBe(decimal.MaxValue);
			_registry.GetBundLettelseBundfradrag(AAR, 17, true).ShouldBe(decimal.MaxValue);
			_registry.GetBundLettelseBundfradrag(AAR, 17, false).ShouldBe(decimal.MaxValue);
			_registry.GetMellemLettelseBundfradrag(AAR).ShouldBe(decimal.MaxValue);
			_registry.GetTopLettelseBundfradrag(AAR).ShouldBe(decimal.MaxValue);
			_registry.GetMellemskatBundfradrag(AAR).ShouldBe(decimal.MaxValue);
			_registry.GetTopskatBundfradrag(AAR).ShouldBe(409100);
			_registry.GetPositivNettoKapitalIndkomstGrundbeloeb(AAR).ShouldBe(40000);
			_registry.GetNegativNettoKapitalIndkomstGrundbeloeb(AAR).ShouldBe(0);
			_registry.GetNegativNettoKapitalIndkomstSats(AAR).ShouldBe(0);
			_registry.GetBeskaeftigelsesfradragGrundbeloeb(AAR).ShouldBe(13600);
			_registry.GetBeskaeftigelsesfradragSats(AAR).ShouldBe(0.0425m);
			_registry.GetGroenCheckPrVoksen(AAR).ShouldBe(1300);
			_registry.GetGroenCheckPrBarn(AAR).ShouldBe(300);
			_registry.GetGroenCheckBundfradrag(AAR).ShouldBe(362800);
			_registry.GetGroenCheckAftrapningssats(AAR).ShouldBe(0.075m);
		}

		[Fact]
		public void Vaerdier2012()
		{
			const int AAR = 2012;

			_registry.GetAktieIndkomstLavesteProgressionsgraense(AAR).ShouldBe(48300);
			_registry.GetAktieIndkomstHoejesteProgressionsgraense(AAR).ShouldBe(decimal.MaxValue);
			_registry.GetAktieIndkomstLavesteSkattesats(AAR).ShouldBe(0.27m);
			_registry.GetAktieIndkomstMellemsteSkattesats(AAR).ShouldBe(0.42m);
			_registry.GetAktieIndkomstHoejesteSkattesats(AAR).ShouldBe(0);
			_registry.GetAMBidragSkattesats(AAR).ShouldBe(0.08m);
			_registry.GetSundhedsbidragSkattesats(AAR).ShouldBe(0.07m);
			_registry.GetBundSkattesats(AAR).ShouldBe(0.0467m);
			_registry.GetMellemSkattesats(AAR).ShouldBe(0);
			_registry.GetTopSkattesats(AAR).ShouldBe(0.15m);
			_registry.GetSkatteloftSkattesats(AAR).ShouldBe(0.515m);
			_registry.GetPersonfradrag(AAR, 20, false).ShouldBe(42900);
			_registry.GetPersonfradrag(AAR, 17, true).ShouldBe(42900);
			_registry.GetPersonfradrag(AAR, 17, false).ShouldBe(32200);
			_registry.GetBundLettelseBundfradrag(AAR, 20, false).ShouldBe(44800);
			_registry.GetBundLettelseBundfradrag(AAR, 17, true).ShouldBe(44800);
			_registry.GetBundLettelseBundfradrag(AAR, 17, false).ShouldBe(33600);
			_registry.GetMellemLettelseBundfradrag(AAR).ShouldBe(362800);
			_registry.GetTopLettelseBundfradrag(AAR).ShouldBe(362800);
			_registry.GetMellemskatBundfradrag(AAR).ShouldBe(decimal.MaxValue);
			_registry.GetTopskatBundfradrag(AAR).ShouldBe(409100);
			_registry.GetPositivNettoKapitalIndkomstGrundbeloeb(AAR).ShouldBe(40000);
			_registry.GetNegativNettoKapitalIndkomstGrundbeloeb(AAR).ShouldBe(50000);
			_registry.GetNegativNettoKapitalIndkomstSats(AAR).ShouldBe(0.01m);
			_registry.GetBeskaeftigelsesfradragGrundbeloeb(AAR).ShouldBe(14100);
			_registry.GetBeskaeftigelsesfradragSats(AAR).ShouldBe(0.044m);
			_registry.GetGroenCheckPrVoksen(AAR).ShouldBe(1300);
			_registry.GetGroenCheckPrBarn(AAR).ShouldBe(300);
			_registry.GetGroenCheckBundfradrag(AAR).ShouldBe(362800);
			_registry.GetGroenCheckAftrapningssats(AAR).ShouldBe(0.075m);
		}

		[Fact]
		public void Vaerdier2013()
		{
			const int AAR = 2013;

			_registry.GetAktieIndkomstLavesteProgressionsgraense(AAR).ShouldBe(48300);
			_registry.GetAktieIndkomstHoejesteProgressionsgraense(AAR).ShouldBe(decimal.MaxValue);
			_registry.GetAktieIndkomstLavesteSkattesats(AAR).ShouldBe(0.27m);
			_registry.GetAktieIndkomstMellemsteSkattesats(AAR).ShouldBe(0.42m);
			_registry.GetAktieIndkomstHoejesteSkattesats(AAR).ShouldBe(0);
			_registry.GetAMBidragSkattesats(AAR).ShouldBe(0.08m);
			_registry.GetSundhedsbidragSkattesats(AAR).ShouldBe(0.06m);
			_registry.GetBundSkattesats(AAR).ShouldBe(0.0567m);
			_registry.GetMellemSkattesats(AAR).ShouldBe(0);
			_registry.GetTopSkattesats(AAR).ShouldBe(0.15m);
			_registry.GetSkatteloftSkattesats(AAR).ShouldBe(0.515m);
			_registry.GetPersonfradrag(AAR, 20, false).ShouldBe(42900);
			_registry.GetPersonfradrag(AAR, 17, true).ShouldBe(42900);
			_registry.GetPersonfradrag(AAR, 17, false).ShouldBe(32200);
			_registry.GetBundLettelseBundfradrag(AAR, 20, false).ShouldBe(44800);
			_registry.GetBundLettelseBundfradrag(AAR, 17, true).ShouldBe(44800);
			_registry.GetBundLettelseBundfradrag(AAR, 17, false).ShouldBe(33600);
			_registry.GetMellemLettelseBundfradrag(AAR).ShouldBe(362800);
			_registry.GetTopLettelseBundfradrag(AAR).ShouldBe(362800);
			_registry.GetMellemskatBundfradrag(AAR).ShouldBe(decimal.MaxValue);
			_registry.GetTopskatBundfradrag(AAR).ShouldBe(409100);
			_registry.GetPositivNettoKapitalIndkomstGrundbeloeb(AAR).ShouldBe(40000);
			_registry.GetNegativNettoKapitalIndkomstGrundbeloeb(AAR).ShouldBe(50000);
			_registry.GetNegativNettoKapitalIndkomstSats(AAR).ShouldBe(0.02m);
			_registry.GetBeskaeftigelsesfradragGrundbeloeb(AAR).ShouldBe(14400);
			_registry.GetBeskaeftigelsesfradragSats(AAR).ShouldBe(0.045m);
			_registry.GetGroenCheckPrVoksen(AAR).ShouldBe(1300);
			_registry.GetGroenCheckPrBarn(AAR).ShouldBe(300);
			_registry.GetGroenCheckBundfradrag(AAR).ShouldBe(362800);
			_registry.GetGroenCheckAftrapningssats(AAR).ShouldBe(0.075m);
		}

		[Fact]
		public void Vaerdier2014()
		{
			const int AAR = 2014;

			_registry.GetAktieIndkomstLavesteProgressionsgraense(AAR).ShouldBe(48300);
			_registry.GetAktieIndkomstHoejesteProgressionsgraense(AAR).ShouldBe(decimal.MaxValue);
			_registry.GetAktieIndkomstLavesteSkattesats(AAR).ShouldBe(0.27m);
			_registry.GetAktieIndkomstMellemsteSkattesats(AAR).ShouldBe(0.42m);
			_registry.GetAktieIndkomstHoejesteSkattesats(AAR).ShouldBe(0);
			_registry.GetAMBidragSkattesats(AAR).ShouldBe(0.08m);
			_registry.GetSundhedsbidragSkattesats(AAR).ShouldBe(0.05m);
			_registry.GetBundSkattesats(AAR).ShouldBe(0.0667m);
			_registry.GetMellemSkattesats(AAR).ShouldBe(0);
			_registry.GetTopSkattesats(AAR).ShouldBe(0.15m);
			_registry.GetSkatteloftSkattesats(AAR).ShouldBe(0.515m);
			_registry.GetPersonfradrag(AAR, 20, false).ShouldBe(42900);
			_registry.GetPersonfradrag(AAR, 17, true).ShouldBe(42900);
			_registry.GetPersonfradrag(AAR, 17, false).ShouldBe(32200);
			_registry.GetBundLettelseBundfradrag(AAR, 20, false).ShouldBe(44800);
			_registry.GetBundLettelseBundfradrag(AAR, 17, true).ShouldBe(44800);
			_registry.GetBundLettelseBundfradrag(AAR, 17, false).ShouldBe(33600);
			_registry.GetMellemLettelseBundfradrag(AAR).ShouldBe(362800);
			_registry.GetTopLettelseBundfradrag(AAR).ShouldBe(362800);
			_registry.GetMellemskatBundfradrag(AAR).ShouldBe(decimal.MaxValue);
			_registry.GetTopskatBundfradrag(AAR).ShouldBe(409100);
			_registry.GetPositivNettoKapitalIndkomstGrundbeloeb(AAR).ShouldBe(40000);
			_registry.GetNegativNettoKapitalIndkomstGrundbeloeb(AAR).ShouldBe(50000);
			_registry.GetNegativNettoKapitalIndkomstSats(AAR).ShouldBe(0.03m);
			_registry.GetBeskaeftigelsesfradragGrundbeloeb(AAR).ShouldBe(14900);
			_registry.GetBeskaeftigelsesfradragSats(AAR).ShouldBe(0.0465m);
			_registry.GetGroenCheckPrVoksen(AAR).ShouldBe(1300);
			_registry.GetGroenCheckPrBarn(AAR).ShouldBe(300);
			_registry.GetGroenCheckBundfradrag(AAR).ShouldBe(362800);
			_registry.GetGroenCheckAftrapningssats(AAR).ShouldBe(0.075m);
		}

		[Fact]
		public void Vaerdier2015()
		{
			const int AAR = 2015;

			_registry.GetAktieIndkomstLavesteProgressionsgraense(AAR).ShouldBe(48300);
			_registry.GetAktieIndkomstHoejesteProgressionsgraense(AAR).ShouldBe(decimal.MaxValue);
			_registry.GetAktieIndkomstLavesteSkattesats(AAR).ShouldBe(0.27m);
			_registry.GetAktieIndkomstMellemsteSkattesats(AAR).ShouldBe(0.42m);
			_registry.GetAktieIndkomstHoejesteSkattesats(AAR).ShouldBe(0);
			_registry.GetAMBidragSkattesats(AAR).ShouldBe(0.08m);
			_registry.GetSundhedsbidragSkattesats(AAR).ShouldBe(0.04m);
			_registry.GetBundSkattesats(AAR).ShouldBe(0.0767m);
			_registry.GetMellemSkattesats(AAR).ShouldBe(0);
			_registry.GetTopSkattesats(AAR).ShouldBe(0.15m);
			_registry.GetSkatteloftSkattesats(AAR).ShouldBe(0.515m);
			_registry.GetPersonfradrag(AAR, 20, false).ShouldBe(42900);
			_registry.GetPersonfradrag(AAR, 17, true).ShouldBe(42900);
			_registry.GetPersonfradrag(AAR, 17, false).ShouldBe(32200);
			_registry.GetBundLettelseBundfradrag(AAR, 20, false).ShouldBe(44800);
			_registry.GetBundLettelseBundfradrag(AAR, 17, true).ShouldBe(44800);
			_registry.GetBundLettelseBundfradrag(AAR, 17, false).ShouldBe(33600);
			_registry.GetMellemLettelseBundfradrag(AAR).ShouldBe(362800);
			_registry.GetTopLettelseBundfradrag(AAR).ShouldBe(362800);
			_registry.GetMellemskatBundfradrag(AAR).ShouldBe(decimal.MaxValue);
			_registry.GetTopskatBundfradrag(AAR).ShouldBe(409100);
			_registry.GetPositivNettoKapitalIndkomstGrundbeloeb(AAR).ShouldBe(40000);
			_registry.GetNegativNettoKapitalIndkomstGrundbeloeb(AAR).ShouldBe(50000);
			_registry.GetNegativNettoKapitalIndkomstSats(AAR).ShouldBe(0.04m);
			_registry.GetBeskaeftigelsesfradragGrundbeloeb(AAR).ShouldBe(15400);
			_registry.GetBeskaeftigelsesfradragSats(AAR).ShouldBe(0.048m);
			_registry.GetGroenCheckPrVoksen(AAR).ShouldBe(1300);
			_registry.GetGroenCheckPrBarn(AAR).ShouldBe(300);
			_registry.GetGroenCheckBundfradrag(AAR).ShouldBe(362800);
			_registry.GetGroenCheckAftrapningssats(AAR).ShouldBe(0.075m);
		}

		[Fact]
		public void Vaerdier2016()
		{
			const int AAR = 2016;

			_registry.GetAktieIndkomstLavesteProgressionsgraense(AAR).ShouldBe(48300);
			_registry.GetAktieIndkomstHoejesteProgressionsgraense(AAR).ShouldBe(decimal.MaxValue);
			_registry.GetAktieIndkomstLavesteSkattesats(AAR).ShouldBe(0.27m);
			_registry.GetAktieIndkomstMellemsteSkattesats(AAR).ShouldBe(0.42m);
			_registry.GetAktieIndkomstHoejesteSkattesats(AAR).ShouldBe(0);
			_registry.GetAMBidragSkattesats(AAR).ShouldBe(0.08m);
			_registry.GetSundhedsbidragSkattesats(AAR).ShouldBe(0.03m);
			_registry.GetBundSkattesats(AAR).ShouldBe(0.0867m);
			_registry.GetMellemSkattesats(AAR).ShouldBe(0);
			_registry.GetTopSkattesats(AAR).ShouldBe(0.15m);
			_registry.GetSkatteloftSkattesats(AAR).ShouldBe(0.515m);
			_registry.GetPersonfradrag(AAR, 20, false).ShouldBe(42900);
			_registry.GetPersonfradrag(AAR, 17, true).ShouldBe(42900);
			_registry.GetPersonfradrag(AAR, 17, false).ShouldBe(32200);
			_registry.GetBundLettelseBundfradrag(AAR, 20, false).ShouldBe(44800);
			_registry.GetBundLettelseBundfradrag(AAR, 17, true).ShouldBe(44800);
			_registry.GetBundLettelseBundfradrag(AAR, 17, false).ShouldBe(33600);
			_registry.GetMellemLettelseBundfradrag(AAR).ShouldBe(362800);
			_registry.GetTopLettelseBundfradrag(AAR).ShouldBe(362800);
			_registry.GetMellemskatBundfradrag(AAR).ShouldBe(decimal.MaxValue);
			_registry.GetTopskatBundfradrag(AAR).ShouldBe(409100);
			_registry.GetPositivNettoKapitalIndkomstGrundbeloeb(AAR).ShouldBe(40000);
			_registry.GetNegativNettoKapitalIndkomstGrundbeloeb(AAR).ShouldBe(50000);
			_registry.GetNegativNettoKapitalIndkomstSats(AAR).ShouldBe(0.05m);
			_registry.GetBeskaeftigelsesfradragGrundbeloeb(AAR).ShouldBe(16000);
			_registry.GetBeskaeftigelsesfradragSats(AAR).ShouldBe(0.05m);
			_registry.GetGroenCheckPrVoksen(AAR).ShouldBe(1300);
			_registry.GetGroenCheckPrBarn(AAR).ShouldBe(300);
			_registry.GetGroenCheckBundfradrag(AAR).ShouldBe(362800);
			_registry.GetGroenCheckAftrapningssats(AAR).ShouldBe(0.075m);
		}

		[Fact]
		public void Vaerdier2017()
		{
			const int AAR = 2017;

			_registry.GetAktieIndkomstLavesteProgressionsgraense(AAR).ShouldBe(48300);
			_registry.GetAktieIndkomstHoejesteProgressionsgraense(AAR).ShouldBe(decimal.MaxValue);
			_registry.GetAktieIndkomstLavesteSkattesats(AAR).ShouldBe(0.27m);
			_registry.GetAktieIndkomstMellemsteSkattesats(AAR).ShouldBe(0.42m);
			_registry.GetAktieIndkomstHoejesteSkattesats(AAR).ShouldBe(0);
			_registry.GetAMBidragSkattesats(AAR).ShouldBe(0.08m);
			_registry.GetSundhedsbidragSkattesats(AAR).ShouldBe(0.02m);
			_registry.GetBundSkattesats(AAR).ShouldBe(0.0967m);
			_registry.GetMellemSkattesats(AAR).ShouldBe(0);
			_registry.GetTopSkattesats(AAR).ShouldBe(0.15m);
			_registry.GetSkatteloftSkattesats(AAR).ShouldBe(0.515m);
			_registry.GetPersonfradrag(AAR, 20, false).ShouldBe(42900);
			_registry.GetPersonfradrag(AAR, 17, true).ShouldBe(42900);
			_registry.GetPersonfradrag(AAR, 17, false).ShouldBe(32200);
			_registry.GetBundLettelseBundfradrag(AAR, 20, false).ShouldBe(44800);
			_registry.GetBundLettelseBundfradrag(AAR, 17, true).ShouldBe(44800);
			_registry.GetBundLettelseBundfradrag(AAR, 17, false).ShouldBe(33600);
			_registry.GetMellemLettelseBundfradrag(AAR).ShouldBe(362800);
			_registry.GetTopLettelseBundfradrag(AAR).ShouldBe(362800);
			_registry.GetMellemskatBundfradrag(AAR).ShouldBe(decimal.MaxValue);
			_registry.GetTopskatBundfradrag(AAR).ShouldBe(409100);
			_registry.GetPositivNettoKapitalIndkomstGrundbeloeb(AAR).ShouldBe(40000);
			_registry.GetNegativNettoKapitalIndkomstGrundbeloeb(AAR).ShouldBe(50000);
			_registry.GetNegativNettoKapitalIndkomstSats(AAR).ShouldBe(0.06m);
			_registry.GetBeskaeftigelsesfradragGrundbeloeb(AAR).ShouldBe(16600);
			_registry.GetBeskaeftigelsesfradragSats(AAR).ShouldBe(0.052m);
			_registry.GetGroenCheckPrVoksen(AAR).ShouldBe(1300);
			_registry.GetGroenCheckPrBarn(AAR).ShouldBe(300);
			_registry.GetGroenCheckBundfradrag(AAR).ShouldBe(362800);
			_registry.GetGroenCheckAftrapningssats(AAR).ShouldBe(0.075m);
		}

		[Fact]
		public void Vaerdier2018()
		{
			const int AAR = 2018;

			_registry.GetAktieIndkomstLavesteProgressionsgraense(AAR).ShouldBe(48300);
			_registry.GetAktieIndkomstHoejesteProgressionsgraense(AAR).ShouldBe(decimal.MaxValue);
			_registry.GetAktieIndkomstLavesteSkattesats(AAR).ShouldBe(0.27m);
			_registry.GetAktieIndkomstMellemsteSkattesats(AAR).ShouldBe(0.42m);
			_registry.GetAktieIndkomstHoejesteSkattesats(AAR).ShouldBe(0);
			_registry.GetAMBidragSkattesats(AAR).ShouldBe(0.08m);
			_registry.GetSundhedsbidragSkattesats(AAR).ShouldBe(0.01m);
			_registry.GetBundSkattesats(AAR).ShouldBe(0.1067m);
			_registry.GetMellemSkattesats(AAR).ShouldBe(0);
			_registry.GetTopSkattesats(AAR).ShouldBe(0.15m);
			_registry.GetSkatteloftSkattesats(AAR).ShouldBe(0.515m);
			_registry.GetPersonfradrag(AAR, 20, false).ShouldBe(42900);
			_registry.GetPersonfradrag(AAR, 17, true).ShouldBe(42900);
			_registry.GetPersonfradrag(AAR, 17, false).ShouldBe(32200);
			_registry.GetBundLettelseBundfradrag(AAR, 20, false).ShouldBe(44800);
			_registry.GetBundLettelseBundfradrag(AAR, 17, true).ShouldBe(44800);
			_registry.GetBundLettelseBundfradrag(AAR, 17, false).ShouldBe(33600);
			_registry.GetMellemLettelseBundfradrag(AAR).ShouldBe(362800);
			_registry.GetTopLettelseBundfradrag(AAR).ShouldBe(362800);
			_registry.GetMellemskatBundfradrag(AAR).ShouldBe(decimal.MaxValue);
			_registry.GetTopskatBundfradrag(AAR).ShouldBe(409100);
			_registry.GetPositivNettoKapitalIndkomstGrundbeloeb(AAR).ShouldBe(40000);
			_registry.GetNegativNettoKapitalIndkomstGrundbeloeb(AAR).ShouldBe(50000);
			_registry.GetNegativNettoKapitalIndkomstSats(AAR).ShouldBe(0.07m);
			_registry.GetBeskaeftigelsesfradragGrundbeloeb(AAR).ShouldBe(17300);
			_registry.GetBeskaeftigelsesfradragSats(AAR).ShouldBe(0.054m);
			_registry.GetGroenCheckPrVoksen(AAR).ShouldBe(1300);
			_registry.GetGroenCheckPrBarn(AAR).ShouldBe(300);
			_registry.GetGroenCheckBundfradrag(AAR).ShouldBe(362800);
			_registry.GetGroenCheckAftrapningssats(AAR).ShouldBe(0.075m);
		}

		[Fact]
		public void Vaerdier2019()
		{
			const int AAR = 2019;

			_registry.GetAktieIndkomstLavesteProgressionsgraense(AAR).ShouldBe(48300);
			_registry.GetAktieIndkomstHoejesteProgressionsgraense(AAR).ShouldBe(decimal.MaxValue);
			_registry.GetAktieIndkomstLavesteSkattesats(AAR).ShouldBe(0.27m);
			_registry.GetAktieIndkomstMellemsteSkattesats(AAR).ShouldBe(0.42m);
			_registry.GetAktieIndkomstHoejesteSkattesats(AAR).ShouldBe(0);
			_registry.GetAMBidragSkattesats(AAR).ShouldBe(0.08m);
			_registry.GetSundhedsbidragSkattesats(AAR).ShouldBe(0);
			_registry.GetBundSkattesats(AAR).ShouldBe(0.1167m);
			_registry.GetMellemSkattesats(AAR).ShouldBe(0);
			_registry.GetTopSkattesats(AAR).ShouldBe(0.15m);
			_registry.GetSkatteloftSkattesats(AAR).ShouldBe(0.515m);
			_registry.GetPersonfradrag(AAR, 20, false).ShouldBe(42900);
			_registry.GetPersonfradrag(AAR, 17, true).ShouldBe(42900);
			_registry.GetPersonfradrag(AAR, 17, false).ShouldBe(32200);
			_registry.GetBundLettelseBundfradrag(AAR, 20, false).ShouldBe(44800);
			_registry.GetBundLettelseBundfradrag(AAR, 17, true).ShouldBe(44800);
			_registry.GetBundLettelseBundfradrag(AAR, 17, false).ShouldBe(33600);
			_registry.GetMellemLettelseBundfradrag(AAR).ShouldBe(362800);
			_registry.GetTopLettelseBundfradrag(AAR).ShouldBe(362800);
			_registry.GetMellemskatBundfradrag(AAR).ShouldBe(decimal.MaxValue);
			_registry.GetTopskatBundfradrag(AAR).ShouldBe(409100);
			_registry.GetPositivNettoKapitalIndkomstGrundbeloeb(AAR).ShouldBe(40000);
			_registry.GetNegativNettoKapitalIndkomstGrundbeloeb(AAR).ShouldBe(50000);
			_registry.GetNegativNettoKapitalIndkomstSats(AAR).ShouldBe(0.08m);
			_registry.GetBeskaeftigelsesfradragGrundbeloeb(AAR).ShouldBe(17900);
			_registry.GetBeskaeftigelsesfradragSats(AAR).ShouldBe(0.056m);
			_registry.GetGroenCheckPrVoksen(AAR).ShouldBe(1300);
			_registry.GetGroenCheckPrBarn(AAR).ShouldBe(300);
			_registry.GetGroenCheckBundfradrag(AAR).ShouldBe(362800);
			_registry.GetGroenCheckAftrapningssats(AAR).ShouldBe(0.075m);
		}

		[Fact]
		public void Vaerdier2029Throws()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => _registry.GetBundSkattesats(2020));
		}
	}
}
