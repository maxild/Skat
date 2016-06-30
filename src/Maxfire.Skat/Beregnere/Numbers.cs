using System;

namespace Maxfire.Skat.Beregnere
{
    public static class Numbers
    {
        public static T Max<T>(T number, params T[] numbers)
        {
            return Find(number, numbers, Operator<T>.GreaterThan);
        }

        public static T Min<T>(T number, params T[] numbers)
        {
            return Find(number, numbers, Operator<T>.LessThan);
        }

        static T Find<T>(T number, T[] numbers, Func<T, T, bool> predicate)
        {
            if (numbers == null)
                return number;

            T elem = number;

            for (int i = 0; i < numbers.Length; i++)
            {
                if (predicate(numbers[i], elem))
                {
                    elem = numbers[i];
                }
            }

            return elem;
        }
    }
}
