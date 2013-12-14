using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;

namespace PropertyDependencyFramework
{
    public interface IPropertyDependencyTypeRegistration
    {
        IPropertyDependencyTypeRegistration On<TSourceOwner, TProperty>(TSourceOwner sourceOwner,
            Expression<Func<TSourceOwner, INotifyPropertyChanged>> source,
            Expression<Func<TProperty>> sourceProperty);

        IPropertyDependencyTypeRegistration AndOn<TSourceOwner, TProperty>(TSourceOwner sourceOwner,
            Expression<Func<TSourceOwner, INotifyPropertyChanged>> source,
            Expression<Func<TProperty>> sourceProperty);


        IPropertyDependencyTypeRegistration OnCollectionChildProperty<TSourceOwner, TCollectionGenericType, TCollectionChildPropertyType>(
            TSourceOwner sourceOwner,
            Expression<Func<TSourceOwner, DependencyFrameworkObservableCollection<TCollectionGenericType>>> sourceCollection,
            //DependencyFrameworkObservableCollection<TCollectionType> collection,
            Expression<Func<TCollectionGenericType, TCollectionChildPropertyType>> collectionChildProperty)
            where TCollectionGenericType : INotifyPropertyChanged;


        IPropertyDependencyTypeRegistration OnCollectionChildProperty<TSourceOwner, TCollectionGenericType, TCollectionChildPropertyType>(
            TSourceOwner sourceOwner,
            Expression<Func<TSourceOwner, INotifyCollectionChanged>> sourceCollection,
            //INotifyCollectionChanged collection,
            Expression<Func<TCollectionGenericType, TCollectionChildPropertyType>> collectionChildProperty)
            where TCollectionGenericType : INotifyPropertyChanged;
    }
}