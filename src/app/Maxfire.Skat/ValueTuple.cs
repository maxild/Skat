using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Maxfire.Core;
using Maxfire.Core.Extensions;

namespace Maxfire.Skat
{
	// The projection T --> IValueTuple<T> preserves the direction of assignment compatibility. That is
	// we can assign an IValueTuple<Derived> to a consumer expection an IValueTuple<Base>, because 
	// assignment compatibility is preserved.
	public interface IValueTuple<out T> : IEnumerable<T> 
	{
		T this[int index] { get; }
		T PartnerOf(int index);
		int Size { get; }
		bool AllZero();
		T Sum();
		IValueTuple<T> Swap();
	}

	/// <summary>
	/// In mathematics and computer science a tuple represents the notion of an ordered list of 
	/// elements. In set theory, an (ordered) n-tuple is a sequence (or ordered list) of n elements, 
	/// where n is a positive integer. There is also one 0-tuple, an empty sequence. An n-tuple is 
	/// defined inductively using the construction of an ordered pair.
	/// </summary>
	public class ValueTuple<T> : IValueTuple<T>, IEquatable<IValueTuple<T>>
	{
		private readonly IList<T> _list;

		public ValueTuple(T first)
		{
			_list = new List<T> { first };
		}

		public ValueTuple(T first, T second)
		{
			_list = new List<T> { first, second };
		}

		public ValueTuple(int size, Func<T> creator)
		{
			if (size != 1 && size != 2)
			{
				throw new ArgumentException("A ValueTuple must contain either one or two elements");
			}
			_list = new List<T>(size);
			for (int i = 0; i < size; i++)
			{
				_list.Add(creator());
			}
		}

		public ValueTuple(IList<T> list)
		{
			list.ThrowIfNull("list");
			if (list.Count != 1 && list.Count != 2)
			{
				throw new ArgumentException("A ValueTuple must contain either one or two elements");
			}
			_list = list;
		}

