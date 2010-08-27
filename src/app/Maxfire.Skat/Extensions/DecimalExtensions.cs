using System;

namespace Maxfire.Skat.Extensions
{
	public static class DecimalExtensions
	{
		public static decimal RoundMoney(this decimal value)
		{
			return Math.Round(value, 2, MidpointRounding.ToEven);
		}
	}
}