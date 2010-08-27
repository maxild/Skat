namespace Maxfire.Skat.Extensions
{
	public static class BeregnModregningerResultExtensions
	{
		public static ValueTuple<BeregnModregningerResult> SwapUnderskud(this ValueTuple<BeregnModregningerResult> result)
		{
			// Vi ombytter modregninger i underskud, men lader modregninger i skatter være
			return new ValueTuple<BeregnModregningerResult>(
				new BeregnModregningerResult(result[1].ModregningUnderskudSkattepligtigIndkomst, result[0].ModregningSkatter, result[1].ModregningUnderskud),
				new BeregnModregningerResult(result[0].ModregningUnderskudSkattepligtigIndkomst, result[1].ModregningSkatter, result[0].ModregningUnderskud)
				);
		}
		
		public static ValueTuple<ModregnUnderskudResult> ToModregnResult(this ValueTuple<BeregnModregningerResult> beregnModregningerResults, 
		                                                                 ValueTuple<SkatterAfPersonligIndkomst> skatter, ValueTuple<decimal> underskud)
		{
			return beregnModregningerResults.Map((result, index) =>
			                                     new ModregnUnderskudResult(underskud[index], result.ModregningUnderskud, result.ModregningUnderskudSkattepligtigIndkomst, skatter[index], result.ModregningSkatter));
		}
	}
}