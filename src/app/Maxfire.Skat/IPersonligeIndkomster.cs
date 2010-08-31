namespace Maxfire.Skat
{
	public interface IPersonligeIndkomster
	{
		decimal AMIndkomst { get; }
		decimal PersonligIndkomst { get; }

		decimal NettoKapitalIndkomst { get; }

		decimal LigningsmaessigtFradrag { get; }
		
		decimal KapitalPensionsindskud { get; }
		
		decimal AktieIndkomst { get; }
	}
}