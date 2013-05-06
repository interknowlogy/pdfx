using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq.Expressions;

namespace PropertyDependencyFramework.DeclarativeAPI
{
	public interface IPropertyDependencyExt
	{
		IPropertyDependencyExt On<TOwner, TProperty>(TOwner owner, Expression<Func<TOwner, TProperty>> property)
		where TOwner : INotifyPropertyChanged;

		IPropertyDependencyExt On<TOwner, TProperty>(TOwner[] owners, Expression<Func<TOwner, TProperty>> property)
where TOwner : INotifyPropertyChanged;

		IPropertyDependencyExt On<TProperty>(Expression<Func<TProperty>> property);

		IPropertyDependencyExt AndOn<TOwner, TProperty>(TOwner owner, Expression<Func<TOwner, TProperty>> property)
			where TOwner : INotifyPropertyChanged;

		IPropertyDependencyExt AndOn<TOwner, TProperty>(TOwner[] owners, Expression<Func<TOwner, TProperty>> property)
where TOwner : INotifyPropertyChanged;

		IPropertyDependencyExt AndOn<TProperty>(Expression<Func<TProperty>> property);


		IPropertyDependencyExt OnCollectionChildProperty<TCollectionType, TCollectionItemPropertyType>(
			DependencyFrameworkObservableCollection<TCollectionType> collection,
			Expression<Func<TCollectionType, TCollectionItemPropertyType>> collectionChildProperty)
			where TCollectionType : INotifyPropertyChanged;

		IPropertyDependencyExt AndOnCollectionChildProperty<TCollectionType, TCollectionItemPropertyType>(
			DependencyFrameworkObservableCollection<TCollectionType> collection,
			Expression<Func<TCollectionType, TCollectionItemPropertyType>> collectionChildProperty)
			where TCollectionType : INotifyPropertyChanged;



		IPropertyDependencyExt On<TOwner, TProperty>(Expression<Func<TOwner>> ownerProvider, Expression<Func<TOwner, TProperty>> property)
			where TOwner : INotifyPropertyChanged;

		IPropertyDependencyExt AndOn<TOwner, TProperty>(Expression<Func<TOwner>> ownerProvider, Expression<Func<TOwner, TProperty>> property)
			where TOwner : INotifyPropertyChanged;

		IPropertyDependencyExt OnCollectionChildProperty<TCollectionType, TCollectionItemPropertyType>(
			Expression<Func<ObservableCollection<TCollectionType>>> collectionPropertyGetter,
			Expression<Func<TCollectionType, TCollectionItemPropertyType>> collectionChildProperty) where TCollectionType : INotifyPropertyChanged;


		IPropertyDependencyExt AndOnCollectionChildProperty<TCollectionType, TCollectionItemPropertyType>(
			Expression<Func<ObservableCollection<TCollectionType>>> collectionPropertyGetter,
			Expression<Func<TCollectionType, TCollectionItemPropertyType>> collectionChildProperty) where TCollectionType : INotifyPropertyChanged;
	}
}