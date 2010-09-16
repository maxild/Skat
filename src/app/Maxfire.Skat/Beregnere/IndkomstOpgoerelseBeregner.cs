using Maxfire.Skat.Extensions;
using Maxfire.Skat.Internal;

namespace Maxfire.Skat.Beregnere
{
	public class IndkomstOpgoerelseBeregner
	{
		private readonly ISkattelovRegistry _skattelovRegistry;

		public IndkomstOpgoerelseBeregner(ISkattelovRegistry skattelovRegistry)
		{
			_skattelovRegistry = skattelovRegistry;
		}

		public IValueTuple<ISpecificeredeSkatteIndkomster> BeregnIndkomster(
			IValueTuple<ISelvangivneBeloeb> selvangivneBeloeb, 
			int skatteAar)
		{
			var specficeredeSelvangivneBeloeb = selvangivneBeloeb.Map(makeSpecificerede);

			var amBidragBeregner = new AMBidragBeregner(_skattelovRegistry);
			var beskaeftigelsesfradragBeregner = new BeskaeftigelsesfradragBeregner(_skattelovRegistry);

			return specficeredeSelvangivneBeloeb.Map(x => 
				new SkatteIndkomster(x, amBidragBeregner, beskaeftigelsesfradragBeregner, skatteAar));
		}

		private static ISpecficeredeSelvangivneBeloeb makeSpecificerede(ISelvangivneBeloeb selvangivneBeloeb)
		{
			var specficeredeSelvangivneBeloeb = selvangivneBeloeb as ISpecficeredeSelvangivneBeloeb;
			return specficeredeSelvangivneBeloeb ?? new DefaultSpecificeredeSelvangivneBeloeb(selvangivneBeloeb);
		}
	}
}