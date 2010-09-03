namespace Maxfire.Skat
{
	public class SkatteberegningResult
	{
		public SkatteberegningResult(
			IValueTuple<ISpecificeredePerson> personer,
			IValueTuple<ISpecificeredeSkatteIndkomster> indkomster, 
			ValueTuple<Skatter> skatter)
		{
			Personer = personer;
			Indkomster = indkomster;
			Skatter = skatter;
		}

		public IValueTuple<ISpecificeredePerson> Personer { get; private set; }

		public IValueTuple<ISpecificeredeSkatteIndkomster> Indkomster { get; private set; }
		
		// TODO: Hvad med topskattegrundlag?

		// Er dette person fradragene
		//public decimal SkattevaerdiFradragKommuneskat { get; set; }
		//public decimal SkattevaerdiFradragBundskat { get; set; }
		//public decimal SkattevaerdiFradragSundhedsbidrag { get; set; }
		// etc...
		//public decimal SkattevaerdiSkatteloft { get; set; }

		//public decimal GroenCheck { get; set; }

		public ValueTuple<Skatter> Skatter { get; private set; }

		// TODO: Mangler
		//  - grøn check
		//  - skatteværdier
		//  - Ejendomsværdiskat
		//  - Grundskyld
	}
}