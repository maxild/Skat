using Maxfire.Skat.Extensions;

namespace Maxfire.Skat
{
	/////////////////////////////////////////////////////////////////////////////////////////
	//
	// PSL § 26.
	// 
	// Stk. 1 For indkomstårene 2012-2019 beregnes en kompensation, hvis forskelsbeløbet 
	// beregnet efter stk. 2 og 3 er negativt. Kompensationen modregnes i skatterne efter 
	// §§ 6, 7, 8 og § 8 a, stk. 2, indkomstskat til kommunen og kirkeskat i den nævnte 
	// rækkefølge.
	//
	// Stk. 2. I beregningen af forskelsbeløbet indgår følgende beløb:
	//
	// 1) 1,5 pct. af grundlaget for bundskatten, jf. § 6,
	// i det omfang grundlaget overstiger et bundfradrag
	// på 44.800 kr. (2010-niveau). For personer, som ved indkomstårets udløb ikke
	// er fyldt 18 år og ikke har indgået ægteskab,
	// er bundfradraget 33.600 kr. (2010-niveau).
	//
	// 2) 6 pct. af den personlige indkomst med tillæg
	// af positiv nettokapitalindkomst, i det omfang
	// grundlaget overstiger et bundfradrag på
	// 362.800 kr. (2010-niveau).
	//
	// 3) 15 pct. af grundlaget for topskat, jf. § 7, stk.
	// 1, i det omfang grundlaget overstiger et
	// bundfradrag på 362.800 kr. (2010-niveau)
	// fratrukket 15 pct. af grundlaget for topskat,
	// jf. § 7, stk. 1, i det omfang grundlaget overstiger
	// bundfradraget anført i § 7, stk. 2.
	//
	// 4) 1 pct. af grundlaget for aktieindkomstskat,
	// jf. § 8 a, stk. 1 og 2.
	//
	// 5) 8 pct. plus procenten ved beregning af indkomstskat
	// til kommunen og kirkeskat af fradraget
	// beregnet efter ligningslovens § 9 J,
	// fratrukket et fradrag opgjort på samme
	// grundlag ved anvendelse af en fradragsprocent
	// på 4,25 og et grundbeløb på 14.200 kr.
	// (2010-niveau).
	//
	// 6) Skatteværdien opgjort efter § 12 af et grundbeløb
	// på 1.900 kr. (2010-niveau).
	//
	// 7) 8 pct. minus skatteprocenten for sundhedsbidraget,
	// jf. § 8, af summen af negativ nettokapitalindkomst,
	// der overstiger beløbsgrænsen i § 11, stk. 1, og udgifter 
	// af den art, der fradrages ved opgørelsen af den skattepligtige
	// indkomst, men ikke ved opgørelsen af personlig
	// indkomst og kapitalindkomst (ligningsmæssige fradrag).
	//
	// Stk. 3. Forskelsbeløbet opgøres som summen af beløbene opgjort efter nr. 1-5 fratrukket beløbet
	// opgjort efter nr. 6. Er beløbet negativt, sættes det til 0. Fra dette beløb trækkes beløbet opgjort
	// efter nr. 7.
	//
	// Stk. 4. Hvis en gift person har negativ nettokapitalindkomst, modregnes dette beløb i den anden
	// ægtefælles positive nettokapitalindkomst, inden beløbet efter stk. 2, nr. 2, beregnes, hvis
	// ægtefællerne er samlevende ved indkomstårets udløb.
	//
	// Stk. 5. Hvis en gift persons personlige indkomst med tillæg af positiv nettokapitalindkomst
	// er lavere end bundfradraget i stk. 2, nr. 2 forhøjes den anden ægtefælles bundfradrag med
	// forskelsbeløbet, inden beløbet efter stk. 2, nr. 2, beregnes, hvis ægtefællerne er samlevende ved
	// indkomstårets udløb. 1. pkt. finder ikke anvendelse for indkomstår, hvor der er valgt beskatning
	// efter kildeskattelovens § 48 F, stk. 1-3.
	//
	// Stk. 6. For ægtefæller finder § 7, stk. 5-10, anvendelse ved beregning af beløbet efter stk. 2, nr.
	// 3, med de bundfradrag, der er anført i stk. 2, nr. 3.
	//
	// Stk. 7. Hvis en gift person har positiv nettokapitalindkomst, modregnes dette beløb i den anden
	// ægtefælles negative nettokapitalindkomst, inden beløbet efter stk. 2, nr. 7, beregnes, hvis
	// ægtefællerne er samlevende ved indkomstårets udløb.
	//
	// Stk. 8. Grundbeløb og bundfradrag i stk. 2, nr. 1, 2, 3, 5 og 6, reguleres efter § 20.
	//
	/////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Der indføres en kompensationsordning, så personer med særligt store fradrag (inkl. ligningsmæssige
	/// fradrag) ikke kan miste mere som følge af begrænsningerne af fradrag, end de får i indkomstskattelettelser.
	/// </summary>
	public class KompensationBeregner
	{
		private readonly ISkattelovRegistry _skattelovRegistry;

