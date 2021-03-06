using System;
using Maxfire.Skat.Beregnere;
using Maxfire.Skat.Extensions;
using Shouldly;
using Xunit;

namespace Maxfire.Skat.UnitTests
{
	public class GroenCheckBeregnerTester
	{
		class FakeSkattelovRegistry : AbstractFakeSkattelovRegistry
		{
			public override decimal GetGroenCheckAftrapningssats(int skatteAar)
			{
				return 0.075m;
			}

			public override decimal GetGroenCheckPrVoksen(int skatteAar)
			{
				return 1300;
			}

			public override decimal GetGroenCheckPrBarn(int skatteAar)
			{
				return 300;
			}

			public override decimal GetGroenCheckBundfradrag(int skatteAar)
			{
				return 362800;
			}

			public override decimal GetPositivNettoKapitalIndkomstGrundbeloeb(int skatteAar)
			{
				return 40000;
			}
		}

		class TestableGroenCheckBeregner : GroenCheckBeregner
		{
			private readonly Func<decimal> _topskattegrundlagProvider;

			public TestableGroenCheckBeregner(ISkattelovRegistry skattelovRegistry, Func<decimal> topskattegrundlagProvider) : base(skattelovRegistry)
			{
				_topskattegrundlagProvider = topskattegrundlagProvider;
			}

			protected override ValueTuple<decimal> GetTopskattegrundlag(
				IValueTuple<IPersonligeIndkomster> indkomster, 
				decimal bundfradrag, 
				decimal positivNettoKapitalIndkomstGrundbeloeb)
			{
				return _topskattegrundlagProvider().ToTupleOfSize(indkomster.Size);
			}
		}

		class MockableGroenCheckBeregner : GroenCheckBeregner
		{
			private readonly Action<IValueTuple<IPersonligeIndkomster>, decimal, decimal> _action;

			public MockableGroenCheckBeregner(
				ISkattelovRegistry skattelovRegistry, 
				Action<IValueTuple<IPersonligeIndkomster>, decimal, decimal> action) : base(skattelovRegistry)
			{
				_action = action;
			}

			protected override ValueTuple<decimal> GetTopskattegrundlag(
				IValueTuple<IPersonligeIndkomster> indkomster, 
				decimal bundfradrag, 
				decimal positivNettoKapitalIndkomstGrundbeloeb)
			{
				_action(indkomster, bundfradrag, positivNettoKapitalIndkomstGrundbeloeb);
				return 0m.ToTupleOfSize(indkomster.Size);
			}
		}

		[Fact]
		public void IngenAftrapning()
		{
			const int SKATTE_AAR = 2010;
			var beregner = new TestableGroenCheckBeregner(new FakeSkattelovRegistry(), () => 0);
			var indkomster = new FakePersonligeIndkomster().ToTupleOfSize(1);

			var aftrapning = beregner.BeregnAftrapning(indkomster, SKATTE_AAR);

			aftrapning[0].ShouldBe(0);
		}

		[Fact]
		public void Aftrapning()
		{
			const int SKATTE_AAR = 2010;
			var beregner = new TestableGroenCheckBeregner(new FakeSkattelovRegistry(), () => 1000);
			var indkomster = new FakePersonligeIndkomster().ToTupleOfSize(1);

			var aftrapning = beregner.BeregnAftrapning(indkomster, SKATTE_AAR);

			aftrapning[0].ShouldBe(75);
		}

		[Fact]
		public void FuldKompensation()
		{
			const int SKATTE_AAR = 2010;
			var beregner = new TestableGroenCheckBeregner(new FakeSkattelovRegistry(), () => 0);
			var skatteydere = new Skatteyder(new DateTime(1970, 6, 3), MedlemAfFolkekirken.Nej, AntalBoern.Et).ToTuple<ISkatteyder>();
			var indkomster = new FakePersonligeIndkomster().ToTuple();

			var aftrapning = beregner.BeregnKompensation(skatteydere, indkomster, SKATTE_AAR);

			aftrapning[0].ShouldBe(1600);
		}

        [Fact]
		public void DelvisKompensation()
		{
			const int SKATTE_AAR = 2010;
			var beregner = new TestableGroenCheckBeregner(new FakeSkattelovRegistry(), () => 1000);
			var skatteydere = new Skatteyder(new DateTime(1970, 6, 3), MedlemAfFolkekirken.Nej, AntalBoern.Et).ToTuple<ISkatteyder>();
			var indkomster = new FakePersonligeIndkomster().ToTuple();

			var aftrapning = beregner.BeregnKompensation(skatteydere, indkomster, SKATTE_AAR);

			aftrapning[0].ShouldBe(1525);
		}

		[Fact]
		public void TopskattegrundlagArguments()
		{
			const int SKATTE_AAR = 2010;
			var indkomster = new FakePersonligeIndkomster().ToTuple();
			var beregner = new MockableGroenCheckBeregner(new FakeSkattelovRegistry(), (i, b, p) =>
			                                                                           	{
			                                                                           		ReferenceEquals(i, indkomster).ShouldBeTrue();
			                                                                           		b.ShouldBe(362800);
			                                                                           		p.ShouldBe(40000);
			                                                                           	});
			beregner.BeregnAftrapning(indkomster, SKATTE_AAR);
		}
	}
}
