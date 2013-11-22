using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using PropertyDependencyFramework.DeclarativeAPI;
using PropertyDependencyFramework.Interfaces;

namespace PropertyDependencyFramework
{
	public class BindableExt : BindableBase, IBindableHiddenRegistrationAPIExt, IBindableExtensionHook, IBindableExtAccessToProtectedFunctionality
	{
		private const int PropertyDeferredDependencyDelayInMilliseconds = 100;

		public BindableExt()
		{
		}

		#region Fields
		private Dictionary<string, DynamicPropertyDependencyRegistration> _dynamicPropertyDependencies = new Dictionary<string, DynamicPropertyDependencyRegistration>();
		#endregion

		#region Imperative Property Dependency Registration API

		#region Public API

		public BindableExt(bool useSmartPropertyChangeNotificationByDefault) : base(useSmartPropertyChangeNotificationByDefault)
		{
		}

		protected void RegisterPropertyDependency<TOwner, TProperty, TDependantPropertyType>(Expression<Func<TOwner>> ownerProvider,
																					 Expression<Func<TOwner, TProperty>> property,
																					 Expression<Func<TDependantPropertyType>> dependantProperty) 
							where TOwner : INotifyPropertyChanged
		{
			HiddenRegistrationAPI.RegisterPropertyDependency(ownerProvider, property, dependantProperty);
		}

		protected void RegisterPropertyDependency<TCollectionType, TCollectionItemPropertyType, TProperty>(Expression<Func<DependencyFrameworkObservableCollection<TCollectionType>>> collectionPropertyGetter,
																								   Expression<Func<TCollectionType, TCollectionItemPropertyType>> collectionChildProperty,
																								   Expression<Func<TProperty>> dependentProperty) 
							where TCollectionType : INotifyPropertyChanged
		{
			HiddenRegistrationAPI.RegisterPropertyDependency(collectionPropertyGetter, collectionChildProperty, dependentProperty);
		}


		protected void RegisterDeferredCallbackDependency<T, T1>(T masterPropertyOwner, Expression<Func<T, T1>> masterProperty, Action callback)
							where T : INotifyPropertyChanged
		{
			HiddenRegistrationAPI.RegisterDeferredCallbackDependency(masterPropertyOwner, masterProperty, callback);
		}

		protected void RegisterDeferredCallbackDependency<T, T1>(T[] masterPropertyOwners, Expression<Func<T, T1>> masterProperty, Action callback)
							where T : INotifyPropertyChanged
		{
			HiddenRegistrationAPI.RegisterDeferredCallbackDependency(masterPropertyOwners, masterProperty, callback);
		}

		protected void RegisterDeferredCallbackDependency<T, T1>(T masterPropertyOwner, Expression<Func<T, T1>> masterProperty, Action callback, int delayInMilliseconds)
							where T : INotifyPropertyChanged
		{
			HiddenRegistrationAPI.RegisterDeferredCallbackDependency(masterPropertyOwner, masterProperty, callback, delayInMilliseconds);
		}

		protected void RegisterDeferredCallbackDependency<T>(T masterPropertyOwner, Action callback)
							where T : INotifyPropertyChanged
		{
			HiddenRegistrationAPI.RegisterDeferredCallbackDependency(masterPropertyOwner, callback, PropertyDeferredDependencyDelayInMilliseconds);
		}

		protected void RegisterDeferredCallbackDependency<T>(T[] masterPropertyOwners, Action callback)
							where T : INotifyPropertyChanged
		{
			HiddenRegistrationAPI.RegisterDeferredCallbackDependency(masterPropertyOwners, callback);
		}

		protected void RegisterDeferredCallbackDependency<T>(T masterPropertyOwner, Action callback, int delayInMilliseconds)
							where T : INotifyPropertyChanged
		{
			HiddenRegistrationAPI.RegisterDeferredCallbackDependency(masterPropertyOwner, callback, delayInMilliseconds);
		}

		protected void RegisterDeferredCallbackDependency<T>(DependencyFrameworkObservableCollection<T> masterPropertyOwnerCollection, Action callback)
		{
			HiddenRegistrationAPI.RegisterDeferredCallbackDependency(masterPropertyOwnerCollection, callback);
		}

		protected void RegisterDeferredCallbackDependency<T>(ObservableCollection<T> masterPropertyOwnerCollection, Action callback)
		{
			HiddenRegistrationAPI.RegisterDeferredCallbackDependency((INotifyCollectionChanged)masterPropertyOwnerCollection, callback);
		}