		public KompensationBeregner(ISkattelovRegistry skattelovRegistry)
		{
			_skattelovRegistry = skattelovRegistry;
		}

		/// <summary>
		/// Beregn det nedslag i skatten der gives som følge af PSL § 26 (kompensationsordning fra forårspakke 2.0)
		/// </summary>
		public ValueTuple<decimal> BeregnKompensation(
			IValueTuple<IPerson> personer,
			IValueTuple<IPersonligeBeloeb> indkomster, 
			IValueTuple<IKommunaleSatser> kommunaleSatser, 
			int skatteAar)
		{
			// TODO: Beregn hvert led 1-7 i hver sin funktion/procedure (nemmere at teste sådan)
			// TODO: Satser er hårdkodede, skal benytte ISkattelovRegistry for alle satser og beløb
			// Note: Med tidligere gældende lovgivning var satsen i 2010 5,26 pct.
			// TODO: Satsen er jo nedsat 5,26 pct. fra 5,26 pct. til 3,67 pct. hvilket er en nedsættelse på 1,59 pct. point (_ikke_ 1,5 pct. point)
			// TODO: For ægtefæller foretages en samlet beregning af kompensationen (1-7). Kompensationen opgøres som summen af det forskelsbeløb
			// der opgøres efter stk 2 og 3 for den enkelte ægtefælle. Herefter fordeles forskelsbeløbet ift. størrelsen af det beløb, der
			// for hver af ægtefællerne medregnes efter stk 2, nr. 7 (dvs. Skatteskærpelse på fradragene)

			var bundSkattelettelse = GetBundSkattelettelse(personer, indkomster, skatteAar);
			var mellemSkattelettelse = GetMellemSkattelettelse(indkomster, skatteAar);
			var topSkattelettelse = GetTopSkattelettelse(indkomster, skatteAar);
			var aktieSkattelettelse = GetAktieSkattelettelse(indkomster);
			var beskaeftigelsesfradragSkattelettelse = GetBeskaeftigelsesfradragSkattelettelse(indkomster, kommunaleSatser, skatteAar);
			var personfradragSkatteskaerpelse = GetPersonfradragSkatteskaerpelse(personer, kommunaleSatser, skatteAar);
			var samletSkatteskaerpelsePaaFradagene = GetSamletSkatteskaerpelsePaaFradragene(indkomster, skatteAar);

			var samletSkattelettelse = +(bundSkattelettelse + mellemSkattelettelse 
			                           + topSkattelettelse + aktieSkattelettelse 
			                           + beskaeftigelsesfradragSkattelettelse - personfradragSkatteskaerpelse);
			
			var forskelsbeloeb = samletSkattelettelse - samletSkatteskaerpelsePaaFradagene;

			return +(-forskelsbeloeb);
		}

