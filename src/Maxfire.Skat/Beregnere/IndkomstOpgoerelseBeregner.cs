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
			var specificeredeSelvangivneBeloeb = selvangivneBeloeb.Map(makeSpecificerede);

			var amBidragBeregner = new AMBidragBeregner(_skattelovRegistry);
			var beskaeftigelsesfradragBeregner = new BeskaeftigelsesfradragBeregner(_skattelovRegistry);

			return specificeredeSelvangivneBeloeb.Map(x => 
				new SkatteIndkomster(x, amBidragBeregner, beskaeftigelsesfradragBeregner, skatteAar));
		}

		private static ISpecificeredeSelvangivneBeloeb makeSpecificerede(ISelvangivneBeloeb selvangivneBeloeb)
		{
			var specificeredeSelvangivneBeloeb = selvangivneBeloeb as ISpecificeredeSelvangivneBeloeb;
			return specificeredeSelvangivneBeloeb ?? new DefaultSpecificeredeSelvangivneBeloeb(selvangivneBeloeb);
		}
	}
}