namespace Maxfire.Skat
{
	// TODO: Mangler restskat i selvangivne beløb
	public interface ISelvangivneBeloeb
	{
		/// <summary>
		/// Her anføres personlig indkomst, hvoraf der skal betales AM/SP-bidrag.
		/// </summary>
		/// <remarks>
		/// Består af lønindkomst mv., honorarer mv., værdi af fri bil, telefon, bolig m.v.
		/// og multimedieskat.
		/// Beløbet skal være efter fradrag af eget bidrag til arbejdsgiverpensionsordning
		/// og eget bidrag til ATP.
		/// </remarks>
		decimal PersonligIndkomstAMIndkomst { get; }
		
		/// <summary>
		/// Personlig indkomst, hvoraf der ikke skal betales AM/SP-bidrag, samt summen af
		/// fradrag i den personlige indkomst (dog uden privat tegnet pensionsindskud, der 
		/// indsættes automastisk????)
		/// </summary>
		/// <remarks>
		/// Består af pensioner, dagpenge, modtaget underholdsbidrag mv.
		/// </remarks>
		///TODO: Hvordan får man fradraget PrivatTegnetPensionsindskud med her automatisk og fool-proof?
		decimal PersonligIndkomstEjAMIndkomst { get; }
		
		decimal PersonligIndkomstFremfoertUnderskud { get; }

		decimal NettoKapitalIndkomst { get; }
		
		decimal LigningsmaessigtFradragMinusBeskaeftigelsesfradrag { get; }
		
		decimal SkattepligtigIndkomstFremfoertUnderskud { get; }

		/// <summary>
		/// Indskud til arbejdsgiver- og privattegnet kapitalpension.
		/// </summary>
		/// <remarks>
		/// For arbejdsgiverpension skal det være efter fradrag af 8% arbejdsmarkedsbidrag.
		/// Beløbet tillægges topskattegrundlaget.
		/// </remarks>
		decimal KapitalPensionsindskud { get; }

		/// <summary>
		/// Indskud til privattegnede pensionsordninger med løbende udbetalinger og 
		/// ratepension samt privattegnet kapitalpension dog højest 46.000 kr. i 2010
		/// </summary>
		/// <remarks>
		/// Fradrages i AM-indkomsten inden opgørelse af arbejdsindkomsten, der benyttes som
		/// grundlag for besjkæftigelsesfradraget.
		/// Fradrages i den personlige indkomst inden skatteberegningen.
		/// </remarks>
		decimal PrivatTegnetPensionsindskud { get; }

		decimal AktieIndkomst { get; }
	}
}