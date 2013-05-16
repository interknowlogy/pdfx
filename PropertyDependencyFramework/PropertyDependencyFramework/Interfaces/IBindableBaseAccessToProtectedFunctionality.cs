using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace PropertyDependencyFramework.Interfaces
{
	public interface IBindableBaseAccessToProtectedFunctionality
	{
		Delegate[] TunnelledGetPropertyChangedInTransactionInvocationList();

		Delegate[] TunnelledGetPropertyChangedInvocationList();

		void TunnelledNotifyPropertyChanged(string propertyName);

		T TunnelledCachedValue<T>(Expression<Func<T>> ofProperty, Func<T> propertyEvaluation);

		void TunnelledRegisterPropertyDependency<T, T1, T2>(T masterPropertyOwner, Expression<Func<T, T1>> masterProperty,
		                                           Expression<Func<T2>> dependentProperty) where T : INotifyPropertyChanged;

		void TunnelledRegisterPropertyDependency<T, T1, T2>(DependencyFrameworkObservableCollection<T> masterPropertyOwnerCollection,
		                                           Expression<Func<T, T1>> masterProperty,
		                                           Expression<Func<T2>> dependentProperty) where T : INotifyPropertyChanged;


		void TunnelledRegisterCallbackDependency<T>(T masterPropertyOwner, Action callback)
			where T : INotifyPropertyChanged;

		void TunnelledRegisterCallbackDependency<T, T1>(T masterPropertyOwner, Expression<Func<T, T1>> masterProperty, Action callback)
			where T: INotifyPropertyChanged;

		void TunnelledRegisterCallbackDependency<T, T1>(T[] masterPropertyOwners, Expression<Func<T, T1>> masterProperty,
		                                       Action callback)
			where T : INotifyPropertyChanged;

		void TunnelledRegisterCallbackDependency<T>(DependencyFrameworkObservableCollection<T> masterPropertyOwnerCollection,
		                                   Action callback);

		void TunnelledRegisterCallbackDependency<T, T1>(DependencyFrameworkObservableCollection<T> masterPropertyOwnerCollection,
		                                       Expression<Func<T, T1>> masterProperty, Action callback)
			where T : INotifyPropertyChanged;

		void TunnelledUnregisterAllPropertyDependencies();

		void TunnelledUnRegisterPropertyDependency(INotifyCollectionChanged masterPropertyOwnerCollection, string masterPropertyName,
		                                  string dependantPropertyName);

		void TunnelledUnRegisterPropertyDependency(INotifyPropertyChanged masterPropertyOwner, string masterPropertyName,
		                                  string dependantPropertyName);

	}
}
