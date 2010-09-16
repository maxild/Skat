using System;

namespace Maxfire.Skat.Extensions
{
	public static class PersonExtensions
	{
		/// <summary>
		/// Beregn personens alder ved indkomstårets udløb.
		/// </summary>
		/// <param name="skatteyder"></param>
		/// <param name="skatteAar">Indkomståret.</param>
		/// <returns>Alder ved indkomstårets udløb.</returns>
		public static int GetAlder(this ISkatteyder skatteyder, int skatteAar)
		{
			DateTime indkomstAaretsUdloeb = new DateTime(skatteAar, 12, 31);
			return skatteyder.GetAlder(indkomstAaretsUdloeb);
		}

		public static int GetAlder(this ISkatteyder skatteyder, DateTime atDate)
		{
			return atDate.Year - skatteyder.Foedselsdato.Year - (skatteyder.Foedselsdato.DayOfYear < atDate.DayOfYear ? 0 : 1);
		}
	}
}