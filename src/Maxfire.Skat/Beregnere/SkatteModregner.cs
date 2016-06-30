using System;
using Maxfire.Skat.Extensions;
using Maxfire.Skat.Reflection;

namespace Maxfire.Skat.Beregnere
{
    public class SkatteModregner<TSkatter>
        where TSkatter : ISumable<decimal>, new()
    {
        private readonly Accessor<TSkatter, decimal>[] _accessors;

        public SkatteModregner(params Accessor<TSkatter, decimal>[] accessors)
        {
            if (accessors == null)
            {
                throw new ArgumentNullException(nameof(accessors));
            }
            if (accessors.Length == 0)
            {
                throw new ArgumentException("At least one accessor must be given.");
            }
            _accessors = accessors;
        }

        public Accessor<TSkatter, decimal> FirstAccessor()
        {
            return _accessors[0];
        }

        public ValueTuple<ModregnSkatterResult<TSkatter>> Modregn(ValueTuple<TSkatter> skatter, ValueTuple<decimal> skattevaerdier)
        {
            return skatter.Map((skat, index) => Modregn(skat, skattevaerdier[index])).ToValueTuple();
        }

        /// <summary>
        /// Beregner modregnede skatter og udnyttede skatteværdier efter modregning af den angivne skatteværdi i skatterne.
        /// </summary>
        /// <param name="skatter">De skatter der skal modregnes skatteværdi i.</param>
        /// <param name="skattevaerdi">Den skatteværdi, der skal modregnes i skatterne.</param>
        /// <returns>Modregnede skatter, udnyttede skatteværdier og fordelingen mellem udnyttet og ikke udnyttet skatteværdi.</returns>
        public ModregnSkatterResult<TSkatter> Modregn(TSkatter skatter, decimal skattevaerdi)
        {
            var modregninger = BeregnModregninger(skatter, skattevaerdi);
            return new ModregnSkatterResult<TSkatter>(skatter, skattevaerdi.NonNegative(), modregninger);
        }

        /// <summary>
        /// Beregner mulige modregninger af den angivne skatteværdi i skatterne.
        /// </summary>
        /// <param name="skatter">De skatter der skal modregnes skatteværdi i</param>
        /// <param name="skattevaerdier">Den skatteværdi, der skal modregnes i skatterne</param>
        /// <returns>De mulige modregninger, der også angiver de udnyttede skatteværdier.</returns>
        public ValueTuple<TSkatter> BeregnModregninger(ValueTuple<TSkatter> skatter, ValueTuple<decimal> skattevaerdier)
        {
            return skatter.Map((skat, index) => BeregnModregninger(skat, skattevaerdier[index])).ToValueTuple();
        }

        /// <summary>
        /// Beregner mulige modregninger af den angivne skatteværdi i skatterne.
        /// </summary>
        /// <param name="skatter">De skatter der skal modregnes skatteværdi i</param>
        /// <param name="skattevaerdi">Den skatteværdi, der skal modregnes i skatterne</param>
        /// <returns>De mulige modregninger, der også angiver de udnyttede skatteværdier.</returns>
        public TSkatter BeregnModregninger(TSkatter skatter, decimal skattevaerdi)
        {
            var modregninger = new TSkatter();

            if (skattevaerdi <= 0)
            {
                return modregninger;
            }

            for (int i = 0; i < _accessors.Length && skattevaerdi > 0; i++)
            {
                var accessor = _accessors[i];
                decimal skat = accessor.GetValue(skatter);
                decimal modregning = accessor.GetValue(modregninger);
                decimal modregningAfSkattevaerdi = Math.Min(skat, skattevaerdi);
                accessor.SetValue(modregninger, modregning + modregningAfSkattevaerdi);
                skattevaerdi -= modregningAfSkattevaerdi;
            }

            return modregninger;
        }
    }
}
