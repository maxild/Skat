namespace Maxfire.Skat
{
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

		public SpecificeredeIndkomstSkatter IndkomstSkatter { get; private set; }

		// TODO: Mangler
		//  - grøn check
		//  - skatteværdier
		//  - Ejendomsværdiskat
		//  - Grundskyld
	}

	public class SpecificeredeIndkomstSkatter
	{
		public SpecificeredeIndkomstSkatter(
			IndkomstSkatter brutto, 
			IndkomstSkatter underskudSkattepligtigIndkomst, 
			IndkomstSkatter personfradrag)
		{
			Brutto = brutto;
			UnderskudSkattepligtigIndkomst = underskudSkattepligtigIndkomst;
			Personfradrag = personfradrag;
		}

		/// <summary>
		/// Indkomstskatterne før diverse nedslag (modregninger, personfradrag mv.).
		/// </summary>
		public IndkomstSkatter Brutto { get; private set; }

		/// <summary>
		/// Modregnet skatteværdi af underskud i skattepligtig indkomst.
		/// </summary>
		public IndkomstSkatter UnderskudSkattepligtigIndkomst { get; private set; }

		/// <summary>
		/// Modregnet skatteværdi af personfradrag (negativt beløb såfremt personfradrag udnyttes).
		/// </summary>
		public IndkomstSkatter Personfradrag { get; private set; }

		/// <summary>
		/// Skatter i alt, dvs. indkomstskatterne efter diverse nedslag (modregninger, personfradrag mv.).
		/// </summary>
		// TODO: Rename to IAlt???
		public IndkomstSkatter Netto
		{
			get { return Brutto - UnderskudSkattepligtigIndkomst - Personfradrag; }
		}
	}
}