		protected void RegisterDeferredCallbackDependency(INotifyCollectionChanged masterPropertyOwnerCollection, Action callback)
		{
			HiddenRegistrationAPI.RegisterDeferredCallbackDependency(masterPropertyOwnerCollection, callback);
		}

		protected void RegisterDeferredCallbackDependency<T, T1>(DependencyFrameworkObservableCollection<T> masterPropertyOwnerCollection, Expression<Func<T, T1>> masterProperty, Action callback)				
							where T : INotifyPropertyChanged
		{
			HiddenRegistrationAPI.RegisterDeferredCallbackDependency(masterPropertyOwnerCollection, masterProperty, callback);
		}

		protected void RegisterDeferredCallbackDependency<T, T1>(ObservableCollection<T> masterPropertyOwnerCollection, Expression<Func<T, T1>> masterProperty, Action callback)
					where T : INotifyPropertyChanged
		{
			HiddenRegistrationAPI.RegisterDeferredCallbackDependency((INotifyCollectionChanged)masterPropertyOwnerCollection, masterProperty, callback);
		}

		protected void RegisterDeferredCallbackDependency<T, T1>(INotifyCollectionChanged masterPropertyOwnerCollection, Expression<Func<T, T1>> masterProperty, Action callback)
			where T : INotifyPropertyChanged
		{
			HiddenRegistrationAPI.RegisterDeferredCallbackDependency(masterPropertyOwnerCollection, masterProperty, callback);
		}
		#endregion

		#region Hidden API for Declarative API

		private IBindableHiddenRegistrationAPIExt HiddenRegistrationAPI
		{
			get { return (IBindableHiddenRegistrationAPIExt)this; }
		}

		void IBindableHiddenRegistrationAPIExt.RegisterPropertyDependency<TOwner, TProperty, TDependantPropertyType>(Expression<Func<TOwner>> ownerProvider,
																	 Expression<Func<TOwner, TProperty>> property,
																	 Expression<Func<TDependantPropertyType>> dependantProperty)
		{
			var masterPropertyName = PropertyNameResolver.GetPropertyName(property);
			var masterPropertyOwnerPropertyGetterName = PropertyNameResolver.GetPropertyName(ownerProvider);
			var dependantPropertyName = PropertyNameResolver.GetPropertyName(dependantProperty);

			var masterPropertyOwnerProvider = ownerProvider.Compile();
			Func<INotifyPropertyChanged> masterPropertyOwnerProviderConcrete = () => masterPropertyOwnerProvider();

			HiddenRegistrationAPI.RegisterPropertyDependency(masterPropertyOwnerPropertyGetterName, masterPropertyOwnerProviderConcrete, masterPropertyName, dependantPropertyName);
		}

		void IBindableHiddenRegistrationAPIExt.RegisterPropertyDependency<TCollectionType, TCollectionItemPropertyType, TProperty>(Expression<Func<DependencyFrameworkObservableCollection<TCollectionType>>> collectionPropertyGetter,
																				   Expression<Func<TCollectionType, TCollectionItemPropertyType>> collectionChildProperty,
																				   Expression<Func<TProperty>> dependentProperty)
		{
			string collectionPropertyName = PropertyNameResolver.GetPropertyName(collectionPropertyGetter);
			string collectionChildPropertyName = PropertyNameResolver.GetPropertyName(collectionChildProperty);
			string dependentPropertyName = PropertyNameResolver.GetPropertyName(dependentProperty);

			HiddenRegistrationAPI.RegisterPropertyDependency(collectionPropertyGetter.Compile(), collectionPropertyName, collectionChildPropertyName, dependentPropertyName);
		}

