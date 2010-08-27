using Maxfire.TestCommons.AssertExtensions;
using Xunit;

namespace Maxfire.Skat.UnitTests
{
	public class SkatterTester
	{
		[Fact]
		public void CanAdd()
		{
			var x = new Skatter(kommuneskat: 1, sundhedsbidrag: 3, kirkeskat: 2, bundskat: 4, mellemskat: 5,
			                    topskat: 6, aktieindkomstskatUnderGrundbeloebet: 7, aktieindkomstskatOverGrundbeloebet: 8);

			var y = new Skatter(kommuneskat: 2, sundhedsbidrag: 6, kirkeskat: 4, bundskat: 8, mellemskat: 10,
			                    topskat: 12, aktieindkomstskatUnderGrundbeloebet: 14, aktieindkomstskatOverGrundbeloebet: 16);

			(x + y).ShouldEqual(new Skatter(kommuneskat: 3, sundhedsbidrag: 9, kirkeskat: 6, bundskat: 12,
			                                mellemskat: 15, topskat: 18, aktieindkomstskatUnderGrundbeloebet: 21, aktieindkomstskatOverGrundbeloebet: 24));
		}
	}
}