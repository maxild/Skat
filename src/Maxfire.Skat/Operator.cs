using System;
using System.Linq.Expressions;
using Maxfire.Skat.Reflection;

namespace Maxfire.Skat
{
	/// <summary>
	/// The Operator class provides easy access to the standard operators
	/// (addition, etc) for generic types, using type inference to simplify
	/// usage.
	/// </summary>
	public static class Operator
	{
		/// <summary>
		/// Indicates if the supplied value is non-null,
		/// for reference-types or Nullable&lt;T&gt;
		/// </summary>
		/// <returns>True for non-null values, else false</returns>
		public static bool HasValue<T>(T value)
		{
			return Operator<T>.NullOp.HasValue(value);
		}
		/// <summary>
		/// Increments the accumulator only
		/// if the value is non-null. If the accumulator
		/// is null, then the accumulator is given the new
		/// value; otherwise the accumulator and value
		/// are added.
		/// </summary>
		/// <param name="accumulator">The current total to be incremented (can be null)</param>
		/// <param name="value">The value to be tested and added to the accumulator</param>
		/// <returns>True if the value is non-null, else false - i.e.
		/// "has the accumulator been updated?"</returns>
		public static bool AddIfNotNull<T>(ref T accumulator, T value)
		{
			return Operator<T>.NullOp.AddIfNotNull(ref accumulator, value);
		}

