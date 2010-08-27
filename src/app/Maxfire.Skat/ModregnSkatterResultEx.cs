namespace Maxfire.Skat
{
	// TODO: Prøv at eliminer en af ModregnSkatterResult og ModregnSkatterResultEx
	// TODO: Sørg for så få kald til ctor, istedet skal ToModregnResult extension method benyttes
	public class ModregnSkatterResultEx<TSkatter> : ModregnSkatterResult<TSkatter>
		where TSkatter : ISumable<decimal>, new()
	{
		public static ModregnSkatterResultEx<TSkatter> Nul(TSkatter skatter)
		{
			return new ModregnSkatterResultEx<TSkatter>(skatter, 0, new TSkatter(), 0, 0);
		}

		public ModregnSkatterResultEx(TSkatter skatter, decimal skattevaerdi, TSkatter udnyttedeSkattevaerdier, 
			decimal fradrag, decimal udnyttetFradrag) 
			: base(skatter, skattevaerdi, udnyttedeSkattevaerdier)
		{
			Fradrag = fradrag;
			UdnyttetFradrag = udnyttetFradrag;
		}

		/// <summary>
		/// Størrelsen af det fradrag, der er forsøgt udnyttet.
		/// </summary>
		public decimal Fradrag { get; private set; }
		
		/// <summary>
		/// Størrelsen af det udnyttede fradrag.
		/// </summary>
		public decimal UdnyttetFradrag { get; private set; }

		/// <summary>
		/// Størrelsen af det resterende ikke udnyttede fradrag.
		/// </summary>
		public decimal IkkeUdnyttetFradrag
		{
			get { return Fradrag - UdnyttetFradrag; }
		}
		
	}
}