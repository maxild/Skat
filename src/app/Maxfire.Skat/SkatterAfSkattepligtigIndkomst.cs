using System;
using System.Text;

namespace Maxfire.Skat
{
	/// <summary>
	/// Indkomst skatter til stat, kommune og kirke, der beregnes på baggrund af den skattepligtige indkomst.
	/// </summary>
	/// <remarks>
	/// Disse indkomstskatter beregnes først efter modregning og fremførsel af indeværende års underskud 
	/// i skattepligtig indkomst og tidligere års underskud i skattepligtig indkomst, jf. PSL § 13, stk. 1 og 2. 
	/// </remarks>
	public class SkatterAfSkattepligtigIndkomst : IEquatable<SkatterAfSkattepligtigIndkomst>, ISumable<decimal>
	{
		public static readonly SkatterAfSkattepligtigIndkomst Nul = new SkatterAfSkattepligtigIndkomst();

		public SkatterAfSkattepligtigIndkomst()
		{
		}

		public SkatterAfSkattepligtigIndkomst(decimal sundhedsbidrag=0, decimal kommuneskat=0, decimal kirkeskat=0)
		{
			Sundhedsbidrag = sundhedsbidrag;
			Kommuneskat = kommuneskat;
			Kirkeskat = kirkeskat;
		}

		public decimal Sundhedsbidrag { get; private set; }

		public decimal Kommuneskat { get; private set; }
		
		public decimal Kirkeskat { get; private set; }

		public decimal KommunalIndkomstskatOgKirkeskat
		{
			get { return Kommuneskat + Kirkeskat; }
		}

		public decimal Sum()
		{
			return Sundhedsbidrag + KommunalIndkomstskatOgKirkeskat;
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
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
				return hashCode;
			}
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as SkatterAfSkattepligtigIndkomst);
		}

		public bool Equals(SkatterAfSkattepligtigIndkomst other)
		{
			if (other == null)
			{
				return false;
			}

			return (Kirkeskat == other.Kirkeskat &&
				Kommuneskat == other.Kommuneskat &&
				Sundhedsbidrag == other.Sundhedsbidrag);
		}

		public static SkatterAfSkattepligtigIndkomst operator +(SkatterAfSkattepligtigIndkomst lhs, SkatterAfSkattepligtigIndkomst rhs)
		{
			return new SkatterAfSkattepligtigIndkomst
			{
				Kirkeskat = lhs.Kirkeskat + rhs.Kirkeskat,
				Kommuneskat = lhs.Kommuneskat + rhs.Kommuneskat,
				Sundhedsbidrag = lhs.Sundhedsbidrag + rhs.Sundhedsbidrag
			};
		}

		public static SkatterAfSkattepligtigIndkomst operator -(SkatterAfSkattepligtigIndkomst lhs, SkatterAfSkattepligtigIndkomst rhs)
		{
			return new SkatterAfSkattepligtigIndkomst
			{
				Kirkeskat = lhs.Kirkeskat - rhs.Kirkeskat,
				Kommuneskat = lhs.Kommuneskat - rhs.Kommuneskat,
				Sundhedsbidrag = lhs.Sundhedsbidrag - rhs.Sundhedsbidrag
			};
		}

		public static SkatterAfSkattepligtigIndkomst operator *(decimal lhs, SkatterAfSkattepligtigIndkomst rhs)
		{
			return new SkatterAfSkattepligtigIndkomst
			{
				Kirkeskat = lhs * rhs.Kirkeskat,
				Kommuneskat = lhs * rhs.Kommuneskat,
				Sundhedsbidrag = lhs * rhs.Sundhedsbidrag
			};
		}

		public static SkatterAfSkattepligtigIndkomst operator *(SkatterAfSkattepligtigIndkomst lhs, decimal rhs)
		{
			return rhs * lhs;
		}
	}
}