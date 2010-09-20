namespace Maxfire.Skat.Beregnere
{
	public class SkatterAfPersonligIndkomstResult
	{
		public SkatterAfPersonligIndkomstResult(IndkomstSkatterAfPersonligIndkomst skatterAfPersonligIndkomst, TopskatResult topskatResult)
		{
			SkatterAfPersonligIndkomst = skatterAfPersonligIndkomst;
			TopskatResult = topskatResult;
		}

		public IndkomstSkatterAfPersonligIndkomst SkatterAfPersonligIndkomst { get; private set; }

		public TopskatResult TopskatResult { get; private set; }
	}
}