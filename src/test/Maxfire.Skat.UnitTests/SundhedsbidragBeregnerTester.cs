using Maxfire.TestCommons.AssertExtensions;
using Xunit;

namespace Maxfire.Skat.UnitTests
{
	public class SundhedsbidragBeregnerTester
	{
		class FakeSkattelovRegistry : AbstractFakeSkattelovRegistry
		{
			public override decimal GetSundhedsbidragSkattesats(int skatteAar)
			{
				return 0.08m;
			}
		}

		[Fact]
		public void BeregnSkat()
		{
			var personligeBeloeb = new ValueTuple<IPersonligeBeloeb>(
				new FakePersonligeBeloeb
				{
					PersonligIndkomst = 100
				}
			);

			var sundhedsbidragBeregner = new SundhedsbidragBeregner(new FakeSkattelovRegistry());

			var sundhedsbidrag = sundhedsbidragBeregner.BeregnSkat(personligeBeloeb, 2010);

			sundhedsbidrag[0].ShouldEqual(8);
		}
	}
}