		/// <summary>
		/// Evaluates unary negation (-) for the given type; this will throw
		/// an InvalidOperationException if the type T does not provide this operator, or for
		/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
		/// </summary>
		public static T Negate<T>(T value)
		{
			return Operator<T>.Negate(value);
		}
		/// <summary>
		/// Evaluates bitwise not (~) for the given type; this will throw
		/// an InvalidOperationException if the type T does not provide this operator, or for
		/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
		/// </summary>
		public static T Not<T>(T value)
		{
			return Operator<T>.Not(value);
		}
		/// <summary>
		/// Evaluates bitwise or (|) for the given type; this will throw
		/// an InvalidOperationException if the type T does not provide this operator, or for
		/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
		/// </summary>
		public static T Or<T>(T value1, T value2)
		{
			return Operator<T>.Or(value1, value2);
		}
		/// <summary>
		/// Evaluates bitwise and (&amp;) for the given type; this will throw
		/// an InvalidOperationException if the type T does not provide this operator, or for
		/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
		/// </summary>
		public static T And<T>(T value1, T value2)
		{
			return Operator<T>.And(value1, value2);
		}
		/// <summary>
		/// Evaluates bitwise xor (^) for the given type; this will throw
		/// an InvalidOperationException if the type T does not provide this operator, or for
		/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
		/// </summary>
		public static T Xor<T>(T value1, T value2)
		{
			return Operator<T>.Xor(value1, value2);
		}
		/// <summary>
		/// Performs a conversion between the given types; this will throw
		/// an InvalidOperationException if the type T does not provide a suitable cast, or for
		/// Nullable&lt;TInner&gt; if TInner does not provide this cast.
		/// </summary>
		public static TTo Convert<TFrom, TTo>(TFrom value)
		{
			return Operator<TFrom, TTo>.Convert(value);
		}
		/// <summary>
		/// Evaluates binary addition (+) for the given type; this will throw
		/// an InvalidOperationException if the type T does not provide this operator, or for
		/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
		/// </summary>
		public static T Add<T>(T value1, T value2)
		{
			return Operator<T>.Add(value1, value2);
		}
		/// <summary>
		/// Evaluates binary addition (+) for the given type(s); this will throw
		/// an InvalidOperationException if the type T does not provide this operator, or for
		/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
		/// </summary>
		public static TArg1 AddAlternative<TArg1, TArg2>(TArg1 value1, TArg2 value2)
		{
			return Operator<TArg2, TArg1>.Add(value1, value2);
		}
		/// <summary>
		/// Evaluates binary subtraction (-) for the given type; this will throw
		/// an InvalidOperationException if the type T does not provide this operator, or for
		/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
		/// </summary>
		public static T Subtract<T>(T value1, T value2)
		{
			return Operator<T>.Subtract(value1, value2);
		}
		/// <summary>
		/// Evaluates binary subtraction(-) for the given type(s); this will throw
		/// an InvalidOperationException if the type T does not provide this operator, or for
		/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
		/// </summary>
		public static TArg1 SubtractAlternative<TArg1, TArg2>(TArg1 value1, TArg2 value2)
		{
			return Operator<TArg2, TArg1>.Subtract(value1, value2);
		}
		/// <summary>
		/// Evaluates binary multiplication (*) for the given type; this will throw
		/// an InvalidOperationException if the type T does not provide this operator, or for
		/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
		/// </summary>
		public static T Multiply<T>(T value1, T value2)
		{
			return Operator<T>.Multiply(value1, value2);
		}
		/// <summary>
		/// Evaluates binary multiplication (*) for the given type(s); this will throw
		/// an InvalidOperationException if the type T does not provide this operator, or for
		/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
		/// </summary>
		public static TArg1 MultiplyAlternative<TArg1, TArg2>(TArg1 value1, TArg2 value2)
		{
			return Operator<TArg2, TArg1>.Multiply(value1, value2);
		}
		/// <summary>
		/// Evaluates binary division (/) for the given type; this will throw
		/// an InvalidOperationException if the type T does not provide this operator, or for
		/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
		/// </summary>
		public static T Divide<T>(T value1, T value2)
		{
			return Operator<T>.Divide(value1, value2);
		}
		/// <summary>
		/// Evaluates binary division (/) for the given type(s); this will throw
		/// an InvalidOperationException if the type T does not provide this operator, or for
		/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
		/// </summary>
		public static TArg1 DivideAlternative<TArg1, TArg2>(TArg1 value1, TArg2 value2)
		{
			return Operator<TArg2, TArg1>.Divide(value1, value2);
		}
		/// <summary>
		/// Evaluates binary equality (==) for the given type; this will throw
		/// an InvalidOperationException if the type T does not provide this operator, or for
		/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
		/// </summary>
		public static bool Equal<T>(T value1, T value2)
		{
			return Operator<T>.Equal(value1, value2);
		}
		/// <summary>
		/// Evaluates binary inequality (!=) for the given type; this will throw
		/// an InvalidOperationException if the type T does not provide this operator, or for
		/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
		/// </summary>
		public static bool NotEqual<T>(T value1, T value2)
		{
			return Operator<T>.NotEqual(value1, value2);
		}
		/// <summary>
		/// Evaluates binary greater-than (&gt;) for the given type; this will throw
		/// an InvalidOperationException if the type T does not provide this operator, or for
		/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
		/// </summary>
		public static bool GreaterThan<T>(T value1, T value2)
		{
			return Operator<T>.GreaterThan(value1, value2);
		}
		/// <summary>
		/// Evaluates binary less-than (&lt;) for the given type; this will throw
		/// an InvalidOperationException if the type T does not provide this operator, or for
		/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
		/// </summary>
		public static bool LessThan<T>(T value1, T value2)
		{
			return Operator<T>.LessThan(value1, value2);
		}
		/// <summary>
		/// Evaluates binary greater-than-on-eqauls (&gt;=) for the given type; this will throw
		/// an InvalidOperationException if the type T does not provide this operator, or for
		/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
		/// </summary>
		public static bool GreaterThanOrEqual<T>(T value1, T value2)
		{
			return Operator<T>.GreaterThanOrEqual(value1, value2);
		}
		/// <summary>
		/// Evaluates binary less-than-or-equal (&lt;=) for the given type; this will throw
		/// an InvalidOperationException if the type T does not provide this operator, or for
		/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
		/// </summary>
		public static bool LessThanOrEqual<T>(T value1, T value2)
		{
			return Operator<T>.LessThanOrEqual(value1, value2);
		}
		/// <summary>
		/// Evaluates integer division (/) for the given type; this will throw
		/// an InvalidOperationException if the type T does not provide this operator, or for
		/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
		/// </summary><remarks>
		/// This operation is particularly useful for computing averages and
		/// similar aggregates.
		/// </remarks>
		public static T DivideInt32<T>(T value, int divisor)
		{
			return Operator<int, T>.Divide(value, divisor);
		}
	}

