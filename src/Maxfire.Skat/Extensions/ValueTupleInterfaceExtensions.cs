using System.Collections.Generic;
using System.Diagnostics;

namespace Maxfire.Skat.Extensions
{
    public static class ValueTupleInterfaceExtensions
    {
        [DebuggerStepThrough]
        public static ValueTuple<T> ToValueTuple<T>(
            this IEnumerable<T> values)
        {
            return new ValueTuple<T>(values);
        }

        /// <summary>
        /// Frembring tuple af værdier, der er mindre eller lig med en en angivet loftværdi.
        /// </summary>
        public static ValueTuple<T> Loft<T>(this IValueTuple<T> tuple, T maksimalOevreGraense)
        {
            return tuple.Map(value => value.Loft(maksimalOevreGraense)).ToValueTuple();
        }

        /// <summary>
        /// Frembring tuple af værdier, der er større eller lig med en angivet bundværdi.
        /// </summary>
        public static ValueTuple<T> Bund<T>(this IValueTuple<T> tuple, T minimalNedreGraense)
        {
            return tuple.Map(value => value.Bund(minimalNedreGraense)).ToValueTuple();
        }
    }
}
