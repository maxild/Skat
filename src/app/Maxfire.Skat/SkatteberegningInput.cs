using System;

namespace Maxfire.Skat
{
	[Obsolete]
	public class SkatteberegningInput
	{
		public SkatteberegningInput()
		{
			PersonligIndkomst = new PersonligIndkomstType();
			Ligningsmaessigefradrag = new LigningsmaessigefradragType();
			NettoKapitalIndkomst = new NettoKapitalIndkomstType();
			ArbejdsgiverPension = new Pensionsbidrag();
		}

		public PersonligIndkomstType PersonligIndkomst { get; private set; }

		public NettoKapitalIndkomstType NettoKapitalIndkomst { get; private set; }

		public LigningsmaessigefradragType Ligningsmaessigefradrag { get; private set; }

		public Pensionsbidrag ArbejdsgiverPension { get; private set; }

		/// <summary>
		/// Bidrag og præmier til privattegnede pensionsordninger med løbende udbetalinger og ratepension 
		/// samt privattegnet kapitalpension dog højest 43.100 kr. i 2007.
		/// </summary>
		/// 
		//TODO: Rename to PrivatPension el.lign.
		//TODO: Skal slettes idet den kun benyttes til opgørelse af den personlige indkomst (OBS: Benyttes ved beregning af beskæftigelsefradrag)
		public decimal Pension
		{
			get { return PersonligIndkomst.Fradrag.PrivatPension.IAlt; }
		}

		/// <summary>
		/// Indbetalinger til arbejdsgiver- og privattegnet kapitalpension. 
		/// </summary>
		/// <remarks>
		/// For arbejdsgiverpension skal det være efter fradrag af 8% arbejdsmarkedsbidrag.
		/// 
		/// Fradrages i personlig indkomst og tillægges igen(!) ved beregning af topskat
		/// 
		/// Må højest udgøre 46.000 kr i 2010 ()
		/// </remarks>
		//TODO: Rename to SamletKapitalPensionIndbetalinger el.lign.
		public decimal KapitalPension
		{
			get { return PersonligIndkomst.Fradrag.PrivatPension.Kapital + ArbejdsgiverPension.Kapital; }
		}

		public decimal UdbytteIndkomst { get; set; }

		/// <summary>
		/// kursgevinst/tab til beskatning.
		/// </summary>
		public decimal Kursgevinst { get; set; } // TODO: ????

		public decimal Kommuneskattesats { get; set; }

		public decimal Kirkeskattesats { get; set; }

		public class PersonligIndkomstType
		{
			public PersonligIndkomstType()
			{
				AMIndkomst = new PersonligIndkomstArbejdsmarkedsbidrag();
				IkkeAMIndkomst = new PersonligIndkomstEjArbejdsmarkedsbidrag();
				Fradrag = new PersonligIndkomstFradrag();
			}

			public PersonligIndkomstArbejdsmarkedsbidrag AMIndkomst { get; private set; }
			public PersonligIndkomstEjArbejdsmarkedsbidrag IkkeAMIndkomst { get; private set; }
			public PersonligIndkomstFradrag Fradrag { get; private set; }

			public decimal IAlt
			{
				get { return AMIndkomst.IAlt + IkkeAMIndkomst.IAlt - Fradrag.IAlt; }
			}

			public class PersonligIndkomstArbejdsmarkedsbidrag
			{
				///<summary>
				/// Rubrik 11: Lønindkomst, bestyrelseshonorar, multimedier (fri telefon mv.), 
				/// fri bil, fri kost og logi efter Skatterådets satser, men før fradrag af AM-bidrag.
				/// </summary>
				/// <remarks>
				/// Beløbet skal være efter fradrag af ATP- og arbejdsgiveradministreret pensionsbidrag, men før fradrag af AM-bidrag.
				/// </remarks>
				public decimal Loen { get; set; }

