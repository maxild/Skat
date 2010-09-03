namespace Maxfire.Skat
{
	public interface IAMIndkomstSkatteberegner
	{
		decimal Beregn(decimal grundlag, int skatteAar);
		decimal Beregn(IArbejdsmarkedIndkomster indkomster, int skatteAar);
	}
}