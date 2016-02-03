using System;
using Maxfire.Skat.Extensions;

namespace Maxfire.Skat.Internal
{
	internal class DefaultSpecificeredeSkatteyder : ISpecificeredeSkatteyder
	{
		private readonly ISkatteyder _skatteyder;
		private readonly ISpecficeredeKommunaleSatser _kommunaleSatser;
		
		public DefaultSpecificeredeSkatteyder(
			ISkatteyder skatteyder, 
			ISpecficeredeKommunaleSatser kommunaleSatser, 
			bool gift)
		{
			_skatteyder = skatteyder;
			_kommunaleSatser = kommunaleSatser;
			Civilstand = gift ? Civilstand.Gift : Civilstand.Ugift;
		}

		public MedlemAfFolkekirken MedlemAfFolkekirken
		{
			get { return _skatteyder.MedlemAfFolkekirken; }
		}

		public DateTime Foedselsdato
		{
			get { return _skatteyder.Foedselsdato; }
		}

		public AntalBoern AntalBoern
		{
			get { return _skatteyder.AntalBoern; }
		}

		public string Skattekommune
		{
			get { return _kommunaleSatser.Skattekommune; }
		}

		public decimal Kommuneskattesats
		{
			get { return _kommunaleSatser.Kommuneskattesats; }
		}

		public decimal Kirkeskattesats
		{
			get { return _kommunaleSatser.GetKirkeskattesatsFor(_skatteyder); }
		}

		public Civilstand Civilstand { get; private set; }
	}
}