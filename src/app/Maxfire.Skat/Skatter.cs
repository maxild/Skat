using System;
using System.Text;
using Maxfire.Skat.Extensions;

namespace Maxfire.Skat
{
	// TODO: Rename to PersonSkatter (or IndkomstSkatter, or SkatterAfIndkomst)
	/// <summary>
	/// De samlede skatter
	/// </summary>
	public class Skatter : IEquatable<Skatter>, ISumable<decimal>
	{
		public static readonly Skatter Nul = new Skatter();
		
		public Skatter()
		{
		}

		public Skatter(decimal sundhedsbidrag = 0, decimal kommuneskat = 0, decimal kirkeskat = 0, 
			decimal bundskat = 0, decimal mellemskat = 0, decimal topskat = 0, 
			decimal aktieindkomstskatUnderGrundbeloebet = 0, decimal aktieindkomstskatOverGrundbeloebet = 0)
		{
			Sundhedsbidrag = sundhedsbidrag;
			Kommuneskat = kommuneskat;
			Kirkeskat = kirkeskat;
			Bundskat = bundskat;
			Mellemskat = mellemskat;
			Topskat = topskat;
			AktieindkomstskatUnderGrundbeloebet = aktieindkomstskatUnderGrundbeloebet;
			AktieindkomstskatOverGrundbeloebet = aktieindkomstskatOverGrundbeloebet;
		}

		public Skatter(SkatterAfPersonligIndkomst skatterAfPersonligIndkomst, SkatterAfSkattepligtigIndkomst skatterAfSkattepligtigIndkomst)
		{
			Sundhedsbidrag = skatterAfSkattepligtigIndkomst.Sundhedsbidrag;
			Kommuneskat = skatterAfSkattepligtigIndkomst.Kommuneskat;
			Kirkeskat = skatterAfSkattepligtigIndkomst.Kirkeskat;
			Bundskat = skatterAfPersonligIndkomst.Bundskat;
			Mellemskat = skatterAfPersonligIndkomst.Mellemskat;
			Topskat = skatterAfPersonligIndkomst.Topskat;
			AktieindkomstskatUnderGrundbeloebet = skatterAfPersonligIndkomst.AktieindkomstskatUnderGrundbeloebet;
			AktieindkomstskatOverGrundbeloebet = skatterAfPersonligIndkomst.AktieindkomstskatOverGrundbeloebet;
		}

		public decimal Sundhedsbidrag { get; private set; }

		public decimal Kirkeskat { get; private set; }

		public decimal Kommuneskat  { get; private set; }
		
		public decimal Bundskat  { get; private set; }
		
		public decimal Mellemskat { get; private set; }
		
		public decimal Topskat { get; private set; }
		
		public decimal KommunalIndkomstskatOgKirkeskat
		{
			get { return Kommuneskat + Kirkeskat; }
		}

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
			return Sundhedsbidrag + Bundskat + Mellemskat + Topskat 
				+ KommunalIndkomstskatOgKirkeskat + Aktieindkomstskat;
		}

