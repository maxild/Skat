using Maxfire.Skat.Beregnere;
using Maxfire.TestCommons.AssertExtensions;
using Xunit;

namespace Maxfire.Skat.UnitTests
{
	public class TopskatBeregnerTester
	{
		class FakeSkattelovRegistry : AbstractFakeSkattelovRegistry
		{
			public override decimal GetPositivNettoKapitalIndkomstGrundbeloeb(int skatteAar)
			{
				return 0;
			}

			public override decimal GetTopskatBundfradrag(int skatteAar)
			{
				return 347200;
			}

			public override decimal GetSundhedsbidragSkattesats(int skatteAar)
			{
				return 0.08m;
			}

			public override decimal GetBundSkattesats(int skatteAar)
			{
				return 0.0504m;
			}

			public override decimal GetMellemSkattesats(int skatteAar)
			{
				return 0.06m;
			}

			public override decimal GetTopSkattesats(int skatteAar)
			{
				return 0.15m;
			}

			public override decimal GetSkatteloftSkattesats(int skatteAar)
			{
				return 0.59m;
			}
		}

		private readonly TopskatBeregner _topskatBeregner;

		public TopskatBeregnerTester()
		{
			_topskatBeregner = new TopskatBeregner(new FakeSkattelovRegistry());
		}

		[Fact]
		public void UdenSkatteloft_Ugift()
		{
			var indkomster = new ValueTuple<IPersonligeIndkomster>(
				new FakePersonligeIndkomster
				{
					PersonligIndkomst = 336000,
					NettoKapitalIndkomst = 28500,
					KapitalPensionsindskud = 32000
				}
			);

			var topskat = _topskatBeregner.BeregnSkat(indkomster, 2009);

			// 15 pct af (336000 + 32000 + 28500 - 347200)
			topskat[0].Topskat.ShouldEqual(7395);
			topskat[0].SkatteloftNedslag.ShouldEqual(0);
			topskat[0].TopskatEfterNedslag.ShouldEqual(7395);
		}

		[Fact]
		public void MedSkatteloft_Ugift()
		{
			var indkomster = new ValueTuple<IPersonligeIndkomster>(
				new FakePersonligeIndkomster
				{
					PersonligIndkomst = 336000,
					NettoKapitalIndkomst = 28500,
					KapitalPensionsindskud = 32000
				}
			);

			var kommunaleSatser = new ValueTuple<IKommunaleSatser>(
				new KommunaleSatser
				{
					Kommuneskattesats = 0.30m
				});

			var topskat = _topskatBeregner.BeregnSkat(indkomster, 2009, kommunaleSatser);

			// (15 pct - 5,04 pct) af (336000 + 32000 + 28500 - 347200)
			topskat[0].Topskat.ShouldEqual(7395);
			topskat[0].SkatteloftNedslag.ShouldEqual(2484.72m);
			topskat[0].TopskatEfterNedslag.ShouldEqual(4910.28m);
		}

		[Fact]
		public void MedSkatteloft_Gift()
		{
			var indkomster = new ValueTuple<IPersonligeIndkomster>(
				new FakePersonligeIndkomster
					{
						PersonligIndkomst = 336000,
						NettoKapitalIndkomst = 28500,
						KapitalPensionsindskud = 32000
					},
				new FakePersonligeIndkomster
					{
						PersonligIndkomst = 92000,
						NettoKapitalIndkomst = 18500
					}
			);

			var kommunaleSatser = new ValueTuple<IKommunaleSatser>(
				new KommunaleSatser
				{
					Kommuneskattesats = 0.30m
				},
				new KommunaleSatser
				{
					Kommuneskattesats = 0.28m
				});

			var topskat = _topskatBeregner.BeregnSkat(indkomster, 2009, kommunaleSatser);

			// (15 pct - 5,04 pct) af (336000 + 32000 + 28500 - 347200), idet uudnyttet bundfradrag ikke kan overføres til ægtefælle
			topskat[0].Topskat.ShouldEqual(7395);
			topskat[0].SkatteloftNedslag.ShouldEqual(2484.72m);
			topskat[0].TopskatEfterNedslag.ShouldEqual(4910.28m);
			//topskat[0].SkatteloftNedslag.ShouldEqual(0);
			// (15 pct - 3,04 pct) af 18500, idet nettokapitalindkomst beskattes hos den med størst grundlag
			topskat[1].Topskat.ShouldEqual(2775);
			topskat[1].SkatteloftNedslag.ShouldEqual(562.40m);
			topskat[1].TopskatEfterNedslag.ShouldEqual(2212.60m);
		}

		[Fact]
		public void UdenSkatteloft_Gift()
		{
			var indkomster = new ValueTuple<IPersonligeIndkomster>(
				new FakePersonligeIndkomster
				{
					PersonligIndkomst = 336000,
					NettoKapitalIndkomst = 28500,
					KapitalPensionsindskud = 32000
				},
				new FakePersonligeIndkomster
				{
					PersonligIndkomst = 92000,
					NettoKapitalIndkomst = 18500
				}
			);

			var topskat = _topskatBeregner.BeregnSkat(indkomster, 2009);

			topskat[0].Topskat.ShouldEqual(7395);
			topskat[0].SkatteloftNedslag.ShouldEqual(0);
			topskat[0].TopskatEfterNedslag.ShouldEqual(7395);
			topskat[1].Topskat.ShouldEqual(2775);
			topskat[1].SkatteloftNedslag.ShouldEqual(0);
			topskat[1].TopskatEfterNedslag.ShouldEqual(2775);
		}
	}
}