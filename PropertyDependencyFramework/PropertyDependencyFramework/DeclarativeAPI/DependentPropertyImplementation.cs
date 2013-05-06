using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace PropertyDependencyFramework
{
	internal class DependentPropertyImplementation : IPropertyDependency, IDependentProperty
	{
		protected internal readonly string _propertyName;
		protected internal readonly IBindableHiddenRegistrationAPI _propertyRegistration;

		public DependentPropertyImplementation(string propertyName, IBindableHiddenRegistrationAPI propertyRegistration)
		{
			_propertyName = propertyName;
			_propertyRegistration = propertyRegistration;
		}

		public IPropertyDependency On<TOwner, TProperty>(TOwner owner, Expression<Func<TOwner, TProperty>> property) where TOwner : INotifyPropertyChanged
		{
			return ThisDependsOn(owner, property);
		}

		public IPropertyDependency On<TOwner, TProperty>(TOwner[] owners, Expression<Func<TOwner, TProperty>> property) where TOwner : INotifyPropertyChanged
		{
			foreach (TOwner owner in owners)
				ThisDependsOn(owner, property);

			return this;
		}

		public IPropertyDependency OnCollectionChildProperty<TCollectionType, TCollectionItemPropertyType>(DependencyFrameworkObservableCollection<TCollectionType> collection, Expression<Func<TCollectionType, TCollectionItemPropertyType>> collectionChildProperty) where TCollectionType : INotifyPropertyChanged
		{
			var masterPropertyName = PropertyNameResolver.GetPropertyName(collectionChildProperty);
			_propertyRegistration.RegisterPropertyDependency((INotifyCollectionChanged)collection, masterPropertyName, _propertyName);

			return this;
		}

		public IPropertyDependency AndOnCollectionChildProperty<TCollectionType, TCollectionItemPropertyType>(DependencyFrameworkObservableCollection<TCollectionType> collection, Expression<Func<TCollectionType, TCollectionItemPropertyType>> collectionChildProperty) where TCollectionType : INotifyPropertyChanged
		{
			return OnCollectionChildProperty(collection, collectionChildProperty);
		}

		public IPropertyDependency On<TProperty>(Expression<Func<TProperty>> property)
		{
			return ThisDependsOn(_propertyRegistration, property);
		}

		public IPropertyDependency AndOn<TOwner, TProperty>(TOwner owner, Expression<Func<TOwner, TProperty>> property) where TOwner : INotifyPropertyChanged
		{
			return ThisDependsOn(owner, property);
		}

		public IPropertyDependency AndOn<TOwner, TProperty>(TOwner[] owners, Expression<Func<TOwner, TProperty>> property) where TOwner : INotifyPropertyChanged
		{
			return On(owners, property);
		}

		public IPropertyDependency AndOn<TProperty>(Expression<Func<TProperty>> property)
		{
			return ThisDependsOn(_propertyRegistration, property);
		}

		IPropertyDependency ThisDependsOn<TOwner>(TOwner owner, LambdaExpression property) where TOwner : INotifyPropertyChanged
		{
			var masterPropertyName = PropertyNameResolver.GetPropertyName(property);
			_propertyRegistration.RegisterPropertyDependency(owner, masterPropertyName, _propertyName);

			return this;
		}

		bool _registeredDependency;
		public IDependentProperty Depends(Action<IPropertyDependency> deferredPropDependencyRegistration)
		{
			if (_registeredDependency)
				return this;

			deferredPropDependencyRegistration(this);

			_registeredDependency = true;
			return this;
		}
	}
}
