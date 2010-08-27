namespace Maxfire.Skat
{
	public interface ISkatteIndkomster
	{
		decimal PersonligIndkomstAMIndkomst { get; }
		decimal PersonligIndkomst { get; }
		decimal NettoKapitalIndkomst { get; }
		decimal LigningsmaessigeFradrag { get; }
		decimal SkattepligtigIndkomst { get; }
		decimal KapitalPensionsindskud { get; }
		decimal AktieIndkomst { get; }
	}
}