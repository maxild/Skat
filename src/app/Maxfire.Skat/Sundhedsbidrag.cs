using Maxfire.Skat.Extensions;

namespace Maxfire.Skat
{
	public class SundhedsbidragBeregner
	{
		private readonly ISkattelovRegistry _skattelovRegistry;

		public SundhedsbidragBeregner(ISkattelovRegistry skattelovRegistry)
		{
			_skattelovRegistry = skattelovRegistry;
		}

		public ValueTuple<decimal> BeregnSkat(IValueTuple<IPersonligeBeloeb> indkomster, int skatteAar)
		{
			var skattepligtigIndkomst = indkomster.Map(x => x.Skattegrundlag.SkattepligtigIndkomst);
			return _skattelovRegistry.GetSundhedsbidragSkattesats(skatteAar) * (+skattepligtigIndkomst);
		}
	}
}