using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Maxfire.Skat.Internal
{
	internal abstract class TextValuePairCollection<TValue> : ISumableEnumerable<TValue, decimal>
	{
		private readonly IEnumerable<TValue> _items;
		private readonly Func<TValue, decimal> _beloebSelector;

		protected TextValuePairCollection(IEnumerable<TValue> items, Func<TValue, decimal> beloebSelector)
		{
			_items = items;
			_beloebSelector = beloebSelector;
		}

		public decimal Sum()
		{
			return _items.Sum(_beloebSelector);
		}

		public IEnumerator<TValue> GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}