		/// <summary>
		/// Beregner skattelettelsen af nedsættelsen af bundskattesatsen (pkt. 1 i PSL § 26, stk. 2).
		/// </summary>
		public ValueTuple<decimal> GetBundSkattelettelse(
			IValueTuple<IPerson> personer, 
			IValueTuple<IPersonligeBeloeb> indkomster, 
			int skatteAar)
		{
			var bundskatBeregner = new BundskatBeregner(_skattelovRegistry);
			var bundLettelseBundfradrag 
				= personer.Map(person => _skattelovRegistry.GetBundLettelseBundfradrag(skatteAar, person.GetAlder(skatteAar), personer.Size > 1));
			return 0.015m * bundskatBeregner.BeregnBruttoGrundlag(indkomster).DifferenceGreaterThan(bundLettelseBundfradrag);
		}

		/// <summary>
		/// Beregner skattelettelsen ved fjernelse af mellemskatten (pkt. 2 i PSL § 26, stk. 2).
		/// </summary>
		public ValueTuple<decimal> GetMellemSkattelettelse(
			IValueTuple<IPersonligeBeloeb> indkomster, 
			int skatteAar)
		{
			var mellemskatBeregner = new MellemskatBeregner(_skattelovRegistry);
			var mellemLettelseBundfradrag = _skattelovRegistry.GetMellemLettelseBundfradrag(skatteAar);
			return 0.06m * mellemskatBeregner.BeregnGrundlag(indkomster, mellemLettelseBundfradrag);
		}

		/// <summary>
		/// Beregner skattelettelsen af forøgelsen af topskattegrænsen (pkt. 3 i PSL § 26, stk. 2).
		/// </summary>
		public ValueTuple<decimal> GetTopSkattelettelse(
			IValueTuple<IPersonligeBeloeb> indkomster, 
			int skatteAar)
		{
			var topskatBeregner = new TopskatBeregner(_skattelovRegistry);
			var topLettelseBundfradrag = _skattelovRegistry.GetTopLettelseBundfradrag(skatteAar);
			var topskatBundfradrag = _skattelovRegistry.GetTopskatBundfradrag(skatteAar);
			var topskatteGrundlagMedTidligereBundfradrag = topskatBeregner.BeregnGrundlag(indkomster, topLettelseBundfradrag, 0);
			var topskatteGrundlagMedNuvaerendeBundfradrag = topskatBeregner.BeregnGrundlag(indkomster, topskatBundfradrag, 0);
			return 0.15m * (topskatteGrundlagMedTidligereBundfradrag - topskatteGrundlagMedNuvaerendeBundfradrag);
		}

		/// <summary>
		/// Beregner skattelettelsen af nedsættelsen af aktieindkomstskattesatsen (pkt. 4 i PSL § 26, stk. 2).
		/// </summary>
		public ValueTuple<decimal> GetAktieSkattelettelse(IValueTuple<IPersonligeBeloeb> indkomster)
		{
			return indkomster.Map(x => 0.01m * x.Skattegrundlag.AktieIndkomst);
		}

		/// <summary>
		/// Beregner skattelettelse af forhøjelsen af beskæftigelsesfradraget (pkt. 5 i PSL § 26, stk. 2).
		/// </summary>
		public ValueTuple<decimal> GetBeskaeftigelsesfradragSkattelettelse(
			IValueTuple<IPersonligeBeloeb> indkomster, 
			IValueTuple<IKommunaleSatser> kommunaleSatser, 
			int skatteAar)
		{
			var skattesats = 0.08m + kommunaleSatser.Map(x => x.GetKommuneOgKirkeskattesats());
			var beskaeftigelsesfradragBeregner = new BeskaeftigelsesfradragBeregner(_skattelovRegistry);
			var amIndkomster = indkomster.Map(x => x.Skattegrundlag.PersonligIndkomstAMIndkomst);
			var beskaeftigelsesfradragMedTidligereSatsOgGrundbeloeb = beskaeftigelsesfradragBeregner.BeregnFradrag(amIndkomster, 0.0425m, 14200);
			var beskaeftigelsesfradragMedNuvaerendeSatsOgGrundbeloeb = beskaeftigelsesfradragBeregner.BeregnFradrag(amIndkomster, skatteAar);
			return skattesats * (beskaeftigelsesfradragMedNuvaerendeSatsOgGrundbeloeb - beskaeftigelsesfradragMedTidligereSatsOgGrundbeloeb);
		}

