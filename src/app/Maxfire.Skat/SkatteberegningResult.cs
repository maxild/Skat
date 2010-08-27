namespace Maxfire.Skat
{
	public class SkatteberegningResult
	{
		public SkatteberegningResult(IValueTuple<IPersonligeBeloeb> beloeb, ValueTuple<Skatter> skatter)
		{
			Beloeb = beloeb;
			Skatter = skatter;
		}

		public IValueTuple<IPersonligeBeloeb> Beloeb { get; private set; }
		
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