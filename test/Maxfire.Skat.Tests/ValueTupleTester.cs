using Maxfire.Skat.Extensions;
using Xunit;
using Shouldly;

namespace Maxfire.Skat.UnitTests
{
	public class ValueTupleTester
	{
		[Fact]
		public void DifferentSign_SizeEqualsOne_ReturnsFalse()
		{
			new ValueTuple<decimal>(1).DifferentSign().ShouldBeFalse();
		}

		[Fact]
		public void DifferentSign_BothPositive_ReturnsFalse()
		{
			new ValueTuple<decimal>(1, 1).DifferentSign().ShouldBeFalse();
		}

		[Fact]
		public void DifferentSign_BothNegative_ReturnsFalse()
		{
			new ValueTuple<decimal>(-1, -1).DifferentSign().ShouldBeFalse();
		}

		[Fact]
		public void DifferentSign_OneZero_ReturnsFalse()
		{
			new ValueTuple<decimal>(0, 1).DifferentSign().ShouldBeFalse();
			new ValueTuple<decimal>(1, 0).DifferentSign().ShouldBeFalse();
		}

		[Fact]
		public void DifferentSign_BothZero_ReturnsFalse()
		{
			new ValueTuple<decimal>(0, 0).DifferentSign().ShouldBeFalse();
		}

		[Fact]
		public void DifferentSign_PositiveAndNegative_ReturnsTrue()
		{
			new ValueTuple<decimal>(1, -1).DifferentSign().ShouldBeTrue();
			new ValueTuple<decimal>(-1, 1).DifferentSign().ShouldBeTrue();
		}

		[Fact]
		public void NedbringPositivtMedEvtNegativt_FuldOverfoersel()
		{
			var tuple = new ValueTuple<decimal>(-3, 2);

			var modregnet = tuple.NedbringPositivtMedEvtNegativt();

			modregnet[0].ShouldBe(-1);
			modregnet[1].ShouldBe(0);
		}

		[Fact]
		public void NedbringPositivtMedEvtNegativt_DelvisOverfoersel()
		{
			var tuple = new ValueTuple<decimal>(-1, 2);

			var modregnet = tuple.NedbringPositivtMedEvtNegativt();

			modregnet[0].ShouldBe(0);
			modregnet[1].ShouldBe(1);
		}

		[Fact]
		public void NedbringPositivtMedEvtNegativt_IngenOverfoersel()
		{
			var tuple = new ValueTuple<decimal>(1, 2);
			tuple.NedbringPositivtMedEvtNegativt().ShouldBe(tuple);

			tuple = new ValueTuple<decimal>(-1, -2);
			tuple.NedbringPositivtMedEvtNegativt().ShouldBe(tuple);

			tuple = new ValueTuple<decimal>(0, 2);
			tuple.NedbringPositivtMedEvtNegativt().ShouldBe(tuple);

			tuple = new ValueTuple<decimal>(2, 0);
			tuple.NedbringPositivtMedEvtNegativt().ShouldBe(tuple);
		}

		[Fact]
		public void BeregnSambeskattetBundfradrag_OverfoerselHvorBundfradragErForLille()
		{
			var bruttoGrundlag = new ValueTuple<decimal>(1000, 500);
			var udnyttetBundfradrag = bruttoGrundlag.BeregnSambeskattetBundfradrag(600);

			udnyttetBundfradrag[0].ShouldBe(700);
			udnyttetBundfradrag[1].ShouldBe(500);
		}

		[Fact]
		public void BeregnSambeskattetBundfradrag_OverfoerselHvorBundfradragErTilpasStort()
		{
			var bruttoGrundlag = new ValueTuple<decimal>(1000, 500);
			var udnyttetBundfradrag = bruttoGrundlag.BeregnSambeskattetBundfradrag(750);

			udnyttetBundfradrag[0].ShouldBe(1000);
			udnyttetBundfradrag[1].ShouldBe(500);
		}

