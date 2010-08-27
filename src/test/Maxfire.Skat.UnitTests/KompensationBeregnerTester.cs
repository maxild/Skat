using System;
using Maxfire.Skat.Extensions;
using Maxfire.TestCommons.AssertExtensions;
using Xunit;

namespace Maxfire.Skat.UnitTests
{
	public class KompensationBeregnerTester
	{
		const int SKATTE_AAR = 2016;
		
		class FakeSkattelovRegistry : AbstractFakeSkattelovRegistry
		{
			public override decimal GetAMBidragSkattesats(int skatteAar)
			{
				return 0.08m;
			}

			public override decimal GetBeskaeftigelsesfradragGrundbeloeb(int skatteAar)
			{
				return 16000;
			}

			public override decimal GetBeskaeftigelsesfradragSats(int skatteAar)
			{
				return 0.05m;
			}

			public override decimal GetBundLettelseBundfradrag(int skatteAar, int alder, bool gift)
			{
				return 44800;
			}

			public override decimal GetMellemLettelseBundfradrag(int skatteAar)
			{
				return 362800;
			}

			public override decimal GetTopskatBundfradrag(int skatteAar)
			{
				return 409100;
			}

			public override decimal GetTopLettelseBundfradrag(int skatteAar)
			{
				return 362800;
			}

			public override decimal GetPersonfradragSkaerpelse(int skatteAar, int alder, bool gift)
			{
				return 1900;
			}

			public override decimal GetBundSkattesats(int skatteAar)
			{
				return 0.0876m;
			}

			public override decimal GetSundhedsbidragSkattesats(int skatteAar)
			{
				return 0.03m;
			}

			public override decimal GetNegativNettoKapitalIndkomstGrundbeloeb(int skatteAar)
			{
				return 39975; // Deflateret værdi af de sædvanlige 50.000
			}
		}

		private readonly KompensationBeregner _kompensationBeregner;
		private readonly IValueTuple<FakePersonligeBeloeb> _indkomster;
		private readonly IValueTuple<IPerson> _personer;
		private readonly IValueTuple<IKommunaleSatser> _kommunaleSatser;

		public KompensationBeregnerTester()
		{
			const decimal loen = 400000;
			var skattelovRegistry = new FakeSkattelovRegistry();

			_personer = new ValueTuple<IPerson>(new Person(new DateTime(1970, 6, 3)));

			_indkomster = new ValueTuple<FakePersonligeBeloeb>(
				new FakePersonligeBeloeb
				{
					PersonligIndkomstAMIndkomst = loen,
					PersonligIndkomst = (1 - 0.08m) * loen, // TODO: Indkomstopgørelse mangler i programmet
					NettoKapitalIndkomst = -110000,
					LigningsmaessigeFradrag = 60000, // NOTE: Ligningsmæssige fradrag eksl. beskæftigelsesfradrag
					AktieIndkomst = 8000
				});

			_kommunaleSatser = new ValueTuple<IKommunaleSatser>(
				new KommunaleSatser
				{
					Kommuneskattesats = 0.255m
				});
			
			var amBidragBeregner = new AMBidragBeregner(skattelovRegistry);
			var amIndkomster = _indkomster.Map(x => x.PersonligIndkomstAMIndkomst);
			var amBidrag = amBidragBeregner.BeregnSkat(amIndkomster, SKATTE_AAR);

			_indkomster[0].PersonligIndkomst.ShouldEqual(368000);
			amBidrag[0].ShouldEqual(32000);

			var beskaeftigelsesfradragBeregner = new BeskaeftigelsesfradragBeregner(skattelovRegistry);
			var beskaeftigelsesfradrag = beskaeftigelsesfradragBeregner.BeregnFradrag(amIndkomster, SKATTE_AAR);

			beskaeftigelsesfradrag[0].ShouldEqual(16000);
			_indkomster[0].LigningsmaessigeFradrag += beskaeftigelsesfradrag[0]; // TODO: Indkomstopgørelse mangler i programmet
			_indkomster[0].LigningsmaessigeFradrag.ShouldEqual(76000);

			_kompensationBeregner = new KompensationBeregner(skattelovRegistry);
		}

		[Fact]
		public void BundSkattelettelse()
		{
			var bundSkattelettelse = _kompensationBeregner.GetBundSkattelettelse(_personer, _indkomster, SKATTE_AAR);
			bundSkattelettelse[0].ShouldEqual(4848);
		}

		[Fact]
		public void MellemSkattelettelse()
		{
			var mellemSkattelettelse = _kompensationBeregner.GetMellemSkattelettelse(_indkomster, SKATTE_AAR);
			mellemSkattelettelse[0].ShouldEqual(312);
		}

		[Fact]
		public void TopSkattelettelse()
		{
			var topSkattelettelse = _kompensationBeregner.GetTopSkattelettelse(_indkomster, SKATTE_AAR);
			topSkattelettelse[0].ShouldEqual(780);
		}

		[Fact]
		public void AktieSkattelettelse()
		{
			var aktieSkattelettelse = _kompensationBeregner.GetAktieSkattelettelse(_indkomster);
			aktieSkattelettelse[0].ShouldEqual(80);
		}

		[Fact]
		public void BeskaeftigelsesfradragSkattelettelse()
		{
			var beskaeftigelsesfradragSkattelettelse = _kompensationBeregner.GetBeskaeftigelsesfradragSkattelettelse(
				_indkomster, _kommunaleSatser, SKATTE_AAR);
			beskaeftigelsesfradragSkattelettelse[0].ShouldEqual(603);
		}

		[Fact]
		public void PersonfradragSkatteskaerpelse()
		{
			var personfradragSkatteskaerpelse = _kompensationBeregner.GetPersonfradragSkatteskaerpelse(_personer,
			                                                                                           _kommunaleSatser,
			                                                                                           SKATTE_AAR);
			personfradragSkatteskaerpelse[0].ShouldEqual(707.94m);
		}

		[Fact]
		public void SamletSkatteskaerpelsePaaFradragene()
		{
			var samletSkatteskaerpelsePaaFradragene = _kompensationBeregner.GetSamletSkatteskaerpelsePaaFradragene(_indkomster, SKATTE_AAR);
			samletSkatteskaerpelsePaaFradragene[0].ShouldEqual(7301.25m);
		}

		[Fact]
		public void Kompensation()
		{
			var kompenstion =_kompensationBeregner.BeregnKompensation(_personer, _indkomster, _kommunaleSatser, SKATTE_AAR);
			kompenstion[0].ShouldEqual(1386.19m);
		}
	}
}