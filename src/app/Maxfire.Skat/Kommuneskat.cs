using Maxfire.Skat.Extensions;

namespace Maxfire.Skat
{	
	public class KommuneskatBeregner
	{
// ReSharper disable MemberCanBeMadeStatic.Global
		public ValueTuple<decimal> BeregnSkat(IValueTuple<IPersonligeBeloeb> indkomster, IValueTuple<IKommunaleSatser> kommunaleSatser)
// ReSharper restore MemberCanBeMadeStatic.Global
		{
			var skattepligtigIndkomst = indkomster.Map(x => x.Skattegrundlag.SkattepligtigIndkomst);
			var kommuneskattesats = kommunaleSatser.Map(x => x.Kommuneskattesats);
			return kommuneskattesats * (+skattepligtigIndkomst);
		}
	}

	public class KirkeskatBeregner
	{
// ReSharper disable MemberCanBeMadeStatic.Global
		public ValueTuple<decimal> BeregnSkat(IValueTuple<IPersonligeBeloeb> indkomster, IValueTuple<IKommunaleSatser> kommunaleSatser)
// ReSharper restore MemberCanBeMadeStatic.Global
		{
			var skattepligtigIndkomst = indkomster.Map(x => x.Skattegrundlag.SkattepligtigIndkomst);
			var kirkeskattesats = kommunaleSatser.Map(x => x.Kirkeskattesats);
			return kirkeskattesats * (+skattepligtigIndkomst);
		}
	}
}