				/// <summary>
				/// Anden personlig indkomst, der skal svares arbejdsmarkedsbidrag af.
				/// </summary>
				/// <remarks>
				/// Rubrik 12: Honorarer og vederlag i form af visse goder mv. før fradrag af AM-bidrag.
				/// Rubrik 14: Jubilæumsgratiale og fratrædelsesgodtgørelse mv. før fradrag af AM-bidrag.
				/// Rubrik 15: Anden personlig indkomst som fx fri telefon, privat dagpleje og hushjælp mv. før fradrag af AM-bidrag.
				/// mv.
				/// </remarks>
				public decimal AndenIndkomst { get; set; }

				public decimal IAlt
				{
					get { return Loen + AndenIndkomst; }
				}
			}

			public class PersonligIndkomstEjArbejdsmarkedsbidrag
			{
				/// <summary>
				/// Stipendier fra SU
				/// </summary>
				// Rubrik 16: Pensioner, dagpenge mv. og stipendier fra SUstyrelsen.
				public decimal SU { get; set; }

				// Rubrik 16: Pensioner, dagpenge mv. og stipendier fra SUstyrelsen.
				public decimal Sygedagpenge { get; set; }

				// Rubrik 16: Pensioner, dagpenge mv. og stipendier fra SUstyrelsen.
				public decimal Arbejdsloeshedsdagpenge { get; set; }

				// Rubrik 16: Pensioner, dagpenge mv. og stipendier fra SUstyrelsen.
				public decimal Kontanthjaelp { get; set; }

				// Rubrik 16: Pensioner, dagpenge mv. og stipendier fra SUstyrelsen.
				public decimal Efterloen { get; set; }

				// Rubrik 16: Pensioner, dagpenge mv. og stipendier fra SUstyrelsen.
				public decimal Pensionsudbetalinger { get; set; }

				/// <summary>
				/// Rubrik 19: Modtaget underholdsbidrag
				/// </summary>
				public decimal ModtagetUnderholdsbidrag { get; set; }

				/// <summary>
				/// Anden personlig indkomst, der ikke skal svares arbejdsmarkedsbidrag af.
				/// </summary>
				/// <remarks>
				/// Rubrik 20: Anden personlig indkomst
				/// </remarks>
				public decimal AndenIndkomst { get; set; }

				// TODO: Mangler
				// Rubrik 17: Uddelinger fra foreninger og fonde mv. Gruppelivsforsikring betalt af pensionskasse. Visse personalegoder.
				// Rubrik 18: Hædersgaver

				public decimal IAlt
				{
					get
					{
						return SU +
							   Sygedagpenge +
							   Arbejdsloeshedsdagpenge +
							   Kontanthjaelp +
							   Efterloen +
							   Pensionsudbetalinger +
							   ModtagetUnderholdsbidrag +
							   AndenIndkomst;
					}
				}
			}

			public class PersonligIndkomstFradrag
			{
				/// <summary>
				/// Privattegnede pensionsordninger fradrages i den personlige indkomst
				/// </summary>
				public Pensionsbidrag PrivatPension { get; set; }

				/// <summary>
				/// Fradragsberettigede indskud på iværksætterkonto.
				/// </summary>
				/// <remarks>
				/// Indskud på en iværksætterkonto vil være fradrag ved opgørelsen af den personlige indkomst. 
				/// Dermed får sådanne fradrag en højere skatteværdi end fradrag for indskud på etableringskonto, 
				/// der 'kun' er ligningsmæssige fradrag.
				/// </remarks>
				public decimal IvaerksaetterKonto { get; set; }

				public decimal IAlt
				{
					get
					{
						return PrivatPension.IAlt;
					}
				}
			}
		}

		public class NettoKapitalIndkomstType
		{
			public NettoKapitalIndkomstType()
			{
				Indtaegter = new KapitalIndkomstIndtaegter();
				Udgifter = new KapitalIndkomstUdgifter();
			}

