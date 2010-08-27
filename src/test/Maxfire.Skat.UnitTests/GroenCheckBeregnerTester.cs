using System;
using Maxfire.Skat.Extensions;
using Maxfire.TestCommons.AssertExtensions;
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
				IValueTuple<IPersonligeBeloeb> indkomster, 
				decimal bundfradrag, 
				decimal positivNettoKapitalIndkomstGrundbeloeb)
			{
				return _topskattegrundlagProvider().ToTupleOfSize(indkomster.Size);
			}
		}

		class MockableGroenCheckBeregner : GroenCheckBeregner
		{
			private readonly Action<IValueTuple<IPersonligeBeloeb>, decimal, decimal> _action;

			public MockableGroenCheckBeregner(ISkattelovRegistry skattelovRegistry, Action<IValueTuple<IPersonligeBeloeb>, decimal, decimal> action) : base(skattelovRegistry)
			{
				_action = action;
			}

			protected override ValueTuple<decimal> GetTopskattegrundlag(
				IValueTuple<IPersonligeBeloeb> indkomster, 
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
			const int skatteAar = 2010;
			var beregner = new TestableGroenCheckBeregner(new FakeSkattelovRegistry(), () => 0);
			var indkomster = new FakePersonligeBeloeb().ToTupleOfSize(1);

			var aftrapning = beregner.BeregnAftrapning(indkomster, skatteAar);

			aftrapning[0].ShouldEqual(0);
		}

		[Fact]
		public void Aftrapning()
		{
			const int skatteAar = 2010;
			var beregner = new TestableGroenCheckBeregner(new FakeSkattelovRegistry(), () => 1000);
			var indkomster = new FakePersonligeBeloeb().ToTupleOfSize(1);

			var aftrapning = beregner.BeregnAftrapning(indkomster, skatteAar);

			aftrapning[0].ShouldEqual(75);
		}

		[Fact]
		public void FuldKompensation()
		{
			const int skatteAar = 2010;
			var beregner = new TestableGroenCheckBeregner(new FakeSkattelovRegistry(), () => 0);
			var personer = new Person(new DateTime(1970, 6, 3), 1).ToTuple<IPerson>();
			var indkomster = new FakePersonligeBeloeb().ToTuple();

			var aftrapning = beregner.BeregnKompensation(personer, indkomster, skatteAar);

			aftrapning[0].ShouldEqual(1600);
		}

		[Fact]
		public void DelvisKompensation()
		{
			const int skatteAar = 2010;
			var beregner = new TestableGroenCheckBeregner(new FakeSkattelovRegistry(), () => 1000);
			var personer = new Person(new DateTime(1970, 6, 3), 1).ToTuple<IPerson>();
			var indkomster = new FakePersonligeBeloeb().ToTuple();

			var aftrapning = beregner.BeregnKompensation(personer, indkomster, skatteAar);

			aftrapning[0].ShouldEqual(1525);
		}

		[Fact]
		public void TopskattegrundlagArguments()
		{
			const int skatteAar = 2010;
			var indkomster = new FakePersonligeBeloeb().ToTuple();
			var beregner = new MockableGroenCheckBeregner(new FakeSkattelovRegistry(), (i, b, p) =>
			                                                                           	{
			                                                                           		ReferenceEquals(i, indkomster);
			                                                                           		b.ShouldEqual(362800);
			                                                                           		p.ShouldEqual(40000);
			                                                                           	});
			beregner.BeregnAftrapning(indkomster, skatteAar);
		}
	}
}