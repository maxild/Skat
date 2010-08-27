using Maxfire.TestCommons.AssertExtensions;
using Xunit;

namespace Maxfire.Skat.UnitTests
{
	public class KommuneskatBeregnerTester
	{
		[Fact]
		public void BeregnSkat()
		{
			var personligeBeloeb = new ValueTuple<IPersonligeBeloeb>(
				new FakePersonligeBeloeb
				{
					PersonligIndkomst = 100
				}
			);

			var kommunaleSatser = new ValueTuple<IKommunaleSatser>(
				new KommunaleSatser
				{
					Kommuneskattesats = 0.25m
				});

			var beregner = new KommuneskatBeregner();

			var kommuneSkat = beregner.BeregnSkat(personligeBeloeb, kommunaleSatser);

			kommuneSkat[0].ShouldEqual(25);
		}
	}
}