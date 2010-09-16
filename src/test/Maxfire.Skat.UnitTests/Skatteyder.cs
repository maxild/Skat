using System;

namespace Maxfire.Skat.UnitTests
{
	public class Skatteyder : ISkatteyder
	{
		public Skatteyder(DateTime foedselsdato, bool medlemAfFolkekirken)
		{
			Foedselsdato = foedselsdato;
			MedlemAfFolkekirken = medlemAfFolkekirken;
		}

		public Skatteyder(DateTime foedselsdato, bool medlemAfFolkekirken, int antalBoern)
			: this(foedselsdato, medlemAfFolkekirken)
		{
			AntalBoern = antalBoern;
		}

		public bool MedlemAfFolkekirken { get; private set; }

		public DateTime Foedselsdato { get; private set; }

		public int AntalBoern { get; private set; }
	}
}