using System;
using Maxfire.Skat.Extensions;

namespace Maxfire.Skat.Internal
{
	internal class DefaultSpecificeredeSkatteyder : ISpecificeredeSkatteyder
	{
		private readonly ISkatteyder _skatteyder;
		private readonly ISpecficeredeKommunaleSatser _kommunaleSatser;
		private readonly bool _gift;

		public DefaultSpecificeredeSkatteyder(
			ISkatteyder skatteyder, 
			ISpecficeredeKommunaleSatser kommunaleSatser, 
			bool gift)
		{
			_skatteyder = skatteyder;
			_kommunaleSatser = kommunaleSatser;
			_gift = gift;
		}

		public bool MedlemAfFolkekirken
		{
			get { return _skatteyder.MedlemAfFolkekirken; }
		}

		public DateTime Foedselsdato
		{
			get { return _skatteyder.Foedselsdato; }
		}

		public int AntalBoern
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

		public bool Gift
		{
			get { return _gift; }
		}
	}
}