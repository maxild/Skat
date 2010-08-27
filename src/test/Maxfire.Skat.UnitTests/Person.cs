using System;

namespace Maxfire.Skat.UnitTests
{
	public class Person : IPerson
	{
		public Person(DateTime foedselsdato)
		{
			Foedselsdato = foedselsdato;
		}

		public Person(DateTime foedselsdato, int antalBoern)
			: this(foedselsdato)
		{
			AntalBoern = antalBoern;
		}

		public DateTime Foedselsdato { get; private set; }

		public int AntalBoern { get; private set; }
	}
}