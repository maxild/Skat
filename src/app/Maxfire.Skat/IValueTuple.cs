using System.Collections.Generic;

namespace Maxfire.Skat
{
	public interface IValueTuple<out T> : IEnumerable<T> 
	{
		T this[int index] { get; }
		T PartnerOf(int index);
		int Size { get; }
		bool AllZero();
		T Sum();
		IValueTuple<T> Swap();
	}
}