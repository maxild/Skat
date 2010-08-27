using Maxfire.TestCommons.AssertExtensions;
using Xunit;

namespace Maxfire.Skat.UnitTests
{
	// TODO: Refactor to Context, Because_Of etc...
	public class SkatteModregnerTester
	{
		[Fact]
		public void FuldModregningAfPositivSkattevaerdi()
		{
			var skatter = new Skatter(bundskat: 800, topskat: 1000, mellemskat: 500);

			var skatteModregner = new SkatteModregner<Skatter>(
				Modregning<Skatter>.Af(x => x.Bundskat),
				Modregning<Skatter>.Af(x => x.Mellemskat));

			var modregnResult = skatteModregner.Modregn(skatter, 1000);

			modregnResult.IkkeUdnyttetSkattevaerdi.ShouldEqual(0);
			modregnResult.UdnyttetSkattevaerdi.ShouldEqual(1000);
			
			modregnResult.UdnyttedeSkattevaerdier.Bundskat.ShouldEqual(800);
			modregnResult.UdnyttedeSkattevaerdier.Mellemskat.ShouldEqual(200);
			modregnResult.UdnyttedeSkattevaerdier.Topskat.ShouldEqual(0);

			modregnResult.ModregnedeSkatter.Bundskat.ShouldEqual(0);
			modregnResult.ModregnedeSkatter.Mellemskat.ShouldEqual(300);
			modregnResult.ModregnedeSkatter.Topskat.ShouldEqual(1000);
		}

		[Fact]
		public void DelvisModregningAfPositivSkattevaerdi()
		{
			var skatter = new Skatter(bundskat: 800, topskat: 1000, mellemskat: 500);

			var skatteModregner = new SkatteModregner<Skatter>(
				Modregning<Skatter>.Af(x => x.Bundskat),
				Modregning<Skatter>.Af(x => x.Mellemskat));

			var modregnResult = skatteModregner.Modregn(skatter, 3000);

			modregnResult.IkkeUdnyttetSkattevaerdi.ShouldEqual(1700);
			modregnResult.UdnyttetSkattevaerdi.ShouldEqual(1300);

			modregnResult.UdnyttedeSkattevaerdier.Bundskat.ShouldEqual(800);
			modregnResult.UdnyttedeSkattevaerdier.Mellemskat.ShouldEqual(500);
			modregnResult.UdnyttedeSkattevaerdier.Topskat.ShouldEqual(0);

			modregnResult.ModregnedeSkatter.Bundskat.ShouldEqual(0);
			modregnResult.ModregnedeSkatter.Mellemskat.ShouldEqual(0);
			modregnResult.ModregnedeSkatter.Topskat.ShouldEqual(1000);
		}

		[Fact]
		public void ModregningAfNegativSkattevaerdi()
		{
			var skatter = new Skatter(bundskat: 800, topskat: 1000, mellemskat: 500);

			var skatteModregner = new SkatteModregner<Skatter>(
				Modregning<Skatter>.Af(x => x.Bundskat),
				Modregning<Skatter>.Af(x => x.Mellemskat));

			var modregnResult = skatteModregner.Modregn(skatter, -140);

			// Bemærk at ikke udnyttet skatteværdi er nul (og ikke -140), idet den skal være ikke-negativ
			modregnResult.IkkeUdnyttetSkattevaerdi.ShouldEqual(0);
			modregnResult.UdnyttetSkattevaerdi.ShouldEqual(0);

			modregnResult.UdnyttedeSkattevaerdier.Bundskat.ShouldEqual(0);
			modregnResult.UdnyttedeSkattevaerdier.Mellemskat.ShouldEqual(0);
			modregnResult.UdnyttedeSkattevaerdier.Topskat.ShouldEqual(0);

			modregnResult.ModregnedeSkatter.ShouldEqual(skatter);
		}
	}
}