		[Fact]
		public void BeregnSambeskattetBundfradrag_OverfoerselHvorBundfradragErRigeligStort()
		{
			var bruttoGrundlag = new ValueTuple<decimal>(1000, 500);
			var udnyttetBundfradrag = bruttoGrundlag.BeregnSambeskattetBundfradrag(900);

			udnyttetBundfradrag[0].ShouldBe(1000);
			udnyttetBundfradrag[1].ShouldBe(800);
		}

		[Fact]
		public void BeregnSambeskattetBundfradrag_IngenOverfoersel()
		{
			var bruttoGrundlag = new ValueTuple<decimal>(1000, 500);
			var udnyttetBundfradrag = bruttoGrundlag.BeregnSambeskattetBundfradrag(400);

			udnyttetBundfradrag[0].ShouldBe(400);
			udnyttetBundfradrag[1].ShouldBe(400);
		}

		[Fact]
		public void NedbringMedSambeskattetBundfradrag_OverfoerselHvorBundfradragErTilpasStort()
		{
			var bruttoGrundlag = new ValueTuple<decimal>(200, 100);
			var grundlag = bruttoGrundlag.NedbringMedSambeskattetBundfradrag(150);

			grundlag[0].ShouldBe(0);
			grundlag[1].ShouldBe(0);
		}

		[Fact]
		public void NedbringMedSambeskattetBundfradrag_OverfoerselHvorBundfradragErForLille()
		{
			var bruttoGrundlag = new ValueTuple<decimal>(200, 100);
			var grundlag = bruttoGrundlag.NedbringMedSambeskattetBundfradrag(125);

			grundlag[0].ShouldBe(50);
			grundlag[1].ShouldBe(0);
		}

		[Fact]
		public void NedbringMedSambeskattetBundfradrag_OverfoerselHvorBundfradragErForStort()
		{
			var bruttoGrundlag = new ValueTuple<decimal>(200, 100);
			var grundlag = bruttoGrundlag.NedbringMedSambeskattetBundfradrag(175);

			grundlag[0].ShouldBe(0);
			grundlag[1].ShouldBe(-50);
		}

		[Fact]
		public void CanAddDecimals()
		{
			var lhs = new ValueTuple<decimal>(1, 2);
			var rhs = new ValueTuple<decimal>(2, 4);

			var result = lhs + rhs;

			result[0].ShouldBe(3);
			result[1].ShouldBe(6);
		}

		[Fact]
		public void CanSubtractDecimals()
		{
			var lhs = new ValueTuple<decimal>(1, 2);
			var rhs = new ValueTuple<decimal>(2, 4);

			var result = lhs - rhs;

			result[0].ShouldBe(-1);
			result[1].ShouldBe(-2);
		}

		[Fact]
		public void CanMultiplyDecimals()
		{
			var lhs = new ValueTuple<decimal>(1, 2);
			var rhs = new ValueTuple<decimal>(2, 4);

			var result = lhs * rhs;

			result[0].ShouldBe(2);
			result[1].ShouldBe(8);
		}

		[Fact]
		public void CanDivideDecimals()
		{
			var lhs = new ValueTuple<decimal>(1, 2);
			var rhs = new ValueTuple<decimal>(2, 4);

			var result = lhs / rhs;

			result[0].ShouldBe(0.5m);
			result[1].ShouldBe(0.5m);
		}

		[Fact]
		public void CanUnaryMinusDecimals()
		{
			var tuple = new ValueTuple<decimal>(1, 2);
			var result = -tuple;

			result[0].ShouldBe(-1);
			result[1].ShouldBe(-2);
		}

		[Fact]
		public void UnaryPlusMakesResultNonNegative()
		{
			var tuple = new ValueTuple<decimal>(-1, 2);
			var result = +tuple;

			result[0].ShouldBe(0);
			result[1].ShouldBe(2);
		}
	}
}
