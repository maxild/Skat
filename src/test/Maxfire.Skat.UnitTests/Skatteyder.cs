using System;

namespace Maxfire.Skat.UnitTests
{
	public class Skatteyder : ISkatteyder
	{
		public Skatteyder(DateTime foedselsdato, MedlemAfFolkekirken medlemAfFolkekirken)
		{
			Foedselsdato = foedselsdato;
			MedlemAfFolkekirken = medlemAfFolkekirken;
		}

		public Skatteyder(DateTime foedselsdato, MedlemAfFolkekirken medlemAfFolkekirken, AntalBoern antalBoern)
			: this(foedselsdato, medlemAfFolkekirken)
		{
			AntalBoern = antalBoern;
		}

		public MedlemAfFolkekirken MedlemAfFolkekirken { get; private set; }

		public DateTime Foedselsdato { get; private set; }

		public AntalBoern AntalBoern { get; private set; }
	}
}