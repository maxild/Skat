using System;

namespace Maxfire.Skat.Extensions
{
	public static class PersonExtensions
	{
		/// <summary>
		/// Beregn personens alder ved indkomstårets udløb.
		/// </summary>
		/// <param name="person"></param>
		/// <param name="skatteAar">Indkomståret.</param>
		/// <returns>Alder ved indkomstårets udløb.</returns>
		public static int GetAlder(this IPerson person, int skatteAar)
		{
			DateTime indkomstAaretsUdloeb = new DateTime(skatteAar, 12, 31);
			return person.GetAlder(indkomstAaretsUdloeb);
		}

		public static int GetAlder(this IPerson person, DateTime atDate)
		{
			return atDate.Year - person.Foedselsdato.Year - (person.Foedselsdato.DayOfYear < atDate.DayOfYear ? 0 : 1);
		}
	}
}