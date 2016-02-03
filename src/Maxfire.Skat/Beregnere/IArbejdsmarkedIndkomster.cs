namespace Maxfire.Skat.Beregnere
{
	public interface IArbejdsmarkedIndkomster
	{
		/// <summary>
		/// Den del af den personlige indkomst (før AM-bidrag), der er 
		/// grundlag for beregningen af arbejdsmarkedsbidrag (AM-bidrag).
		/// </summary>
		decimal AMIndkomst { get; }

		/// <summary>
		/// Grundlaget for beskæftigelsesfradrag.
		/// </summary>
		decimal Arbejdsindkomst { get; }
	}
}