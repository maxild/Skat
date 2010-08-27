using Maxfire.Core;

namespace Maxfire.Skat
{
	public class ModregnSkatterResult<TSkatter>
		where TSkatter : ISumable<decimal>
	{
		public ModregnSkatterResult(TSkatter skatter, decimal skattevaerdi, TSkatter modregninger)
		{
			Skatter = skatter;
			Skattevaerdi = skattevaerdi; 
			UdnyttedeSkattevaerdier = modregninger;
		}

		/// <summary>
		/// Størrelsen af den skatteværdi, der er forsøgt modregnet.
		/// </summary>
		public decimal Skattevaerdi { get; private set; }

		/// <summary>
		/// Størrelsen af de skatter, der skal modregnes i.
		/// </summary>
		public TSkatter Skatter { get; private set; }

		/// <summary>
		/// De udnyttede skatteværdier, der svarer til de mulige modregninger i skatterne.
		/// </summary>
		public TSkatter UdnyttedeSkattevaerdier { get; private set; }

		/// <summary>
		/// Skatternes størrelse efter modregning.
		/// </summary>
		public TSkatter ModregnedeSkatter
		{
			get { return Operator<TSkatter>.Subtract(Skatter, UdnyttedeSkattevaerdier); }
		}
		
		/// <summary>
		/// Skatteværdien af det ikke udnyttede fradrag, underskud el.lign.
		/// </summary>
		public decimal IkkeUdnyttetSkattevaerdi
		{
			get { return Skattevaerdi - UdnyttetSkattevaerdi; }
		}
		
		/// <summary>
		/// Skatteværdien af det udnyttede fradrag, underskud el.lign.
		/// </summary>
		public decimal UdnyttetSkattevaerdi
		{
			get { return UdnyttedeSkattevaerdier.Sum(); }
		}
	}
}