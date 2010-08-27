namespace Maxfire.Skat
{
	public interface IPersonligeBeloeb
	{
		ISkatteIndkomster Selvangiven { get; }
		ISkatteIndkomster Skattegrundlag { get; }

		decimal FremfoertUnderskudPersonligIndkomst { get; set; } // Hvorfor mutable?
		decimal ModregnetUnderskudPersonligIndkomst { get; set; }
		decimal UnderskudPersonligIndkomstTilFremfoersel { get; set; }

		decimal ModregnetUnderskudNettoKapitalIndkomst { get; set; }

		decimal FremfoertUnderskudSkattepligtigIndkomst { get; set; } // Hvorfor mutable
		decimal ModregnetUnderskudSkattepligtigIndkomst { get; set; }
		decimal UnderskudSkattepligtigIndkomstTilFremfoersel { get; set; }

		decimal ModregnetUnderskudKapitalPensionsindskud { get; set; }
	}
}