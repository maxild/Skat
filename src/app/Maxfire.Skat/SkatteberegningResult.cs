namespace Maxfire.Skat
{
	// TODO: Mangler f�lgende:
	//  - gr�n check
	//  - Ejendomsv�rdiskat
	//  - Grundskyld
	public class SkatteberegningResult
	{
		public SkatteberegningResult(
			ISpecificeredeSkatteyder skatteyder,
			ISpecificeredeSkatteIndkomster indkomster, 
			SpecificeredeIndkomstSkatter indkomstSkatter)
		{
			Skatteyder = skatteyder;
			Indkomster = indkomster;
			IndkomstSkatter = indkomstSkatter;
		}

		/// <summary>
		/// Personoplysninger om skatteyderen.
		/// </summary>
		public ISpecificeredeSkatteyder Skatteyder { get; private set; }

		/// <summary>
		/// Specifikation af indkomstopg�relse.
		/// </summary>
		public ISpecificeredeSkatteIndkomster Indkomster { get; private set; }

		/// <summary>
		/// Specifikation af beregnet skat mv.
		/// </summary>
		public SpecificeredeIndkomstSkatter IndkomstSkatter { get; private set; }
	}
}