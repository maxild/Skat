using System;
using System.Linq.Expressions;
using Maxfire.Skat.Reflection;

namespace Maxfire.Skat
{
	public static class Modregning<TSkatter>
	{
		public static Accessor<TSkatter, decimal> Af(Expression<Func<TSkatter, decimal>> expression)
		{
			return IntrospectionOf<TSkatter>.GetAccessorFor(expression);
		}
	}
}