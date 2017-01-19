using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;

namespace PropertyDependencyFramework.Interfaces
{
    public interface ITypeRegistrationAPI
    {
        Dictionary<Type, TypeDependencies> DependenciesByType { get; }
     
        void RegisterPropertyDependencyForType<TSourceOwner, TSource, TSourceProp>(
            TSourceOwner sourceOwner, 
            Expression<Func<TSourceOwner, TSource>> sourceExpression, 
            Expression<Func<TSourceProp>> sourcePropertyExpression, 
            string dependentPropertyName, 
            Type dependentType)
            where TSource : INotifyPropertyChanged;

        void RegisterPropertyDependencyForType<TSourceOwner, TCollectionType, TCollectionItemPropertyType>(
            TSourceOwner sourceOwner,
            Expression<Func<TSourceOwner, INotifyCollectionChanged>> sourceCollection,
            Expression<Func<TCollectionType, TCollectionItemPropertyType>> collectionChildProperty,
            string dependentPropertyName,
            Type dependentType)
            where TCollectionType : INotifyPropertyChanged;
    }
}