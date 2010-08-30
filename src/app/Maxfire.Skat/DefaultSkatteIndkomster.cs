namespace Maxfire.Skat
{
	public class SkatteIndkomster : ISkatteIndkomster
	{
		public SkatteIndkomster(
			decimal personligIndkomstAMIndkomst, 
			decimal personligIndkomst, 
			decimal nettoKapitalIndkomst, 
			decimal ligningsmaessigeFradrag,
			decimal skattepligtigIndkomst, 
			decimal kapitalPensionsindskud, 
			decimal aktieIndkomst)
		{
			PersonligIndkomstAMIndkomst = personligIndkomstAMIndkomst;
			PersonligIndkomst = personligIndkomst;
			NettoKapitalIndkomst = nettoKapitalIndkomst;
			LigningsmaessigtFradrag = ligningsmaessigeFradrag;
			SkattepligtigIndkomst = skattepligtigIndkomst;
			KapitalPensionsindskud = kapitalPensionsindskud;
			AktieIndkomst = aktieIndkomst;
		}

		public decimal PersonligIndkomstAMIndkomst { get; private set; }
		public decimal PersonligIndkomst { get; private set; }
		public decimal NettoKapitalIndkomst { get; private set; }
		public decimal LigningsmaessigtFradrag { get; private set; }
		public decimal SkattepligtigIndkomst { get; private set; }
		public decimal KapitalPensionsindskud { get; private set; }
		public decimal AktieIndkomst { get; private set; }
	}

	public class SpecificeredeSkatteIndkomster
	{

	}
}