	/// <summary>
	/// Provides standard operators (such as addition) that operate over operands of
	/// different types. For operators, the return type is assumed to match the first
	/// operand.
	/// </summary>
	/// <seealso cref="Operator&lt;T&gt;"/>
	/// <seealso cref="Operator"/>
	public static class Operator<TValue, TResult>
	{
		private static readonly Func<TValue, TResult> _convert;
		/// <summary>
		/// Returns a delegate to convert a value between two types; this delegate will throw
		/// an InvalidOperationException if the type T does not provide a suitable cast, or for
		/// Nullable&lt;TInner&gt; if TInner does not provide this cast.
		/// </summary>
		public static Func<TValue, TResult> Convert { get { return _convert; } }
		static Operator()
		{
			_convert = ExpressionHelper.CreateExpression<TValue, TResult>(body => Expression.Convert(body, typeof(TResult)));
			_add = ExpressionHelper.CreateExpression<TResult, TValue, TResult>(Expression.Add, true);
			_subtract = ExpressionHelper.CreateExpression<TResult, TValue, TResult>(Expression.Subtract, true);
			_multiply = ExpressionHelper.CreateExpression<TResult, TValue, TResult>(Expression.Multiply, true);
			_divide = ExpressionHelper.CreateExpression<TResult, TValue, TResult>(Expression.Divide, true);
		}

		private static readonly Func<TResult, TValue, TResult> _add, _subtract, _multiply, _divide;
		/// <summary>
		/// Returns a delegate to evaluate binary addition (+) for the given types; this delegate will throw
		/// an InvalidOperationException if the type T does not provide this operator, or for
		/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
		/// </summary>
		public static Func<TResult, TValue, TResult> Add { get { return _add; } }
		/// <summary>
		/// Returns a delegate to evaluate binary subtraction (-) for the given types; this delegate will throw
		/// an InvalidOperationException if the type T does not provide this operator, or for
		/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
		/// </summary>
		public static Func<TResult, TValue, TResult> Subtract { get { return _subtract; } }
		/// <summary>
		/// Returns a delegate to evaluate binary multiplication (*) for the given types; this delegate will throw
		/// an InvalidOperationException if the type T does not provide this operator, or for
		/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
		/// </summary>
		public static Func<TResult, TValue, TResult> Multiply { get { return _multiply; } }
		/// <summary>
		/// Returns a delegate to evaluate binary division (/) for the given types; this delegate will throw
		/// an InvalidOperationException if the type T does not provide this operator, or for
		/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
		/// </summary>
		public static Func<TResult, TValue, TResult> Divide { get { return _divide; } }
	}

	/// <summary>
	/// Provides standard operators (such as addition) over a single type
	/// </summary>
	/// <seealso cref="Operator"/>
	/// <seealso cref="Operator&lt;TValue,TResult&gt;"/>
	public static class Operator<T>
	{
		static readonly INullOp<T> _nullOp;
		internal static INullOp<T> NullOp { get { return _nullOp; } }

		static readonly T _zero;
		/// <summary>
		/// Returns the zero value for value-types (even full Nullable&lt;TInner&gt;) - or null for reference types
		/// </summary>
		public static T Zero { get { return _zero; } }