		/// <summary>
		/// Tuples are usually written by listing the elements within parentheses '( )' and 
		/// separated by commas; for example, (2, 7, 4, 1, 7) denotes a 5-tuple.
		/// </summary>
		public override string ToString()
		{
			return "(" + string.Join(", ", _list.Select(x => x.ToString()).ToArray()) + ")";
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = 1;
				for (int i = 0; i < Size; i++)
				{
					hashCode = (hashCode * 397) ^ this[i].GetHashCode();
				}
				return hashCode;
			}
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as ValueTuple<T>);
		}

		public bool Equals(IValueTuple<T> other)
		{
			if (other == null)
			{
				return false;
			}

			if (ReferenceEquals(this, other))
			{
				return true;
			}

			if (Size != other.Size)
			{
				return false;
			}

			for (int i = 0; i < Size; i++)
			{
				if (false == this[i].Equals(other[i]))
				{
					return false;
				}
			}

			return true;
		}

		public T this[int index]
		{
			get { return _list[index]; }
		}

		public int Size
		{
			get { return _list.Count; }
		}

		public bool AllZero()
		{
			for (int i = 0; i < Size; i++)
			{
				if (Operator<T>.Equal(this[i], Operator<T>.Zero))
				{
					return true;
				}
			}
			return false;
		}
		
		public T PartnerOf(int index)
		{
			if (index == 0 || index == 1)
			{
				return _list[index == 0 ? 1 : 0];
			}
			throw new IndexOutOfRangeException();
		}

		public T Sum()
		{
			T total = Operator<T>.Zero;
			for (int i = 0; i < Size; i++)
			{
				total = Operator<T>.Add(total, this[i]);
			}
			return total;
		}

		public ValueTuple<T> Swap()
		{
			return Size > 1 ? new ValueTuple<T>(this[1], this[0]) : this;
		}

		IValueTuple<T> IValueTuple<T>.Swap()
		{
			return Swap();
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// The unary plus (+) operator is overloaded to mean NonNegative.
		/// </summary>
		public static ValueTuple<T> operator +(ValueTuple<T> tuple)
		{
			return unaryOp(tuple, x =>
			                       	{
			                       		T zero = Operator<T>.Zero;
			                       		if (Operator<T>.LessThan(x, zero))
			                       		{
			                       			return zero;
			                       		}
			                       		return x;
			                       	});
		}

		public static ValueTuple<T> operator -(ValueTuple<T> tuple)
		{
			return unaryOp(tuple, Operator<T>.Negate);
		}

		private static ValueTuple<T> unaryOp(IValueTuple<T> tuple, Func<T, T> op)
		{
			var list = new List<T>(tuple.Size);
			for (int i = 0; i < tuple.Size; i++)
			{
				list.Add(op(tuple[i]));
			}

			return new ValueTuple<T>(list);
		}

		public static ValueTuple<T> operator +(ValueTuple<T> lhs, ValueTuple<T> rhs)
		{
			return binaryOp(lhs, rhs, Operator<T>.Add, "add");
		}

		public static ValueTuple<T> operator +(T lhs, ValueTuple<T> rhs)
		{
			return binaryOp(lhs, rhs, Operator<T>.Add);
		}

		public static ValueTuple<T> operator +(ValueTuple<T> lhs, T rhs)
		{
			return binaryOp(lhs, rhs, Operator<T>.Add);
		}

		public static ValueTuple<T> operator -(ValueTuple<T> lhs, ValueTuple<T> rhs)
		{
			return binaryOp(lhs, rhs, Operator<T>.Subtract, "subtract");
		}
		
		public static ValueTuple<T> operator -(T lhs, ValueTuple<T> rhs)
		{
			return binaryOp(lhs, rhs, Operator<T>.Subtract);
		}

		public static ValueTuple<T> operator -(ValueTuple<T> lhs, T rhs)
		{
			return binaryOp(lhs, rhs, Operator<T>.Subtract);
		}

		public static ValueTuple<T> operator *(ValueTuple<T> lhs, ValueTuple<T> rhs)
		{
			return binaryOp(lhs, rhs, Operator<T>.Multiply, "multiply");
		}

		public static ValueTuple<T> operator *(T lhs, ValueTuple<T> rhs)
		{
			return binaryOp(lhs, rhs, Operator<T>.Multiply);
		}

		public static ValueTuple<T> operator *(ValueTuple<T> lhs, T rhs)
		{
			return binaryOp(lhs, rhs, Operator<T>.Multiply);
		}
		
		public static ValueTuple<T> operator /(ValueTuple<T> lhs, ValueTuple<T> rhs)
		{
			return binaryOp(lhs, rhs, Operator<T>.Divide, "divide");
		}

		public static ValueTuple<T> operator /(T lhs, ValueTuple<T> rhs)
		{
			return binaryOp(lhs, rhs, Operator<T>.Divide);
		}

		public static ValueTuple<T> operator /(ValueTuple<T> lhs, T rhs)
		{
			return binaryOp(lhs, rhs, Operator<T>.Divide);
		}

		private static ValueTuple<T> binaryOp(IValueTuple<T> lhs, IValueTuple<T> rhs, Func<T, T, T> op, string s)
		{
			if (lhs.Size != rhs.Size)
			{
				throw new ArgumentException(string.Format("Cannot {0} tuples of different size.", s));
			}

			var list = new List<T>(lhs.Size);
			for (int i = 0; i < lhs.Size; i++)
			{
				list.Add(op(lhs[i], rhs[i]));
			}

			return new ValueTuple<T>(list);
		}

		private static ValueTuple<T> binaryOp(T lhs, IValueTuple<T> rhs, Func<T, T, T> op)
		{
			var list = new List<T>(rhs.Size);
			for (int i = 0; i < rhs.Size; i++)
			{
				list.Add(op(lhs, rhs[i]));
			}

			return new ValueTuple<T>(list);
		}

		private static ValueTuple<T> binaryOp(IValueTuple<T> lhs, T rhs, Func<T, T, T> op)
		{
			var list = new List<T>(lhs.Size);
			for (int i = 0; i < lhs.Size; i++)
			{
				list.Add(op(lhs[i], rhs));
			}

			return new ValueTuple<T>(list);
		}
	}
}