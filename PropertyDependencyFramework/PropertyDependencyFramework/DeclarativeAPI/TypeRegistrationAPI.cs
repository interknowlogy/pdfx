﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using PropertyDependencyFramework.Interfaces;

namespace PropertyDependencyFramework
{
    internal class TypeRegistrationAPI : ITypeRegistrationAPI
    {
        private readonly Dictionary<Type, TypeDependencies> _dependenciesByType = new Dictionary<Type, TypeDependencies>();
        public Dictionary<Type, TypeDependencies> DependenciesByType { get { return _dependenciesByType; } }

        public void RegisterPropertyDependencyForType<TSourceOwner, TSource, TSourceProp>(Expression<Func<TSourceOwner, TSource>> sourceExpression, Expression<Func<TSourceProp>> sourcePropertyExpression,
            string dependentPropertyName, Type dependentType)
            where TSource : INotifyPropertyChanged
        {
            Type sourceType = typeof(TSource);
            string sourcePropertyName = PropertyNameResolver.GetPropertyName(sourcePropertyExpression);


            if (!_dependenciesByType.ContainsKey(dependentType))
            {
                _dependenciesByType.Add(dependentType, new TypeDependencies());
            }

            TypeDependencies typeDependencies = _dependenciesByType[dependentType];

            if (!typeDependencies.SourceProviders.ContainsKey(sourceType))
            {
                typeDependencies.SourceProviders.Add(sourceType, new SourceProvider(sourceOwner => sourceExpression.Compile()((TSourceOwner)sourceOwner)));
            }

            SourceProvider sourceProvider = typeDependencies.SourceProviders[sourceType];

            if (!sourceProvider.SourceProperties.ContainsKey(sourcePropertyName))
            {
                sourceProvider.SourceProperties.Add(sourcePropertyName, new SourceProperty(sourcePropertyName));
            }

            SourceProperty sourceProperty = sourceProvider.SourceProperties[sourcePropertyName];

            sourceProperty.DependentPropertyNames.Add(dependentPropertyName);
        }
    }
}