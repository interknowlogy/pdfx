using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;

namespace PropertyDependencyFramework.Interfaces
{
	internal interface IBindableHiddenRegistrationAPIExt : IBindableHiddenRegistrationAPI
	{
		void RegisterPropertyDependency<TOwner, TProperty, TDependantPropertyType>(Expression<Func<TOwner>> ownerProvider, Expression<Func<TOwner, TProperty>> property, Expression<Func<TDependantPropertyType>> dependantProperty) where TOwner : INotifyPropertyChanged;

		void RegisterPropertyDependency<TCollectionType, TCollectionItemPropertyType, TProperty>(Expression<Func<DependencyFrameworkObservableCollection<TCollectionType>>> collectionPropertyGetter, Expression<Func<TCollectionType, TCollectionItemPropertyType>> collectionChildProperty, Expression<Func<TProperty>> dependentProperty) where TCollectionType : INotifyPropertyChanged;

		void RegisterPropertyDependency(string masterPropertyOwnerGetterPropertyName, Func<INotifyPropertyChanged> masterPropertyOwnerProvider, string masterPropertyName, string dependantPropertyName);

		void RegisterPropertyDependency(Func<INotifyCollectionChanged> masterPropertyOwnerCollectionGetter, string masterPropertyOwnerCollectionGetterName, string masterPropertyName, string dependentPropertyName);


		void RegisterDeferredCallbackDependency<T, T1>(INotifyCollectionChanged masterPropertyOwnerCollection, Expression<Func<T, T1>> masterProperty, Action callback) where T : INotifyPropertyChanged;

		void RegisterDeferredCallbackDependency<T, T1>(INotifyCollectionChanged masterPropertyOwnerCollection, Expression<Func<T, T1>> masterProperty, Action callback, int delayInMilliseconds) where T : INotifyPropertyChanged;

		void RegisterDeferredCallbackDependency(INotifyCollectionChanged masterPropertyOwnerCollection, Action callback, int delayInMilliseconds);
		
		void RegisterDeferredCallbackDependency<T, T1>(T masterPropertyOwner, Expression<Func<T, T1>> masterProperty, Action callback) where T : INotifyPropertyChanged;

		void RegisterDeferredCallbackDependency<T, T1>(T[] masterPropertyOwners, Expression<Func<T, T1>> masterProperty, Action callback) where T : INotifyPropertyChanged;

		void RegisterDeferredCallbackDependency<T, T1>(T masterPropertyOwner, Expression<Func<T, T1>> masterProperty, Action callback, int delayInMilliseconds) where T : INotifyPropertyChanged;

		void RegisterDeferredCallbackDependency<T>(T masterPropertyOwner, Action callback) where T : INotifyPropertyChanged;

		void RegisterDeferredCallbackDependency<T>(T[] masterPropertyOwners, Action callback) where T : INotifyPropertyChanged;

		void RegisterDeferredCallbackDependency<T>(T masterPropertyOwner, Action callback, int delayInMilliseconds) where T : INotifyPropertyChanged;
		
		void RegisterDeferredCallbackDependency<T>(DependencyFrameworkObservableCollection<T> masterPropertyOwnerCollection, Action callback);

		void RegisterDeferredCallbackDependency(INotifyCollectionChanged masterPropertyOwnerCollection, Action callback);

		void RegisterDeferredCallbackDependency<T, T1>(DependencyFrameworkObservableCollection<T> masterPropertyOwnerCollection, Expression<Func<T, T1>> masterProperty, Action callback) where T : INotifyPropertyChanged;
	}
}