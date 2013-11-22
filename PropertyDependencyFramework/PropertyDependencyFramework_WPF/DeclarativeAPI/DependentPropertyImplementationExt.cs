using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;
using PropertyDependencyFramework.Interfaces;

namespace PropertyDependencyFramework.DeclarativeAPI
{
    internal class DependentPropertyImplementationExt : IPropertyDependencyExt, IDependentPropertyExt
    {
        readonly string _propertyName;
        readonly IBindableHiddenRegistrationAPIExt _propertyRegistration;

        public DependentPropertyImplementationExt(string propertyName, IBindableHiddenRegistrationAPIExt propertyRegistration)
        {
            _propertyName = propertyName;
            _propertyRegistration = propertyRegistration;
        }


        #region Base Functionality
        public IPropertyDependencyExt On<TOwner, TProperty>(TOwner owner, Expression<Func<TOwner, TProperty>> property) where TOwner : INotifyPropertyChanged
        {
            return ThisDependsOn(owner, property);
        }

        public IPropertyDependencyExt On<TOwner, TProperty>(TOwner[] owners, Expression<Func<TOwner, TProperty>> property) where TOwner : INotifyPropertyChanged
        {
            foreach (TOwner owner in owners)
                ThisDependsOn(owner, property);

            return this;
        }

        public IPropertyDependencyExt OnCollectionChildProperty<TCollectionType, TCollectionItemPropertyType>(DependencyFrameworkObservableCollection<TCollectionType> collection, Expression<Func<TCollectionType, TCollectionItemPropertyType>> collectionChildProperty) where TCollectionType : INotifyPropertyChanged
        {
            return OnCollectionChildProperty((INotifyCollectionChanged)collection, collectionChildProperty);
        }

        public IPropertyDependencyExt AndOnCollectionChildProperty<TCollectionType, TCollectionItemPropertyType>(DependencyFrameworkObservableCollection<TCollectionType> collection, Expression<Func<TCollectionType, TCollectionItemPropertyType>> collectionChildProperty) where TCollectionType : INotifyPropertyChanged
        {
            return OnCollectionChildProperty(collection, collectionChildProperty);
        }

        public IPropertyDependencyExt OnCollectionChildProperty<TCollectionType, TCollectionItemPropertyType>(ObservableCollection<TCollectionType> collection, Expression<Func<TCollectionType, TCollectionItemPropertyType>> collectionChildProperty) where TCollectionType : INotifyPropertyChanged
        {
            return OnCollectionChildProperty((INotifyCollectionChanged)collection, collectionChildProperty);
        }

        public IPropertyDependencyExt AndOnCollectionChildProperty<TCollectionType, TCollectionItemPropertyType>(ObservableCollection<TCollectionType> collection, Expression<Func<TCollectionType, TCollectionItemPropertyType>> collectionChildProperty) where TCollectionType : INotifyPropertyChanged
        {
            return OnCollectionChildProperty(collection, collectionChildProperty);
        }

        public IPropertyDependencyExt OnCollectionChildProperty<TCollectionType, TCollectionItemPropertyType>(INotifyCollectionChanged collection, Expression<Func<TCollectionType, TCollectionItemPropertyType>> collectionChildProperty) where TCollectionType : INotifyPropertyChanged
        {
            var masterPropertyName = PropertyNameResolver.GetPropertyName(collectionChildProperty);
            _propertyRegistration.RegisterPropertyDependency((INotifyCollectionChanged)collection, masterPropertyName, _propertyName);

            return this;
        }

        public IPropertyDependencyExt AndOnCollectionChildProperty<TCollectionType, TCollectionItemPropertyType>(INotifyCollectionChanged collection, Expression<Func<TCollectionType, TCollectionItemPropertyType>> collectionChildProperty) where TCollectionType : INotifyPropertyChanged
        {
            return OnCollectionChildProperty(collection, collectionChildProperty);
        }

        public IPropertyDependencyExt On<TProperty>(Expression<Func<TProperty>> property)
        {
            return ThisDependsOn(_propertyRegistration, property);
        }

