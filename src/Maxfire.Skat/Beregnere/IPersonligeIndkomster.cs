namespace Maxfire.Skat.Beregnere
{
    // AM-bidragspligtig indkomst

    public interface IPersonligeIndkomster : IArbejdsmarkedIndkomster
    {
        /// <summary>
        /// Personlig indkomst (før AM-bidrag), der bygger på de selvangivne beløb (dvs. før modregning).
        /// </summary>
        decimal PersonligIndkomstFoerAMBidrag { get; }

        /// <summary>
        /// Personlig indkomst (efter AM-bidrag), der bygger på de selvangivne beløb (dvs. før modregning).
        /// </summary>
        decimal PersonligIndkomst { get; }

        /// <summary>
        /// Personlig indkomst (efter AM-bidrag) vedr. bund-, mellem og topskat (dvs. efter modregning).
        /// </summary>
        decimal PersonligIndkomstSkattegrundlag { get; }

        /// <summary>
        /// Nettokapitalindkomst, der bygger på de selvangivne beløb (dvs. før modregning).
        /// </summary>
        decimal NettoKapitalIndkomst { get; }

        /// <summary>
        /// Nettokapitalindkomst vedr. beregning af bund-, mellem- og topskat (dvs efter modregninger).
        /// </summary>
        decimal NettoKapitalIndkomstSkattegrundlag { get; }

        /// <summary>
        /// Det samlede ligningsmæssige fradrag.
        /// </summary>
        decimal LigningsmaessigtFradrag { get; }

        /// <summary>
        /// Indskud på kapitalpension, der bygger på de selvangivne beløb (dvs. før modregning).
        /// </summary>
        decimal KapitalPensionsindskud { get; }

        /// <summary>
        /// Indskud på kapitalpension vedr. beregning af bund-, mellem- og topskat (dvs efter modregninger).
        /// </summary>
        decimal KapitalPensionsindskudSkattegrundlag { get; }

        /// <summary>
        /// Aktieindkomst.
        /// </summary>
        decimal AktieIndkomst { get; }
    }
}
