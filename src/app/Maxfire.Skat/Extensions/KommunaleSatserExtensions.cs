namespace Maxfire.Skat.Extensions
{
	public static class KommunaleSatserExtensions
	{
		public static decimal GetKommuneOgKirkeskattesats(this IKommunaleSatser kommunaleSatser)
		{
			return kommunaleSatser.Kommuneskattesats + kommunaleSatser.Kirkeskattesats;
		}

		public static decimal GetKirkeskattesatsFor(this IKommunaleSatser kommunaleSatser, ISkatteyder skatteyder)
		{
			return skatteyder.MedlemAfFolkekirken == MedlemAfFolkekirken.Ja ? kommunaleSatser.Kirkeskattesats : 0m;
		}
	}
}