		void IBindableHiddenRegistrationAPIExt.RegisterPropertyDependency(Func<INotifyCollectionChanged> masterPropertyOwnerCollectionGetter, string masterPropertyOwnerCollectionGetterName, string masterPropertyName, string dependentPropertyName)
		{
#if DEBUG
			if (ArePropertyDependencySanityChecksEnabled)
			{
				ValidatePropertyHasSetter(masterPropertyOwnerCollectionGetterName);
			}
#endif

			INotifyCollectionChanged currentMasterPropertyOwnerCollection = masterPropertyOwnerCollectionGetter();
			if (currentMasterPropertyOwnerCollection != null)
				HiddenRegistrationAPI.RegisterPropertyDependency(currentMasterPropertyOwnerCollection, masterPropertyName, dependentPropertyName);

			if (!_dynamicPropertyDependencies.ContainsKey(masterPropertyOwnerCollectionGetterName))
				_dynamicPropertyDependencies.Add(masterPropertyOwnerCollectionGetterName, new DynamicPropertyDependencyRegistration() { MasterPropertyOwnerGetterPropertyName = masterPropertyOwnerCollectionGetterName, MasterPropertyOwnerPropertyGetter = masterPropertyOwnerCollectionGetter });

			DynamicPropertyDependencyRegistration dynamicRegistration = _dynamicPropertyDependencies[masterPropertyOwnerCollectionGetterName];
			dynamicRegistration.OldMasterPropertyOwner = currentMasterPropertyOwnerCollection;
			dynamicRegistration.RegistrationOfPropertyDependencyActions.Add(newOwner => HiddenRegistrationAPI.RegisterPropertyDependency((INotifyCollectionChanged)newOwner, masterPropertyName, dependentPropertyName));
			dynamicRegistration.UnRegistrationOfPropertyDependencyActions.Add(newOwner => HiddenRegistrationAPI.UnRegisterPropertyDependency((INotifyCollectionChanged)newOwner, masterPropertyName, dependentPropertyName));
		}

		void IBindableHiddenRegistrationAPIExt.RegisterPropertyDependency(string masterPropertyOwnerGetterPropertyName, Func<INotifyPropertyChanged> masterPropertyOwnerProvider, string masterPropertyName, string dependantPropertyName)
		{
#if DEBUG
			if (ArePropertyDependencySanityChecksEnabled)
			{
				ValidatePropertyHasSetter(masterPropertyOwnerGetterPropertyName);
			}
#endif

			INotifyPropertyChanged currentMasterPropertyOwner = masterPropertyOwnerProvider();
			if (currentMasterPropertyOwner != null)
				HiddenRegistrationAPI.RegisterPropertyDependency(currentMasterPropertyOwner, masterPropertyName, dependantPropertyName);

			if (!_dynamicPropertyDependencies.ContainsKey(masterPropertyOwnerGetterPropertyName))
				_dynamicPropertyDependencies.Add(masterPropertyOwnerGetterPropertyName, new DynamicPropertyDependencyRegistration() { MasterPropertyOwnerGetterPropertyName = masterPropertyOwnerGetterPropertyName, MasterPropertyOwnerPropertyGetter = masterPropertyOwnerProvider });

			DynamicPropertyDependencyRegistration dynamicRegistration = _dynamicPropertyDependencies[masterPropertyOwnerGetterPropertyName];
			dynamicRegistration.OldMasterPropertyOwner = currentMasterPropertyOwner;
			dynamicRegistration.RegistrationOfPropertyDependencyActions.Add(newOwner => HiddenRegistrationAPI.RegisterPropertyDependency((INotifyPropertyChanged)newOwner, masterPropertyName, dependantPropertyName));
			dynamicRegistration.UnRegistrationOfPropertyDependencyActions.Add(newOwner => HiddenRegistrationAPI.UnRegisterPropertyDependency((INotifyPropertyChanged)newOwner, masterPropertyName, dependantPropertyName));
		}

		void IBindableHiddenRegistrationAPIExt.RegisterDeferredCallbackDependency<T, T1>(T masterPropertyOwner, Expression<Func<T, T1>> masterProperty, Action callback)
		{
			HiddenRegistrationAPI.RegisterDeferredCallbackDependency(masterPropertyOwner, masterProperty, callback, PropertyDeferredDependencyDelayInMilliseconds);
		}

		void IBindableHiddenRegistrationAPIExt.RegisterDeferredCallbackDependency<T, T1>(T[] masterPropertyOwners, Expression<Func<T, T1>> masterProperty, Action callback)
		{
			foreach (T masterPropertyOwner in masterPropertyOwners)
			{
				HiddenRegistrationAPI.RegisterDeferredCallbackDependency(masterPropertyOwner, masterProperty, callback);
			}
		}

		void IBindableHiddenRegistrationAPIExt.RegisterDeferredCallbackDependency<T, T1>(T masterPropertyOwner, Expression<Func<T, T1>> masterProperty, Action callback, int delayInMilliseconds)
		{
			HiddenRegistrationAPI.RegisterCallbackDependency(masterPropertyOwner, masterProperty, new DeferredCallbackContainer(callback, delayInMilliseconds));
		}

