using System;

namespace Maxfire.Skat
{
	public interface IPerson
	{
		DateTime Foedselsdato { get; }
		int AntalBoern { get; }
	}
}