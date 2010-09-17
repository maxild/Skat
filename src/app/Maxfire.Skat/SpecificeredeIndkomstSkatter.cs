using Maxfire.Skat.Beregnere;

namespace Maxfire.Skat
{
	// TODO: Skatteloft nedslag i topskatten
	public class SpecificeredeIndkomstSkatter
	{
		public SpecificeredeIndkomstSkatter(
			IndkomstSkatter brutto, 
			IndkomstSkatterAfPersonligIndkomst underskudSkattepligtigIndkomst, 
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
		public IndkomstSkatterAfPersonligIndkomst UnderskudSkattepligtigIndkomst { get; private set; }

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
			// TODO: Skatteloft: - new IndkomstSkatter(topskat: skatteloft);
			get { return Brutto - SkatteUtility.CombineSkat(UnderskudSkattepligtigIndkomst) - Personfradrag; }
		}
	}
}