			public KapitalIndkomstIndtaegter Indtaegter { get; private set; }
			public KapitalIndkomstUdgifter Udgifter { get; private set; }

			public decimal IAlt
			{
				get { return Indtaegter.IAlt - Udgifter.IAlt; }
			}

			public class KapitalIndkomstIndtaegter
			{
				/// <summary>
				/// Rubrik 31: Renteindtægter af indestående i pengeinstitut, obligationer og pantebreve i 
				/// depot samt udlodning fra obligationsbaserede investeringsforeninger.
				/// </summary>
				public decimal Renteindtaegter { get; set; }

				/// <summary>
				/// Rubrik 33: Reservefondsudlodninger og kapitalværdistigninger af pensionsordninger
				/// </summary>
				public decimal Pensionsordninger { get; set; }

				/// <summary>
				/// Rubrik 34: Udlodning fra investeringsselskab/-forening, hvor der er indeholdt udbytteskat
				/// </summary>
				public decimal Udlodninger { get; set; }

				/// <summary>
				/// Rubrik 35: Over- /underskud ved visse skibsprojekter (underskud angives med minus).
				/// Overskud ved anden anpartsvirksomhed.
				/// </summary>
				public decimal SkibsprojekterLoebendeOverskud { get; set; }

				/// <summary>
				/// Rubrik 36: Fortjeneste/tab ved ophør af visse skibsprojekter (tab angives med minus).
				/// Fortjeneste ved ophør af anden anpartsvirksomhed
				/// </summary>
				public decimal SkibsprojekterFortjenesteVedOphoer { get; set; }

				//
				/// <summary>
				/// Rubrik 37: Lejeindtægt ved udleje af helårsbolig en del af året samt sommerhus- og værelsesudlejning. 
				/// Gælder kun, hvis du selv ejer boligen.
				/// </summary>
				public decimal BoligUdlejningsindtaegter { get; set; }

				/// <summary>
				/// Rubrik 38: Renter af pantebreve, der ikke er i depot. Gevinst/tab på bevis i investeringsselskab 
				/// og i udloddende blandet og obligationsbaseret investeringsforening.
				/// </summary>
				public decimal AndreRenteindtaegter { get; set; }

				/// <summary>
				/// Rubrik 39: Anden kapitalindkomst fx finansielle kontrakter og aftaler.
				/// </summary>
				public decimal AndenKapitalIndkomst { get; set; }

				public decimal IAlt
				{
					get
					{
						return Renteindtaegter +
							   Pensionsordninger +
							   Udlodninger +
							   SkibsprojekterLoebendeOverskud +
							   SkibsprojekterFortjenesteVedOphoer +
							   BoligUdlejningsindtaegter +
							   AndreRenteindtaegter +
							   AndenKapitalIndkomst;
					}
				}
			}

			/// <summary>
			/// Fradrag i kapitalindkomsten.
			/// </summary>
			public class KapitalIndkomstUdgifter
			{
				/// <summary>
				/// Rubrik 41: Renteudgifter af gæld til realkreditinstitutter og reallånefonde samt 
				/// fradragsberettigede kurstab ved omlægning af kontantlån.
				/// </summary>
				public decimal RenteudgifterRealkredit { get; set; }

				/// <summary>
				/// Rubrik 42: Renteudgifter af gæld til pengeinstitutter, pensionskasser, forsikrings- og 
				/// finansieringsselskaber, kontokortordninger samt af pantebreve i depot.
				/// </summary>
				public decimal RenteudgifterPengeinstitutter { get; set; }

				/// <summary>
				/// Rubrik 43: Renteudgifter af studielån fra Økonomistyrelsen
				/// </summary>
				public decimal RenteudgifterStudielaan { get; set; }

