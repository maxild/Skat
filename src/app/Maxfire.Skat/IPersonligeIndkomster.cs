namespace Maxfire.Skat
{
	public interface IPersonligeIndkomster
	{
		decimal AMIndkomst { get; }

		decimal PersonligIndkomstSkattegrundlag { get; }
		
		decimal NettoKapitalIndkomstSkattegrundlag { get; }
		
		decimal LigningsmaessigtFradrag { get; }
		
		decimal KapitalPensionsindskudSkattegrundlag { get; }

		decimal AktieIndkomst { get; }
	}

	public interface IPersonligeIndkomsterModregning : IPersonligeIndkomster
	{
		decimal PersonligIndkomstAaretsUnderskud { get; }
		decimal PersonligIndkomstFremfoertUnderskud { get; }
		decimal PersonligIndkomstUnderskudTilFremfoersel { get; }

		/// <summary>
		/// Nedbring underskuddet i den personlige indkomst med det angvne overførte underskud.
		/// </summary>
		/// <param name="text">Beskrivende tekst.</param>
		/// <param name="overfoertUnderskud">
		/// Det udnyttede underskud, der kan overføres og rummes i den personlige indkomst _andetsteds_.
		/// </param>
		void NedbringUnderskudForPersonligIndkomst(string text, decimal overfoertUnderskud);

		/// <summary>
		/// Nedbring den personlige indkomst med det angivne overførte underskud.
		/// </summary>
		/// <param name="text">Beskrivende tekst.</param>
		/// <param name="overfoertUnderskud">
		/// Det udnyttede underskud, der kan overføres og rummes i den personlige indkomst.
		/// </param>
		void NedbringPersonligIndkomst(string text, decimal overfoertUnderskud);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="text"></param>
		/// <param name="overfoertUnderskud"></param>
		void NedbringFremfoertUnderskudForPersonligIndkomst(string text, decimal overfoertUnderskud);

		/// <summary>
		/// Tilføj uudnyttet underskud til næste skatteår.
		/// </summary>
		/// <param name="text">Beskrivende tekst.</param>
		/// <param name="overfoertUnderskud">Underskud der skal fremføres til næste skatteår.</param>
		void TilfoejUnderskudTilFremfoerselForPersonligIndkomst(string text, decimal overfoertUnderskud);

		void NedbringNettoKapitalIndkomst(string text, decimal overfoertUnderskud);

		void NedbringKapitalPensionsindskud(string text, decimal overfoertUnderskud);
	}
}