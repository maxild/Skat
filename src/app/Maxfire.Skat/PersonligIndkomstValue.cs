using Maxfire.Core;

namespace Maxfire.Skat
{
	public class PersonligIndkomstValue
	{
		public static ITextValuePair<PersonligIndkomstValue> Create(string text, decimal foerAMBidrag, decimal? amBidrag = null)
		{
			return new TextValuePair<PersonligIndkomstValue>(text, new PersonligIndkomstValue(foerAMBidrag, amBidrag));
		}

		public PersonligIndkomstValue(decimal foerAMBidrag, decimal? amBidrag = null)
		{
			FoerAMBidrag = foerAMBidrag;
			AMBidrag = amBidrag;
		}

		public decimal FoerAMBidrag { get; private set; }

		public decimal? AMBidrag { get; private set; }

		public decimal EfterAMBidrag
		{
			get
			{
				decimal efterAMBidrag = FoerAMBidrag;
				if (AMBidrag.HasValue)
				{
					efterAMBidrag -= AMBidrag.Value;
				}
				return efterAMBidrag;
			}
		}
	}
}