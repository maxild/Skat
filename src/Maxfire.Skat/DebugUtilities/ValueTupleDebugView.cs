using System;
using System.Diagnostics;
using System.Linq;

namespace Maxfire.Skat.DebugUtilities
{
    internal class ValueTupleDebugView<T>
    {
        private readonly IValueTuple<T> _tuple;

        public ValueTupleDebugView (IValueTuple<T> tuple)
        {
            if (tuple == null)
            {
                throw new ArgumentNullException(nameof(tuple));
            }
            _tuple = tuple;
        }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public T[] Items => _tuple.ToArray();

    }
}
