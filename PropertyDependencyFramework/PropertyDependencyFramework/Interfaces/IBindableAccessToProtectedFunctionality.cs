using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace PropertyDependencyFramework.Interfaces
{
	public interface IBindableAccessToProtectedFunctionality : IBindableBaseAccessToProtectedFunctionality
	{
		IDependentProperty TunnelledProperty<T>(Expression<Func<T>> property);
	}
}
