using Maxfire.Skat.Extensions;

namespace Maxfire.Skat.Beregnere
{
	public static class SkatteUtility
	{
		public static ValueTuple<IndkomstSkatter> CombineSkat(ValueTuple<IndkomstSkatterAfPersonligIndkomst> skatterAfPersonligeIndkomster, ValueTuple<SkatterAfSkattepligtigIndkomst> skatterAfSkattepligtigeIndkomster)
		{
			return skatterAfPersonligeIndkomster.Size == 1 ?
				new IndkomstSkatter(skatterAfPersonligeIndkomster[0], skatterAfSkattepligtigeIndkomster[0]).ToTuple() 
				:
				new ValueTuple<IndkomstSkatter>(
					new IndkomstSkatter(skatterAfPersonligeIndkomster[0], skatterAfSkattepligtigeIndkomster[0]),
					new IndkomstSkatter(skatterAfPersonligeIndkomster[1], skatterAfSkattepligtigeIndkomster[1])
				);
		}

		public static ValueTuple<IndkomstSkatter> CombineSkat(ValueTuple<IndkomstSkatterAfPersonligIndkomst> skatterAfPersonligeIndkomster)
		{
			return skatterAfPersonligeIndkomster.Size == 1 ?
				new IndkomstSkatter(skatterAfPersonligeIndkomster[0], SkatterAfSkattepligtigIndkomst.Nul).ToTuple()
				:
				new ValueTuple<IndkomstSkatter>(
					new IndkomstSkatter(skatterAfPersonligeIndkomster[0], SkatterAfSkattepligtigIndkomst.Nul),
					new IndkomstSkatter(skatterAfPersonligeIndkomster[1], SkatterAfSkattepligtigIndkomst.Nul)
				);
		}

		public static IndkomstSkatter CombineSkat(IndkomstSkatterAfPersonligIndkomst skatterAfPersonligIndkomst)
		{
			return new IndkomstSkatter(skatterAfPersonligIndkomst, SkatterAfSkattepligtigIndkomst.Nul);
		}
	}
}