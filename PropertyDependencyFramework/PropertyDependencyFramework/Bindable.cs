using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using PropertyDependencyFramework.Interfaces;

namespace PropertyDependencyFramework
{
	public abstract class Bindable : BindableBase, IBindableAccessToProtectedFunctionality
	{
		protected Bindable()
		{
		}

		protected Bindable(bool useSmartPropertyChangeNotificationByDefault)
			: base(useSmartPropertyChangeNotificationByDefault)
		{
		}

		#region Declarative Property Dependency Registration API
		internal Dictionary<string, DependentPropertyImplementation> _properties = new Dictionary<string, DependentPropertyImplementation>();

		protected IDependentProperty Property<T>(Expression<Func<T>> property)
		{
			string dependantPropertyName = PropertyNameResolver.GetPropertyName(property);

			if (_properties.ContainsKey(dependantPropertyName) == false)
			{
				_properties.Add(dependantPropertyName, new DependentPropertyImplementation(dependantPropertyName, this));
			}

			return _properties[dependantPropertyName];
		}
		#endregion

		#region IBindableAccessToProtectedFunctionality
		IDependentProperty IBindableAccessToProtectedFunctionality.TunnelledProperty<T>(Expression<Func<T>> property)
		{
			return Property(property);
		}
		#endregion
	}
}