		static readonly Func<T, T> _negate, _not;
		static readonly Func<T, T, T> _or, _and, _xor;
		/// <summary>
		/// Returns a delegate to evaluate unary negation (-) for the given type; this delegate will throw
		/// an InvalidOperationException if the type T does not provide this operator, or for
		/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
		/// </summary>
		public static Func<T, T> Negate { get { return _negate; } }
		/// <summary>
		/// Returns a delegate to evaluate bitwise not (~) for the given type; this delegate will throw
		/// an InvalidOperationException if the type T does not provide this operator, or for
		/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
		/// </summary>
		public static Func<T, T> Not { get { return _not; } }
		/// <summary>
		/// Returns a delegate to evaluate bitwise or (|) for the given type; this delegate will throw
		/// an InvalidOperationException if the type T does not provide this operator, or for
		/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
		/// </summary>
		public static Func<T, T, T> Or { get { return _or; } }
		/// <summary>
		/// Returns a delegate to evaluate bitwise and (&amp;) for the given type; this delegate will throw
		/// an InvalidOperationException if the type T does not provide this operator, or for
		/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
		/// </summary>
		public static Func<T, T, T> And { get { return _and; } }
		/// <summary>
		/// Returns a delegate to evaluate bitwise xor (^) for the given type; this delegate will throw
		/// an InvalidOperationException if the type T does not provide this operator, or for
		/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
		/// </summary>
		public static Func<T, T, T> Xor { get { return _xor; } }

		static readonly Func<T, T, T> _add, _subtract, _multiply, _divide;
		/// <summary>
		/// Returns a delegate to evaluate binary addition (+) for the given type; this delegate will throw
		/// an InvalidOperationException if the type T does not provide this operator, or for
		/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
		/// </summary>
		public static Func<T, T, T> Add { get { return _add; } }
		/// <summary>
		/// Returns a delegate to evaluate binary subtraction (-) for the given type; this delegate will throw
		/// an InvalidOperationException if the type T does not provide this operator, or for
		/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
		/// </summary>
		public static Func<T, T, T> Subtract { get { return _subtract; } }
		/// <summary>
		/// Returns a delegate to evaluate binary multiplication (*) for the given type; this delegate will throw
		/// an InvalidOperationException if the type T does not provide this operator, or for
		/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
		/// </summary>
		public static Func<T, T, T> Multiply { get { return _multiply; } }
		/// <summary>
		/// Returns a delegate to evaluate binary division (/) for the given type; this delegate will throw
		/// an InvalidOperationException if the type T does not provide this operator, or for
		/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
		/// </summary>
		public static Func<T, T, T> Divide { get { return _divide; } }


		static readonly Func<T, T, bool> _equal, _notEqual, _greaterThan, _lessThan, _greaterThanOrEqual, _lessThanOrEqual;
		/// <summary>
		/// Returns a delegate to evaluate binary equality (==) for the given type; this delegate will throw
		/// an InvalidOperationException if the type T does not provide this operator, or for
		/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
		/// </summary>
		public static Func<T, T, bool> Equal { get { return _equal; } }
		/// <summary>
		/// Returns a delegate to evaluate binary inequality (!=) for the given type; this delegate will throw
		/// an InvalidOperationException if the type T does not provide this operator, or for
		/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
		/// </summary>
		public static Func<T, T, bool> NotEqual { get { return _notEqual; } }
		/// <summary>
		/// Returns a delegate to evaluate binary greater-then (&gt;) for the given type; this delegate will throw
		/// an InvalidOperationException if the type T does not provide this operator, or for
		/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
		/// </summary>
		public static Func<T, T, bool> GreaterThan { get { return _greaterThan; } }
		/// <summary>
		/// Returns a delegate to evaluate binary less-than (&lt;) for the given type; this delegate will throw
		/// an InvalidOperationException if the type T does not provide this operator, or for
		/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
		/// </summary>
		public static Func<T, T, bool> LessThan { get { return _lessThan; } }
		/// <summary>
		/// Returns a delegate to evaluate binary greater-than-or-equal (&gt;=) for the given type; this delegate will throw
		/// an InvalidOperationException if the type T does not provide this operator, or for
		/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
		/// </summary>
		public static Func<T, T, bool> GreaterThanOrEqual { get { return _greaterThanOrEqual; } }
		/// <summary>
		/// Returns a delegate to evaluate binary less-than-or-equal (&lt;=) for the given type; this delegate will throw
		/// an InvalidOperationException if the type T does not provide this operator, or for
		/// Nullable&lt;TInner&gt; if TInner does not provide this operator.
		/// </summary>
		public static Func<T, T, bool> LessThanOrEqual { get { return _lessThanOrEqual; } }


