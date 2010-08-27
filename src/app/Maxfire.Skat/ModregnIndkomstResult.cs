namespace Maxfire.Skat
{
	public class ModregnIndkomstResult
	{
		public ModregnIndkomstResult(decimal underskud, decimal indkomst, decimal modregning)
		{
			Underskud = underskud;
			Indkomst = indkomst;
			UdnyttetUnderskud = modregning;
		}

		/// <summary>
		/// Det underskud der bliver forsøgt udnyttet til at nedbringe indkomsten.
		/// </summary>
		public decimal Underskud { get; private set; }
			
		/// <summary>
		/// Den del af underskuddet, der kan rummes i indkomsten.
		/// </summary>
		public decimal UdnyttetUnderskud { get; private set; }
			
		/// <summary>
		/// Den del af underskuddet, der ikke kan rummes i indkomsten.
		/// </summary>
		public decimal IkkeUdnyttetUnderskud
		{
			get { return Underskud - UdnyttetUnderskud; }
		}
			
		/// <summary>
		/// Indkomsten før modregning af underskud.
		/// </summary>
		public decimal Indkomst { get; private set; }
			
		/// <summary>
		/// Indkomsten efter modregning af underskud.
		/// </summary>
		public decimal ModregnetIndkomst
		{
			get { return Indkomst - UdnyttetUnderskud; }
		}
	}
}