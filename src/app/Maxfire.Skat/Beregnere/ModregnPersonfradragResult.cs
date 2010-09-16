namespace Maxfire.Skat.Beregnere
{
	// TODO: Fjern denne type
	public class ModregnPersonfradragResult
	{
		public ModregnPersonfradragResult(IndkomstSkatter indkomstSkatter, IndkomstSkatter modregninger, IndkomstSkatter ikkeUdnyttedeSkattevaerdier)
		{
			IndkomstSkatter = indkomstSkatter;
			UdnyttedeSkattevaerdier = modregninger;
			IkkeUdnyttedeSkattevaerdier = ikkeUdnyttedeSkattevaerdier;
		}

		/// <summary>
		/// Størrelsen af de skatter, der skal modregnes i.
		/// </summary>
		public IndkomstSkatter IndkomstSkatter { get; private set; }
		
		/// <summary>
		/// De udnyttede skatteværdier, der svarer til modregningerne i skatterne.
		/// </summary>
		public IndkomstSkatter UdnyttedeSkattevaerdier { get; private set; }

		/// <summary>
		/// Skatternes størrelse efter modregning.
		/// </summary>
		public IndkomstSkatter ModregnedeIndkomstSkatter
		{
			get { return IndkomstSkatter - UdnyttedeSkattevaerdier; }
		}

		/// <summary>
		/// Skatteværdien af det ikke udnyttede fradrag, underskud el.lign.
		/// </summary>
		public decimal IkkeUdnyttetSkattevaerdi
		{
			get { return IkkeUdnyttedeSkattevaerdier.Sum(); }
		}

		/// <summary>
		/// Skatteværdien af det udnyttede fradrag, underskud el.lign.
		/// </summary>
		public decimal UdnyttetSkattevaerdi
		{
			get { return UdnyttedeSkattevaerdier.Sum(); }
		}

		/// <summary>
		/// De ikke udnyttede skatteværdier.
		/// </summary>
		public IndkomstSkatter IkkeUdnyttedeSkattevaerdier { get; private set; } // Note: Den er særlig for personfradrag
	}
}