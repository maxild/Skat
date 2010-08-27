using System;
using Maxfire.TestCommons.AssertExtensions;
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
			const int aar = 2009;
			
			_registry.GetAktieIndkomstLavesteProgressionsgraense(aar).ShouldEqual(48300);
			_registry.GetAktieIndkomstHoejesteProgressionsgraense(aar).ShouldEqual(106100);
			_registry.GetAktieIndkomstLavesteSkattesats(aar).ShouldEqual(0.28m);
			_registry.GetAktieIndkomstMellemsteSkattesats(aar).ShouldEqual(0.43m);
			_registry.GetAktieIndkomstHoejesteSkattesats(aar).ShouldEqual(0.45m);
			_registry.GetAMBidragSkattesats(aar).ShouldEqual(0.08m);
			_registry.GetSundhedsbidragSkattesats(aar).ShouldEqual(0.08m);
			_registry.GetBundSkattesats(aar).ShouldEqual(0.0504m);
			_registry.GetMellemSkattesats(aar).ShouldEqual(0.06m);
			_registry.GetTopSkattesats(aar).ShouldEqual(0.15m);
			_registry.GetSkatteloftSkattesats(aar).ShouldEqual(0.59m);
			_registry.GetPersonfradrag(aar, 20, false).ShouldEqual(42900);
			_registry.GetPersonfradrag(aar, 17, true).ShouldEqual(42900);
			_registry.GetPersonfradrag(aar, 17, false).ShouldEqual(32200);
			_registry.GetBundLettelseBundfradrag(aar, 20, false).ShouldEqual(decimal.MaxValue);
			_registry.GetBundLettelseBundfradrag(aar, 17, true).ShouldEqual(decimal.MaxValue);
			_registry.GetBundLettelseBundfradrag(aar, 17, false).ShouldEqual(decimal.MaxValue);
			_registry.GetMellemLettelseBundfradrag(aar).ShouldEqual(decimal.MaxValue);
			_registry.GetTopLettelseBundfradrag(aar).ShouldEqual(decimal.MaxValue);
			_registry.GetMellemskatBundfradrag(aar).ShouldEqual(347200);
			_registry.GetTopskatBundfradrag(aar).ShouldEqual(347200);
			_registry.GetPositivNettoKapitalIndkomstGrundbeloeb(aar).ShouldEqual(0);
			_registry.GetNegativNettoKapitalIndkomstGrundbeloeb(aar).ShouldEqual(0);
			_registry.GetNegativNettoKapitalIndkomstSats(aar).ShouldEqual(0);
			_registry.GetBeskaeftigelsesfradragGrundbeloeb(aar).ShouldEqual(13600);
			_registry.GetBeskaeftigelsesfradragSats(aar).ShouldEqual(0.0425m);
			_registry.GetGroenCheckPrVoksen(aar).ShouldEqual(0);
			_registry.GetGroenCheckPrBarn(aar).ShouldEqual(0);
			_registry.GetGroenCheckBundfradrag(aar).ShouldEqual(decimal.MaxValue);
			_registry.GetGroenCheckAftrapningssats(aar).ShouldEqual(0);
		}

		[Fact]
		public void Vaerdier2010()
		{
			const int aar = 2010;

			_registry.GetAktieIndkomstLavesteProgressionsgraense(aar).ShouldEqual(48300);
			_registry.GetAktieIndkomstHoejesteProgressionsgraense(aar).ShouldEqual(decimal.MaxValue);
			_registry.GetAktieIndkomstLavesteSkattesats(aar).ShouldEqual(0.28m);
			_registry.GetAktieIndkomstMellemsteSkattesats(aar).ShouldEqual(0.42m);
			_registry.GetAktieIndkomstHoejesteSkattesats(aar).ShouldEqual(0);
			_registry.GetAMBidragSkattesats(aar).ShouldEqual(0.08m);
			_registry.GetSundhedsbidragSkattesats(aar).ShouldEqual(0.08m);
			_registry.GetBundSkattesats(aar).ShouldEqual(0.0367m);
			_registry.GetMellemSkattesats(aar).ShouldEqual(0);
			_registry.GetTopSkattesats(aar).ShouldEqual(0.15m);
			_registry.GetSkatteloftSkattesats(aar).ShouldEqual(0.515m);
			_registry.GetPersonfradrag(aar, 20, false).ShouldEqual(42900);
			_registry.GetPersonfradrag(aar, 17, true).ShouldEqual(42900);
			_registry.GetPersonfradrag(aar, 17, false).ShouldEqual(32200);
			_registry.GetBundLettelseBundfradrag(aar, 20, false).ShouldEqual(decimal.MaxValue);
			_registry.GetBundLettelseBundfradrag(aar, 17, true).ShouldEqual(decimal.MaxValue);
			_registry.GetBundLettelseBundfradrag(aar, 17, false).ShouldEqual(decimal.MaxValue);
			_registry.GetMellemLettelseBundfradrag(aar).ShouldEqual(decimal.MaxValue);
			_registry.GetTopLettelseBundfradrag(aar).ShouldEqual(decimal.MaxValue);
			_registry.GetMellemskatBundfradrag(aar).ShouldEqual(decimal.MaxValue);
			_registry.GetTopskatBundfradrag(aar).ShouldEqual(389900);
			_registry.GetPositivNettoKapitalIndkomstGrundbeloeb(aar).ShouldEqual(40000);
			_registry.GetNegativNettoKapitalIndkomstGrundbeloeb(aar).ShouldEqual(0);
			_registry.GetNegativNettoKapitalIndkomstSats(aar).ShouldEqual(0);
			_registry.GetBeskaeftigelsesfradragGrundbeloeb(aar).ShouldEqual(13600);
			_registry.GetBeskaeftigelsesfradragSats(aar).ShouldEqual(0.0425m);
			_registry.GetGroenCheckPrVoksen(aar).ShouldEqual(1300);
			_registry.GetGroenCheckPrBarn(aar).ShouldEqual(300);
			_registry.GetGroenCheckBundfradrag(aar).ShouldEqual(362800);
			_registry.GetGroenCheckAftrapningssats(aar).ShouldEqual(0.075m);
		}

		[Fact]
		public void Vaerdier2011()
		{
			const int aar = 2011;

			_registry.GetAktieIndkomstLavesteProgressionsgraense(aar).ShouldEqual(48300);
			_registry.GetAktieIndkomstHoejesteProgressionsgraense(aar).ShouldEqual(decimal.MaxValue);
			_registry.GetAktieIndkomstLavesteSkattesats(aar).ShouldEqual(0.28m);
			_registry.GetAktieIndkomstMellemsteSkattesats(aar).ShouldEqual(0.42m);
			_registry.GetAktieIndkomstHoejesteSkattesats(aar).ShouldEqual(0);
			_registry.GetAMBidragSkattesats(aar).ShouldEqual(0.08m);
			_registry.GetSundhedsbidragSkattesats(aar).ShouldEqual(0.08m);
			_registry.GetBundSkattesats(aar).ShouldEqual(0.0367m);
			_registry.GetMellemSkattesats(aar).ShouldEqual(0);
			_registry.GetTopSkattesats(aar).ShouldEqual(0.15m);
			_registry.GetSkatteloftSkattesats(aar).ShouldEqual(0.515m);
			_registry.GetPersonfradrag(aar, 20, false).ShouldEqual(42900);
			_registry.GetPersonfradrag(aar, 17, true).ShouldEqual(42900);
			_registry.GetPersonfradrag(aar, 17, false).ShouldEqual(32200);
			_registry.GetBundLettelseBundfradrag(aar, 20, false).ShouldEqual(decimal.MaxValue);
			_registry.GetBundLettelseBundfradrag(aar, 17, true).ShouldEqual(decimal.MaxValue);
			_registry.GetBundLettelseBundfradrag(aar, 17, false).ShouldEqual(decimal.MaxValue);
			_registry.GetMellemLettelseBundfradrag(aar).ShouldEqual(decimal.MaxValue);
			_registry.GetTopLettelseBundfradrag(aar).ShouldEqual(decimal.MaxValue);
			_registry.GetMellemskatBundfradrag(aar).ShouldEqual(decimal.MaxValue);
			_registry.GetTopskatBundfradrag(aar).ShouldEqual(409100);
			_registry.GetPositivNettoKapitalIndkomstGrundbeloeb(aar).ShouldEqual(40000);
			_registry.GetNegativNettoKapitalIndkomstGrundbeloeb(aar).ShouldEqual(0);
			_registry.GetNegativNettoKapitalIndkomstSats(aar).ShouldEqual(0);
			_registry.GetBeskaeftigelsesfradragGrundbeloeb(aar).ShouldEqual(13600);
			_registry.GetBeskaeftigelsesfradragSats(aar).ShouldEqual(0.0425m);
			_registry.GetGroenCheckPrVoksen(aar).ShouldEqual(1300);
			_registry.GetGroenCheckPrBarn(aar).ShouldEqual(300);
			_registry.GetGroenCheckBundfradrag(aar).ShouldEqual(362800);
			_registry.GetGroenCheckAftrapningssats(aar).ShouldEqual(0.075m);
		}

		[Fact]
		public void Vaerdier2012()
		{
			const int aar = 2012;

			_registry.GetAktieIndkomstLavesteProgressionsgraense(aar).ShouldEqual(48300);
			_registry.GetAktieIndkomstHoejesteProgressionsgraense(aar).ShouldEqual(decimal.MaxValue);
			_registry.GetAktieIndkomstLavesteSkattesats(aar).ShouldEqual(0.27m);
			_registry.GetAktieIndkomstMellemsteSkattesats(aar).ShouldEqual(0.42m);
			_registry.GetAktieIndkomstHoejesteSkattesats(aar).ShouldEqual(0);
			_registry.GetAMBidragSkattesats(aar).ShouldEqual(0.08m);
			_registry.GetSundhedsbidragSkattesats(aar).ShouldEqual(0.07m);
			_registry.GetBundSkattesats(aar).ShouldEqual(0.0467m);
			_registry.GetMellemSkattesats(aar).ShouldEqual(0);
			_registry.GetTopSkattesats(aar).ShouldEqual(0.15m);
			_registry.GetSkatteloftSkattesats(aar).ShouldEqual(0.515m);
			_registry.GetPersonfradrag(aar, 20, false).ShouldEqual(42900);
			_registry.GetPersonfradrag(aar, 17, true).ShouldEqual(42900);
			_registry.GetPersonfradrag(aar, 17, false).ShouldEqual(32200);
			_registry.GetBundLettelseBundfradrag(aar, 20, false).ShouldEqual(44800);
			_registry.GetBundLettelseBundfradrag(aar, 17, true).ShouldEqual(44800);
			_registry.GetBundLettelseBundfradrag(aar, 17, false).ShouldEqual(33600);
			_registry.GetMellemLettelseBundfradrag(aar).ShouldEqual(362800);
			_registry.GetTopLettelseBundfradrag(aar).ShouldEqual(362800);
			_registry.GetMellemskatBundfradrag(aar).ShouldEqual(decimal.MaxValue);
			_registry.GetTopskatBundfradrag(aar).ShouldEqual(409100);
			_registry.GetPositivNettoKapitalIndkomstGrundbeloeb(aar).ShouldEqual(40000);
			_registry.GetNegativNettoKapitalIndkomstGrundbeloeb(aar).ShouldEqual(50000);
			_registry.GetNegativNettoKapitalIndkomstSats(aar).ShouldEqual(0.01m);
			_registry.GetBeskaeftigelsesfradragGrundbeloeb(aar).ShouldEqual(14100);
			_registry.GetBeskaeftigelsesfradragSats(aar).ShouldEqual(0.044m);
			_registry.GetGroenCheckPrVoksen(aar).ShouldEqual(1300);
			_registry.GetGroenCheckPrBarn(aar).ShouldEqual(300);
			_registry.GetGroenCheckBundfradrag(aar).ShouldEqual(362800);
			_registry.GetGroenCheckAftrapningssats(aar).ShouldEqual(0.075m);
		}

		[Fact]
		public void Vaerdier2013()
		{
			const int aar = 2013;

			_registry.GetAktieIndkomstLavesteProgressionsgraense(aar).ShouldEqual(48300);
			_registry.GetAktieIndkomstHoejesteProgressionsgraense(aar).ShouldEqual(decimal.MaxValue);
			_registry.GetAktieIndkomstLavesteSkattesats(aar).ShouldEqual(0.27m);
			_registry.GetAktieIndkomstMellemsteSkattesats(aar).ShouldEqual(0.42m);
			_registry.GetAktieIndkomstHoejesteSkattesats(aar).ShouldEqual(0);
			_registry.GetAMBidragSkattesats(aar).ShouldEqual(0.08m);
			_registry.GetSundhedsbidragSkattesats(aar).ShouldEqual(0.06m);
			_registry.GetBundSkattesats(aar).ShouldEqual(0.0567m);
			_registry.GetMellemSkattesats(aar).ShouldEqual(0);
			_registry.GetTopSkattesats(aar).ShouldEqual(0.15m);
			_registry.GetSkatteloftSkattesats(aar).ShouldEqual(0.515m);
			_registry.GetPersonfradrag(aar, 20, false).ShouldEqual(42900);
			_registry.GetPersonfradrag(aar, 17, true).ShouldEqual(42900);
			_registry.GetPersonfradrag(aar, 17, false).ShouldEqual(32200);
			_registry.GetBundLettelseBundfradrag(aar, 20, false).ShouldEqual(44800);
			_registry.GetBundLettelseBundfradrag(aar, 17, true).ShouldEqual(44800);
			_registry.GetBundLettelseBundfradrag(aar, 17, false).ShouldEqual(33600);
			_registry.GetMellemLettelseBundfradrag(aar).ShouldEqual(362800);
			_registry.GetTopLettelseBundfradrag(aar).ShouldEqual(362800);
			_registry.GetMellemskatBundfradrag(aar).ShouldEqual(decimal.MaxValue);
			_registry.GetTopskatBundfradrag(aar).ShouldEqual(409100);
			_registry.GetPositivNettoKapitalIndkomstGrundbeloeb(aar).ShouldEqual(40000);
			_registry.GetNegativNettoKapitalIndkomstGrundbeloeb(aar).ShouldEqual(50000);
			_registry.GetNegativNettoKapitalIndkomstSats(aar).ShouldEqual(0.02m);
			_registry.GetBeskaeftigelsesfradragGrundbeloeb(aar).ShouldEqual(14400);
			_registry.GetBeskaeftigelsesfradragSats(aar).ShouldEqual(0.045m);
			_registry.GetGroenCheckPrVoksen(aar).ShouldEqual(1300);
			_registry.GetGroenCheckPrBarn(aar).ShouldEqual(300);
			_registry.GetGroenCheckBundfradrag(aar).ShouldEqual(362800);
			_registry.GetGroenCheckAftrapningssats(aar).ShouldEqual(0.075m);
		}

		[Fact]
		public void Vaerdier2014()
		{
			const int aar = 2014;

			_registry.GetAktieIndkomstLavesteProgressionsgraense(aar).ShouldEqual(48300);
			_registry.GetAktieIndkomstHoejesteProgressionsgraense(aar).ShouldEqual(decimal.MaxValue);
			_registry.GetAktieIndkomstLavesteSkattesats(aar).ShouldEqual(0.27m);
			_registry.GetAktieIndkomstMellemsteSkattesats(aar).ShouldEqual(0.42m);
			_registry.GetAktieIndkomstHoejesteSkattesats(aar).ShouldEqual(0);
			_registry.GetAMBidragSkattesats(aar).ShouldEqual(0.08m);
			_registry.GetSundhedsbidragSkattesats(aar).ShouldEqual(0.05m);
			_registry.GetBundSkattesats(aar).ShouldEqual(0.0667m);
			_registry.GetMellemSkattesats(aar).ShouldEqual(0);
			_registry.GetTopSkattesats(aar).ShouldEqual(0.15m);
			_registry.GetSkatteloftSkattesats(aar).ShouldEqual(0.515m);
			_registry.GetPersonfradrag(aar, 20, false).ShouldEqual(42900);
			_registry.GetPersonfradrag(aar, 17, true).ShouldEqual(42900);
			_registry.GetPersonfradrag(aar, 17, false).ShouldEqual(32200);
			_registry.GetBundLettelseBundfradrag(aar, 20, false).ShouldEqual(44800);
			_registry.GetBundLettelseBundfradrag(aar, 17, true).ShouldEqual(44800);
			_registry.GetBundLettelseBundfradrag(aar, 17, false).ShouldEqual(33600);
			_registry.GetMellemLettelseBundfradrag(aar).ShouldEqual(362800);
			_registry.GetTopLettelseBundfradrag(aar).ShouldEqual(362800);
			_registry.GetMellemskatBundfradrag(aar).ShouldEqual(decimal.MaxValue);
			_registry.GetTopskatBundfradrag(aar).ShouldEqual(409100);
			_registry.GetPositivNettoKapitalIndkomstGrundbeloeb(aar).ShouldEqual(40000);
			_registry.GetNegativNettoKapitalIndkomstGrundbeloeb(aar).ShouldEqual(50000);
			_registry.GetNegativNettoKapitalIndkomstSats(aar).ShouldEqual(0.03m);
			_registry.GetBeskaeftigelsesfradragGrundbeloeb(aar).ShouldEqual(14900);
			_registry.GetBeskaeftigelsesfradragSats(aar).ShouldEqual(0.0465m);
			_registry.GetGroenCheckPrVoksen(aar).ShouldEqual(1300);
			_registry.GetGroenCheckPrBarn(aar).ShouldEqual(300);
			_registry.GetGroenCheckBundfradrag(aar).ShouldEqual(362800);
			_registry.GetGroenCheckAftrapningssats(aar).ShouldEqual(0.075m);
		}

		[Fact]
		public void Vaerdier2015()
		{
			const int aar = 2015;

			_registry.GetAktieIndkomstLavesteProgressionsgraense(aar).ShouldEqual(48300);
			_registry.GetAktieIndkomstHoejesteProgressionsgraense(aar).ShouldEqual(decimal.MaxValue);
			_registry.GetAktieIndkomstLavesteSkattesats(aar).ShouldEqual(0.27m);
			_registry.GetAktieIndkomstMellemsteSkattesats(aar).ShouldEqual(0.42m);
			_registry.GetAktieIndkomstHoejesteSkattesats(aar).ShouldEqual(0);
			_registry.GetAMBidragSkattesats(aar).ShouldEqual(0.08m);
			_registry.GetSundhedsbidragSkattesats(aar).ShouldEqual(0.04m);
			_registry.GetBundSkattesats(aar).ShouldEqual(0.0767m);
			_registry.GetMellemSkattesats(aar).ShouldEqual(0);
			_registry.GetTopSkattesats(aar).ShouldEqual(0.15m);
			_registry.GetSkatteloftSkattesats(aar).ShouldEqual(0.515m);
			_registry.GetPersonfradrag(aar, 20, false).ShouldEqual(42900);
			_registry.GetPersonfradrag(aar, 17, true).ShouldEqual(42900);
			_registry.GetPersonfradrag(aar, 17, false).ShouldEqual(32200);
			_registry.GetBundLettelseBundfradrag(aar, 20, false).ShouldEqual(44800);
			_registry.GetBundLettelseBundfradrag(aar, 17, true).ShouldEqual(44800);
			_registry.GetBundLettelseBundfradrag(aar, 17, false).ShouldEqual(33600);
			_registry.GetMellemLettelseBundfradrag(aar).ShouldEqual(362800);
			_registry.GetTopLettelseBundfradrag(aar).ShouldEqual(362800);
			_registry.GetMellemskatBundfradrag(aar).ShouldEqual(decimal.MaxValue);
			_registry.GetTopskatBundfradrag(aar).ShouldEqual(409100);
			_registry.GetPositivNettoKapitalIndkomstGrundbeloeb(aar).ShouldEqual(40000);
			_registry.GetNegativNettoKapitalIndkomstGrundbeloeb(aar).ShouldEqual(50000);
			_registry.GetNegativNettoKapitalIndkomstSats(aar).ShouldEqual(0.04m);
			_registry.GetBeskaeftigelsesfradragGrundbeloeb(aar).ShouldEqual(15400);
			_registry.GetBeskaeftigelsesfradragSats(aar).ShouldEqual(0.048m);
			_registry.GetGroenCheckPrVoksen(aar).ShouldEqual(1300);
			_registry.GetGroenCheckPrBarn(aar).ShouldEqual(300);
			_registry.GetGroenCheckBundfradrag(aar).ShouldEqual(362800);
			_registry.GetGroenCheckAftrapningssats(aar).ShouldEqual(0.075m);
		}

		[Fact]
		public void Vaerdier2016()
		{
			const int aar = 2016;

			_registry.GetAktieIndkomstLavesteProgressionsgraense(aar).ShouldEqual(48300);
			_registry.GetAktieIndkomstHoejesteProgressionsgraense(aar).ShouldEqual(decimal.MaxValue);
			_registry.GetAktieIndkomstLavesteSkattesats(aar).ShouldEqual(0.27m);
			_registry.GetAktieIndkomstMellemsteSkattesats(aar).ShouldEqual(0.42m);
			_registry.GetAktieIndkomstHoejesteSkattesats(aar).ShouldEqual(0);
			_registry.GetAMBidragSkattesats(aar).ShouldEqual(0.08m);
			_registry.GetSundhedsbidragSkattesats(aar).ShouldEqual(0.03m);
			_registry.GetBundSkattesats(aar).ShouldEqual(0.0867m);
			_registry.GetMellemSkattesats(aar).ShouldEqual(0);
			_registry.GetTopSkattesats(aar).ShouldEqual(0.15m);
			_registry.GetSkatteloftSkattesats(aar).ShouldEqual(0.515m);
			_registry.GetPersonfradrag(aar, 20, false).ShouldEqual(42900);
			_registry.GetPersonfradrag(aar, 17, true).ShouldEqual(42900);
			_registry.GetPersonfradrag(aar, 17, false).ShouldEqual(32200);
			_registry.GetBundLettelseBundfradrag(aar, 20, false).ShouldEqual(44800);
			_registry.GetBundLettelseBundfradrag(aar, 17, true).ShouldEqual(44800);
			_registry.GetBundLettelseBundfradrag(aar, 17, false).ShouldEqual(33600);
			_registry.GetMellemLettelseBundfradrag(aar).ShouldEqual(362800);
			_registry.GetTopLettelseBundfradrag(aar).ShouldEqual(362800);
			_registry.GetMellemskatBundfradrag(aar).ShouldEqual(decimal.MaxValue);
			_registry.GetTopskatBundfradrag(aar).ShouldEqual(409100);
			_registry.GetPositivNettoKapitalIndkomstGrundbeloeb(aar).ShouldEqual(40000);
			_registry.GetNegativNettoKapitalIndkomstGrundbeloeb(aar).ShouldEqual(50000);
			_registry.GetNegativNettoKapitalIndkomstSats(aar).ShouldEqual(0.05m);
			_registry.GetBeskaeftigelsesfradragGrundbeloeb(aar).ShouldEqual(16000);
			_registry.GetBeskaeftigelsesfradragSats(aar).ShouldEqual(0.05m);
			_registry.GetGroenCheckPrVoksen(aar).ShouldEqual(1300);
			_registry.GetGroenCheckPrBarn(aar).ShouldEqual(300);
			_registry.GetGroenCheckBundfradrag(aar).ShouldEqual(362800);
			_registry.GetGroenCheckAftrapningssats(aar).ShouldEqual(0.075m);
		}

		[Fact]
		public void Vaerdier2017()
		{
			const int aar = 2017;

			_registry.GetAktieIndkomstLavesteProgressionsgraense(aar).ShouldEqual(48300);
			_registry.GetAktieIndkomstHoejesteProgressionsgraense(aar).ShouldEqual(decimal.MaxValue);
			_registry.GetAktieIndkomstLavesteSkattesats(aar).ShouldEqual(0.27m);
			_registry.GetAktieIndkomstMellemsteSkattesats(aar).ShouldEqual(0.42m);
			_registry.GetAktieIndkomstHoejesteSkattesats(aar).ShouldEqual(0);
			_registry.GetAMBidragSkattesats(aar).ShouldEqual(0.08m);
			_registry.GetSundhedsbidragSkattesats(aar).ShouldEqual(0.02m);
			_registry.GetBundSkattesats(aar).ShouldEqual(0.0967m);
			_registry.GetMellemSkattesats(aar).ShouldEqual(0);
			_registry.GetTopSkattesats(aar).ShouldEqual(0.15m);
			_registry.GetSkatteloftSkattesats(aar).ShouldEqual(0.515m);
			_registry.GetPersonfradrag(aar, 20, false).ShouldEqual(42900);
			_registry.GetPersonfradrag(aar, 17, true).ShouldEqual(42900);
			_registry.GetPersonfradrag(aar, 17, false).ShouldEqual(32200);
			_registry.GetBundLettelseBundfradrag(aar, 20, false).ShouldEqual(44800);
			_registry.GetBundLettelseBundfradrag(aar, 17, true).ShouldEqual(44800);
			_registry.GetBundLettelseBundfradrag(aar, 17, false).ShouldEqual(33600);
			_registry.GetMellemLettelseBundfradrag(aar).ShouldEqual(362800);
			_registry.GetTopLettelseBundfradrag(aar).ShouldEqual(362800);
			_registry.GetMellemskatBundfradrag(aar).ShouldEqual(decimal.MaxValue);
			_registry.GetTopskatBundfradrag(aar).ShouldEqual(409100);
			_registry.GetPositivNettoKapitalIndkomstGrundbeloeb(aar).ShouldEqual(40000);
			_registry.GetNegativNettoKapitalIndkomstGrundbeloeb(aar).ShouldEqual(50000);
			_registry.GetNegativNettoKapitalIndkomstSats(aar).ShouldEqual(0.06m);
			_registry.GetBeskaeftigelsesfradragGrundbeloeb(aar).ShouldEqual(16600);
			_registry.GetBeskaeftigelsesfradragSats(aar).ShouldEqual(0.052m);
			_registry.GetGroenCheckPrVoksen(aar).ShouldEqual(1300);
			_registry.GetGroenCheckPrBarn(aar).ShouldEqual(300);
			_registry.GetGroenCheckBundfradrag(aar).ShouldEqual(362800);
			_registry.GetGroenCheckAftrapningssats(aar).ShouldEqual(0.075m);
		}

		[Fact]
		public void Vaerdier2018()
		{
			const int aar = 2018;

			_registry.GetAktieIndkomstLavesteProgressionsgraense(aar).ShouldEqual(48300);
			_registry.GetAktieIndkomstHoejesteProgressionsgraense(aar).ShouldEqual(decimal.MaxValue);
			_registry.GetAktieIndkomstLavesteSkattesats(aar).ShouldEqual(0.27m);
			_registry.GetAktieIndkomstMellemsteSkattesats(aar).ShouldEqual(0.42m);
			_registry.GetAktieIndkomstHoejesteSkattesats(aar).ShouldEqual(0);
			_registry.GetAMBidragSkattesats(aar).ShouldEqual(0.08m);
			_registry.GetSundhedsbidragSkattesats(aar).ShouldEqual(0.01m);
			_registry.GetBundSkattesats(aar).ShouldEqual(0.1067m);
			_registry.GetMellemSkattesats(aar).ShouldEqual(0);
			_registry.GetTopSkattesats(aar).ShouldEqual(0.15m);
			_registry.GetSkatteloftSkattesats(aar).ShouldEqual(0.515m);
			_registry.GetPersonfradrag(aar, 20, false).ShouldEqual(42900);
			_registry.GetPersonfradrag(aar, 17, true).ShouldEqual(42900);
			_registry.GetPersonfradrag(aar, 17, false).ShouldEqual(32200);
			_registry.GetBundLettelseBundfradrag(aar, 20, false).ShouldEqual(44800);
			_registry.GetBundLettelseBundfradrag(aar, 17, true).ShouldEqual(44800);
			_registry.GetBundLettelseBundfradrag(aar, 17, false).ShouldEqual(33600);
			_registry.GetMellemLettelseBundfradrag(aar).ShouldEqual(362800);
			_registry.GetTopLettelseBundfradrag(aar).ShouldEqual(362800);
			_registry.GetMellemskatBundfradrag(aar).ShouldEqual(decimal.MaxValue);
			_registry.GetTopskatBundfradrag(aar).ShouldEqual(409100);
			_registry.GetPositivNettoKapitalIndkomstGrundbeloeb(aar).ShouldEqual(40000);
			_registry.GetNegativNettoKapitalIndkomstGrundbeloeb(aar).ShouldEqual(50000);
			_registry.GetNegativNettoKapitalIndkomstSats(aar).ShouldEqual(0.07m);
			_registry.GetBeskaeftigelsesfradragGrundbeloeb(aar).ShouldEqual(17300);
			_registry.GetBeskaeftigelsesfradragSats(aar).ShouldEqual(0.054m);
			_registry.GetGroenCheckPrVoksen(aar).ShouldEqual(1300);
			_registry.GetGroenCheckPrBarn(aar).ShouldEqual(300);
			_registry.GetGroenCheckBundfradrag(aar).ShouldEqual(362800);
			_registry.GetGroenCheckAftrapningssats(aar).ShouldEqual(0.075m);
		}

		[Fact]
		public void Vaerdier2019()
		{
			const int aar = 2019;

			_registry.GetAktieIndkomstLavesteProgressionsgraense(aar).ShouldEqual(48300);
			_registry.GetAktieIndkomstHoejesteProgressionsgraense(aar).ShouldEqual(decimal.MaxValue);
			_registry.GetAktieIndkomstLavesteSkattesats(aar).ShouldEqual(0.27m);
			_registry.GetAktieIndkomstMellemsteSkattesats(aar).ShouldEqual(0.42m);
			_registry.GetAktieIndkomstHoejesteSkattesats(aar).ShouldEqual(0);
			_registry.GetAMBidragSkattesats(aar).ShouldEqual(0.08m);
			_registry.GetSundhedsbidragSkattesats(aar).ShouldEqual(0);
			_registry.GetBundSkattesats(aar).ShouldEqual(0.1167m);
			_registry.GetMellemSkattesats(aar).ShouldEqual(0);
			_registry.GetTopSkattesats(aar).ShouldEqual(0.15m);
			_registry.GetSkatteloftSkattesats(aar).ShouldEqual(0.515m);
			_registry.GetPersonfradrag(aar, 20, false).ShouldEqual(42900);
			_registry.GetPersonfradrag(aar, 17, true).ShouldEqual(42900);
			_registry.GetPersonfradrag(aar, 17, false).ShouldEqual(32200);
			_registry.GetBundLettelseBundfradrag(aar, 20, false).ShouldEqual(44800);
			_registry.GetBundLettelseBundfradrag(aar, 17, true).ShouldEqual(44800);
			_registry.GetBundLettelseBundfradrag(aar, 17, false).ShouldEqual(33600);
			_registry.GetMellemLettelseBundfradrag(aar).ShouldEqual(362800);
			_registry.GetTopLettelseBundfradrag(aar).ShouldEqual(362800);
			_registry.GetMellemskatBundfradrag(aar).ShouldEqual(decimal.MaxValue);
			_registry.GetTopskatBundfradrag(aar).ShouldEqual(409100);
			_registry.GetPositivNettoKapitalIndkomstGrundbeloeb(aar).ShouldEqual(40000);
			_registry.GetNegativNettoKapitalIndkomstGrundbeloeb(aar).ShouldEqual(50000);
			_registry.GetNegativNettoKapitalIndkomstSats(aar).ShouldEqual(0.08m);
			_registry.GetBeskaeftigelsesfradragGrundbeloeb(aar).ShouldEqual(17900);
			_registry.GetBeskaeftigelsesfradragSats(aar).ShouldEqual(0.056m);
			_registry.GetGroenCheckPrVoksen(aar).ShouldEqual(1300);
			_registry.GetGroenCheckPrBarn(aar).ShouldEqual(300);
			_registry.GetGroenCheckBundfradrag(aar).ShouldEqual(362800);
			_registry.GetGroenCheckAftrapningssats(aar).ShouldEqual(0.075m);
		}

		[Fact]
		public void Vaerdier2029Throws()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => _registry.GetBundSkattesats(2020));
		}
	}
}