				/// <summary>
				/// Rubrik 44: Renteudgifter af anden gæld, herunder af statsgaranterede studielån 
				/// i et pengeinstitut samt af pantebreve, der ikke er i depot
				/// </summary>
				public decimal RenteudgifterAndenGaeld { get; set; }

				public decimal IAlt
				{
					get
					{
						return RenteudgifterRealkredit +
							   RenteudgifterPengeinstitutter +
							   RenteudgifterStudielaan +
							   RenteudgifterAndenGaeld;
					}
				}
			}

		}

		public class LigningsmaessigefradragType
		{
			/// <summary>
			/// Rubrik 51: Befordring
			/// </summary>
			public decimal Befordring { get; set; }

			/// <summary>
			/// Rubrik 52: Fagligt kontingent, bidrag til A-kasse, efterlønsordning og fleksydelse. 
			/// Ansattes årlige betaling for pc-ordning (højst 3.500 kroner)
			/// </summary>
			public decimal Faglig { get; set; }

			/// <summary>
			/// Rubrik 53: Øvrige lønmodtagerudgifter - 5.500 kroner
			/// </summary>
			/// <remarks>
			/// Det kan være beklædning, arbejdsværelse, dobbelt husførelse, faglitteratur, flytteudgifter, 
			/// forskudt arbejdstid m.m. efter gældende regler.
			/// </remarks>
			public decimal OevrigeLoenmodtagerUdgifter { get; set; }

			/// <summary>
			/// Rubrik 54: Standardfradrag for børnedagplejere og fiskere. Fradrag vedr. 
			/// DIS-indkomst (begrænset fart). Handicappede og kronisk syges befordringsudgifter.
			/// </summary>
			// TODO: Skal måske opslittes i mere logiske klumper
			public decimal Rubrik54 { get; set; }

			/// <summary>
			/// Rubrik 56: Underholdsbidrag til tidligere ægtefælle, børnebidrag og aftægtsforpligtelser mv.
			/// </summary>
			public decimal Underholdsbidrag { get; set; }

			/// <summary>
			/// Rubrik 57: Fradragsberettigede indskud på etableringskonto.
			/// </summary>
			public decimal Etableringskonto { get; set; }

			/// <summary>
			/// Gaver til almenvelgørende og almennyttige foreninger, stiftelser og institutioner mm., 
			/// hvis midler anvendes til fordel for en større kreds af personer.
			/// </summary>
			/// <remarks>
			/// Fradraget kan kun indrømmes for det beløb, hvormed gaverne tilsammen overstiger 500 kr. og 
			/// kan højst udgøre 13.700 kr. i 2007 (14.000 kr. i 2008, 14500 i 2010???)
			/// </remarks>
			public decimal GaverTilForeninger { get; set; }

			/// <summary>
			/// Fradrag for rejseudgifter til kost, småfornødenheder og/eller logi.
			/// </summary>
			/// <remarks>
			/// Der er fra indkomståret 2010 kommet nye regler om, hvor meget du i alt kan fradrage for 
			/// rejseudgifter. Hvis du som lønmodtager benytter muligheden for at trække udgifter til 
			/// kost og logi fra med standardsatserne, eller hvis du vælger at tage fradrag for de faktisk 
			/// dokumenterede udgifter, kan du højest gøre dette med 50.000 kr. om året.
			/// </remarks>
			public decimal Rejseudgifter { get; set; }

			public decimal IAlt
			{
				get
				{
					return Befordring +
						   Faglig +
						   OevrigeLoenmodtagerUdgifter +
						   Rubrik54 +
						   Underholdsbidrag +
						   Etableringskonto +
						   GaverTilForeninger +
						   Rejseudgifter;
				}
			}
		}

		/// <summary>
		/// Årlige bidrag og præmier.
		/// </summary>
		public class Pensionsbidrag
		{
			public decimal Rate { get; set; }
			public decimal Kapital { get; set; }

			public decimal IAlt
			{
				get { return Rate + Kapital; }
			}
		}

	}
}