        public IPropertyDependencyExt AndOn<TOwner, TProperty>(TOwner owner, Expression<Func<TOwner, TProperty>> property) where TOwner : INotifyPropertyChanged
        {
            return ThisDependsOn(owner, property);
        }

        public IPropertyDependencyExt AndOn<TOwner, TProperty>(TOwner[] owners, Expression<Func<TOwner, TProperty>> property) where TOwner : INotifyPropertyChanged
        {
            return On(owners, property);
        }

        public IPropertyDependencyExt AndOn<TProperty>(Expression<Func<TProperty>> property)
        {
            return ThisDependsOn(_propertyRegistration, property);
        }

        IPropertyDependencyExt ThisDependsOn<TOwner>(TOwner owner, LambdaExpression property) where TOwner : INotifyPropertyChanged
        {
            var masterPropertyName = PropertyNameResolver.GetPropertyName(property);
            _propertyRegistration.RegisterPropertyDependency(owner, masterPropertyName, _propertyName);

            return this;
        }

        bool _registeredDependency;
        public IDependentPropertyExt Depends(Action<IPropertyDependencyExt> deferredPropDependencyRegistration)
        {
            if (_registeredDependency)
                return this;

            deferredPropDependencyRegistration(this);

            _registeredDependency = true;
            return this;
        }
        #endregion

        #region Extended Functionality
        public IPropertyDependencyExt On<TOwner, TProperty>(Expression<Func<TOwner>> ownerProvider, Expression<Func<TOwner, TProperty>> property) where TOwner : INotifyPropertyChanged
        {
            return ThisDependsOn(ownerProvider, property);
        }

        public IPropertyDependencyExt AndOn<TOwner, TProperty>(Expression<Func<TOwner>> ownerProvider, Expression<Func<TOwner, TProperty>> property) where TOwner : INotifyPropertyChanged
        {
            return ThisDependsOn(ownerProvider, property);
        }

        public IPropertyDependencyExt OnCollectionChildProperty<TCollectionType, TCollectionItemPropertyType>(Expression<Func<DependencyFrameworkObservableCollection<TCollectionType>>> collectionPropertyGetter, Expression<Func<TCollectionType, TCollectionItemPropertyType>> collectionChildProperty) where TCollectionType : INotifyPropertyChanged
        {
            Func<DependencyFrameworkObservableCollection<TCollectionType>> compiledCollectionPropertyGetter = collectionPropertyGetter.Compile();

            string collectionPropertyName = PropertyNameResolver.GetPropertyName(collectionPropertyGetter);
            string collectionChildPropertyName = PropertyNameResolver.GetPropertyName(collectionChildProperty);

            _propertyRegistration.RegisterPropertyDependency(compiledCollectionPropertyGetter, collectionPropertyName, collectionChildPropertyName, _propertyName);

            return this;
        }

        public IPropertyDependencyExt AndOnCollectionChildProperty<TCollectionType, TCollectionItemPropertyType>(Expression<Func<DependencyFrameworkObservableCollection<TCollectionType>>> collectionPropertyGetter, Expression<Func<TCollectionType, TCollectionItemPropertyType>> collectionChildProperty) where TCollectionType : INotifyPropertyChanged
        {
            return OnCollectionChildProperty(collectionPropertyGetter, collectionChildProperty);
        }

        public IPropertyDependencyExt OnCollectionChildProperty<TCollectionType, TCollectionItemPropertyType>(Expression<Func<ObservableCollection<TCollectionType>>> collectionPropertyGetter, Expression<Func<TCollectionType, TCollectionItemPropertyType>> collectionChildProperty) where TCollectionType : INotifyPropertyChanged
        {
            Func<ObservableCollection<TCollectionType>> compiledCollectionPropertyGetter = collectionPropertyGetter.Compile();

            string collectionPropertyName = PropertyNameResolver.GetPropertyName(collectionPropertyGetter);
            string collectionChildPropertyName = PropertyNameResolver.GetPropertyName(collectionChildProperty);

            _propertyRegistration.RegisterPropertyDependency(compiledCollectionPropertyGetter, collectionPropertyName, collectionChildPropertyName, _propertyName);

            return this;
        }

