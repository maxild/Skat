namespace Maxfire.Skat.UnitTests
{
	public class FakeSkatteIndkomster : ISkatteIndkomster
	{
		public decimal AMIndkomst { get; set; }

		public decimal PersonligIndkomstSkattegrundlag
		{
			get { return PersonligIndkomst; }
		}

		public decimal NettoKapitalIndkomstSkattegrundlag
		{
			get { return NettoKapitalIndkomst; }
		}

		public decimal PersonligIndkomst{ get; set; }
		public decimal NettoKapitalIndkomst { get; set; }
		public decimal LigningsmaessigtFradrag { get; set; }

		public decimal KapitalPensionsindskudSkattegrundlag
		{
			get { return KapitalPensionsindskud; }
		}

		public decimal KapitalPensionsindskud { get; set; }
		public decimal AktieIndkomst { get; set; }
		public decimal SkattepligtigIndkomst { get; set; }

		public decimal SkattepligtigIndkomstSkattegrundlag
		{
			get { return SkattepligtigIndkomst; }
		}
	}
}