		void IBindableHiddenRegistrationAPIExt.RegisterDeferredCallbackDependency<T, T1>(INotifyCollectionChanged masterPropertyOwnerCollection, Expression<Func<T, T1>> masterProperty, Action callback)
		{
			HiddenRegistrationAPI.RegisterDeferredCallbackDependency(masterPropertyOwnerCollection, masterProperty, callback, PropertyDeferredDependencyDelayInMilliseconds);
		}

		void IBindableHiddenRegistrationAPIExt.RegisterDeferredCallbackDependency<T, T1>(INotifyCollectionChanged masterPropertyOwnerCollection, Expression<Func<T, T1>> masterProperty, Action callback, int delayInMilliseconds)
		{
			HiddenRegistrationAPI.RegisterCallbackDependency(masterPropertyOwnerCollection, masterProperty, new DeferredCallbackContainer(callback, delayInMilliseconds));
		}

		void IBindableHiddenRegistrationAPIExt.RegisterDeferredCallbackDependency(INotifyCollectionChanged masterPropertyOwnerCollection, Action callback, int delayInMilliseconds)
		{
			HiddenRegistrationAPI.RegisterCallbackDependency(masterPropertyOwnerCollection, new DeferredCallbackContainer(callback, delayInMilliseconds));
		}

		void IBindableHiddenRegistrationAPIExt.RegisterDeferredCallbackDependency<T>(T masterPropertyOwner, Action callback)
		{
			HiddenRegistrationAPI.RegisterDeferredCallbackDependency(masterPropertyOwner, callback, PropertyDeferredDependencyDelayInMilliseconds);
		}

		void IBindableHiddenRegistrationAPIExt.RegisterDeferredCallbackDependency<T>(T[] masterPropertyOwners, Action callback)
		{
			foreach (T masterPropertyOwner in masterPropertyOwners)
			{
				HiddenRegistrationAPI.RegisterDeferredCallbackDependency(masterPropertyOwner, callback, PropertyDeferredDependencyDelayInMilliseconds);
			}
		}

		void IBindableHiddenRegistrationAPIExt.RegisterDeferredCallbackDependency<T>(T masterPropertyOwner, Action callback, int delayInMilliseconds)
		{
			HiddenRegistrationAPI.RegisterCallbackDependency(masterPropertyOwner, new DeferredCallbackContainer(callback, delayInMilliseconds));
		}

		void IBindableHiddenRegistrationAPIExt.RegisterDeferredCallbackDependency<T>(DependencyFrameworkObservableCollection<T> masterPropertyOwnerCollection, Action callback)
		{
			HiddenRegistrationAPI.RegisterDeferredCallbackDependency((INotifyCollectionChanged)masterPropertyOwnerCollection, callback);
		}

		void IBindableHiddenRegistrationAPIExt.RegisterDeferredCallbackDependency(INotifyCollectionChanged masterPropertyOwnerCollection, Action callback)
		{
			HiddenRegistrationAPI.RegisterDeferredCallbackDependency(masterPropertyOwnerCollection, callback, PropertyDeferredDependencyDelayInMilliseconds);
		}

		void IBindableHiddenRegistrationAPIExt.RegisterDeferredCallbackDependency<T, T1>(DependencyFrameworkObservableCollection<T> masterPropertyOwnerCollection, Expression<Func<T, T1>> masterProperty,
								Action callback)
		{
			HiddenRegistrationAPI.RegisterDeferredCallbackDependency((INotifyCollectionChanged)masterPropertyOwnerCollection, masterProperty, callback);
		}
		#endregion

		#region BindableBase Hooks

		void IBindableExtensionHook.AfterUnregisterAllPropertyDependencies()
		{
			_dynamicPropertyDependencies.Clear();
		}

		#endregion

		#endregion

		#region Declarative Property Dependency Registration API
		internal Dictionary<string, DependentPropertyImplementationExt> _properties = new Dictionary<string, DependentPropertyImplementationExt>();
        protected IDependentPropertyExt Property<T>(Expression<Func<T>> property)
        {
            string dependantPropertyName = PropertyNameResolver.GetPropertyName(property);

#if DEBUG
            if (ArePropertyDependencySanityChecksEnabled)
            {
                PerformSanityChecksOnRequestedDeclarativePropertyRegistration(dependantPropertyName);
            }
#endif

            if (_properties.ContainsKey(dependantPropertyName) == false)
            {
                _properties.Add(dependantPropertyName, new DependentPropertyImplementationExt(dependantPropertyName, this));
            }

            return _properties[dependantPropertyName];
        }

