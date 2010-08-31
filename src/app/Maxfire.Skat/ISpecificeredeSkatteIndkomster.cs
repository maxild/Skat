namespace Maxfire.Skat
{
	public interface ISpecificeredeSkatteIndkomster : ISkatteIndkomster
	{
		IPersonligIndkomstBeloebCollection PersonligeIndkomsterAMIndkomster { get; }
		IPersonligIndkomstBeloebCollection PersonligeIndkomsterEjAMIndkomster { get; }
		IPersonligIndkomstBeloebCollection PersonligeIndkomster { get; }

		IBeloebCollection NettoKapitalIndkomster { get; }

		IBeloebCollection LigningsmaessigeFradrag { get; }

		IBeloebCollection SkattepligtigIndkomster { get; }
	}
}