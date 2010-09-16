using System;
using Maxfire.Skat.Extensions;
using Maxfire.TestCommons.AssertExtensions;
using Xunit;

namespace Maxfire.Skat.UnitTests
{
	public class PersonfradragBeregnerTester
	{
		class FakeSkattelovRegistry : AbstractFakeSkattelovRegistry
		{
			public override decimal GetPersonfradrag(int skatteAar, int alder, bool gift)
			{
				return 100;
			}

			public override decimal GetSundhedsbidragSkattesats(int skatteAar)
			{
				return 0.1m;
			}

			public override decimal GetBundSkattesats(int skatteAar)
			{
				return 0.05m;
			}
		}

		private readonly PersonfradragBeregner _personfradragBeregner;

		public PersonfradragBeregnerTester()
		{
			_personfradragBeregner = new PersonfradragBeregner(new FakeSkattelovRegistry());
		}

		[Fact]
		public void BeregnSkattevaerdierAfPersonfradrag()
		{
			var skatteydere = new ValueTuple<ISkatteyder>(new Skatteyder(new DateTime(1970, 6, 3), medlemAfFolkekirken: true));

			var kommunaleSatser = new ValueTuple<IKommunaleSatser>(
				new KommunaleSatser
				{
					Kommuneskattesats = 0.25m,
					Kirkeskattesats = 0.01m
				});

			var skattevaerdier = _personfradragBeregner.BeregnSkattevaerdierAfPersonfradrag(skatteydere, kommunaleSatser, 2010);

			skattevaerdier[0].Sundhedsbidrag.ShouldEqual(10);
			skattevaerdier[0].Bundskat.ShouldEqual(5);
			skattevaerdier[0].Kommuneskat.ShouldEqual(25);
			skattevaerdier[0].Kirkeskat.ShouldEqual(1);
		}

		[Fact]
		public void BeregnSkattevaerdierAfPersonfradragIkkeMedlemAfFolkekirken()
		{
			var skatteydere = new ValueTuple<ISkatteyder>(new Skatteyder(new DateTime(1970, 6, 3), medlemAfFolkekirken: false));

			var kommunaleSatser = new ValueTuple<IKommunaleSatser>(
				new KommunaleSatser
				{
					Kommuneskattesats = 0.25m,
					Kirkeskattesats = 0.01m
				});

			var skattevaerdier = _personfradragBeregner.BeregnSkattevaerdierAfPersonfradrag(skatteydere, kommunaleSatser, 2010);

			skattevaerdier[0].Sundhedsbidrag.ShouldEqual(10);
			skattevaerdier[0].Bundskat.ShouldEqual(5);
			skattevaerdier[0].Kommuneskat.ShouldEqual(25);
			skattevaerdier[0].Kirkeskat.ShouldEqual(0);
		}

		[Fact]
		public void FuldUdnyttelseAfSkattevaerdiPaaSelveSkatten_Ugift()
		{
			var skatter = new ValueTuple<Skatter>(new Skatter(sundhedsbidrag: 100, kommuneskat: 500, bundskat: 200, kirkeskat: 50));

			var skatteydere = new ValueTuple<ISkatteyder>(new Skatteyder(new DateTime(1970, 6, 3), medlemAfFolkekirken: true));

			var kommunaleSatser = new ValueTuple<IKommunaleSatser>(
				new KommunaleSatser
				{
					Kommuneskattesats = 0.25m,
					Kirkeskattesats = 0.01m
				});

			var modregnResults = _personfradragBeregner.ModregningAfPersonfradrag(skatteydere, skatter, kommunaleSatser, 2010);

			var modregninger = modregnResults.Map(x => x.UdnyttedeSkattevaerdier);
			var modregnedeSkatter = modregnResults.Map(x => x.ModregnedeSkatter);

			modregnResults[0].UdnyttetFradrag.ShouldEqual(100);
			modregnResults[0].IkkeUdnyttetFradrag.ShouldEqual(0);
			modregnResults[0].UdnyttetSkattevaerdi.ShouldEqual(41);
			modregnResults[0].IkkeUdnyttetSkattevaerdi.ShouldEqual(0);

			modregninger[0].Sundhedsbidrag.ShouldEqual(10);
			modregninger[0].Bundskat.ShouldEqual(5);
			modregninger[0].Kommuneskat.ShouldEqual(25);
			modregninger[0].Kirkeskat.ShouldEqual(1);

			modregnedeSkatter[0].Sundhedsbidrag.ShouldEqual(90);
			modregnedeSkatter[0].Bundskat.ShouldEqual(195);
			modregnedeSkatter[0].Kommuneskat.ShouldEqual(475);
			modregnedeSkatter[0].Kirkeskat.ShouldEqual(49);
		}

