using Maxfire.Skat.Extensions;

namespace Maxfire.Skat.Beregnere
{
	public static class SkatteUtility
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

		public static ValueTuple<IndkomstSkatter> CombineSkat(ValueTuple<SkatterAfPersonligIndkomst> skatterAfPersonligeIndkomster)
		{
			return skatterAfPersonligeIndkomster.Size == 1 ?
				new IndkomstSkatter(skatterAfPersonligeIndkomster[0], SkatterAfSkattepligtigIndkomst.Nul).ToTuple()
				:
				new ValueTuple<IndkomstSkatter>(
					new IndkomstSkatter(skatterAfPersonligeIndkomster[0], SkatterAfSkattepligtigIndkomst.Nul),
					new IndkomstSkatter(skatterAfPersonligeIndkomster[1], SkatterAfSkattepligtigIndkomst.Nul)
				);
		}

		public static ValueTuple<IndkomstSkatter> CombineSkat(ValueTuple<SkatterAfSkattepligtigIndkomst> skatterAfSkattepligtigeIndkomster)
		{
			return skatterAfSkattepligtigeIndkomster.Size == 1 ?
				new IndkomstSkatter(SkatterAfPersonligIndkomst.Nul, skatterAfSkattepligtigeIndkomster[0]).ToTuple()
				:
				new ValueTuple<IndkomstSkatter>(
					new IndkomstSkatter(SkatterAfPersonligIndkomst.Nul, skatterAfSkattepligtigeIndkomster[0]),
					new IndkomstSkatter(SkatterAfPersonligIndkomst.Nul, skatterAfSkattepligtigeIndkomster[1])
				);
		}
	}
}