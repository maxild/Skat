using System;

namespace Maxfire.Skat
{
	public interface ISkatteyder
	{
		bool MedlemAfFolkekirken { get; }
		DateTime Foedselsdato { get; }
		int AntalBoern { get; }
	}
}