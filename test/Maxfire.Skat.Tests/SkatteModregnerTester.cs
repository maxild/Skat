using Maxfire.Skat.Beregnere;
using Shouldly;
using Xunit;

namespace Maxfire.Skat.UnitTests
{
	// TODO: Refactor to Context, Because_Of etc...
	public class SkatteModregnerTester
	{
		[Fact]
		public void FuldModregningAfPositivSkattevaerdi()
		{
			var skatter = new IndkomstSkatter(bundskat: 800, topskat: 1000, mellemskat: 500);

			var skatteModregner = new SkatteModregner<IndkomstSkatter>(
				Modregning<IndkomstSkatter>.Af(x => x.Bundskat),
				Modregning<IndkomstSkatter>.Af(x => x.Mellemskat));

			var modregnResult = skatteModregner.Modregn(skatter, 1000);

			modregnResult.IkkeUdnyttetSkattevaerdi.ShouldBe(0);
			modregnResult.UdnyttetSkattevaerdi.ShouldBe(1000);
			
			modregnResult.UdnyttedeSkattevaerdier.Bundskat.ShouldBe(800);
			modregnResult.UdnyttedeSkattevaerdier.Mellemskat.ShouldBe(200);
			modregnResult.UdnyttedeSkattevaerdier.Topskat.ShouldBe(0);

			modregnResult.ModregnedeSkatter.Bundskat.ShouldBe(0);
			modregnResult.ModregnedeSkatter.Mellemskat.ShouldBe(300);
			modregnResult.ModregnedeSkatter.Topskat.ShouldBe(1000);
		}

		[Fact]
		public void DelvisModregningAfPositivSkattevaerdi()
		{
			var skatter = new IndkomstSkatter(bundskat: 800, topskat: 1000, mellemskat: 500);

			var skatteModregner = new SkatteModregner<IndkomstSkatter>(
				Modregning<IndkomstSkatter>.Af(x => x.Bundskat),
				Modregning<IndkomstSkatter>.Af(x => x.Mellemskat));

			var modregnResult = skatteModregner.Modregn(skatter, 3000);

			modregnResult.IkkeUdnyttetSkattevaerdi.ShouldBe(1700);
			modregnResult.UdnyttetSkattevaerdi.ShouldBe(1300);

			modregnResult.UdnyttedeSkattevaerdier.Bundskat.ShouldBe(800);
			modregnResult.UdnyttedeSkattevaerdier.Mellemskat.ShouldBe(500);
			modregnResult.UdnyttedeSkattevaerdier.Topskat.ShouldBe(0);

			modregnResult.ModregnedeSkatter.Bundskat.ShouldBe(0);
			modregnResult.ModregnedeSkatter.Mellemskat.ShouldBe(0);
			modregnResult.ModregnedeSkatter.Topskat.ShouldBe(1000);
		}

		[Fact]
		public void ModregningAfNegativSkattevaerdi()
		{
			var skatter = new IndkomstSkatter(bundskat: 800, topskat: 1000, mellemskat: 500);

			var skatteModregner = new SkatteModregner<IndkomstSkatter>(
				Modregning<IndkomstSkatter>.Af(x => x.Bundskat),
				Modregning<IndkomstSkatter>.Af(x => x.Mellemskat));

			var modregnResult = skatteModregner.Modregn(skatter, -140);

			// Bemærk at ikke udnyttet skatteværdi er nul (og ikke -140), idet den skal være ikke-negativ
			modregnResult.IkkeUdnyttetSkattevaerdi.ShouldBe(0);
			modregnResult.UdnyttetSkattevaerdi.ShouldBe(0);

			modregnResult.UdnyttedeSkattevaerdier.Bundskat.ShouldBe(0);
			modregnResult.UdnyttedeSkattevaerdier.Mellemskat.ShouldBe(0);
			modregnResult.UdnyttedeSkattevaerdier.Topskat.ShouldBe(0);

			modregnResult.ModregnedeSkatter.ShouldBe(skatter);
		}
	}
}
