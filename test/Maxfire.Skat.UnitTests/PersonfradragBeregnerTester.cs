using System;
using Maxfire.Skat.Beregnere;
using Maxfire.Skat.Extensions;
using Shouldly;
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
			var skatteydere = new ValueTuple<ISkatteyder>(new Skatteyder(new DateTime(1970, 6, 3), MedlemAfFolkekirken.Ja));

			var kommunaleSatser = new ValueTuple<IKommunaleSatser>(
				new KommunaleSatser
				{
					Kommuneskattesats = 0.25m,
					Kirkeskattesats = 0.01m
				});

			var skattevaerdier = _personfradragBeregner.BeregnSkattevaerdierAfPersonfradrag(skatteydere, kommunaleSatser, 2010);

			skattevaerdier[0].Sundhedsbidrag.ShouldBe(10);
			skattevaerdier[0].Bundskat.ShouldBe(5);
			skattevaerdier[0].Kommuneskat.ShouldBe(25);
			skattevaerdier[0].Kirkeskat.ShouldBe(1);
		}

		[Fact]
		public void BeregnSkattevaerdierAfPersonfradragIkkeMedlemAfFolkekirken()
		{
			var skatteydere = new ValueTuple<ISkatteyder>(new Skatteyder(new DateTime(1970, 6, 3), MedlemAfFolkekirken.Nej));

			var kommunaleSatser = new ValueTuple<IKommunaleSatser>(
				new KommunaleSatser
				{
					Kommuneskattesats = 0.25m,
					Kirkeskattesats = 0.01m
				});

			var skattevaerdier = _personfradragBeregner.BeregnSkattevaerdierAfPersonfradrag(skatteydere, kommunaleSatser, 2010);

			skattevaerdier[0].Sundhedsbidrag.ShouldBe(10);
			skattevaerdier[0].Bundskat.ShouldBe(5);
			skattevaerdier[0].Kommuneskat.ShouldBe(25);
			skattevaerdier[0].Kirkeskat.ShouldBe(0);
		}

		[Fact]
		public void FuldUdnyttelseAfSkattevaerdiPaaSelveSkatten_Ugift()
		{
			var skatter = new ValueTuple<IndkomstSkatter>(new IndkomstSkatter(sundhedsbidrag: 100, kommuneskat: 500, bundskat: 200, kirkeskat: 50));

			var skatteydere = new ValueTuple<ISkatteyder>(new Skatteyder(new DateTime(1970, 6, 3), MedlemAfFolkekirken.Ja));

			var kommunaleSatser = new ValueTuple<IKommunaleSatser>(
				new KommunaleSatser
				{
					Kommuneskattesats = 0.25m,
					Kirkeskattesats = 0.01m
				});

			var modregnResults = _personfradragBeregner.ModregningAfPersonfradrag(skatteydere, skatter, kommunaleSatser, 2010);

			var modregninger = modregnResults.Map(x => x.UdnyttedeSkattevaerdier);
			var modregnedeSkatter = modregnResults.Map(x => x.ModregnedeSkatter);

			modregnResults[0].UdnyttetFradrag.ShouldBe(100);
			modregnResults[0].IkkeUdnyttetFradrag.ShouldBe(0);
			modregnResults[0].UdnyttetSkattevaerdi.ShouldBe(41);
			modregnResults[0].IkkeUdnyttetSkattevaerdi.ShouldBe(0);

			modregninger[0].Sundhedsbidrag.ShouldBe(10);
			modregninger[0].Bundskat.ShouldBe(5);
			modregninger[0].Kommuneskat.ShouldBe(25);
			modregninger[0].Kirkeskat.ShouldBe(1);

			modregnedeSkatter[0].Sundhedsbidrag.ShouldBe(90);
			modregnedeSkatter[0].Bundskat.ShouldBe(195);
			modregnedeSkatter[0].Kommuneskat.ShouldBe(475);
			modregnedeSkatter[0].Kirkeskat.ShouldBe(49);
		}

		[Fact]
		public void DelvisUdnyttelseAfSkattevaerdiPåSelveSkatten_Ugift()
		{
			var skatter = new ValueTuple<IndkomstSkatter>(new IndkomstSkatter(sundhedsbidrag: 5, kommuneskat: 500, bundskat: 200, kirkeskat: 50));

			var skatteydere = new ValueTuple<ISkatteyder>(
				new Skatteyder(new DateTime(1970, 6, 3), MedlemAfFolkekirken.Ja));

			var kommunaleSatser = new ValueTuple<IKommunaleSatser>(
				new KommunaleSatser
				{
					Kommuneskattesats = 0.25m,
					Kirkeskattesats = 0.01m
				});

			var modregnResults = _personfradragBeregner.ModregningAfPersonfradrag(skatteydere, skatter, kommunaleSatser, 2010);
			var modregnedeSkatter = modregnResults.Map(x => x.ModregnedeSkatter);

			// Resterende skatteværdi af personfradrag mht. sundhedsbidrag på 5 overvæltes i reduktionen af bundskat
			modregnedeSkatter[0].Sundhedsbidrag.ShouldBe(0);
			modregnedeSkatter[0].Bundskat.ShouldBe(190); // <-- reduktion her
			modregnedeSkatter[0].Kommuneskat.ShouldBe(475);
			modregnedeSkatter[0].Kirkeskat.ShouldBe(49);
		}

		[Fact]
		public void ModregningAfPersonfradragEgneSkatter()
		{
			var skatter = new ValueTuple<IndkomstSkatter>(new IndkomstSkatter(sundhedsbidrag: 5, kommuneskat: 20, bundskat: 2));

			var skatteydere = new ValueTuple<ISkatteyder>(new Skatteyder(new DateTime(1970, 6, 3), MedlemAfFolkekirken.Ja));

			var kommunaleSatser = new ValueTuple<IKommunaleSatser>(
				new KommunaleSatser
				{
					Kommuneskattesats = 0.25m,
					Kirkeskattesats = 0.01m
				});

			var modregnResults = _personfradragBeregner.ModregningAfPersonfradragEgneSkatter(skatteydere, skatter, kommunaleSatser, 2010);
			var modregnedeSkatter = modregnResults.Map(x => x.ModregnedeSkatter);
			var modregninger = modregnResults.Map(x => x.UdnyttedeSkattevaerdier);

			modregnResults[0].Skattevaerdi.ShouldBe(41);
			modregnResults[0].IkkeUdnyttetSkattevaerdi.ShouldBe(14);
			modregnResults[0].UdnyttetSkattevaerdi.ShouldBe(27);

			modregninger.ShouldBe(skatter);

			modregnedeSkatter[0].Sundhedsbidrag.ShouldBe(0);
			modregnedeSkatter[0].Bundskat.ShouldBe(0);
			modregnedeSkatter[0].Kommuneskat.ShouldBe(0);
			modregnedeSkatter[0].Kirkeskat.ShouldBe(0);
		}

		[Fact]
		public void DelvisUdnyttelseAfSkattevaerdierOgModregningHosAegtefaelle()
		{
			var skatter = new ValueTuple<IndkomstSkatter>(
				new IndkomstSkatter(sundhedsbidrag: 5, kommuneskat: 20, bundskat: 2),
				new IndkomstSkatter(sundhedsbidrag: 100, kommuneskat: 500, bundskat: 200));

			var skatteydere = new ValueTuple<ISkatteyder>(
				new Skatteyder(new DateTime(1970, 6, 3), MedlemAfFolkekirken.Ja),
				new Skatteyder(new DateTime(1970, 6, 3), MedlemAfFolkekirken.Nej));

			var kommunaleSatser = new KommunaleSatser
			                      	{
			                      		Kommuneskattesats = 0.25m,
			                      		Kirkeskattesats = 0.01m
			                      	}.ToTupleOfSize(2);

			var modregnResults = _personfradragBeregner.ModregningAfPersonfradrag(skatteydere, skatter, kommunaleSatser, 2010);
			var modregnedeSkatter = modregnResults.Map(x => x.ModregnedeSkatter);

			// Skatterne nulstilles af værdien af personfradraget,
			modregnedeSkatter[0].Sundhedsbidrag.ShouldBe(0);
			modregnedeSkatter[0].Bundskat.ShouldBe(0);
			modregnedeSkatter[0].Kommuneskat.ShouldBe(0);
			modregnedeSkatter[0].Kirkeskat.ShouldBe(0);

			// og det uudnyttede personfradrag overføres til ægtefællen
			modregnedeSkatter[1].Sundhedsbidrag.ShouldBe(90 - 3.42m);
			modregnedeSkatter[1].Bundskat.ShouldBe(195 - 1.71m);
			modregnedeSkatter[1].Kommuneskat.ShouldBe(475 - 8.54m);
			modregnedeSkatter[1].Kirkeskat.ShouldBe(0);
		}
	}
}
