using System.Collections.Generic;
using Maxfire.Core;

namespace Maxfire.Skat
{
	// TODO: Fremførte underskud ikke understøttet
	public interface ISelvangivneBeloeb
	{
		decimal PersonligIndkomstAMIndkomst { get; }
		decimal PersonligIndkomstEjAMIndkomst { get; }

		decimal NettoKapitalIndkomst { get; }
		
		decimal LigningsmaessigtFradragMinusBeskaeftigelsesfradrag { get; }

		decimal KapitalPensionsindskud { get; }

		// TODO: Dette er et kendt fradrag i den personlige indkomst, skal det slettes og klienten have ansvaret for indkomst opgørelsen mht. dette fradrag?
		//decimal PrivatTegnetPensionsindskud { get; }

		decimal AktieIndkomst { get; }
	}

	public interface ISpecficeredeSelvangivneBeloeb : ISelvangivneBeloeb
	{
		IBeloebCollection PersonligeIndkomsterAMIndkomster { get; }
		IBeloebCollection PersonligeIndkomsterEjAMIndkomster { get; }
		IBeloebCollection PersonligeIndkomster { get; }

		IBeloebCollection NettoKapitalIndkomster { get; }

		IBeloebCollection LigningsmaessigeFradragMinusBeskaeftigelsesfradrag { get; }
	}

	public static class SelvangivetBeloeb
	{
		public static ITextValuePair<decimal> Create(string text, decimal beloeb)
		{
			return new TextValuePair<decimal>(text, beloeb);
		}
	}

	public static class SelvangivneBeloeb
	{
		public static IBeloebCollection Create(params ITextValuePair<decimal>[] selvangivneBeloeb)
		{
			return new BeloebCollection(selvangivneBeloeb);
		}
		public static IBeloebCollection Create(IEnumerable<ITextValuePair<decimal>> selvangivneBeloeb)
		{
			return new BeloebCollection(selvangivneBeloeb);
		}
	}
}