        internal static Dictionary<string, DependentPropertyImplementationForTypeExt> _typeProperties = new Dictionary<string, DependentPropertyImplementationForTypeExt>();
        protected IDependentPropertyForTypeExt TypeProperty<T>(Expression<Func<T>> property)
        {
            string dependantPropertyName = PropertyNameResolver.GetPropertyName(property);

#if DEBUG
            if (ArePropertyDependencySanityChecksEnabled)
            {
                PerformSanityChecksOnRequestedDeclarativePropertyRegistration(dependantPropertyName);
            }
#endif

            if (_properties.ContainsKey(dependantPropertyName) == false)
            {
                _typeProperties.Add(dependantPropertyName, new DependentPropertyImplementationForTypeExt(dependantPropertyName, this, this.GetType()));
            }

            return _typeProperties[dependantPropertyName];
        }
        #endregion

		#region Property Change Framework Implementation

		void IBindableExtensionHook.AfterOnPropertyChanged(string propertyName)
		{
			if (DependencyFrameworkNotifyPropertyChangedScope.IsPropertyChangeConcatenationEnabled)
			{
				if (_dynamicPropertyDependencies.ContainsKey(propertyName))
				{
					var dynamicPropertyDependency = _dynamicPropertyDependencies[propertyName];
					object newMasterPropertyOwner = dynamicPropertyDependency.MasterPropertyOwnerPropertyGetter();

					if (dynamicPropertyDependency.OldMasterPropertyOwner != newMasterPropertyOwner)
					{
						if (dynamicPropertyDependency.OldMasterPropertyOwner != null)
						{
							foreach (var unregisterAction in dynamicPropertyDependency.UnRegistrationOfPropertyDependencyActions)
								unregisterAction(dynamicPropertyDependency.OldMasterPropertyOwner);
						}

						if (newMasterPropertyOwner != null)
						{
							foreach (var registerAction in dynamicPropertyDependency.RegistrationOfPropertyDependencyActions)
							{
								registerAction(newMasterPropertyOwner);
							}

							if (newMasterPropertyOwner is INotifyCollectionChanged)
								RaiseAllDependenciesOfMasterPropertyOwnerCollection((INotifyCollectionChanged)newMasterPropertyOwner);
							else
								RaiseAllDependenciesOfMasterPropertyOwner((INotifyPropertyChanged)newMasterPropertyOwner);
						}

						dynamicPropertyDependency.OldMasterPropertyOwner = newMasterPropertyOwner;
					}
				}
			}
		}

		private void RaiseAllDependenciesOfMasterPropertyOwner(INotifyPropertyChanged masterPropertyOwner)
		{
			if (DependencyFrameworkNotifyPropertyChangedScope.IsPropertyChangeConcatenationEnabled == false)
				return;

			if (!_propertyDependencies.ContainsKey(masterPropertyOwner))
				return;

			foreach (string dependencyProperty in _propertyDependencies[masterPropertyOwner].PropertyDependencies.Keys)
			{
				foreach (string dependantProperty in _propertyDependencies[masterPropertyOwner].PropertyDependencies[dependencyProperty].DependentProperties)
					HiddenBaseAPI.OnPropertyChanged(dependantProperty);
			}
		}

		private void RaiseAllDependenciesOfMasterPropertyOwnerCollection(INotifyCollectionChanged masterPropertyOwner)
		{
			if (DependencyFrameworkNotifyPropertyChangedScope.IsPropertyChangeConcatenationEnabled == false)
				return;

			var propertyDependencyRegistration = _collectionDependencies[masterPropertyOwner];

			foreach (string property in propertyDependencyRegistration.DependentProperties)
			{
				HiddenBaseAPI.OnPropertyChanged(property);
			}
		}

		#endregion
		
		#region Sanity Checks

		void IBindableExtensionHook.BeforeCachedValue(string propertyName)
		{
			#if DEBUG
				if (ArePropertyDependencySanityChecksEnabled)
				{
					PerformSanityChecksOnRequestedCacheRegistration(propertyName);
				}
			#endif
		}

		void PerformSanityChecksOnRequestedDeclarativePropertyRegistration(string propertyName)
		{
			PlatformSpecificSanityChecks.TryVerifyLastPropertySetterEquals(propertyName);
		}

		void PerformSanityChecksOnRequestedCacheRegistration(string propertyName)
		{
			PlatformSpecificSanityChecks.TryVerifyLastPropertySetterEquals(propertyName);
		}

