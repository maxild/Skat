using Maxfire.Skat.Beregnere;

namespace Maxfire.Skat
{
	public class SpecificeredeIndkomstSkatter : ISumable<decimal>
	{
		public SpecificeredeIndkomstSkatter(
			IndkomstSkatter brutto,
			decimal skatteloftNedslag,
			IndkomstSkatterAfPersonligIndkomst underskudSkattepligtigIndkomst, 
			IndkomstSkatter personfradrag)
		{
			Brutto = brutto;
			SkatteloftNedslag = skatteloftNedslag;
			UnderskudSkattepligtigIndkomst = underskudSkattepligtigIndkomst;
			Personfradrag = personfradrag;
		}

		/// <summary>
		/// Indkomstskatterne før diverse nedslag (modregninger, personfradrag mv.).
		/// </summary>
		public IndkomstSkatter Brutto { get; private set; }

		/// <summary>
		/// Modregnet skatteværdi af underskud i skattepligtig indkomst (negativt beløb såfremt modregninger finder sted).
		/// </summary>
		public IndkomstSkatterAfPersonligIndkomst UnderskudSkattepligtigIndkomst { get; private set; }

		/// <summary>
		/// Modregnet skatteværdi af personfradrag (negativt beløb såfremt personfradrag udnyttes).
		/// </summary>
		public IndkomstSkatter Personfradrag { get; private set; }

		/// <summary>
		/// Nedslag i topskatten som følge af (det skrå) skatteloft (negativt beløb såfremt skatteloft udnyttes).
		/// </summary>
		public decimal SkatteloftNedslag { get; private set; }
		
		/// <summary>
		/// Skatter i alt, dvs. indkomstskatterne efter diverse nedslag (modregninger, personfradrag mv.).
		/// </summary>
		// TODO: Rename to IAlt???
		public IndkomstSkatter Netto
		{
			get
			{
				return Brutto 
					+ SkatteUtility.CombineSkat(UnderskudSkattepligtigIndkomst) 
					+ new IndkomstSkatter(topskat: SkatteloftNedslag) 
					+ Personfradrag;
			}
		}

		public decimal Sum()
		{
			return Netto.Sum();
		}
	}
}