using System;
using Maxfire.Skat.Beregnere;
using Maxfire.TestCommons.AssertExtensions;
using Xunit;

namespace Maxfire.Skat.UnitTests
{
	public class KirkeskatBeregnerTester
	{
		[Fact]
		public void BeregnSkat()
		{
			var skatteydere = new ValueTuple<ISkatteyder>(
				new Skatteyder(new DateTime(1970, 6, 3), MedlemAfFolkekirken.Ja)
			);

			var indkomster = new ValueTuple<ISkattepligtigeIndkomster>(
				new FakeSkattepligtigeIndkomster
				{
					SkattepligtigIndkomst = 100
				}
			);

			var kommunaleSatser = new ValueTuple<IKommunaleSatser>(
				new KommunaleSatser
				{
					Kirkeskattesats = 0.01m
				}
			);

			var beregner = new KirkeskatBeregner();

			var kommuneSkat = beregner.BeregnSkat(skatteydere, indkomster, kommunaleSatser);

			kommuneSkat[0].ShouldEqual(1);
		}


		[Fact]
		public void BeregnNulSkat()
		{
			var skatteydere = new ValueTuple<ISkatteyder>(
				new Skatteyder(new DateTime(1970, 6, 3), MedlemAfFolkekirken.Nej)
			);

			var indkomster = new ValueTuple<ISkattepligtigeIndkomster>(
				new FakeSkattepligtigeIndkomster
				{
					SkattepligtigIndkomst = 100
				}
			);

			var kommunaleSatser = new ValueTuple<IKommunaleSatser>(
				new KommunaleSatser
				{
					Kirkeskattesats = 0.01m
				}
			);

			var beregner = new KirkeskatBeregner();

			var kommuneSkat = beregner.BeregnSkat(skatteydere, indkomster, kommunaleSatser);

			kommuneSkat[0].ShouldEqual(0);
		}
	}
}