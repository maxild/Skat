namespace Maxfire.Skat
{
	// TODO: Fjern denne type
	public class ModregnPersonfradragResult
	{
		public ModregnPersonfradragResult(Skatter skatter, Skatter modregninger, Skatter ikkeUdnyttedeSkattevaerdier)
		{
			Skatter = skatter;
			UdnyttedeSkattevaerdier = modregninger;
			IkkeUdnyttedeSkattevaerdier = ikkeUdnyttedeSkattevaerdier;
		}

		/// <summary>
		/// Størrelsen af de skatter, der skal modregnes i.
		/// </summary>
		public Skatter Skatter { get; private set; }
		
		/// <summary>
		/// De udnyttede skatteværdier, der svarer til modregningerne i skatterne.
		/// </summary>
		public Skatter UdnyttedeSkattevaerdier { get; private set; }

		/// <summary>
		/// Skatternes størrelse efter modregning.
		/// </summary>
		public Skatter ModregnedeSkatter
		{
			get { return Skatter - UdnyttedeSkattevaerdier; }
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
		public Skatter IkkeUdnyttedeSkattevaerdier { get; private set; } // Note: Den er særlig for personfradrag
	}
}