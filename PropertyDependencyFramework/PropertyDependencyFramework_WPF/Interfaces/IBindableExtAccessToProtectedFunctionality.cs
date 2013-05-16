using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using PropertyDependencyFramework.Interfaces;

namespace PropertyDependencyFramework.DeclarativeAPI
{
	public interface IBindableExtAccessToProtectedFunctionality : IBindableBaseAccessToProtectedFunctionality
	{
		IDependentPropertyExt TunnelledProperty<T>(Expression<Func<T>> property);

		void TunnelledRegisterPropertyDependency<TOwner, TProperty, TDependantPropertyType>(Expression<Func<TOwner>> ownerProvider,
		                                                                           Expression<Func<TOwner, TProperty>>
			                                                                           property,
		                                                                           Expression<Func<TDependantPropertyType>>
			                                                                           dependantProperty)
			where TOwner : INotifyPropertyChanged;

		void TunnelledRegisterPropertyDependency<TCollectionType, TCollectionItemPropertyType, TProperty>(
			Expression<Func<DependencyFrameworkObservableCollection<TCollectionType>>> collectionPropertyGetter,
			Expression<Func<TCollectionType, TCollectionItemPropertyType>> collectionChildProperty,
			Expression<Func<TProperty>> dependentProperty)
			where TCollectionType : INotifyPropertyChanged;

		void TunnelledRegisterDeferredCallbackDependency<T, T1>(T masterPropertyOwner, Expression<Func<T, T1>> masterProperty,
		                                               Action callback)
			where T : INotifyPropertyChanged;

		void TunnelledRegisterDeferredCallbackDependency<T, T1>(T[] masterPropertyOwners, Expression<Func<T, T1>> masterProperty,
		                                               Action callback)
			where T : INotifyPropertyChanged;

		void TunnelledRegisterDeferredCallbackDependency<T, T1>(T masterPropertyOwner, Expression<Func<T, T1>> masterProperty,
		                                               Action callback, int delayInMilliseconds)
			where T : INotifyPropertyChanged;

		void TunnelledRegisterDeferredCallbackDependency<T>(T masterPropertyOwner, Action callback)
			where T : INotifyPropertyChanged;

		void TunnelledRegisterDeferredCallbackDependency<T>(T[] masterPropertyOwners, Action callback)
			where T : INotifyPropertyChanged;

		void TunnelledRegisterDeferredCallbackDependency<T>(T masterPropertyOwner, Action callback, int delayInMilliseconds)
			where T : INotifyPropertyChanged;

		void TunnelledRegisterDeferredCallbackDependency<T>(DependencyFrameworkObservableCollection<T> masterPropertyOwnerCollection,
		                                           Action callback);

		void TunnelledRegisterDeferredCallbackDependency<T, T1>(
			DependencyFrameworkObservableCollection<T> masterPropertyOwnerCollection, Expression<Func<T, T1>> masterProperty,
			Action callback)
			where T : INotifyPropertyChanged;
	}
}