		[Fact]
		public void DelvisUdnyttelseAfSkattevaerdiP�SelveSkatten_Ugift()
		{
			var skatter = new ValueTuple<Skatter>(new Skatter(sundhedsbidrag: 5, kommuneskat: 500, bundskat: 200, kirkeskat: 50));

			var skatteydere = new ValueTuple<ISkatteyder>(
				new Skatteyder(new DateTime(1970, 6, 3), medlemAfFolkekirken: true));

			var kommunaleSatser = new ValueTuple<IKommunaleSatser>(
				new KommunaleSatser
				{
					Kommuneskattesats = 0.25m,
					Kirkeskattesats = 0.01m
				});

			var modregnResults = _personfradragBeregner.ModregningAfPersonfradrag(skatteydere, skatter, kommunaleSatser, 2010);
			var modregnedeSkatter = modregnResults.Map(x => x.ModregnedeSkatter);

			// Resterende skattev�rdi af personfradrag mht. sundhedsbidrag p� 5 overv�ltes i reduktionen af bundskat
			modregnedeSkatter[0].Sundhedsbidrag.ShouldEqual(0);
			modregnedeSkatter[0].Bundskat.ShouldEqual(190); // <-- reduktion her
			modregnedeSkatter[0].Kommuneskat.ShouldEqual(475);
			modregnedeSkatter[0].Kirkeskat.ShouldEqual(49);
		}

		[Fact]
		public void ModregningAfPersonfradragEgneSkatter()
		{
			var skatter = new ValueTuple<Skatter>(new Skatter(sundhedsbidrag: 5, kommuneskat: 20, bundskat: 2));

			var skatteydere = new ValueTuple<ISkatteyder>(new Skatteyder(new DateTime(1970, 6, 3), medlemAfFolkekirken: true));

			var kommunaleSatser = new ValueTuple<IKommunaleSatser>(
				new KommunaleSatser
				{
					Kommuneskattesats = 0.25m,
					Kirkeskattesats = 0.01m
				});

			var modregnResults = _personfradragBeregner.ModregningAfPersonfradragEgneSkatter(skatteydere, skatter, kommunaleSatser, 2010);
			var modregnedeSkatter = modregnResults.Map(x => x.ModregnedeSkatter);
			var modregninger = modregnResults.Map(x => x.UdnyttedeSkattevaerdier);

			modregnResults[0].Skattevaerdi.ShouldEqual(41);
			modregnResults[0].IkkeUdnyttetSkattevaerdi.ShouldEqual(14);
			modregnResults[0].UdnyttetSkattevaerdi.ShouldEqual(27);

			modregninger.ShouldEqual(skatter);

			modregnedeSkatter[0].Sundhedsbidrag.ShouldEqual(0);
			modregnedeSkatter[0].Bundskat.ShouldEqual(0);
			modregnedeSkatter[0].Kommuneskat.ShouldEqual(0);
			modregnedeSkatter[0].Kirkeskat.ShouldEqual(0);
		}

		[Fact]
		public void DelvisUdnyttelseAfSkattevaerdierOgModregningHosAegtefaelle()
		{
			var skatter = new ValueTuple<Skatter>(
				new Skatter(sundhedsbidrag: 5, kommuneskat: 20, bundskat: 2),
				new Skatter(sundhedsbidrag: 100, kommuneskat: 500, bundskat: 200));

			var skatteydere = new ValueTuple<ISkatteyder>(
				new Skatteyder(new DateTime(1970, 6, 3), medlemAfFolkekirken: true),
				new Skatteyder(new DateTime(1970, 6, 3), medlemAfFolkekirken: false));

			var kommunaleSatser = new KommunaleSatser
			                      	{
			                      		Kommuneskattesats = 0.25m,
			                      		Kirkeskattesats = 0.01m
			                      	}.ToTupleOfSize(2);

			var modregnResults = _personfradragBeregner.ModregningAfPersonfradrag(skatteydere, skatter, kommunaleSatser, 2010);
			var modregnedeSkatter = modregnResults.Map(x => x.ModregnedeSkatter);

			// Skatterne nulstilles af v�rdien af personfradraget,
			modregnedeSkatter[0].Sundhedsbidrag.ShouldEqual(0);
			modregnedeSkatter[0].Bundskat.ShouldEqual(0);
			modregnedeSkatter[0].Kommuneskat.ShouldEqual(0);
			modregnedeSkatter[0].Kirkeskat.ShouldEqual(0);

			// og det uudnyttede personfradrag overf�res til �gtef�llen
			modregnedeSkatter[1].Sundhedsbidrag.ShouldEqual(90 - 3.42m);
			modregnedeSkatter[1].Bundskat.ShouldEqual(195 - 1.71m);
			modregnedeSkatter[1].Kommuneskat.ShouldEqual(475 - 8.54m);
			modregnedeSkatter[1].Kirkeskat.ShouldEqual(0);
		}
	}
}