using System;

namespace Maxfire.Skat
{
	public interface IPerson
	{
		DateTime Foedselsdato { get; }
		int AntalBoern { get; }
	}

	public interface ISpecificeredePerson : IPerson
	{
		string Skattekommune { get; }
		bool Gift { get; }
		bool MedlemAfFolkekirken { get; }
	}
}