namespace Maxfire.Skat.UnitTests
{
	public class FakeSkatteIndkomster : ISkatteIndkomster
	{
		public decimal AMIndkomst { get; set; }
		public decimal PersonligIndkomst{ get; set; }
		public decimal NettoKapitalIndkomst { get; set; }
		public decimal LigningsmaessigtFradrag { get; set; }
		public decimal KapitalPensionsindskud { get; set; }
		public decimal AktieIndkomst { get; set; }
		public decimal SkattepligtigIndkomst { get; set; }
	}
}