		/// <summary>
		/// Beregn skatteskærpelsen af nulreguleringen af personfradraget (pkt. 6 i PSL § 26, stk. 2).
		/// </summary>
		public ValueTuple<decimal> GetPersonfradragSkatteskaerpelse(
			IValueTuple<IPerson> personer, 
			IValueTuple<IKommunaleSatser> kommunaleSatser, 
			int skatteAar)
		{
			var personfradragBeregner = new PersonfradragBeregner(_skattelovRegistry);
			var personfradragSkaerpelse = personer.Map(person => 
				_skattelovRegistry.GetPersonfradragSkaerpelse(skatteAar, person.GetAlder(skatteAar), personer.Size > 1));
			return personfradragBeregner.BeregnSkattevaerdierAfPersonfradrag(kommunaleSatser, skatteAar, personfradragSkaerpelse)
				.Map(x => x.Sum());
		}

		/// <summary>
		/// Beregn den samlede skatteskærpelse på fradragene
		/// </summary>
		public ValueTuple<decimal> GetSamletSkatteskaerpelsePaaFradragene(
			IValueTuple<IPersonligeBeloeb> indkomster, 
			int skatteAar)
		{
			decimal sats = 0.08m - _skattelovRegistry.GetSundhedsbidragSkattesats(skatteAar);
			var negativNettoKapitalIndkomstGrundbeloeb = _skattelovRegistry.GetNegativNettoKapitalIndkomstGrundbeloeb(skatteAar);
			var nettoKapitalIndkomst = indkomster.Map(x => x.Skattegrundlag.NettoKapitalIndkomst);
			var nettoKapitalIndkomstTilBeskatning = nettoKapitalIndkomst.NedbringPositivtMedEvtNegativt();
			// TODO: For ægtefæller er det summen, da ubenyttet grundbeløb kan overføres
			var negativNettoKapitalIndkomstOverGrundbeloebet = (+(-nettoKapitalIndkomstTilBeskatning))
				.DifferenceGreaterThan(negativNettoKapitalIndkomstGrundbeloeb);
			var ligningsmaesigeFradrag = indkomster.Map(x => x.Skattegrundlag.LigningsmaessigeFradrag);
			return sats * (negativNettoKapitalIndkomstOverGrundbeloebet + ligningsmaesigeFradrag);
		}

		public ValueTuple<ModregnSkatterResult<Skatter>> ModregnMedKompensation(
			ValueTuple<Skatter> skatter, 
			IValueTuple<IPerson> personer,
			IValueTuple<IPersonligeBeloeb> indkomster, 
			IValueTuple<IKommunaleSatser> kommunaleSatser, 
			int skatteAar)
		{
			var kompensation = BeregnKompensation(personer, indkomster, kommunaleSatser, skatteAar);
			var skatteModeregner = getSkatteModregner();
			return skatteModeregner.Modregn(skatter, kompensation);
		}

		private static SkatteModregner<Skatter> getSkatteModregner()
		{
			return new SkatteModregner<Skatter>(
				Modregning<Skatter>.Af(x => x.Bundskat),
				Modregning<Skatter>.Af(x => x.Topskat),
				Modregning<Skatter>.Af(x => x.Sundhedsbidrag),
				Modregning<Skatter>.Af(x => x.AktieindkomstskatOverGrundbeloebet),
				Modregning<Skatter>.Af(x => x.Kommuneskat),
				Modregning<Skatter>.Af(x => x.Kirkeskat)
			);
		}
	}
}