		public Skatter RoundMoney()
		{
			return new Skatter
			{
				Kommuneskat = Kommuneskat.RoundMoney(),
				Kirkeskat = Kirkeskat.RoundMoney(),
				Sundhedsbidrag = Sundhedsbidrag.RoundMoney(),
				Bundskat = Bundskat.RoundMoney(),
				Mellemskat = Mellemskat.RoundMoney(),
				Topskat = Topskat.RoundMoney(),
				AktieindkomstskatUnderGrundbeloebet = AktieindkomstskatUnderGrundbeloebet.RoundMoney(),
				AktieindkomstskatOverGrundbeloebet = AktieindkomstskatOverGrundbeloebet.RoundMoney()
			};
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
			sb.Append(Environment.NewLine);
			sb.AppendFormat("Sundhedsbidrag: {0}", Sundhedsbidrag);
			sb.Append(Environment.NewLine);
			sb.AppendFormat("Kommuneskat: {0}", Kommuneskat);
			sb.Append(Environment.NewLine);
			sb.AppendFormat("Kirkeskat: {0}", Kirkeskat);
			return sb.ToString();
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = Kirkeskat.GetHashCode();
				hashCode = (hashCode * 397) ^ Kommuneskat.GetHashCode();
				hashCode = (hashCode * 397) ^ Sundhedsbidrag.GetHashCode();
				hashCode = (hashCode * 397) ^ Bundskat.GetHashCode();
				hashCode = (hashCode * 397) ^ Mellemskat.GetHashCode();
				hashCode = (hashCode * 397) ^ Topskat.GetHashCode();
				hashCode = (hashCode * 397) ^ AktieindkomstskatUnderGrundbeloebet.GetHashCode();
				hashCode = (hashCode * 397) ^ AktieindkomstskatOverGrundbeloebet.GetHashCode();
				return hashCode;
			}
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as Skatter);
		}

		public bool Equals(Skatter other)
		{
			if (other == null)
			{
				return false;
			}

			return (Kirkeskat == other.Kirkeskat &&
				Kommuneskat == other.Kommuneskat &&
				Sundhedsbidrag == other.Sundhedsbidrag &&
				Bundskat == other.Bundskat &&
				Mellemskat == other.Mellemskat &&
				Topskat == other.Topskat && 
				AktieindkomstskatUnderGrundbeloebet == other.AktieindkomstskatUnderGrundbeloebet &&
				AktieindkomstskatOverGrundbeloebet == other.AktieindkomstskatOverGrundbeloebet);
		}

		public static Skatter operator+(Skatter lhs, Skatter rhs)
		{
			return new Skatter
			{
				Kirkeskat = lhs.Kirkeskat + rhs.Kirkeskat,
				Kommuneskat = lhs.Kommuneskat + rhs.Kommuneskat,
				Sundhedsbidrag = lhs.Sundhedsbidrag + rhs.Sundhedsbidrag,
				Bundskat = lhs.Bundskat + rhs.Bundskat,
				Mellemskat = lhs.Mellemskat + rhs.Mellemskat,
				Topskat = lhs.Topskat + rhs.Topskat,
				AktieindkomstskatUnderGrundbeloebet = lhs.AktieindkomstskatUnderGrundbeloebet + rhs.AktieindkomstskatUnderGrundbeloebet,
				AktieindkomstskatOverGrundbeloebet = lhs.AktieindkomstskatOverGrundbeloebet + rhs.AktieindkomstskatOverGrundbeloebet,
			};
		}

		public static Skatter operator -(Skatter lhs, Skatter rhs)
		{
			return new Skatter
			{
				Kirkeskat = lhs.Kirkeskat - rhs.Kirkeskat,
				Kommuneskat = lhs.Kommuneskat - rhs.Kommuneskat,
				Sundhedsbidrag = lhs.Sundhedsbidrag - rhs.Sundhedsbidrag,
				Bundskat = lhs.Bundskat - rhs.Bundskat,
				Mellemskat = lhs.Mellemskat - rhs.Mellemskat,
				Topskat = lhs.Topskat - rhs.Topskat,
				AktieindkomstskatUnderGrundbeloebet = lhs.AktieindkomstskatUnderGrundbeloebet - rhs.AktieindkomstskatUnderGrundbeloebet,
				AktieindkomstskatOverGrundbeloebet = lhs.AktieindkomstskatOverGrundbeloebet - rhs.AktieindkomstskatOverGrundbeloebet
			};
		}

		public static Skatter operator *(decimal lhs, Skatter rhs)
		{
			return new Skatter
			{
				Kirkeskat = lhs * rhs.Kirkeskat,
				Kommuneskat = lhs * rhs.Kommuneskat,
				Sundhedsbidrag = lhs * rhs.Sundhedsbidrag,
				Bundskat = lhs * rhs.Bundskat,
				Mellemskat = lhs * rhs.Mellemskat,
				Topskat = lhs * rhs.Topskat,
				AktieindkomstskatUnderGrundbeloebet = lhs * rhs.AktieindkomstskatUnderGrundbeloebet,
				AktieindkomstskatOverGrundbeloebet = lhs * rhs.AktieindkomstskatOverGrundbeloebet
			};
		}

		public static Skatter operator *(Skatter lhs, decimal rhs)
		{
			return rhs * lhs;
		}
	}
}