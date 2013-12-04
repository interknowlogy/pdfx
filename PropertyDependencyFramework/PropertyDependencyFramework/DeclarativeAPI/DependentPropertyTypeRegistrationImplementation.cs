﻿using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;
using PropertyDependencyFramework.Interfaces;

namespace PropertyDependencyFramework
{
    internal class DependentPropertyTypeRegistrationImplementation : IPropertyDependencyTypeRegistration, IDependentPropertyTypeRegistration
    {
        private readonly Type _dependentType;
        private readonly string _dependentPropertyName;
        private readonly ITypeRegistrationAPI _typeRegistrationApi;

        public DependentPropertyTypeRegistrationImplementation(Type dependentType, string dependentPropertyName, ITypeRegistrationAPI typeRegistrationApi)
        {
            _dependentType = dependentType;
            _dependentPropertyName = dependentPropertyName;
            _typeRegistrationApi = typeRegistrationApi;
        }

        public IPropertyDependencyTypeRegistration On<TSourceOwner, TSource, TProperty>(Expression<Func<TSourceOwner, TSource>> source, Expression<Func<TProperty>> sourceProperty) where TSource : INotifyPropertyChanged
        {
            return ThisDependsOn(source, sourceProperty);
        }

        private bool _registeredDependency;
        public IDependentPropertyTypeRegistration Depends(Action<IPropertyDependencyTypeRegistration> deferredPropDependency)
        {
            if (_registeredDependency)
                return this;

            deferredPropDependency(this);

            _registeredDependency = true;
            return this;
        }

        IPropertyDependencyTypeRegistration ThisDependsOn<TSourceOwner, TSource, TSourceProp>(Expression<Func<TSourceOwner, TSource>> source, Expression<Func<TSourceProp>> sourceProperty)
            where TSource : INotifyPropertyChanged
        {
            _typeRegistrationApi.RegisterPropertyDependencyForType(source, sourceProperty, _dependentPropertyName, _dependentType);

            return this;
        }
    }
}