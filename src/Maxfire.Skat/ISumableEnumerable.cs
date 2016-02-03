using System.Collections.Generic;

namespace Maxfire.Skat
{
	public interface ISumableEnumerable<out TItem, out TValue>: IEnumerable<TItem>, ISumable<TValue>
	{
	}
}