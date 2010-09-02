using System.Collections.Generic;
using Maxfire.Core;

namespace Maxfire.Skat
{
	// TODO: Mangler restskat i selvangivne beløb
	// TODO: Mangler nogle 'standard' fradrag i den personlige indkomst såsom iværksætter konto + atp eget bidrag + private tegnet pensionsindskud
	public interface ISelvangivneBeloeb
	{
		decimal PersonligIndkomstAMIndkomst { get; }
		decimal PersonligIndkomstEjAMIndkomst { get; }
		decimal PersonligIndkomstFremfoertUnderskud { get; }

		decimal NettoKapitalIndkomst { get; }
		
		decimal LigningsmaessigtFradragMinusBeskaeftigelsesfradrag { get; }
		
		decimal SkattepligtigIndkomstFremfoertUnderskud { get; }

		decimal KapitalPensionsindskud { get; }

		decimal AktieIndkomst { get; }
	}

	public static class Beloeb
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