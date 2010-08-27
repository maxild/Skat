using System;
using System.Text;

namespace Maxfire.Skat
{
	/// <summary>
	/// Indkomstskatter til staten, der beregnes på baggrund af den personlige indkomst og
	/// positiv nettokapitalindkomst.
	/// </summary>
	/// <remarks>
	/// Disse indkomstskatter til staten beregnes først efter modregning og fremførsel af
	/// indeværende års underskud i personlig indkomst og tidligere års underskud i personlig
	/// indkomst, jf. PSL § 13, stk. 3 og 4. 
	/// 
	/// Det er i disse skatter, der sker modregning af underskudsværdier (skatteværdier) 
	/// af negativ skattepligtig indkomst, jf. PSL § 13, stk. 1 og 2.
	/// </remarks>
	public class SkatterAfPersonligIndkomst : IEquatable<SkatterAfPersonligIndkomst>, ISumable<decimal>
	{
		public static readonly SkatterAfPersonligIndkomst Nul = new SkatterAfPersonligIndkomst();

		public SkatterAfPersonligIndkomst()
		{
		}

		public SkatterAfPersonligIndkomst(decimal bundskat=0, decimal mellemskat=0, decimal topskat=0,
			decimal aktieindkomstskatUnderGrundbeloebet=0, decimal aktieindkomstskatOverGrundbeloebet=0)
		{
			Bundskat = bundskat;
			Mellemskat = mellemskat;
			Topskat = topskat;
			AktieindkomstskatUnderGrundbeloebet = aktieindkomstskatUnderGrundbeloebet;
			AktieindkomstskatOverGrundbeloebet = aktieindkomstskatOverGrundbeloebet;
		}

		public decimal Bundskat { get; private set; }

		public decimal Mellemskat { get; private set; }

		public decimal Topskat { get; private set; }

		/// <summary>
		/// I følge bestemmelser i PSL § 8a, stk. 1 bliver skat af aktieindkomst, som ikke overstiger
		/// grundbeløbet (48.300 kr. i 2009 og 2010) beregnet som en endelig skat på 28 pct. Indeholdt
		/// udbytteskat efter KSL § 65 af aktieindkomst, der ikke overstiger grundbeløbet, er endelig 
		/// betaling af skatten, og udbytteskatten modregnes ikke i slutskatten efter KSL § 67.
		/// </summary>
		/// <remarks>
		/// Skatten af aktieindkomst under progressionsgrænsen er endelig og svarer for udbytters 
		/// vedkommende til den udbytteskat, som selskabet har indeholdt. At skatten er endelig 
		/// indebærer, at der ikke kan modregnes skatteværdi af negativ skattepligtig indkomst 
		/// eller skatteværdi af personfradrag. Dette gælder også i de tilfælde, hvor der ikke 
		/// er indeholdt skat, f.eks. ved maskeret udlodning og i tilfælde, hvor aktieavancer 
		/// indgår i aktieindkomsten. I det omfang der ikke er indeholdt udbytteskat af 
		/// aktieindkomsten, forhøjes modtagerens slutskat med det manglende beløb. 
		/// </remarks>
		public decimal AktieindkomstskatUnderGrundbeloebet { get; private set; }

		/// <summary>
		/// I følge bestemmelser i PSL § 8a, stk. 2 vil skat af aktieindkomst, der overstiger
		/// grundbeløbet (48.300 kr. i 2009 og 2010) indgå i slutskatten, og den udbytteskat
		/// der er indeholdt i denne del af udbyttet efter KSL § 65 modregnes i slutskatten 
		/// efter KSL § 67.
		/// </summary>
		/// <remarks>
		/// Den overskydende del af aktieindkomstskatten indgår altså i slutskatten og indeholdt
		/// udbytteskat modregnes.
		/// Aktieindkomst, der overstiger progressionsgrænsen, beskattes med 42 pct. af det 
		/// overskydende beløb. Denne skat er i modsætning til den lave skat ikke endelig, og 
		/// der kan således foretages modregning heri af skatteværdi af personfradrag og 
		/// negativ skattepligtig indkomst. 
		/// </remarks>
		public decimal AktieindkomstskatOverGrundbeloebet { get; private set; }

