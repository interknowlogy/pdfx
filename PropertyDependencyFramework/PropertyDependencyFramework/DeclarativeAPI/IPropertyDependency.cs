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
    public interface IPropertyDependency
    {
        IPropertyDependency On<TOwner, TProperty>(TOwner owner, Expression<Func<TOwner, TProperty>> property)
                where TOwner : INotifyPropertyChanged;

        IPropertyDependency On<TOwner, TProperty>(TOwner[] owners, Expression<Func<TOwner, TProperty>> property)
                where TOwner : INotifyPropertyChanged;

        IPropertyDependency On<TProperty>(Expression<Func<TProperty>> property);

        IPropertyDependency AndOn<TOwner, TProperty>(TOwner owner, Expression<Func<TOwner, TProperty>> property)
            where TOwner : INotifyPropertyChanged;

        IPropertyDependency AndOn<TOwner, TProperty>(TOwner[] owners, Expression<Func<TOwner, TProperty>> property)
            where TOwner : INotifyPropertyChanged;

        IPropertyDependency AndOn<TProperty>(Expression<Func<TProperty>> property);


        IPropertyDependency OnCollectionChildProperty<TCollectionType, TCollectionItemPropertyType>(
            DependencyFrameworkObservableCollection<TCollectionType> collection,
            Expression<Func<TCollectionType, TCollectionItemPropertyType>> collectionChildProperty)
            where TCollectionType : INotifyPropertyChanged;

        IPropertyDependency AndOnCollectionChildProperty<TCollectionType, TCollectionItemPropertyType>(
            DependencyFrameworkObservableCollection<TCollectionType> collection,
            Expression<Func<TCollectionType, TCollectionItemPropertyType>> collectionChildProperty)
            where TCollectionType : INotifyPropertyChanged;

        IPropertyDependency OnCollectionChildProperty<TCollectionType, TCollectionItemPropertyType>(
            INotifyCollectionChanged collection,
            Expression<Func<TCollectionType, TCollectionItemPropertyType>> collectionChildProperty)
            where TCollectionType : INotifyPropertyChanged;

        IPropertyDependency AndOnCollectionChildProperty<TCollectionType, TCollectionItemPropertyType>(
            INotifyCollectionChanged collection,
            Expression<Func<TCollectionType, TCollectionItemPropertyType>> collectionChildProperty)
            where TCollectionType : INotifyPropertyChanged;

        IPropertyDependency OnCollectionChildProperty<TCollectionType, TCollectionItemPropertyType>(
            ObservableCollection<TCollectionType> collection,
            Expression<Func<TCollectionType, TCollectionItemPropertyType>> collectionChildProperty)
            where TCollectionType : INotifyPropertyChanged;

        IPropertyDependency AndOnCollectionChildProperty<TCollectionType, TCollectionItemPropertyType>(
            ObservableCollection<TCollectionType> collection,
            Expression<Func<TCollectionType, TCollectionItemPropertyType>> collectionChildProperty)
            where TCollectionType : INotifyPropertyChanged;
    }
}
