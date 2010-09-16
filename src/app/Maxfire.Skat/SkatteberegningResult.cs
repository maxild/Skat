namespace Maxfire.Skat
{
	public class SkatteberegningResult
	{
		public SkatteberegningResult(
			ISpecificeredeSkatteyder skatteyder,
			ISpecificeredeSkatteIndkomster indkomster, 
			Skatter skatter)
		{
			Skatteyder = skatteyder;
			Indkomster = indkomster;
			Skatter = skatter;
		}

		public ISpecificeredeSkatteyder Skatteyder { get; private set; }

		public ISpecificeredeSkatteIndkomster Indkomster { get; private set; }
		
		// TODO: Hvad med topskattegrundlag?

		// Er dette person fradragene
		//public decimal SkattevaerdiFradragKommuneskat { get; set; }
		//public decimal SkattevaerdiFradragBundskat { get; set; }
		//public decimal SkattevaerdiFradragSundhedsbidrag { get; set; }
		// etc...
		//public decimal SkattevaerdiSkatteloft { get; set; }

		//public decimal GroenCheck { get; set; }

		public Skatter Skatter { get; private set; }

		// TODO: Mangler
		//  - grøn check
		//  - skatteværdier
		//  - Ejendomsværdiskat
		//  - Grundskyld
	}
}