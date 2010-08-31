using Maxfire.Skat.Extensions;

namespace Maxfire.Skat
{
	public class SundhedsbidragBeregner : SkattepligtigIndkomstSkatteberegner
	{
		private readonly ISkattelovRegistry _skattelovRegistry;

		public SundhedsbidragBeregner(ISkattelovRegistry skattelovRegistry)
		{
			_skattelovRegistry = skattelovRegistry;
		}

		public ValueTuple<decimal> BeregnSkat(IValueTuple<IPersonligeBeloeb> indkomster, int skatteAar)
		{
			return BeregnSkatCore(indkomster,
			                      () => _skattelovRegistry.GetSundhedsbidragSkattesats(skatteAar).ToTupleOfSize(indkomster.Size));
		}
	}
}