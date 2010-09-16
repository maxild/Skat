namespace Maxfire.Skat.Beregnere
{
	public interface ISkattepligtigeIndkomster
	{
		/// <summary>
		/// Skattepligtig indkomst, , der bygger på de selvangivne beløb (dvs. før modregning).
		/// </summary>
		decimal SkattepligtigIndkomst { get; }
		
		/// <summary>
		/// Skattepligtig indkomst vedr beregning af kommune- og kirkeskat samt sundhedsbidrag.
		/// </summary>
		decimal SkattepligtigIndkomstSkattegrundlag { get; }
	}

	public interface ISkattepligtigeIndkomsterModregning : ISkattepligtigeIndkomster
	{
		/// <summary>
		/// Årets underskud i den skattepligtige indkomst.
		/// </summary>
		decimal SkattepligtigIndkomstAaretsUnderskud { get; }
		
		/// <summary>
		/// Fremført underskud i den skattepligtige indkomst.
		/// </summary>
		decimal SkattepligtigIndkomstFremfoertUnderskud { get; }
		
		// TODO: Er fortegnskonventionen god nok her?
		/// <summary>
		/// De samlede modregninger i den skattepligtige indkomst.
		/// </summary>
		/// <remarks>
		/// Et negativt tal betyder at den skattepligtige indkomst har kunne rumme enten 
		/// et fremført underskud fra tidligere skatteår eller ægtefællens underskud.
		/// Et positivt tal betyder at negativ skattepligtig indkomst er nulstillet via enten
		/// overførsel til ægtefælle eller fremførsel til næste skatteår.
		/// </remarks>
		decimal SkattepligtigIndkomstModregninger { get; }
		
		/// <summary>
		/// Det uudnyttede restunderskud i den skattepligtige til fremførsel til næste skatteår.
		/// </summary>
		decimal SkattepligtigIndkomstUnderskudTilFremfoersel { get; }

		/// <summary>
		/// Nedbring underskuddet i den skattepligtige indkomst med det angvne overførte underskud.
		/// </summary>
		/// <param name="text">Beskrivende tekst.</param>
		/// <param name="overfoertUnderskud">
		/// Størrelsen på det underskud, der overføres til andetsteds.
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

		/// <summary>
		/// Nedbring årets underskud i den skattepligtige indkomst med det angivne overførte underskud.
		/// </summary>
		/// <param name="text">Beskrivende tekst.</param>
		/// <param name="overfoertUnderskud">
		/// Størrelsen på det underskud, der overføres til andetsteds.
		/// </param>
		void NedbringFremfoertUnderskudForSkattepligtigIndkomst(string text, decimal overfoertUnderskud);

		/// <summary>
		/// Fremfør uudnyttet underskud i den skattepligtige indkomst til næste skatteår.
		/// </summary>
		/// <param name="text">Beskrivende tekst.</param>
		/// <param name="uudnyttetUnderskud">Det uudnyttede underskud, der skal fremføres til næste skatteår.</param>
		void FremfoerUnderskudForSkattepligtigIndkomst(string text, decimal uudnyttetUnderskud);
	}
}