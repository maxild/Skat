namespace Maxfire.Skat.Beregnere
{
	public interface IPersonligeIndkomsterModregning : IPersonligeIndkomster
	{
		/// <summary>
		/// Årets underskud i den personlige indkomst.
		/// </summary>
		decimal PersonligIndkomstAaretsUnderskud { get; }
		
		/// <summary>
		/// Fremført underskud i den personlige indkomst.
		/// </summary>
		decimal PersonligIndkomstFremfoertUnderskud { get; }

		/// <summary>
		/// Modregninger i den personlige indkomst, der har betydning for skatteberegningen.
		/// </summary>
		/// <remarks>
		/// Disse modregninger skyldes overførsler mellem ægtefæller 
		/// baseret på negativ personlig indkomst hos en af ægtefællerne.
		/// </remarks>
		decimal PersonligIndkomstModregninger { get; }
		
		/// <summary>
		/// Det uudnyttede underskud i den personlige indkomst, der fremføres til næste skatteår.
		/// </summary>
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
		/// Det udnyttede underskud, der kan rummes i den personlige indkomst.
		/// </param>
		void NedbringPersonligIndkomst(string text, decimal overfoertUnderskud);

		/// <summary>
		/// Nedbring årets underskud i den personlige indkomst med det angivne overførte underskud.
		/// </summary>
		/// <param name="text">Beskrivende tekst.</param>
		/// <param name="overfoertUnderskud">Størrelsen på det underskud, der overføres til andetsteds.</param>
		void NedbringFremfoertUnderskudForPersonligIndkomst(string text, decimal overfoertUnderskud);

		/// <summary>
		/// Fremfør uudnyttet underskud i den personlige indkomst til næste skatteår.
		/// </summary>
		/// <param name="text">Beskrivende tekst.</param>
		/// <param name="uudnyttetUnderskud">Det uudnytttede underskud, der skal fremføres til næste skatteår.</param>
		void FremfoerUnderskudForPersonligIndkomst(string text, decimal uudnyttetUnderskud);

		/// <summary>
		/// 
		/// </summary>
		decimal NettoKapitalIndkomstModregninger { get; }

		/// <summary>
		/// Nedbring nettokapitalindkomst med det angivne overførte underskud.
		/// </summary>
		/// <param name="text">Beskrivende tekst.</param>
		/// <param name="overfoertUnderskud">Det udnyttede underskud, der kan rummes i nettokapitalindkomsten.</param>
		void NedbringNettoKapitalIndkomst(string text, decimal overfoertUnderskud);

		/// <summary>
		/// 
		/// </summary>
		decimal KapitalPensionsindskudModregninger { get; }

		/// <summary>
		/// Nedbring kapitalpensionsindskud med det angivne overførte underskud.
		/// </summary>
		/// <param name="text">Beskrivende tekst.</param>
		/// <param name="overfoertUnderskud">Det udnyttede underskud, der kan rummes i kapitalpensionsindskud.</param>
		void NedbringKapitalPensionsindskud(string text, decimal overfoertUnderskud);
	}
}