		public decimal Aktieindkomstskat
		{
			get { return AktieindkomstskatUnderGrundbeloebet + AktieindkomstskatOverGrundbeloebet; }
		}

		public decimal Sum()
		{
			return Bundskat + Mellemskat + Topskat + Aktieindkomstskat;
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.AppendFormat("Bundskat: {0}", Bundskat);
			sb.Append(Environment.NewLine);
			sb.AppendFormat("Mellemskat: {0}", Mellemskat);
			sb.Append(Environment.NewLine); 
			sb.AppendFormat("Topskat: {0}", Topskat);
			sb.Append(Environment.NewLine);
			sb.AppendFormat("Aktieindkomstskat1: {0}", AktieindkomstskatUnderGrundbeloebet);
			sb.Append(Environment.NewLine); 
			sb.AppendFormat("Aktieindkomstskat2: {0}", AktieindkomstskatOverGrundbeloebet);
			return sb.ToString();
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = Bundskat.GetHashCode();
				hashCode = (hashCode * 397) ^ Mellemskat.GetHashCode();
				hashCode = (hashCode * 397) ^ Topskat.GetHashCode();
				hashCode = (hashCode * 397) ^ AktieindkomstskatUnderGrundbeloebet.GetHashCode();
				hashCode = (hashCode * 397) ^ AktieindkomstskatOverGrundbeloebet.GetHashCode();
				return hashCode;
			}
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as SkatterAfPersonligIndkomst);
		}

		public bool Equals(SkatterAfPersonligIndkomst other)
		{
			if (other == null)
			{
				return false;
			}

			return (Bundskat == other.Bundskat &&
				Mellemskat == other.Mellemskat &&
				Topskat == other.Topskat &&
				AktieindkomstskatUnderGrundbeloebet == other.AktieindkomstskatUnderGrundbeloebet &&
				AktieindkomstskatOverGrundbeloebet == other.AktieindkomstskatOverGrundbeloebet);
		}

		public static SkatterAfPersonligIndkomst operator +(SkatterAfPersonligIndkomst lhs, SkatterAfPersonligIndkomst rhs)
		{
			return new SkatterAfPersonligIndkomst
			{
				Bundskat = lhs.Bundskat + rhs.Bundskat,
				Mellemskat = lhs.Mellemskat + rhs.Mellemskat,
				Topskat = lhs.Topskat + rhs.Topskat,
				AktieindkomstskatUnderGrundbeloebet = lhs.AktieindkomstskatUnderGrundbeloebet + rhs.AktieindkomstskatUnderGrundbeloebet,
				AktieindkomstskatOverGrundbeloebet = lhs.AktieindkomstskatOverGrundbeloebet + rhs.AktieindkomstskatOverGrundbeloebet,
			};
		}

		public static SkatterAfPersonligIndkomst operator -(SkatterAfPersonligIndkomst lhs, SkatterAfPersonligIndkomst rhs)
		{
			return new SkatterAfPersonligIndkomst
			{
				Bundskat = lhs.Bundskat - rhs.Bundskat,
				Mellemskat = lhs.Mellemskat - rhs.Mellemskat,
				Topskat = lhs.Topskat - rhs.Topskat,
				AktieindkomstskatUnderGrundbeloebet = lhs.AktieindkomstskatUnderGrundbeloebet - rhs.AktieindkomstskatUnderGrundbeloebet,
				AktieindkomstskatOverGrundbeloebet = lhs.AktieindkomstskatOverGrundbeloebet - rhs.AktieindkomstskatOverGrundbeloebet
			};
		}

		public static SkatterAfPersonligIndkomst operator *(decimal lhs, SkatterAfPersonligIndkomst rhs)
		{
			return new SkatterAfPersonligIndkomst
			{
				Bundskat = lhs * rhs.Bundskat,
				Mellemskat = lhs * rhs.Mellemskat,
				Topskat = lhs * rhs.Topskat,
				AktieindkomstskatUnderGrundbeloebet = lhs * rhs.AktieindkomstskatUnderGrundbeloebet,
				AktieindkomstskatOverGrundbeloebet = lhs * rhs.AktieindkomstskatOverGrundbeloebet
			};
		}

		public static SkatterAfPersonligIndkomst operator *(SkatterAfPersonligIndkomst lhs, decimal rhs)
		{
			return rhs * lhs;
		}
	}
}