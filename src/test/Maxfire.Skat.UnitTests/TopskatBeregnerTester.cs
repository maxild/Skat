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
			var indkomster = new ValueTuple<IPersonligeBeloeb>(
				new FakePersonligeBeloeb
				{
					PersonligIndkomst = 336000,
					NettoKapitalIndkomst = 28500,
					KapitalPensionsindskud = 32000
				}
			);

			var topskat = _topskatBeregner.BeregnSkat(indkomster, 2009);

			// 15 pct af (336000 + 32000 + 28500 - 347200)
			topskat[0].ShouldEqual(7395);
		}

		[Fact]
		public void MedSkatteloft_Ugift()
		{
			var indkomster = new ValueTuple<IPersonligeBeloeb>(
				new FakePersonligeBeloeb
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
			topskat[0].ShouldEqual(4910.28m);
		}

		[Fact]
		public void UdenSkatteloft_Gift()
		{
			var indkomster = new ValueTuple<IPersonligeBeloeb>(
				new FakePersonligeBeloeb
					{
						PersonligIndkomst = 336000,
						NettoKapitalIndkomst = 28500,
						KapitalPensionsindskud = 32000
					},
				new FakePersonligeBeloeb
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
			topskat[0].ShouldEqual(4910.28m);
			// (15 pct - 3,04 pct) af 18500, idet nettokapitalindkomst beskattes hos den med størst grundlag
			topskat[1].ShouldEqual(2212.60m);
		}

		[Fact]
		public void MedSkatteloft_Gift()
		{
			var indkomster = new ValueTuple<IPersonligeBeloeb>(
				new FakePersonligeBeloeb
				{
					PersonligIndkomst = 336000,
					NettoKapitalIndkomst = 28500,
					KapitalPensionsindskud = 32000
				},
				new FakePersonligeBeloeb
				{
					PersonligIndkomst = 92000,
					NettoKapitalIndkomst = 18500
				}
			);

			var topskat = _topskatBeregner.BeregnSkat(indkomster, 2009);

			topskat[0].ShouldEqual(7395);
			topskat[1].ShouldEqual(2775);
		}
	}
}