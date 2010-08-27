using Maxfire.Core;

namespace Maxfire.Skat.Extensions
{
	public static class NumberExtensions
	{
		public static int Sign<T>(this T number)
		{
			return Operator<T>.Sign(number);
		}

		public static bool DifferentSign<T>(this T lhs, T rhs)
		{
			return lhs.Sign() * rhs.Sign() == -1;
		}

		public static T NonNegative<T>(this T number)
		{
			return Operator<T>.Max(Operator<T>.Zero, number);
		}

		public static T Bund<T>(this T number, T minimalNedreGraense)
		{
			return Operator<T>.Max(number, minimalNedreGraense);
		}

		public static T Loft<T>(this T number, T maksimalOevreGraense)
		{
			return Operator<T>.Min(number, maksimalOevreGraense);
		}

		public static T DifferenceGreaterThan<T>(this T number, T limit)
		{
			return Operator<T>.Subtract(number, limit).NonNegative();
		}
	}
}