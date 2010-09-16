namespace Maxfire.Skat.Internal
{
	internal class DefaultSpecificeredeKommunaleSatser : ISpecficeredeKommunaleSatser
	{
		private readonly string _skatteKommune;
		private readonly IKommunaleSatser _kommunaleSatser;

		public DefaultSpecificeredeKommunaleSatser(IKommunaleSatser kommunaleSatser, string skatteKommune)
		{
			_kommunaleSatser = kommunaleSatser;
			_skatteKommune = skatteKommune;
		}

		public string Skattekommune
		{
			get { return _skatteKommune; }
		}

		public decimal Kommuneskattesats
		{
			get { return _kommunaleSatser.Kommuneskattesats; }
		}

		public decimal Kirkeskattesats
		{
			get { return _kommunaleSatser.Kirkeskattesats; }
		}
	}
}