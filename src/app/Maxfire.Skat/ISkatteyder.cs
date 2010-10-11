using System;

namespace Maxfire.Skat
{
	public interface ISkatteyder
	{
		MedlemAfFolkekirken MedlemAfFolkekirken { get; }
		DateTime Foedselsdato { get; }
		AntalBoern AntalBoern { get; }
	}
}