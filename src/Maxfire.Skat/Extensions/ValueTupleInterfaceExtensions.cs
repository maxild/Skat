using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Maxfire.Skat.Extensions
{
	public static class ValueTupleInterfaceExtensions
	{
		[DebuggerStepThrough]
		public static ValueTuple<TResult> Map<T, TResult>(
			this IValueTuple<T> tuple,
			Func<T, TResult> projection)
		{
			var list = new List<TResult>(tuple.Size);
			for (int i = 0; i < tuple.Size; i++)
			{
				list.Add(projection(tuple[i]));
			}
			return new ValueTuple<TResult>(list);
		}

		[DebuggerStepThrough]
		public static ValueTuple<TResult> Map<T, TResult>(
			this IValueTuple<T> tuple,
			Func<T, int, TResult> projectionWithIndex)
		{
			var list = new List<TResult>(tuple.Size);
			for (int i = 0; i < tuple.Size; i++)
			{
				list.Add(projectionWithIndex(tuple[i], i));
			}
			return new ValueTuple<TResult>(list);
		}

		[DebuggerStepThrough]
		public static ValueTuple<TItem> MapByIndex<TParent, TItem>(
			this IValueTuple<TParent> tuple,
			Func<int, TItem> projectionWithIndex)
		{
			var list = new List<TItem>(tuple.Size);
			for (int i = 0; i < tuple.Size; i++)
			{
				list.Add(projectionWithIndex(i));
			}
			return new ValueTuple<TItem>(list);
		}

		/// <summary>
		/// Frembring tuple af værdier, der er mindre eller lig med en en angivet loftværdi.
		/// </summary>
		public static ValueTuple<T> Loft<T>(this IValueTuple<T> tuple, T maksimalOevreGraense)
		{
			return tuple.Map(value => value.Loft(maksimalOevreGraense));
		}

		/// <summary>
		/// Frembring tuple af værdier, der er større eller lig med en angivet bundværdi.
		/// </summary>
		public static ValueTuple<T> Bund<T>(this IValueTuple<T> tuple, T minimalNedreGraense)
		{
			return tuple.Map(value => value.Bund(minimalNedreGraense));
		}
	}
}