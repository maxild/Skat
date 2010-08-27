namespace Maxfire.Skat
{
	// TODO: Hvordan returneres indkomst opgørelsen inklusing AMBidrag og beskæftigelsesfradrag?
	//public class PersonligIndkomstValue
	//{
	//    public PersonligIndkomstValue(decimal value)
	//    {
	//        FoerAMBidrag = value;
	//        AMBidrag = null;
	//    }
	//    public PersonligIndkomstValue(decimal foerAMBidrag, decimal? amBidrag)
	//    {
	//        FoerAMBidrag = foerAMBidrag;
	//        AMBidrag = amBidrag;
	//    }

	//    public decimal FoerAMBidrag { get; private set; }
		
	//    public decimal? AMBidrag { get; private set; }
		
	//    public decimal EfterAMBidrag { 
	//        get
	//        {
	//            decimal efterAMBidrag = FoerAMBidrag;
	//            if (AMBidrag.HasValue)
	//            {
	//                efterAMBidrag -= AMBidrag.Value;
	//            }
	//            return efterAMBidrag;				
	//        }
	//    }
	//}

	/// <summary>
	/// Input der varierer pr. person (personlig indkomst, nettokapitalindkomst, 
	/// skattepligtig indkomst, ligningsmæssige fradrag, aktieindkomst, kapitalpensionsindskud)
	/// </summary>
	internal class PersonligeBeloeb : IPersonligeBeloeb
	{
		public PersonligeBeloeb(ISelvangivneBeloeb selvangivneBeloeb, decimal amBidrag, decimal beskaeftigelsesfradrag)
		{
			SelvangivneBeloeb = selvangivneBeloeb;
			AMBidrag = amBidrag;
			Beskaeftigelsesfradrag = beskaeftigelsesfradrag;
		}

		public ISelvangivneBeloeb SelvangivneBeloeb { get; private set; }

		/// <summary>
		/// Den del af den personlige indkomst, der skal betales arbejdsmarkedsbidrag af.
		/// </summary>
		public decimal PersonligIndkomstAMIndkomst
		{
			get { return SelvangivneBeloeb.PersonligIndkomstAMIndkomst; }
		}

		/// <summary>
		/// Personlig indkomst før betaling af AM-bidrag mv.
		/// </summary>
		public decimal PersonligIndkomstFoerAMBidrag
		{
			get 
			{ 
				return SelvangivneBeloeb.PersonligIndkomstAMIndkomst 
				+ SelvangivneBeloeb.PersonligIndkomstEjAMIndkomst 
				- SelvangivneBeloeb.FradragPersonligIndkomst; 
			}
		}

		/// <summary>
		/// AM-bidrag i alt
		/// </summary>
		public decimal AMBidrag { get; private set; }

		/// <summary>
		/// Personlig indkomst efter betaling af AM-bidrag mv.
		/// </summary>
		public decimal PersonligIndkomst 
		{
			get { return PersonligIndkomstFoerAMBidrag - AMBidrag; }
		}

		private SkatteIndkomster _selvangiven;
		public ISkatteIndkomster Selvangiven
		{
			get
			{
				return _selvangiven ?? (_selvangiven = new SkatteIndkomster(
				                                       	PersonligIndkomstAMIndkomst,
				                                       	PersonligIndkomst,
				                                       	NettoKapitalIndkomst,
														SelvangivneBeloeb.LigningsmaessigeFradragMinusBeskaeftigelsesfradrag,
				                                       	SkattepligtigIndkomst,
				                                       	KapitalPensionsindskud,
				                                       	AktieIndkomst));
			}
		}

		private SkatteIndkomster _skattegrundlag;
		public ISkatteIndkomster Skattegrundlag
		{
			get
			{
				return _skattegrundlag ?? (_skattegrundlag = new SkatteIndkomster(
				                                             	PersonligIndkomstAMIndkomst,
				                                             	PersonligIndkomstSkattegrundlag,
				                                             	NettoKapitalIndkomstSkattegrundlag,
																LigningsmaessigeFradrag,
				                                             	SkattepligtigIndkomstSkattegrundlag,
				                                             	KapitalPensionsindskudSkattegrundlag,
				                                             	AktieIndkomst));
			}
		}

		public decimal FremfoertUnderskudPersonligIndkomst { get; set; }

		private decimal _modregnetUnderskudPersonligIndkomst;
		public decimal ModregnetUnderskudPersonligIndkomst
		{
			get { return _modregnetUnderskudPersonligIndkomst; }
			set
			{
				_modregnetUnderskudPersonligIndkomst = value;
				_skattegrundlag = null;
			}
		}

		/// <summary>
		/// Underskud i personlig indkomst til fremførsel i efterfølgende skatteår.
		/// </summary>
		public decimal UnderskudPersonligIndkomstTilFremfoersel { get; set; }
		
		/// <summary>
		/// Personligindkomst der bliver brugt i skattegrundlaget for bundskat, mellemskat og topskat.
		/// </summary>
		public decimal PersonligIndkomstSkattegrundlag 
		{
			get { return PersonligIndkomst - ModregnetUnderskudPersonligIndkomst; }
		}

		private decimal _modregnetUnderskudNettoKapitalIndkomst;
		/// <summary>
		/// Modregning af underskud i personlig indkomst i nettokapitalindkomsten.
		/// </summary>
		public decimal ModregnetUnderskudNettoKapitalIndkomst
		{
			get { return _modregnetUnderskudNettoKapitalIndkomst; }
			set
			{
				_modregnetUnderskudNettoKapitalIndkomst = value;
				_skattegrundlag = null;
			}
		}

		public decimal NettoKapitalIndkomst
		{
			get { return SelvangivneBeloeb.KapitalIndkomst - SelvangivneBeloeb.FradragKapitalIndkomst; }
		}
		
		/// <summary>
		/// Nettokapitalindkomst der bliver brugt i skattegrundlaget for bundskat, mellemskat og topskat.
		/// </summary>
		public decimal NettoKapitalIndkomstSkattegrundlag
		{
			get { return NettoKapitalIndkomst - ModregnetUnderskudNettoKapitalIndkomst; }
		}

		/// <summary>
		/// Et fremført underskud i skattepligtig indkomst fra tidligere indkomstår.
		/// </summary>
		public decimal FremfoertUnderskudSkattepligtigIndkomst { get; set; }

		private decimal _modregnetUnderskudSkattepligtigIndkomst;
		/// <summary>
		/// Modregning af underskud i skattepligtig indkomst i årets positive skattepligtige indkomst.
		/// </summary>
		/// <remarks>
		/// Der er tre årsager til modregning i den positive skattepligtige indkomst:
		///    1) Årets underskud, der er overført og modregnet i ægtefælles positive skattepligtige indkomst.
		///    2) Fremført underskud, der er modregnet i egen positive skattepligtige indkomst.
		///    3) Fremført underskud, der er overført og modregnet i ægtefælles positive skattepligtige indkomst.
		/// </remarks>
		public decimal ModregnetUnderskudSkattepligtigIndkomst
		{
			get { return _modregnetUnderskudSkattepligtigIndkomst; }
			set 
			{
				_modregnetUnderskudSkattepligtigIndkomst = value;
				_skattegrundlag = null;
			}
		}

		/// <summary>
		/// Underskud i skattepligtig indkomst til fremførsel i efterfølgende skatteår.
		/// </summary>
		public decimal UnderskudSkattepligtigIndkomstTilFremfoersel { get; set; }
		
		/// <summary>
		/// Skattepligtig indkomst før modregning af underskud (dvs. det selvangivne beløb)
		/// </summary>
		public decimal SkattepligtigIndkomst
		{
			get { return PersonligIndkomst + NettoKapitalIndkomst - LigningsmaessigeFradrag; }
		}
		
		/// <summary>
		/// Skattepligtig indkomst efter modregning af underskud (dvs. skattegrundlaget).
		/// </summary>
		/// <remarks>
		/// Først efter modregning og fremførsel af underskud er udført vil SkattepligtigIndkomst indeholde værdien af
		/// et fremført underskud. Derfor er det yderst vigtigt at modregning og fremførsel af underskud sker inden 
		/// beregning af skatter baseret på den skattepligtige indkomst.
		/// </remarks>
		public decimal SkattepligtigIndkomstSkattegrundlag
		{
			get { return SkattepligtigIndkomst - ModregnetUnderskudSkattepligtigIndkomst; }
		}

		// Note: Ingen sondring mellem tab/gevinst på aktier og udbytte, og dermed ingen justering af indeholdt udbytte på årsopgørelsen
		/// <summary>
		/// Aktieindkomst uden indeholdt udbytteskat.
		/// </summary>
		public decimal AktieIndkomst
		{
			get { return 0m; }
		}

		/// <summary>
		/// Ligningsmæssige fradrag (inklusiv beskæftigelsesfradrag).
		/// </summary>
		public decimal LigningsmaessigeFradrag
		{
			get
			{
				return SelvangivneBeloeb.LigningsmaessigeFradragMinusBeskaeftigelsesfradrag
				       + Beskaeftigelsesfradrag;
			}
		}

		/// <summary>
		/// Beregnet beskæftigelsesfradrag
		/// </summary>
		public decimal Beskaeftigelsesfradrag { get; private set; }

		private decimal _modregnetUnderskudKapitalPensionsindskud;
		/// <summary>
		/// Modregning af underskud i personlig indkomst i kapitalpensionsindskud.
		/// </summary>
		public decimal ModregnetUnderskudKapitalPensionsindskud
		{
			get { return _modregnetUnderskudKapitalPensionsindskud; }
			set
			{
				_modregnetUnderskudKapitalPensionsindskud = value;
				_skattegrundlag = null;
			}
		}

		/// <summary>
		/// PBL § 16, stk. 1, omhandler indskud til kapitalpensionsordninger, kapitalforsikring 
		/// og bidrag til supplerende engangsydelser fra pensionskasser, hvor der maksimalt kan 
		/// indbetales op til 46.000 kr. (2009 og 2010).
		/// </summary>
		public decimal KapitalPensionsindskud
		{
			get { return SelvangivneBeloeb.KapitalPensionsindskud; }
		}

		public decimal KapitalPensionsindskudSkattegrundlag
		{
			get { return KapitalPensionsindskud - ModregnetUnderskudKapitalPensionsindskud; }
		}
	}
}