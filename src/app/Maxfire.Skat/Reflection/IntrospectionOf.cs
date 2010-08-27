using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Maxfire.Skat.Reflection
{
	public static class IntrospectionOf<TObject>
	{
		public static Getter<TObject, TPropertyValue> GetGetterFor<TPropertyValue>(Expression<Func<TObject, TPropertyValue>> expression)
		{
			var propertyInfo = GetProperty(expression);
			return new PropertyAccessor<TObject, TPropertyValue>(propertyInfo);
		}

		public static Setter<TObject, TPropertyValue> GetSetterFor<TPropertyValue>(Expression<Func<TObject, TPropertyValue>> expression)
		{
			var propertyInfo = GetProperty(expression);
			return new PropertyAccessor<TObject, TPropertyValue>(propertyInfo);
		}

		public static Accessor<TObject, TPropertyValue> GetAccessorFor<TPropertyValue>(Expression<Func<TObject, TPropertyValue>> expression)
		{
			var propertyInfo = GetProperty(expression);
			return new PropertyAccessor<TObject, TPropertyValue>(propertyInfo);
		}

		public static PropertyInfo GetProperty<TPropertyValue>(Expression<Func<TObject, TPropertyValue>> expression)
		{
			MemberExpression memberExpression = getMemberExpression(expression);
			return (PropertyInfo)memberExpression.Member;
		}

		private static MemberExpression getMemberExpression<TPropertyValue>(Expression<Func<TObject, TPropertyValue>> expression)
		{
			MemberExpression memberExpression = null;
			if (expression.Body.NodeType == ExpressionType.Convert)
			{
				var body = (UnaryExpression)expression.Body;
				memberExpression = body.Operand as MemberExpression;
			}
			else if (expression.Body.NodeType == ExpressionType.MemberAccess)
			{
				memberExpression = expression.Body as MemberExpression;
			}

			if (memberExpression == null)
			{
				throw new ArgumentException("Not a member access", "expression");
			}

			return memberExpression;
		}
	}
}