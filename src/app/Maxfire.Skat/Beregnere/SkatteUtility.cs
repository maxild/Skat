using Maxfire.Skat.Extensions;

namespace Maxfire.Skat.Beregnere
{
	public class SkatteUtility
	{
		public static ValueTuple<IndkomstSkatter> CombineSkat(ValueTuple<SkatterAfPersonligIndkomst> skatterAfPersonligeIndkomster, ValueTuple<SkatterAfSkattepligtigIndkomst> skatterAfSkattepligtigeIndkomster)
		{
			return skatterAfPersonligeIndkomster.Size == 1 ?
				new IndkomstSkatter(skatterAfPersonligeIndkomster[0], skatterAfSkattepligtigeIndkomster[0]).ToTuple() 
				:
				new ValueTuple<IndkomstSkatter>(
					new IndkomstSkatter(skatterAfPersonligeIndkomster[0], skatterAfSkattepligtigeIndkomster[0]),
					new IndkomstSkatter(skatterAfPersonligeIndkomster[1], skatterAfSkattepligtigeIndkomster[1])
				);
		}
	}
}