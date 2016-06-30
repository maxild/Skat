using System;
using System.Collections.Generic;

namespace Maxfire.Skat
{
    // TODO: Maybe have Map as direct method
    public interface IValueTuple<out T> : IEnumerable<T>
    {
        T this[int index] { get; }
        int IndexOf(Func<T, bool> predicate);
        T PartnerOf(int index);
        int Size { get; }
        bool AllZero();
        T Sum();
        IValueTuple<T> Swap();
    }
}
