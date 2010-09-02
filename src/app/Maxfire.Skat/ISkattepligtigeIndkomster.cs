namespace Maxfire.Skat
{
	public interface ISkattepligtigeIndkomster
	{
		decimal SkattepligtigIndkomstSkattegrundlag { get; }
	}

	public interface ISkattepligtigeIndkomsterModregning : ISkattepligtigeIndkomster
	{
		decimal SkattepligtigIndkomstAaretsUnderskud { get; }
		decimal SkattepligtigIndkomstFremfoertUnderskud { get; }
		decimal SkattepligtigIndkomstUnderskudTilFremfoersel { get; }

		/// <summary>
		/// Nedbring underskuddet i den skattepligtige indkomst med det angvne overførte underskud.
		/// </summary>
		/// <param name="text">Beskrivende tekst.</param>
		/// <param name="overfoertUnderskud">
		/// Det udnyttede underskud, der kan overføres og rummes i den skattepligtige indkomst _andetsteds_.
		/// </param>
		void NedbringUnderskudForSkattepligtigIndkomst(string text, decimal overfoertUnderskud);

		/// <summary>
		/// Nedbring den skattepligtige indkomst med det angivne overførte underskud.
		/// </summary>
		/// <param name="text">Beskrivende tekst.</param>
		/// <param name="overfoertUnderskud">
		/// Det udnyttede underskud, der kan overføres og rummes i den skattepligtige indkomst.
		/// </param>
		void NedbringSkattepligtigIndkomst(string text, decimal overfoertUnderskud);

		void NedbringFremfoertUnderskudForSkattepligtigIndkomst(string text, decimal overfoertUnderskud);

		void TilfoejUnderskudTilFremfoerselForSkattepligtigIndkomst(string text, decimal overfoertUnderskud);
	}
}