		public static Func<T, bool> GreaterThanZero
		{
			get { return x => _greaterThan(x, Zero); }
		}

		public static Func<T, bool> LessThanZero
		{
			get { return x => _lessThan(x, Zero); }
		}

		public static Func<T, bool> GreaterThanOrEqualToZero
		{
			get { return x => _greaterThanOrEqual(x, Zero); }
		}

		public static Func<T, bool> LessThanOrEqualToZero
		{
			get { return x => _lessThanOrEqual(x, Zero); }
		}

		public static Func<T, T, T> Max
		{
			get { return (x, y) => GreaterThanOrEqual(x, y) ? x : y; }
		}

		public static Func<T, T, T> Min
		{
			get { return (x, y) => LessThanOrEqual(x, y) ? x : y; }
		}

		public static Func<T, int> Sign
		{
			get
			{
				return x =>
				       	{
				       		T zero = Zero;
							if (GreaterThan(x, zero))
							{
								return 1;
							}
							if (LessThan(x, zero))
							{
								return -1;
							}
				       		return 0;
				       	};
			}
		}

		static Operator()
		{
			_add = ExpressionHelper.CreateExpression<T, T, T>(Expression.Add);
			_subtract = ExpressionHelper.CreateExpression<T, T, T>(Expression.Subtract);
			_divide = ExpressionHelper.CreateExpression<T, T, T>(Expression.Divide);
			_multiply = ExpressionHelper.CreateExpression<T, T, T>(Expression.Multiply);

			_greaterThan = ExpressionHelper.CreateExpression<T, T, bool>(Expression.GreaterThan);
			_greaterThanOrEqual = ExpressionHelper.CreateExpression<T, T, bool>(Expression.GreaterThanOrEqual);
			_lessThan = ExpressionHelper.CreateExpression<T, T, bool>(Expression.LessThan);
			_lessThanOrEqual = ExpressionHelper.CreateExpression<T, T, bool>(Expression.LessThanOrEqual);
			_equal = ExpressionHelper.CreateExpression<T, T, bool>(Expression.Equal);
			_notEqual = ExpressionHelper.CreateExpression<T, T, bool>(Expression.NotEqual);

			_negate = ExpressionHelper.CreateExpression<T, T>(Expression.Negate);
			_and = ExpressionHelper.CreateExpression<T, T, T>(Expression.And);
			_or = ExpressionHelper.CreateExpression<T, T, T>(Expression.Or);
			_not = ExpressionHelper.CreateExpression<T, T>(Expression.Not);
			_xor = ExpressionHelper.CreateExpression<T, T, T>(Expression.ExclusiveOr);

			Type typeT = typeof(T);
			if (typeT.IsValueType && typeT.IsGenericType && (typeT.GetGenericTypeDefinition() == typeof(Nullable<>)))
			{
				// get the *inner* zero (not a null Nullable<TValue>, but default(TValue))
				Type nullType = typeT.GetGenericArguments()[0];
				_zero = (T)Activator.CreateInstance(nullType);
				_nullOp = (INullOp<T>)Activator.CreateInstance(
				                     	typeof(StructNullOp<>).MakeGenericType(nullType));
			}
			else
			{
				_zero = default(T);
				if (typeT.IsValueType)
				{
					_nullOp = (INullOp<T>)Activator.CreateInstance(
					                     	typeof(StructNullOp<>).MakeGenericType(typeT));
				}
				else
				{
					_nullOp = (INullOp<T>)Activator.CreateInstance(
					                     	typeof(ClassNullOp<>).MakeGenericType(typeT));
				}
			}
		}
	}
}
