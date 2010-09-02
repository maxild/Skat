namespace Maxfire.Skat
{
	public interface ISpecificeredeSkatteIndkomster : ISkatteIndkomsterModregning
	{
		IPersonligIndkomstBeloebCollection PersonligeIndkomster { get; }

		IBeloebCollection KapitalIndkomster { get; }

		IBeloebCollection LigningsmaessigeFradrag { get; }

		IBeloebCollection SkattepligtigIndkomster { get; }
	}
}