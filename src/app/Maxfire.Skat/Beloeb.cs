using Maxfire.Core;

namespace Maxfire.Skat
{
	public static class Beloeb
	{
		public static ITextValuePair<decimal> Create(string text, decimal beloeb)
		{
			return new TextValuePair<decimal>(text, beloeb);
		}
	}
}