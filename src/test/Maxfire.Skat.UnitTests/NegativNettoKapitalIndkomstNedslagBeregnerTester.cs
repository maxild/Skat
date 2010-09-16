using Maxfire.Skat.Beregnere;
using Maxfire.TestCommons.AssertExtensions;
using Xunit;

namespace Maxfire.Skat.UnitTests
{
	public class NegativNettoKapitalIndkomstNedslagBeregnerTester
	{
		class FakeSkattelovRegistry : AbstractFakeSkattelovRegistry
		{
			public override decimal GetNegativNettoKapitalIndkomstGrundbeloeb(int skatteAar)
			{
				return 50000;
			}

			public override decimal GetNegativNettoKapitalIndkomstSats(int skatteAar)
			{
				return 0.05m;
			}
		}

		[Fact]
		public void IngenNegativNettoKapitalIndkomst_BeregnNedslag_Ugift()
		{
			var indkomster = new ValueTuple<IPersonligeIndkomster>(
				new FakePersonligeIndkomster
					{
						NettoKapitalIndkomst = 10000
					});
			
			var negativNettoKapitalIndkomstNedslagBeregner = new NegativNettoKapitalIndkomstNedslagBeregner(new FakeSkattelovRegistry());
			
			var nedslag = negativNettoKapitalIndkomstNedslagBeregner.BeregnNedslag(indkomster, 2012);

			nedslag[0].ShouldEqual(0);
		}

		[Fact]
		public void NegativNettoKapitalIndkomstUnderGrundbeloeb_BeregnNedslag_Ugift()
		{
			var indkomster = new ValueTuple<IPersonligeIndkomster>(
				new FakePersonligeIndkomster
				{
					NettoKapitalIndkomst = -25000
				});

			var negativNettoKapitalIndkomstNedslagBeregner = new NegativNettoKapitalIndkomstNedslagBeregner(new FakeSkattelovRegistry());

			var nedslag = negativNettoKapitalIndkomstNedslagBeregner.BeregnNedslag(indkomster, 2012);

			nedslag[0].ShouldEqual(1250);
		}

		[Fact]
		public void NegativNettoKapitalIndkomstOverGrundbeloeb_BeregnNedslag_Ugift()
		{
			var indkomster = new ValueTuple<IPersonligeIndkomster>(
				new FakePersonligeIndkomster
				{
					NettoKapitalIndkomst = -60000
				});

			var negativNettoKapitalIndkomstNedslagBeregner = new NegativNettoKapitalIndkomstNedslagBeregner(new FakeSkattelovRegistry());

			var nedslag = negativNettoKapitalIndkomstNedslagBeregner.BeregnNedslag(indkomster, 2012);

			nedslag[0].ShouldEqual(2500);
		}

		[Fact]
		public void FuldUdnyttelse_ModregnMedNedslag_Ugift()
		{
			var nedslag = new ValueTuple<decimal>(10);
			var skatter = new ValueTuple<IndkomstSkatter>(new IndkomstSkatter(bundskat: 1000));

			var negativNettoKapitalIndkomstNedslagBeregner = new NegativNettoKapitalIndkomstNedslagBeregner(new FakeSkattelovRegistry());
			var modregnResults = negativNettoKapitalIndkomstNedslagBeregner.ModregnMedNedslag(skatter, nedslag);

			modregnResults[0].UdnyttetSkattevaerdi.ShouldEqual(10);
			modregnResults[0].IkkeUdnyttetSkattevaerdi.ShouldEqual(0);
			modregnResults[0].ModregnedeSkatter.ShouldEqual(new IndkomstSkatter(bundskat: 990));
		}

		[Fact]
		public void DelvisUdnyttelse_ModregnMedNedslag_Ugift()
		{
			var nedslag = new ValueTuple<decimal>(2000);
			var skatter = new ValueTuple<IndkomstSkatter>(new IndkomstSkatter(bundskat: 1000));

			var negativNettoKapitalIndkomstNedslagBeregner = new NegativNettoKapitalIndkomstNedslagBeregner(new FakeSkattelovRegistry());
			var modregnResults = negativNettoKapitalIndkomstNedslagBeregner.ModregnMedNedslag(skatter, nedslag);

			modregnResults[0].UdnyttetSkattevaerdi.ShouldEqual(1000);
			modregnResults[0].IkkeUdnyttetSkattevaerdi.ShouldEqual(1000);
			modregnResults[0].ModregnedeSkatter.ShouldEqual(IndkomstSkatter.Nul);
		}

		[Fact]
		public void IngenNegativNettoKapitalIndkomst_BeregnNedslag_Gift()
		{
			var indkomster = new ValueTuple<IPersonligeIndkomster>(
				new FakePersonligeIndkomster
				{
					NettoKapitalIndkomst = 10000
				},
				new FakePersonligeIndkomster
				{
					NettoKapitalIndkomst = 10000
				});

			var negativNettoKapitalIndkomstNedslagBeregner = new NegativNettoKapitalIndkomstNedslagBeregner(new FakeSkattelovRegistry());

			var nedslag = negativNettoKapitalIndkomstNedslagBeregner.BeregnNedslag(indkomster, 2012);

			nedslag[0].ShouldEqual(0);
			nedslag[1].ShouldEqual(0);
		}

