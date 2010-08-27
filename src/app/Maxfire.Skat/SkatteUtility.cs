using Maxfire.Skat.Extensions;

namespace Maxfire.Skat
{
	public class SkatteUtility
	{
		public static ValueTuple<Skatter> CombineSkat(ValueTuple<SkatterAfPersonligIndkomst> skatterAfPersonligeIndkomster, ValueTuple<SkatterAfSkattepligtigIndkomst> skatterAfSkattepligtigeIndkomster)
		{
			return skatterAfPersonligeIndkomster.Size == 1 ?
				new Skatter(skatterAfPersonligeIndkomster[0], skatterAfSkattepligtigeIndkomster[0]).ToTuple() 
				:
				new ValueTuple<Skatter>(
					new Skatter(skatterAfPersonligeIndkomster[0], skatterAfSkattepligtigeIndkomster[0]),
					new Skatter(skatterAfPersonligeIndkomster[1], skatterAfSkattepligtigeIndkomster[1])
				);
		}
	}
}