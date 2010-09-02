namespace Maxfire.Skat.UnitTests
{
	public class FakeSkattepligtigeIndkomster : ISkattepligtigeIndkomster
	{
		public decimal SkattepligtigIndkomst { get; set; }

		public decimal SkattepligtigIndkomstSkattegrundlag
		{
			get { return SkattepligtigIndkomst; }
		}
	}
}