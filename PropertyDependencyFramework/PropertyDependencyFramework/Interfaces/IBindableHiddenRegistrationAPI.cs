using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;

namespace PropertyDependencyFramework
{
	internal interface IBindableHiddenRegistrationAPI : INotifyPropertyChanged
	{
        void RegisterPropertyDependency<T, T1, T2>(T masterPropertyOwner, Expression<Func<T, T1>> masterProperty,
		                                           Expression<Func<T2>> dependentProperty) where T : INotifyPropertyChanged;

        void RegisterPropertyDependency(INotifyPropertyChanged masterPropertyOwner, string masterPropertyName, string dependantPropertyName);
		
		void RegisterPropertyDependency(INotifyCollectionChanged masterPropertyOwnerCollection, string masterPropertyName, string dependantPropertyName);

		void RegisterPropertyDependency<T, T1, T2>(DependencyFrameworkObservableCollection<T> masterPropertyOwnerCollection, Expression<Func<T, T1>> masterProperty, Expression<Func<T2>> dependentProperty) where T: INotifyPropertyChanged;

		void RegisterPropertyDependency<T, T1, T2>(INotifyCollectionChanged masterPropertyOwnerCollection, Expression<Func<T, T1>> masterProperty, Expression<Func<T2>> dependentProperty) where T : INotifyPropertyChanged;
		
		
		void RegisterCallbackDependency<T>(T masterPropertyOwner, Action callback) where T : INotifyPropertyChanged;

		void RegisterCallbackDependency<T>(T masterPropertyOwner, CallbackContainer callbackContainer) where T : INotifyPropertyChanged;

		void RegisterCallbackDependency<T, T1>(T masterPropertyOwner, Expression<Func<T, T1>> masterProperty, Action callback) where T : INotifyPropertyChanged;

		void RegisterCallbackDependency<T, T1>(T masterPropertyOwner, Expression<Func<T, T1>> masterProperty, CallbackContainer callbackContainer) where T : INotifyPropertyChanged;

		void RegisterCallbackDependency<T, T1>(T[] masterPropertyOwners, Expression<Func<T, T1>> masterProperty, Action callback) where T : INotifyPropertyChanged;

		void RegisterCallbackDependency<T>(T masterPropertyOwner, string masterPropertyName, CallbackContainer callbackContainer)
			where T : INotifyPropertyChanged;
		
		void RegisterCallbackDependency<T>(DependencyFrameworkObservableCollection<T> masterPropertyOwnerCollection, Action callback);

		void RegisterCallbackDependency<T, T1>(DependencyFrameworkObservableCollection<T> masterPropertyOwnerCollection, Expression<Func<T, T1>> masterProperty, Action callback) where T : INotifyPropertyChanged;
		
		void RegisterCallbackDependency<T, T1>(INotifyCollectionChanged masterPropertyOwnerCollection, Expression<Func<T, T1>> masterProperty, Action callback) where T : INotifyPropertyChanged;

		void RegisterCallbackDependency<T, T1>(INotifyCollectionChanged masterPropertyOwnerCollection, Expression<Func<T, T1>> masterProperty, CallbackContainer callbackContainer) where T : INotifyPropertyChanged;

		void RegisterCallbackDependency(INotifyCollectionChanged masterPropertyOwnerCollection, Action callback);

		void RegisterCallbackDependency(INotifyCollectionChanged masterPropertyOwnerCollection, CallbackContainer callbackContainer);		
		
		void RegisterCallbackDependency<T>(INotifyCollectionChanged masterPropertyOwnerCollection, string masterPropertyName, CallbackContainer callbackContainer)
			where T : INotifyPropertyChanged;


		void UnregisterAllPropertyDependencies();
		void UnRegisterPropertyDependency(INotifyCollectionChanged masterPropertyOwnerCollection, string masterPropertyName, string dependantPropertyName);
		void UnRegisterPropertyDependency(INotifyPropertyChanged masterPropertyOwner, string masterPropertyName, string dependantPropertyName);

	}
}