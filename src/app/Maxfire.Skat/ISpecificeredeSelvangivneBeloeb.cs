﻿namespace Maxfire.Skat
{
	public interface ISpecificeredeSelvangivneBeloeb : ISelvangivneBeloeb
	{
		IBeloebCollection PersonligeIndkomsterAMIndkomster { get; }
		IBeloebCollection PersonligeIndkomsterEjAMIndkomster { get; }
		IBeloebCollection PersonligeIndkomster { get; }

		IBeloebCollection KapitalIndkomster { get; }

		IBeloebCollection LigningsmaessigeFradragMinusBeskaeftigelsesfradrag { get; }
	}
}