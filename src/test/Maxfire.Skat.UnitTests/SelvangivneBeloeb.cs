namespace Maxfire.Skat.UnitTests
{
	public class SelvangivneBeloeb : ISelvangivneBeloeb
	{
		public decimal Bruttoloen { get; set; }
		public decimal AtpEgetBidrag { get; set; }
		public decimal PrivatTegnetKapitalPensionsindskud { get; set; }
		public decimal Renteindt { get; set; }
		public decimal Renteudg { get; set; }
		public decimal AndenKapitalIndkomst { get; set; }
		public decimal LigningsmaessigeFradrag { get; set; }

		public virtual decimal PersonligIndkomstAMIndkomst
		{
			get { return Bruttoloen; }
		}

		public virtual decimal PersonligIndkomstEjAMIndkomst
		{
			get { return 0m; }
		}

		public virtual decimal FradragPersonligIndkomst
		{
			// TODO: iværksætter konto + atp eget bidrag
			get { return PrivatTegnetPensionsindskud + AtpEgetBidrag; }
		}

		public virtual decimal KapitalIndkomst
		{
			get { return Renteindt + AndenKapitalIndkomst; }
		}

		public virtual decimal FradragKapitalIndkomst
		{
			get { return Renteudg; }
		}

		public virtual decimal LigningsmaessigeFradragMinusBeskaeftigelsesfradrag
		{
			get { return LigningsmaessigeFradrag; }
		}

		public virtual decimal KapitalPensionsindskud
		{
			get { return PrivatTegnetKapitalPensionsindskud; }
		}

		public virtual decimal PrivatTegnetPensionsindskud
		{
			get { return PrivatTegnetKapitalPensionsindskud; }
		}

		public decimal AktieIndkomst
		{
			get { return 0m; }
		}
	}
}