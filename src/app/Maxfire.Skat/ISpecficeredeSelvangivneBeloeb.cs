namespace Maxfire.Skat
{
	public interface ISpecficeredeSelvangivneBeloeb : ISelvangivneBeloeb
	{
		IBeloebCollection PersonligeIndkomsterAMIndkomster { get; }
		IBeloebCollection PersonligeIndkomsterEjAMIndkomster { get; }
		IBeloebCollection PersonligeIndkomster { get; }

		IBeloebCollection NettoKapitalIndkomster { get; }

		IBeloebCollection LigningsmaessigeFradragMinusBeskaeftigelsesfradrag { get; }
	}
}