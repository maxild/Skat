using System;
using System.Collections.Generic;
using System.Linq;
using Maxfire.Core;

namespace Maxfire.Skat.Extensions
{
	public static class ValueTupleExtensions
	{
		public static ValueTuple<T> ToTuple<T>(this T value)
		{
			return new ValueTuple<T>(value);
		}

		public static ValueTuple<T> ToTuple<T>(this T first, T second)
		{
			return new ValueTuple<T>(first, second);
		}

		public static ValueTuple<T> ToTuple<T>(this IEnumerable<T> coll)
		{
			return new ValueTuple<T>(coll.ToList());
		}

		public static ValueTuple<T> ToTupleOfSize<T>(this T value, int size)
		{
			return new ValueTuple<T>(size, () => value);
		}

		public static ValueTuple<decimal> ToUnitTuple(this int index)
		{
			return index == 0 ? new ValueTuple<decimal>(1, 0) : new ValueTuple<decimal>(0, 1);
		}

		/// <summary>
		/// Har tuple værdier modsat fortegn?
		/// </summary>
		public static bool DifferentSign<T>(this ValueTuple<T> tuple)
		{
			if (tuple.Size == 1)
			{
				return false;
			}

			return Operator<T>.Sign(tuple[0]) * Operator<T>.Sign(tuple[1]) == -1;
		}

		/// <summary>
		/// Hvis tuple består af to elementer med forskelligt fortegn, så nedbringes det positive element
		/// med størrelsen af det negative element, dog under hensyn til at det nedbragte element ikke må 
		/// blive negativt.
		/// </summary>
		public static ValueTuple<T> NedbringPositivtMedEvtNegativt<T>(this ValueTuple<T> tuple)
		{
			if (tuple.DifferentSign() == false)
			{
				return tuple;
			}

			T first, second;
			if (Operator<T>.LessThanZero(tuple[0]))
			{
				T delta = Operator<T>.Min(Operator<T>.Negate(tuple[0]), tuple[1]);
				first = Operator<T>.Add(tuple[0], delta);
				second = Operator<T>.Subtract(tuple[1], delta);
			}
			else
			{
				T delta = Operator<T>.Min(tuple[0], Operator<T>.Negate(tuple[1]));
				first = Operator<T>.Subtract(tuple[0], delta);
				second = Operator<T>.Add(tuple[1], delta);
			}

			return new ValueTuple<T>(first, second);
		}

		/// <summary>
		/// Hvis tuple (bruttoGrundlag) består af to elementer, hvor det ene element er mindre end bundfradraget, og 
		/// det andet element er større end bundfradraget, så er det muligt at overføre det uudnyttede bundfradrag 
		/// fra den ene ægtefælle til den anden ægtefælle. Metoden returnerer det modregnede nettogrundlag, hvor 
		/// bruttogrundlaget er nedbragt med ægtefællernes 'sambeskattede' bundfradrag, efter der evt. er sket overførsel 
		/// af bundfradrag mellem ægtefællerne.
		/// </summary>
		/// <remarks>
		/// Dette princip bliver benyttet ved beregning af mellemskattegrundlaget og topskattegrundlaget.
		/// </remarks>
		public static ValueTuple<decimal> NedbringMedSambeskattetBundfradrag(
			this ValueTuple<decimal> bruttoGrundlag, 
			decimal bundfradrag)
		{
			var nettoGrundlag = bruttoGrundlag - bundfradrag;
			var modregnetNettoGrundlag = nettoGrundlag.NedbringPositivtMedEvtNegativt();
			return modregnetNettoGrundlag;
		}

		/// <summary>
		/// Hvis tuple (bruttoGrundlag) består af to elementer, hvor det ene element er mindre end bundfradraget, og 
		/// det andet element er større end bundfradraget, så er det muligt at overføre det uudnyttede bundfradrag 
		/// fra den ene ægtefælle til den anden ægtefælle. Metoden returnerer det 'effektive' bundfradrag, der summer 
		/// til personernes samlede bundfradrag, men hvor sådan overførsel har fundet sted.
		/// </summary>
		/// <remarks>
		/// Dette princip bliver benyttet ved beregning af mellemskattegrundlaget og topskattegrundlaget.
		/// </remarks>
		public static ValueTuple<decimal> BeregnSambeskattetBundfradrag(
			this ValueTuple<decimal> bruttoGrundlag, 
			decimal bundfradrag)
		{
			var modregnetNettoGundlag = bruttoGrundlag.NedbringMedSambeskattetBundfradrag(bundfradrag);
			var overfoertBundfradrag = bruttoGrundlag - modregnetNettoGundlag;
			return overfoertBundfradrag;
		}

		/// <summary>
		/// Beregn den del af en tuple af underskud, der kan rummes i en tuple af beløb.
		/// </summary>
		/// <param name="indkomster">De beløb, der skal rumme underskuddene</param>
		/// <param name="underskud">De underskud, der skal rummes i beløbene</param>
		/// <returns>Den del af underskuddet, der kan rummes i beløbene uden at beløbene ved modregning bliver negative.</returns>
		public static ValueTuple<decimal> GetMuligeModregninger(
			this ValueTuple<decimal> indkomster, 
			ValueTuple<decimal> underskud)
		{
			return indkomster.Map((beloeb, index) => Math.Min(beloeb, underskud[index]).NonNegative());
		}

		public static ValueTuple<ModregnIndkomstResult> ModregnUnderskud(
			this ValueTuple<decimal> indkomster, 
			ValueTuple<decimal> underskud)
		{
			var modregninger = indkomster.GetMuligeModregninger(underskud);
			return modregninger.Map((modregning, index) => new ModregnIndkomstResult(underskud[index], indkomster[index], modregning));
		}

		/// <summary>
		/// Øre-afrunding af tuple af beløb.
		/// </summary>
		public static ValueTuple<decimal> RoundMoney(this ValueTuple<decimal> tuple)
		{
			IList<decimal> list = new List<decimal>(tuple.Size);
			for (int i = 0; i < tuple.Size; i++)
			{
				list.Add(Math.Round(tuple[i], 2, MidpointRounding.ToEven));
			}
			return new ValueTuple<decimal>(list);
		}

		/// <summary>
		/// Frembring tuple af ikke negative værdier af forskelle, der er større end en angivet grænseværdi.
		/// </summary>
		public static ValueTuple<T> DifferenceGreaterThan<T>(this ValueTuple<T> tuple, T limit)
		{
			return tuple.DifferenceGreaterThan(limit.ToTupleOfSize(tuple.Size));
		}

		/// <summary>
		/// Frembring tuple af ikke negative værdier af forskelle, der er større end en angivet grænseværdi.
		/// </summary>
		public static ValueTuple<T> DifferenceGreaterThan<T>(this ValueTuple<T> tuple, ValueTuple<T> limits)
		{
			return +(tuple - limits);
		}

		public static ValueTuple<T> NormalizeAndele<T>(this ValueTuple<T> andele)
		{
			return andele / andele.Sum();
		}
	}
}