using System;
using Maxfire.Skat.Extensions;

namespace Maxfire.Skat.Beregnere
{
    public abstract class SkattepligtigIndkomstSkatteberegner
    {
        protected ValueTuple<decimal> BeregnSkatCore(
            IValueTuple<ISkattepligtigeIndkomster> indkomster,
            Func<ValueTuple<decimal>> skattesatsProvider)
        {
            var skattepligtigIndkomst = indkomster.Map(x => x.SkattepligtigIndkomstSkattegrundlag).ToValueTuple();
            var skattesats = skattesatsProvider();
            return skattesats * (+skattepligtigIndkomst);
        }
    }
}