        public IPropertyDependencyExt AndOnCollectionChildProperty<TCollectionType, TCollectionItemPropertyType>(Expression<Func<ObservableCollection<TCollectionType>>> collectionPropertyGetter, Expression<Func<TCollectionType, TCollectionItemPropertyType>> collectionChildProperty) where TCollectionType : INotifyPropertyChanged
        {
            return OnCollectionChildProperty(collectionPropertyGetter, collectionChildProperty);
        }

        public IPropertyDependencyExt OnCollectionChildProperty<TCollectionType, TCollectionItemPropertyType>(Expression<Func<INotifyCollectionChanged>> collectionPropertyGetter, Expression<Func<TCollectionType, TCollectionItemPropertyType>> collectionChildProperty) where TCollectionType : INotifyPropertyChanged
        {
            Func<INotifyCollectionChanged> compiledCollectionPropertyGetter = collectionPropertyGetter.Compile();

            string collectionPropertyName = PropertyNameResolver.GetPropertyName(collectionPropertyGetter);
            string collectionChildPropertyName = PropertyNameResolver.GetPropertyName(collectionChildProperty);

            _propertyRegistration.RegisterPropertyDependency(compiledCollectionPropertyGetter, collectionPropertyName, collectionChildPropertyName, _propertyName);

            return this;
        }

        public IPropertyDependencyExt AndOnCollectionChildProperty<TCollectionType, TCollectionItemPropertyType>(Expression<Func<INotifyCollectionChanged>> collectionPropertyGetter, Expression<Func<TCollectionType, TCollectionItemPropertyType>> collectionChildProperty) where TCollectionType : INotifyPropertyChanged
        {
            return OnCollectionChildProperty(collectionPropertyGetter, collectionChildProperty);
        }


        IPropertyDependencyExt ThisDependsOn<TOwner>(Expression<Func<TOwner>> ownerProvider, LambdaExpression property) where TOwner : INotifyPropertyChanged
        {
            var masterPropertyName = PropertyNameResolver.GetPropertyName(property);
            var masterPropertyOwnerPropertyGetterName = PropertyNameResolver.GetPropertyName(ownerProvider);

            var masterPropertyOwnerProvider = ownerProvider.Compile();
            Func<INotifyPropertyChanged> masterPropertyOwnerProviderConcrete = () => masterPropertyOwnerProvider();

            _propertyRegistration.RegisterPropertyDependency(masterPropertyOwnerPropertyGetterName, masterPropertyOwnerProviderConcrete, masterPropertyName, _propertyName);

            return this;
        }
        #endregion
    }

    internal class DependentPropertyImplementationForTypeExt : IPropertyDependencyForTypeExt, IDependentPropertyForTypeExt
    {
        readonly string _dependentPropertyName;
        readonly IBindableHiddenRegistrationAPIExt _propertyRegistrationApi;
        private readonly Type _dependentType;

        public DependentPropertyImplementationForTypeExt(string dependentPropertyName, IBindableHiddenRegistrationAPIExt propertyRegistrationApi, Type dependentType)
        {
            _dependentType = dependentType;
            _dependentPropertyName = dependentPropertyName;
            _propertyRegistrationApi = propertyRegistrationApi;
        }

        public IPropertyDependencyForTypeExt OnImmutableSource<TProperty>(Expression<Func<TProperty>> property)
        {
            throw new NotImplementedException();
        }

        public IDependentPropertyForTypeExt Depends(Action<IPropertyDependencyForTypeExt> deferredPropDependency)
        {
            throw new NotImplementedException();
        }

        IPropertyDependencyForTypeExt ThisDependsOn<TOwner>(LambdaExpression source, LambdaExpression sourceProperty) where TOwner : INotifyPropertyChanged
        {
            _propertyRegistrationApi.RegisterPropertyDependencyForType(source, sourceProperty, _dependentPropertyName, _dependentType);

            return this;
        }

    }
}
