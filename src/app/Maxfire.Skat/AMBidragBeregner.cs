using Maxfire.Skat.Extensions;

namespace Maxfire.Skat
{
	//////////////////////////////////////////////////////////////////////////////////
	// Du betaler arbejdsmarkedsbidrag af din bruttoindtægt og derfor også af den 
	// del af indtægten, der går til pension. 
	//
	// AM-bidrag trækkes af arbejdsgivere m.fl. i forbindelse med hver udbetaling 
	// af løn, vederlag mv.
	//
	// Derudover trækker pensionsinstitutterne, herunder ATP, AM-bidrag af de beløb, 
	// arbejdsgivere indbetaler til pensionsordninger for de ansatte samt af de 
	// indbetalinger, A-kasser, kommuner m.fl. indbetaler til ATP. Derfor 
	// kan man fradrage eget ATP-bidrag i AM-indkomsten, inden arbejdsgiver betaler
	// det beregnede AM-bidrag.
	//
	// Hvis din arbejdsgiver indbetaler pensionsbidrag, afregner livsforsikrings- 
	// eller pensionsselskabet arbejdsmarkedsbidraget overfor skattevæsenet. Derfor 
	// kan man fradrage eget bidrag til arbejdsgiver administreret ordning i 
	// AM-indkomsten. inden arbejdsgiver betaler det beregnede AM-bidrag.
	//
	// Da pensionsindskud og ATP er langsigtede investeringer, der ikke påvirker 
	// likviditeten i dag (på nær indskuddene), så er det AM-bidrag der går fra i 
	// lønnen mindre end det samlede AM-bidrag, som SKAT modtager, der jo også 
	// indeholder AM-bidrag af indbetalinger til alle pensioner (herunder arbejdsgiver
	// ordninger og ATP).
	//////////////////////////////////////////////////////////////////////////////////
	public class AMBidragBeregner : IAMIndkomstSkatteberegner
	{
		private readonly ISkattelovRegistry _skattelovRegistry;

		public AMBidragBeregner(ISkattelovRegistry skattelovRegistry)
		{
			_skattelovRegistry = skattelovRegistry;
		}

		public ValueTuple<decimal> BeregnSkat(ValueTuple<decimal> amIndkomster, int skatteAar)
		{
			return amIndkomster.Map((decimal amIndkomst) => BeregnSkat(amIndkomst, skatteAar));
		}

		public decimal BeregnSkat(decimal amIndkomst, int skatteAar)
		{
			decimal amBidragSkattesats = _skattelovRegistry.GetAMBidragSkattesats(skatteAar);
			return amBidragSkattesats * amIndkomst.NonNegative();
		}

		public ValueTuple<decimal> BeregnSkat(IValueTuple<IArbejdsmarkedIndkomster> indkomster, int skatteAar)
		{
			return indkomster.Map(indkomst => BeregnSkat(indkomst, skatteAar));
		}

		public decimal BeregnSkat(IArbejdsmarkedIndkomster indkomster, int skatteAar)
		{
			return BeregnSkat(indkomster.AMIndkomst, skatteAar);
		}

		decimal IAMIndkomstSkatteberegner.Beregn(decimal grundlag, int skatteAr)
		{
			return BeregnSkat(grundlag, skatteAr);
		}

		decimal IAMIndkomstSkatteberegner.Beregn(IArbejdsmarkedIndkomster indkomster, int skatteAr)
		{
			return BeregnSkat(indkomster.AMIndkomst, skatteAr);
		}
	}
}