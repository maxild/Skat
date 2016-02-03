namespace Maxfire.Skat.UnitTests
{
	public class SelvangivneBeloeb : ISelvangivneBeloeb
	{
		/// <summary>
		/// Løn før AM-bidrag
		/// </summary>
		public decimal Bruttoloen { get; set; }

		/// <summary>
		/// Løn efter AM-bidrag
		/// </summary>
		public decimal Nettoloen { get; set; }

		/// <summary>
		/// Personlig indkomst, der ikke indgår i AM-indkomst
		/// </summary>
		public decimal Pension { get; set; }

		public decimal AtpEgetBidrag { get; set; }

		public decimal PrivatTegnetKapitalPensionsindskud { get; set; }
		public decimal PrivatTegnetRatePensionsindskud { get; set; }

		public decimal ArbejdsgiverAdminKapitalPensionsindskud { get; set; }
		
		public virtual decimal NettoKapitalIndkomst { get; set; }

		public decimal LigningsmaessigtFradrag { get; set; }

		public decimal PersonligIndkomstFremfoertUnderskud { get; set; }

		public decimal SkattepligtigIndkomstFremfoertUnderskud { get; set; }

		public decimal AktieIndkomst { get; set; }
		
		public virtual decimal PersonligIndkomstAMIndkomst
		{
			get { return Bruttoloen + Nettoloen / (1 - 0.08m); }
		}

		public virtual decimal PersonligIndkomstEjAMIndkomst
		{
			get { return Pension - FradragPersonligIndkomst; }
		}

		private decimal FradragPersonligIndkomst
		{
			// TODO: iværksætter konto
			get { return PrivatTegnetPensionsindskud + AtpEgetBidrag; }
		}

		public virtual decimal LigningsmaessigtFradragMinusBeskaeftigelsesfradrag
		{
			get { return LigningsmaessigtFradrag; }
		}

		public virtual decimal KapitalPensionsindskud
		{
			get { return PrivatTegnetKapitalPensionsindskud + ArbejdsgiverAdminKapitalPensionsindskud; }
		}

		public decimal PrivatTegnetPensionsindskud
		{
			get { return PrivatTegnetKapitalPensionsindskud + PrivatTegnetRatePensionsindskud; }
		}
	}
}