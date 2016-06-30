using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Maxfire.Skat
{
    public static class PreludeExtensions
    {
        [DebuggerStepThrough]
        public static IEnumerable Each(this IEnumerable values, Action<object> eachAction)
        {
            // ReSharper disable PossibleMultipleEnumeration
            foreach (var item in values)
            {
                eachAction(item);
            }

            return values;
            // ReSharper restore PossibleMultipleEnumeration
        }

        [DebuggerStepThrough]
        public static IEnumerable Each(this IEnumerable values, Action<object, int> eachAction)
        {
            int i = 0;
            // ReSharper disable PossibleMultipleEnumeration
            foreach (var item in values)
            {
                eachAction(item, i);
                i++;
            }

            return values;
            // ReSharper restore PossibleMultipleEnumeration
        }

        [DebuggerStepThrough]
        public static IEnumerable<T> Each<T>(this IEnumerable<T> values, Action<T> eachAction)
        {
            // ReSharper disable PossibleMultipleEnumeration
            foreach (var item in values)
            {
                eachAction(item);
            }

            return values;
            // ReSharper restore PossibleMultipleEnumeration
        }

        [DebuggerStepThrough]
        public static IEnumerable<T> Each<T>(this IEnumerable<T> values, Action<T, int> eachAction)
        {
            int i = 0;
            // ReSharper disable PossibleMultipleEnumeration
            foreach (var item in values)
            {
                eachAction(item, i);
                i++;
            }

            return values;
            // ReSharper restore PossibleMultipleEnumeration
        }

        [DebuggerStepThrough]
        public static IEnumerable<TResult> Map<T, TResult>(this IEnumerable<T> values, Func<T, TResult> projection)
        {
            // ReSharper disable LoopCanBeConvertedToQuery
            foreach (T item in values)
                // ReSharper restore LoopCanBeConvertedToQuery
            {
                yield return projection(item);
            }
        }

        [DebuggerStepThrough]
        public static IEnumerable<TResult> Map<T, TResult>(this IEnumerable<T> values, Func<T, int, TResult> projectionWithIndex)
        {
            int index = 0;
            foreach (T item in values)
            {
                yield return projectionWithIndex(item, index);
                index++;
            }
        }
    }
}