		[Fact]
		public void OverfoerselAfStorPositivTilNegativNettoKapitalIndkomst_BeregnNedslag_Gift()
		{
			var indkomster = new ValueTuple<IPersonligeIndkomster>(
				new FakePersonligeIndkomster
				{
					NettoKapitalIndkomst = -5000
				},
				new FakePersonligeIndkomster
				{
					NettoKapitalIndkomst = 10000
				});

			var negativNettoKapitalIndkomstNedslagBeregner = new NegativNettoKapitalIndkomstNedslagBeregner(new FakeSkattelovRegistry());

			var nedslag = negativNettoKapitalIndkomstNedslagBeregner.BeregnNedslag(indkomster, 2012);

			nedslag[0].ShouldEqual(0);
			nedslag[1].ShouldEqual(0);
		}

		[Fact]
		public void OverfoerselAfLillePositivTilNegativNettoKapitalIndkomst_BeregnNedslag_Gift()
		{
			var indkomster = new ValueTuple<IPersonligeIndkomster>(
				new FakePersonligeIndkomster
				{
					NettoKapitalIndkomst = -50000
				},
				new FakePersonligeIndkomster
				{
					NettoKapitalIndkomst = 10000
				});

			var negativNettoKapitalIndkomstNedslagBeregner = new NegativNettoKapitalIndkomstNedslagBeregner(new FakeSkattelovRegistry());

			var nedslag = negativNettoKapitalIndkomstNedslagBeregner.BeregnNedslag(indkomster, 2012);

			nedslag[0].ShouldEqual(2000);
			nedslag[1].ShouldEqual(0);
		}

		[Fact]
		public void OverfoerselAfPositivTilNegativNettoKapitalIndkomstOgLoft_BeregnNedslag_Gift()
		{
			var indkomster = new ValueTuple<IPersonligeIndkomster>(
				new FakePersonligeIndkomster
				{
					NettoKapitalIndkomst = -80000
				},
				new FakePersonligeIndkomster
				{
					NettoKapitalIndkomst = 10000
				});

			var negativNettoKapitalIndkomstNedslagBeregner = new NegativNettoKapitalIndkomstNedslagBeregner(new FakeSkattelovRegistry());

			var nedslag = negativNettoKapitalIndkomstNedslagBeregner.BeregnNedslag(indkomster, 2012);

			nedslag[0].ShouldEqual(2500);
			nedslag[1].ShouldEqual(0);
		}

		[Fact]
		public void FuldUdnyttelseUdenOverfoersel_Gift()
		{
			var nedslag = new ValueTuple<decimal>(10, 30);
			var skatter = new ValueTuple<IndkomstSkatter>(new IndkomstSkatter(bundskat: 1000), new IndkomstSkatter(bundskat: 1000));

			var negativNettoKapitalIndkomstNedslagBeregner = new NegativNettoKapitalIndkomstNedslagBeregner(new FakeSkattelovRegistry());
			var modregnResults = negativNettoKapitalIndkomstNedslagBeregner.ModregnMedNedslag(skatter, nedslag);

			modregnResults[0].UdnyttetSkattevaerdi.ShouldEqual(10);
			modregnResults[0].IkkeUdnyttetSkattevaerdi.ShouldEqual(0);
			modregnResults[0].ModregnedeSkatter.ShouldEqual(new IndkomstSkatter(bundskat: 990));
			modregnResults[1].UdnyttetSkattevaerdi.ShouldEqual(30);
			modregnResults[1].IkkeUdnyttetSkattevaerdi.ShouldEqual(0);
			modregnResults[1].ModregnedeSkatter.ShouldEqual(new IndkomstSkatter(bundskat: 970));
		}

		[Fact]
		public void DelvisUdnyttelseOgOverfoersel_ModregnMedNedslag_Gift()
		{
			var nedslag = new ValueTuple<decimal>(2000, 500);
			var skatter = new ValueTuple<IndkomstSkatter>(new IndkomstSkatter(bundskat: 1000), new IndkomstSkatter(bundskat: 1000));

			var negativNettoKapitalIndkomstNedslagBeregner = new NegativNettoKapitalIndkomstNedslagBeregner(new FakeSkattelovRegistry());
			var modregnResults = negativNettoKapitalIndkomstNedslagBeregner.ModregnMedNedslag(skatter, nedslag);

			modregnResults[0].UdnyttetSkattevaerdi.ShouldEqual(1000); // udnyttet i egne skatter
			modregnResults[0].IkkeUdnyttetSkattevaerdi.ShouldEqual(500); // de resterende 500 er overf�rt
			modregnResults[0].ModregnedeSkatter.ShouldEqual(IndkomstSkatter.Nul);
			modregnResults[1].UdnyttetSkattevaerdi.ShouldEqual(1000); // eget nedslag + overf�rt nedslag
			modregnResults[1].IkkeUdnyttetSkattevaerdi.ShouldEqual(0);
			modregnResults[1].ModregnedeSkatter.ShouldEqual(IndkomstSkatter.Nul);
		}
	}
}