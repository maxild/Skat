namespace Maxfire.Skat
{
	public interface IAktieindkomstRegistry
	{
		/// <summary>
		/// Aflæs laveste progressionsgrænse for beskatning af aktieindkomst for det givne skatteår (PSL § 8 a, stk.1 1 og 2).
		/// </summary>
		/// <param name="skatteAar">Skatteåret</param>
		/// <returns>Laveste progressionsgrænse for beskatning af aktieindkomst for det givne skatteår.</returns>
		decimal GetAktieIndkomstLavesteProgressionsgraense(int skatteAar);

		/// <summary>
		/// Aflæs højeste progressionsgrænse (ophæves fra og med 2010) for beskatning af aktieindkomst for det givne skatteår (PSL § 8 a, stk.1 1 og 2).
		/// </summary>
		/// <param name="skatteAar">Skatteåret</param>
		/// <returns>Laveste progressionsgrænse for beskatning af aktieindkomst for det givne skatteår.</returns>
		decimal GetAktieIndkomstHoejesteProgressionsgraense(int skatteAar);

		/// <summary>
		/// Aflæs skattesats for aktieindkomst på det laveste trin for det givne skatteår (PSL § 8 a, stk. 1 og 2).
		/// </summary>
		/// <param name="skatteAar">Skatteåret</param>
		/// <returns>Skattesats for aktieindkomst for det givne skatteår</returns>
		decimal GetAktieIndkomstLavesteSkattesats(int skatteAar);

		/// <summary>
		/// Aflæs skattesats for aktieindkomst på det mellemste trin for det givne skatteår (PSL § 8 a, stk. 1 og 2).
		/// </summary>
		/// <param name="skatteAar">Skatteåret</param>
		/// <returns>Skattesats for aktieindkomst for det givne skatteår</returns>
		decimal GetAktieIndkomstMellemsteSkattesats(int skatteAar);

		/// <summary>
		/// Aflæs skattesats for aktieindkomst på det øverste trin (ophæves fra og med 2010) for det givne skatteår (PSL § 8 a, stk. 1 og 2).
		/// </summary>
		/// <param name="skatteAar">Skatteåret</param>
		/// <returns>Skattesats for aktieindkomst for det givne skatteår</returns>
		decimal GetAktieIndkomstHoejesteSkattesats(int skatteAar);
	}

	public interface ISkattepligtigIndkomstRegistry
	{
		decimal GetAMBidragSkattesats(int skatteAar);

		decimal GetSundhedsbidragSkattesats(int skatteAar);

		decimal GetBundSkattesats(int skatteAar);

		decimal GetMellemSkattesats(int skatteAar);

		decimal GetMellemskatBundfradrag(int skatteAar);

		decimal GetTopSkattesats(int skatteAar);
		decimal GetTopskatBundfradrag(int skatteAar);
		decimal GetPositivNettoKapitalIndkomstGrundbeloeb(int skatteAar);

		decimal GetPersonfradrag(int skatteAar, int alder, bool gift);
		decimal GetBundLettelseBundfradrag(int skatteAar, int alder, bool gift);
		decimal GetMellemLettelseBundfradrag(int skatteAar);
		decimal GetTopLettelseBundfradrag(int skatteAar);
		decimal GetPersonfradragSkaerpelse(int skatteAar, int alder, bool gift);

		decimal GetSkatteloftSkattesats(int skatteAar);

		decimal GetNegativNettoKapitalIndkomstGrundbeloeb(int skatteAar);
		decimal GetNegativNettoKapitalIndkomstSats(int skatteAar);

		decimal GetBeskaeftigelsesfradragGrundbeloeb(int skatteAar);
		decimal GetBeskaeftigelsesfradragSats(int skatteAar);

		decimal GetGroenCheckPrVoksen(int skatteAar);
		decimal GetGroenCheckPrBarn(int skatteAar);
		decimal GetGroenCheckAftrapningssats(int skatteAar);
		decimal GetGroenCheckBundfradrag(int skatteAar);
	}

	public interface ISkattelovRegistry : IAktieindkomstRegistry, ISkattepligtigIndkomstRegistry
	{
	}
}