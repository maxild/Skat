namespace Maxfire.Skat
{
	// TODO: Mangler følgende:
	//  - grøn check
	//  - Ejendomsværdiskat
	//  - Grundskyld
	public class SkatteberegningResult
	{
		public SkatteberegningResult(
			int skatteAar,
			ISpecificeredeSkatteyder skatteyder,
			ISpecificeredeSkatteIndkomster indkomster, 
			SpecificeredeIndkomstSkatter indkomstSkatter)
		{
			SkatteAar = skatteAar;
			Skatteyder = skatteyder;
			Indkomster = indkomster;
			IndkomstSkatter = indkomstSkatter;
		}

		public int SkatteAar { get; private set; }

		/// <summary>
		/// Personoplysninger om skatteyderen.
		/// </summary>
		public ISpecificeredeSkatteyder Skatteyder { get; private set; }

		/// <summary>
		/// Specifikation af indkomstopgørelse.
		/// </summary>
		public ISpecificeredeSkatteIndkomster Indkomster { get; private set; }

		/// <summary>
		/// Specifikation af beregnet skat mv.
		/// </summary>
		public SpecificeredeIndkomstSkatter IndkomstSkatter { get; private set; }
	}
}