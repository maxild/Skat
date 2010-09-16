namespace Maxfire.Skat
{
	public interface ISpecificeredeSkatteyder : ISkatteyder
	{
		string Skattekommune { get; }
		
		decimal Kommuneskattesats { get; }
		
		decimal Kirkeskattesats { get; }
		
		bool Gift { get; }
	}
}