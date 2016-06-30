using System.Collections.Generic;
using System.Linq;

namespace Maxfire.Skat.Internal
{
    /// <summary>
    /// Adapter that wraps collection of ITextValuePair&lt;decimal&gt; in an IBeloebCollection instance.
    /// </summary>
    internal class BeloebCollection : TextValuePairCollection<ITextValuePair<decimal>>, IBeloebCollection
    {
        public static readonly BeloebCollection Empty = new BeloebCollection(Enumerable.Empty<ITextValuePair<decimal>>());

        public BeloebCollection(IEnumerable<ITextValuePair<decimal>> items)
            : base(items, item => item.Value)
        {
        }
    }
}
