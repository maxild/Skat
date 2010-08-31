using System.Collections.Generic;
using System.Linq;
using Maxfire.Core;

namespace Maxfire.Skat
{
	public interface IPersonligeBeloeb
	{
		ISkatteIndkomster Selvangiven { get; }
		ISkatteIndkomster Skattegrundlag { get; }

		decimal FremfoertUnderskudPersonligIndkomst { get; set; } // Hvorfor mutable?
		decimal ModregnetUnderskudPersonligIndkomst { get; set; }
		decimal UnderskudPersonligIndkomstTilFremfoersel { get; set; }

		decimal ModregnetUnderskudNettoKapitalIndkomst { get; set; }

		decimal FremfoertUnderskudSkattepligtigIndkomst { get; set; } // Hvorfor mutable
		decimal ModregnetUnderskudSkattepligtigIndkomst { get; set; }
		decimal UnderskudSkattepligtigIndkomstTilFremfoersel { get; set; }

		decimal ModregnetUnderskudKapitalPensionsindskud { get; set; }
	}

	// Note: Denne klasse er resultat typen på metoden IndkomstOpgoerelseBeregner.BeregnIndkomster
	public class SkatteIndkomster : ISpecificeredeSkatteIndkomster
	{
		private readonly ISpecficeredeSelvangivneBeloeb _selvangivneBeloeb;
		private readonly List<ITextValuePair<PersonligIndkomstValue>> _personligeIndkomster;
		private readonly List<ITextValuePair<decimal>> _kapitalIndkomster;
		private readonly List<ITextValuePair<decimal>> _ligningsmaessigeFradrag;
		private readonly List<ITextValuePair<decimal>> _skattepligtigeIndkomster;

		public SkatteIndkomster(ISpecficeredeSelvangivneBeloeb selvangivneBeloeb)
		{
			_selvangivneBeloeb = selvangivneBeloeb;
		}

		public decimal AMIndkomst
		{
			// Note: Dette er den del af personlig indkomst der vedr. arbejdsmarkedsbidrag (dvs. AM-indkomst, der skal opgøres før modregning)
			get { return 0m; }
		}

		public decimal PersonligIndkomst
		{
			// Note: Dette er personlig indkomst vedr. bund-, mellem og topskat (dvs. efter modregning)
			get { return 0m; }
		}

		public decimal NettoKapitalIndkomst
		{
			// Note: Dette er nettokapitalindkomst vedr. bund-, mellem og topskat (dvs. efter modregning)
			get { return 0m; }
		}

		public decimal LigningsmaessigtFradrag
		{
			// Note: Dette er summen af alle ligningsmæssige fradrag (også beskæftigelsesfradraget)
			get { return 0m; }
		}

		public decimal KapitalPensionsindskud
		{
			// Note: Dette er kapitalpensionsindskud vedr. bund-, mellem og topskat (dvs. efter modregning)
			get { return 0m; }
		}

		public decimal AktieIndkomst
		{
			get { return 0m; }
		}

		public decimal SkattepligtigIndkomst
		{
			// Note: Dette er PersonligIndkomst (før modregning) + NettoKapitalIndkomst (før modregning) - Ligniningsmæssige fradrag
			get { return 0m; }
		}

		public IPersonligIndkomstBeloebCollection PersonligeIndkomsterAMIndkomster
		{
			// Note: AM-indkomster
			get { return null; }
		}

		public IPersonligIndkomstBeloebCollection PersonligeIndkomsterEjAMIndkomster
		{
			// Note: Ej AM-indkomster og modregninger
			get { return null; }
		}

		public IPersonligIndkomstBeloebCollection PersonligeIndkomster
		{
			get { return new PersonligIndkomstBeloebCollection(PersonligeIndkomsterAMIndkomster.Concat(PersonligeIndkomsterEjAMIndkomster)); }
		}

		public IBeloebCollection NettoKapitalIndkomster
		{
			get { return null; }
		}

		public IBeloebCollection LigningsmaessigeFradrag
		{
			get { return null; }
		}

		public IBeloebCollection SkattepligtigIndkomster
		{
			get { return null; }
		}
	}
}