		protected void ValidatePropertyHasSetter(string propertyName)
		{
			PlatformSpecificSanityChecks.TryValidatePropertyHasSetter(this.GetType(), propertyName);
		}

		#endregion

		#region Private Properties
		IBindableHiddenBaseAPI HiddenBaseAPI
		{
			get { return this; }
		}
		#endregion

		#region IBindableExtAccessToProtectedFunctionality

		IDependentPropertyExt IBindableExtAccessToProtectedFunctionality.TunnelledProperty<T>(Expression<Func<T>> property)
		{
			return Property(property);
		}

		void IBindableExtAccessToProtectedFunctionality.TunnelledRegisterPropertyDependency<TOwner, TProperty, TDependantPropertyType>(Expression<Func<TOwner>> ownerProvider,
		                                                                                                                               Expression<Func<TOwner, TProperty>> property,
		                                                                                                                               Expression<Func<TDependantPropertyType>> dependantProperty)
		{
			RegisterPropertyDependency<TOwner, TProperty, TDependantPropertyType>(ownerProvider,
			                                                                      property,
			                                                                      dependantProperty);
		}

		void IBindableExtAccessToProtectedFunctionality.TunnelledRegisterPropertyDependency<TCollectionType, TCollectionItemPropertyType, TProperty>(
			Expression<Func<DependencyFrameworkObservableCollection<TCollectionType>>> collectionPropertyGetter, Expression<Func<TCollectionType, TCollectionItemPropertyType>> collectionChildProperty, Expression<Func<TProperty>> dependentProperty)
		{
			RegisterPropertyDependency<TCollectionType, TCollectionItemPropertyType, TProperty>(
				collectionPropertyGetter,
				collectionChildProperty,
				dependentProperty);
		}

		void IBindableExtAccessToProtectedFunctionality.TunnelledRegisterDeferredCallbackDependency<T, T1>(T masterPropertyOwner, Expression<Func<T, T1>> masterProperty,
		                                                                                                   Action callback)
		{
			RegisterDeferredCallbackDependency<T, T1>(masterPropertyOwner, masterProperty, callback);
		}

		void IBindableExtAccessToProtectedFunctionality.TunnelledRegisterDeferredCallbackDependency<T, T1>(T[] masterPropertyOwners, Expression<Func<T, T1>> masterProperty,
		                                                                                                   Action callback)
		{
			RegisterDeferredCallbackDependency<T, T1>(masterPropertyOwners, masterProperty, callback);
			;
		}

		void IBindableExtAccessToProtectedFunctionality.TunnelledRegisterDeferredCallbackDependency<T, T1>(T masterPropertyOwner, Expression<Func<T, T1>> masterProperty,
		                                                                                                   Action callback, int delayInMilliseconds)
		{
			RegisterDeferredCallbackDependency<T, T1>(masterPropertyOwner, masterProperty, callback, delayInMilliseconds);
			;
		}

		void IBindableExtAccessToProtectedFunctionality.TunnelledRegisterDeferredCallbackDependency<T>(T masterPropertyOwner, Action callback)
		{
			RegisterDeferredCallbackDependency<T>(masterPropertyOwner, callback);
		}

		void IBindableExtAccessToProtectedFunctionality.TunnelledRegisterDeferredCallbackDependency<T>(T[] masterPropertyOwners, Action callback)
		{
			RegisterDeferredCallbackDependency<T>(masterPropertyOwners, callback);
		}

		void IBindableExtAccessToProtectedFunctionality.TunnelledRegisterDeferredCallbackDependency<T>(T masterPropertyOwner, Action callback, int delayInMilliseconds)
		{
			RegisterDeferredCallbackDependency<T>(masterPropertyOwner, callback, delayInMilliseconds);
		}

		void IBindableExtAccessToProtectedFunctionality.TunnelledRegisterDeferredCallbackDependency<T>(
			DependencyFrameworkObservableCollection<T> masterPropertyOwnerCollection, Action callback)
		{
			RegisterDeferredCallbackDependency<T>(
			masterPropertyOwnerCollection, callback);
		}

		void IBindableExtAccessToProtectedFunctionality.TunnelledRegisterDeferredCallbackDependency<T, T1>(
			DependencyFrameworkObservableCollection<T> masterPropertyOwnerCollection, Expression<Func<T, T1>> masterProperty, Action callback)
		{
			RegisterDeferredCallbackDependency<T, T1>(
				masterPropertyOwnerCollection, masterProperty, callback);
		}

		#endregion
	}
}
