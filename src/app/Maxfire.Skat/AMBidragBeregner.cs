namespace Maxfire.Skat
{
	public class AMBidragBeregner
	{
		private readonly ISkattelovRegistry _skattelovRegistry;

		public AMBidragBeregner(ISkattelovRegistry skattelovRegistry)
		{
			_skattelovRegistry = skattelovRegistry;
		}

		public ValueTuple<decimal> BeregnSkat(ValueTuple<decimal> amIndkomster, int skatteAar)
		{
			decimal amBidragSkattesats = _skattelovRegistry.GetAMBidragSkattesats(skatteAar);
			return amBidragSkattesats * amIndkomster;
		}
	}
}