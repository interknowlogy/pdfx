using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;
using PropertyDependencyFramework.Interfaces;

namespace PropertyDependencyFramework
{
    internal class TypeRegistrationAPI : ITypeRegistrationAPI
    {
        private readonly Dictionary<Type, TypeDependencies> _dependenciesByType = new Dictionary<Type, TypeDependencies>();
        public Dictionary<Type, TypeDependencies> DependenciesByType { get { return _dependenciesByType; } }

        public void RegisterPropertyDependencyForType<TSourceOwner, TSource, TSourceProp>(
            TSourceOwner sourceOwner,
            Expression<Func<TSourceOwner, TSource>> sourceExpression,
            Expression<Func<TSourceProp>> sourcePropertyExpression,
            string dependentPropertyName,
            Type dependentType)
            where TSource : INotifyPropertyChanged
        {
            Func<object, INotifyPropertyChanged> sourceRetrievalFunc = sourceOwnerInstance => sourceExpression.Compile()((TSourceOwner)sourceOwnerInstance);

            Type sourceType = sourceRetrievalFunc(sourceOwner).GetType();
            string sourcePropertyName = PropertyNameResolver.GetPropertyName(sourcePropertyExpression);


            if (!_dependenciesByType.ContainsKey(dependentType))
            {
                _dependenciesByType.Add(dependentType, new TypeDependencies());
            }

            TypeDependencies typeDependencies = _dependenciesByType[dependentType];

            if (!typeDependencies.SourceProviders.ContainsKey(sourceType))
            {
                typeDependencies.SourceProviders.Add(sourceType, new SourceProvider(sourceRetrievalFunc));
            }

            SourceProvider sourceProvider = typeDependencies.SourceProviders[sourceType];

            if (!sourceProvider.SourceProperties.ContainsKey(sourcePropertyName))
            {
                sourceProvider.SourceProperties.Add(sourcePropertyName, new SourceProperty(sourcePropertyName));
            }

            SourceProperty sourceProperty = sourceProvider.SourceProperties[sourcePropertyName];

            sourceProperty.DependentPropertyNames.Add(dependentPropertyName);
        }

        public void RegisterPropertyDependencyForType<TSourceOwner, TCollectionType, TCollectionItemPropertyType>(
            TSourceOwner sourceOwner,
            Expression<Func<TSourceOwner, INotifyCollectionChanged>> sourceCollection,
            Expression<Func<TCollectionType, TCollectionItemPropertyType>> collectionChildProperty,
            string dependentPropertyName,
            Type dependentType)
            where TCollectionType : INotifyPropertyChanged
        {
            Func<object, INotifyCollectionChanged> sourceRetrievalFunc =
                sourceOwnerInstance => sourceCollection.Compile()((TSourceOwner) sourceOwnerInstance);

            Type sourceType = sourceRetrievalFunc(sourceOwner).GetType();
            string childSourcePropertyName = PropertyNameResolver.GetPropertyName(collectionChildProperty);

            if (!_dependenciesByType.ContainsKey(dependentType))
            {
                _dependenciesByType.Add(dependentType, new TypeDependencies());
            }

            TypeDependencies typeDependencies = _dependenciesByType[dependentType];

            if (!typeDependencies.CollectionSourceProviders.ContainsKey(sourceType))
            {
                typeDependencies.CollectionSourceProviders.Add(sourceType, new CollectionSourceProvider(sourceRetrievalFunc));
            }

            CollectionSourceProvider collectionSourceProvider = typeDependencies.CollectionSourceProviders[sourceType];

            if (!collectionSourceProvider.ChildSourceProperties.ContainsKey(childSourcePropertyName))
            {
                collectionSourceProvider.ChildSourceProperties.Add(childSourcePropertyName, new ChildSourceProperty(childSourcePropertyName));
            }

            ChildSourceProperty childSourceProperty =
                collectionSourceProvider.ChildSourceProperties[childSourcePropertyName];

            childSourceProperty.DependentPropertyNames.Add(dependentPropertyName);
        }
    }
}