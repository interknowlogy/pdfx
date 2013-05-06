using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace PropertyDependencyFramework
{
	public static class PropertyNameResolver
	{
		public static string GetPropertyName(LambdaExpression propertyExpression)
		{
			MemberExpression memberExpr;
			switch (propertyExpression.Body.NodeType)
			{
				case ExpressionType.Convert:
					memberExpr = ((UnaryExpression)propertyExpression.Body).Operand as MemberExpression;
					break;
				case ExpressionType.MemberAccess:
					memberExpr = propertyExpression.Body as MemberExpression;
					break;
				default:
					return null;
			}

			if (memberExpr == null)
			{
				return null;
			}

			return memberExpr.Member.Name;
		}
	}
}
