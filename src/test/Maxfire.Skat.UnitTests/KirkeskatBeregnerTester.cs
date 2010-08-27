using Maxfire.TestCommons.AssertExtensions;
using Xunit;

namespace Maxfire.Skat.UnitTests
{
	public class KirkeskatBeregnerTester
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
					Kirkeskattesats = 0.01m
				});

			var beregner = new KirkeskatBeregner();

			var kommuneSkat = beregner.BeregnSkat(personligeBeloeb, kommunaleSatser);

			kommuneSkat[0].ShouldEqual(1);
		}
	}
}