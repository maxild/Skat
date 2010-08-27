namespace Maxfire.Skat.Extensions
{
	public static class ModregnSkatterResultExtensions
	{
		public static ModregnSkatterResultEx<TSkatter> ToModregnSkatterResultEx<TSkatter>(this ModregnSkatterResult<TSkatter> modregnSkatterResult, ISkattevaerdiOmregner skattevaerdiOmregner)
			where TSkatter : ISumable<decimal>, new()
		{
			decimal underskud = skattevaerdiOmregner.BeregnFradragsbeloeb(modregnSkatterResult.Skattevaerdi);
			decimal udnyttetUnderskud = skattevaerdiOmregner.BeregnFradragsbeloeb(modregnSkatterResult.UdnyttetSkattevaerdi);

			return new ModregnSkatterResultEx<TSkatter>(modregnSkatterResult.Skatter, modregnSkatterResult.Skattevaerdi, 
			                          modregnSkatterResult.UdnyttedeSkattevaerdier, underskud, udnyttetUnderskud);
		}

		public static ValueTuple<ModregnSkatterResultEx<TSkatter>> ToModregnSkatterResultEx<TSkatter>(this ValueTuple<ModregnSkatterResult<TSkatter>> modregnSkatterResults, IValueTuple<ISkattevaerdiOmregner> skattevaerdiOmregnere)
			where TSkatter : ISumable<decimal>, new()
		{
			return modregnSkatterResults.Map((modregResult, index) => modregResult.ToModregnSkatterResultEx(skattevaerdiOmregnere[index]));
		}
	}
}