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

        public decimal FoerAMBidrag { get; }

        public decimal? AMBidrag { get; }

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
