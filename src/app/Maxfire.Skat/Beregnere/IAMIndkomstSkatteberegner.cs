namespace Maxfire.Skat.Beregnere
{
	public interface IAMIndkomstSkatteberegner
	{
		decimal Beregn(decimal grundlag, int skatteAar);
		decimal Beregn(IArbejdsmarkedIndkomster indkomster, int skatteAar);
	}
}