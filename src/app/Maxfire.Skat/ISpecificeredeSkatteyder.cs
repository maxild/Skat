namespace Maxfire.Skat
{
	public interface ISpecificeredeSkatteyder : ISkatteyder
	{
		string Skattekommune { get; }
		
		decimal Kommuneskattesats { get; }
		
		decimal Kirkeskattesats { get; }
		
		Civilstand Civilstand { get; }
	}
}