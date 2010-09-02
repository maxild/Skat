namespace Maxfire.Skat
{
	public interface IAMIndkomstSkatteberegner
	{
		decimal Beregn(decimal amIndkomst, int skatteAar);
	}
}