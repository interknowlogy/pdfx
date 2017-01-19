using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;

namespace PropertyDependencyFramework
{
    public interface IPropertyDependencyTypeRegistration
    {
        IPropertyDependencyTypeRegistration On<TSourceOwner, TProperty>(
            TSourceOwner sourceOwner,
            Expression<Func<TSourceOwner, INotifyPropertyChanged>> source,
            Expression<Func<TProperty>> sourceProperty);

        IPropertyDependencyTypeRegistration AndOn<TSourceOwner, TProperty>(
            TSourceOwner sourceOwner,
            Expression<Func<TSourceOwner, INotifyPropertyChanged>> source,
            Expression<Func<TProperty>> sourceProperty);


        IPropertyDependencyTypeRegistration OnCollectionChildProperty<TSourceOwner, TCollectionChildSource, TCollectionChildPropertyType>(
            TSourceOwner sourceOwner,
            Expression<Func<TSourceOwner, DependencyFrameworkObservableCollection<TCollectionChildSource>>> sourceCollection,
            Expression<Func<TCollectionChildSource, TCollectionChildPropertyType>> collectionChildProperty)
            where TCollectionChildSource : INotifyPropertyChanged;

        IPropertyDependencyTypeRegistration AndOnCollectionChildProperty<TSourceOwner, TCollectionChildSource, TCollectionChildPropertyType>(
            TSourceOwner sourceOwner,
            Expression<Func<TSourceOwner, DependencyFrameworkObservableCollection<TCollectionChildSource>>> sourceCollection,
            Expression<Func<TCollectionChildSource, TCollectionChildPropertyType>> collectionChildProperty)
            where TCollectionChildSource : INotifyPropertyChanged;

        IPropertyDependencyTypeRegistration OnCollectionChildProperty<TSourceOwner, TCollectionChildSource, TCollectionChildPropertyType>(
            TSourceOwner sourceOwner,
            Expression<Func<TSourceOwner, INotifyCollectionChanged>> sourceCollection,
            Expression<Func<TCollectionChildSource, TCollectionChildPropertyType>> collectionChildProperty)
            where TCollectionChildSource : INotifyPropertyChanged;

        IPropertyDependencyTypeRegistration AndOnCollectionChildProperty<TSourceOwner, TCollectionChildSource, TCollectionChildPropertyType>(
            TSourceOwner sourceOwner,
            Expression<Func<TSourceOwner, INotifyCollectionChanged>> sourceCollection,
            Expression<Func<TCollectionChildSource, TCollectionChildPropertyType>> collectionChildProperty)
            where TCollectionChildSource : INotifyPropertyChanged;
    }
}