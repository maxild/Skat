using Maxfire.Skat.Beregnere;
using Shouldly;
using Xunit;

namespace Maxfire.Skat.UnitTests
{
	public class KommuneskatBeregnerTester
	{
		[Fact]
		public void BeregnSkat()
		{
			var indkomster = new ValueTuple<ISkattepligtigeIndkomster>(
				new FakeSkattepligtigeIndkomster()
				{
					SkattepligtigIndkomst = 100
				}
			);

			var kommunaleSatser = new ValueTuple<IKommunaleSatser>(
				new KommunaleSatser
				{
					Kommuneskattesats = 0.25m
				});

			var beregner = new KommuneskatBeregner();

			var kommuneSkat = beregner.BeregnSkat(indkomster, kommunaleSatser);

			kommuneSkat[0].ShouldBe(25);
		}
	}
}
