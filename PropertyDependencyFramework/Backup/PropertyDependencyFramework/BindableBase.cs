using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using PropertyDependencyFramework.Annotations;
using PropertyDependencyFramework.Interfaces;

namespace PropertyDependencyFramework
{
	public abstract class BindableBase : INotifyPropertyChanged,
									 IDependencyFrameworkNotifyPropertyChangedInTransaction,
									 IDisposable,
									 IBindableHiddenRegistrationAPI,
									 IBindableHiddenBaseAPI,
									 IBindableBaseAccessToProtectedFunctionality
	{
		public bool ArePropertyDependencySanityChecksEnabled = true;

		protected BindableBase()
		{
			UseSmartPropertyChangeNotificationByDefault = true;
		}

		protected BindableBase( bool useSmartPropertyChangeNotificationByDefault )
		{
			UseSmartPropertyChangeNotificationByDefault = useSmartPropertyChangeNotificationByDefault;
		}

		#region Private Fields
		private bool _disposed;
		#endregion

		#region Protected Internal Fields
		internal Dictionary<INotifyPropertyChanged, ObjectPropertyDependencyRegistration> _propertyDependencies = new Dictionary<INotifyPropertyChanged, ObjectPropertyDependencyRegistration>();
		internal Dictionary<INotifyCollectionChanged, CollectionPropertyDependencyRegistration> _collectionDependencies = new Dictionary<INotifyCollectionChanged, CollectionPropertyDependencyRegistration>();
		#endregion


		#region Properties
		public Boolean IsDisposed { get { return _disposed; } }

		protected bool UseSmartPropertyChangeNotificationByDefault { get; set; }
		#endregion


		#region Events
		public event PropertyChangedEventHandler PropertyChanged;

		public event EventHandler<PropertyChangedEventArgs> PropertyChangedInTransaction;
		#endregion

		#region Force DeferalCode
		private static DependencyFrameworkNotifyPropertyChangedScope _deferredOnlyScope;
		public static bool CanDeferNotifications()
		{
			return DependencyFrameworkNotifyPropertyChangedScope.Current == null;
		}
		public static void DeferNotifications()
		{
			_deferredOnlyScope = new DependencyFrameworkNotifyPropertyChangedScope( forDeferalOnly: true );
		}
		public static bool NotificationsDefered()
		{
			return _deferredOnlyScope != null;
		}
		public static void ResumeNotifications()
		{
			_deferredOnlyScope.Dispose();
			_deferredOnlyScope = null;
		}
		#endregion

		#region Public Methods
		void IDependencyFrameworkNotifyPropertyChangedInTransaction.FirePropertyChanged( string propertyName )
		{
			OnPropertyChanged( propertyName );
		}

		/// <summary>
		/// Releases unmanaged and managed resources
		/// </summary>
		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}
		#endregion

		#region Protected Methods
		/// <summary>
		/// Releases unmanaged resources during Dispose or finalization.
		/// </summary>
		/// <remarks>
		/// This method should be overriden in any derived class that creates its 
		/// own unmanaged resources. A call to the base method should always be included.
		/// </remarks>
		protected virtual void ReleaseUnmanagedResources()
		{
			UnsubscribeAllNotifications();
		}

		/// <summary>
		/// Releases managed resources during Dispose.
		/// </summary>
		/// <remarks>
		/// This method should be overriden in any derived class that creates its 
		/// own managed resources. A call to the base method should always be included.
		/// </remarks>
		protected virtual void ReleaseManagedResources()
		{ }
		#endregion

		#region Private Methods
		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		private void Dispose( bool disposing )
		{
			if ( _disposed )
			{
				// decided that it's OK for dispose to be called multiple times, just silently return
				return;
			}
			// set _disposed RIGHT AFTER the above check in case there's a complex object graph and we come back in here
			// from the below call to ReleaseManagedResources()
			_disposed = true;

			ReleaseUnmanagedResources();
			if ( disposing )
			{
				ReleaseManagedResources();
			}

			UnregisterAllPropertyDependencies();
		}

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="BindableBase"/> is reclaimed by garbage collection.
		/// </summary>
		~BindableBase()
		{
			Dispose( false );
		}
		private void UnsubscribeAllNotifications()
		{
			if ( PropertyChanged == null )
				return;
			Delegate[] invocationList = PropertyChanged.GetInvocationList();
			if ( invocationList.Length == 0 )
				return;
#if DEBUG
			if ( ArePropertyDependencySanityChecksEnabled )
			{
				foreach ( Delegate attached in invocationList )
				{
					Debug.WriteLine( String.Format( CultureInfo.CurrentCulture, "Detaching: {0};{1}", attached.Target, attached.Method.Name ) );
				}
			}
#endif
			PropertyChanged = null;
		}
		#endregion



		#region Imperative Property Dependency Registration API

		#region Public API for Derived Classes
		protected void RegisterPropertyDependency<T, T1, T2>( T masterPropertyOwner, Expression<Func<T, T1>> masterProperty, Expression<Func<T2>> dependentProperty ) where T : INotifyPropertyChanged
		{
			HiddenRegistrationAPI.RegisterPropertyDependency( masterPropertyOwner, masterProperty, dependentProperty );
		}

		protected void RegisterPropertyDependency<T, T1, T2>( DependencyFrameworkObservableCollection<T> masterPropertyOwnerCollection,
															 Expression<Func<T, T1>> masterProperty,
															 Expression<Func<T2>> dependentProperty ) where T : INotifyPropertyChanged
		{
			HiddenRegistrationAPI.RegisterPropertyDependency( masterPropertyOwnerCollection, masterProperty, dependentProperty );
		}

		protected void RegisterPropertyDependency<T, T1, T2>( ObservableCollection<T> masterPropertyOwnerCollection,
													 Expression<Func<T, T1>> masterProperty,
													 Expression<Func<T2>> dependentProperty ) where T : INotifyPropertyChanged
		{
			HiddenRegistrationAPI.RegisterPropertyDependency( (INotifyCollectionChanged)masterPropertyOwnerCollection, masterProperty, dependentProperty );
		}

		protected void RegisterPropertyDependency<T, T1, T2>( INotifyCollectionChanged masterPropertyOwnerCollection,
													 Expression<Func<T, T1>> masterProperty,
													 Expression<Func<T2>> dependentProperty ) where T : INotifyPropertyChanged
		{
			HiddenRegistrationAPI.RegisterPropertyDependency( masterPropertyOwnerCollection, masterProperty, dependentProperty );
		}


		protected void RegisterCallbackDependency<T>( T masterPropertyOwner, Action callback )
			where T : INotifyPropertyChanged
		{
			HiddenRegistrationAPI.RegisterCallbackDependency( masterPropertyOwner, callback );
		}

		protected void RegisterCallbackDependency<T, T1>( T masterPropertyOwner, Expression<Func<T, T1>> masterProperty, Action callback )
			where T : INotifyPropertyChanged
		{
			HiddenRegistrationAPI.RegisterCallbackDependency( masterPropertyOwner, masterProperty, callback );
		}

		protected void RegisterCallbackDependency<T, T1>( T[] masterPropertyOwners, Expression<Func<T, T1>> masterProperty, Action callback )
			where T : INotifyPropertyChanged
		{
			HiddenRegistrationAPI.RegisterCallbackDependency( masterPropertyOwners, masterProperty, callback );
		}

		protected void RegisterCallbackDependency<T>( DependencyFrameworkObservableCollection<T> masterPropertyOwnerCollection, Action callback )
		{
			HiddenRegistrationAPI.RegisterCallbackDependency( masterPropertyOwnerCollection, callback );
		}

		protected void RegisterCallbackDependency<T>( ObservableCollection<T> masterPropertyOwnerCollection, Action callback )
		{
			HiddenRegistrationAPI.RegisterCallbackDependency( (INotifyCollectionChanged)masterPropertyOwnerCollection, callback );
		}

		protected void RegisterCallbackDependency( INotifyCollectionChanged masterPropertyOwnerCollection, Action callback )
		{
			HiddenRegistrationAPI.RegisterCallbackDependency( masterPropertyOwnerCollection, callback );
		}

		protected void RegisterCallbackDependency<T, T1>( DependencyFrameworkObservableCollection<T> masterPropertyOwnerCollection, Expression<Func<T, T1>> masterProperty, Action callback )
				where T : INotifyPropertyChanged
		{
			HiddenRegistrationAPI.RegisterCallbackDependency( masterPropertyOwnerCollection, masterProperty, callback );
		}

		protected void RegisterCallbackDependency<T, T1>( ObservableCollection<T> masterPropertyOwnerCollection, Expression<Func<T, T1>> masterProperty, Action callback )
		where T : INotifyPropertyChanged
		{
			HiddenRegistrationAPI.RegisterCallbackDependency( masterPropertyOwnerCollection, masterProperty, callback );
		}

		protected void RegisterCallbackDependency<T, T1>( INotifyCollectionChanged masterPropertyOwnerCollection, Expression<Func<T, T1>> masterProperty, Action callback )
		where T : INotifyPropertyChanged
		{
			HiddenRegistrationAPI.RegisterCallbackDependency( masterPropertyOwnerCollection, masterProperty, callback );
		}

		protected void UnregisterAllPropertyDependencies()
		{
			HiddenRegistrationAPI.UnregisterAllPropertyDependencies();
		}

		protected void UnRegisterPropertyDependency( INotifyCollectionChanged masterPropertyOwnerCollection, string masterPropertyName, string dependantPropertyName )
		{
			HiddenRegistrationAPI.UnRegisterPropertyDependency( masterPropertyOwnerCollection, masterPropertyName, dependantPropertyName );
		}

		protected void UnRegisterPropertyDependency( INotifyPropertyChanged masterPropertyOwner, string masterPropertyName, string dependantPropertyName )
		{
			HiddenRegistrationAPI.UnRegisterPropertyDependency( masterPropertyOwner, masterPropertyName, dependantPropertyName );
		}
		#endregion

		#region Hidden API for Declarative API
		private IBindableHiddenRegistrationAPI HiddenRegistrationAPI
		{
			get { return (IBindableHiddenRegistrationAPI)this; }
		}

		void IBindableHiddenRegistrationAPI.RegisterPropertyDependency<T, T1, T2>( DependencyFrameworkObservableCollection<T> masterPropertyOwnerCollection,
													 Expression<Func<T, T1>> masterProperty,
													 Expression<Func<T2>> dependentProperty )
		{
			HiddenRegistrationAPI.RegisterPropertyDependency( (INotifyCollectionChanged)masterPropertyOwnerCollection, masterProperty, dependentProperty );
		}

		void IBindableHiddenRegistrationAPI.RegisterPropertyDependency<T, T1, T2>( T masterPropertyOwner,
																					 Expression<Func<T, T1>> masterProperty,
																					 Expression<Func<T2>> dependentProperty )
		{
			HiddenRegistrationAPI.RegisterPropertyDependency( masterPropertyOwner, PropertyNameResolver.GetPropertyName( masterProperty ), PropertyNameResolver.GetPropertyName( dependentProperty ) );
		}

		void IBindableHiddenRegistrationAPI.RegisterPropertyDependency<T, T1, T2>( INotifyCollectionChanged masterPropertyOwnerCollection, Expression<Func<T, T1>> masterProperty, Expression<Func<T2>> dependentProperty )
		{
			HiddenRegistrationAPI.RegisterPropertyDependency( masterPropertyOwnerCollection, PropertyNameResolver.GetPropertyName( masterProperty ), PropertyNameResolver.GetPropertyName( dependentProperty ) );
		}


		void IBindableHiddenRegistrationAPI.RegisterCallbackDependency<T>( T masterPropertyOwner, Action callback )
		{
			HiddenRegistrationAPI.RegisterCallbackDependency( masterPropertyOwner, new CallbackContainer( callback ) );
		}

		void IBindableHiddenRegistrationAPI.RegisterCallbackDependency<T, T1>( T masterPropertyOwner, Expression<Func<T, T1>> masterProperty, Action callback )
		{
			HiddenRegistrationAPI.RegisterCallbackDependency( masterPropertyOwner, masterProperty, new CallbackContainer( callback ) );
		}

		void IBindableHiddenRegistrationAPI.RegisterCallbackDependency<T, T1>( T masterPropertyOwner, Expression<Func<T, T1>> masterProperty, CallbackContainer callbackContainer )
		{
			HiddenRegistrationAPI.RegisterCallbackDependency( masterPropertyOwner, PropertyNameResolver.GetPropertyName( masterProperty ), callbackContainer );
		}

		void IBindableHiddenRegistrationAPI.RegisterCallbackDependency<T, T1>( T[] masterPropertyOwners, Expression<Func<T, T1>> masterProperty, Action callback )
		{
			foreach ( T masterPropertyOwner in masterPropertyOwners )
			{
				HiddenRegistrationAPI.RegisterCallbackDependency( masterPropertyOwner, masterProperty, callback );
			}
		}

		void IBindableHiddenRegistrationAPI.RegisterCallbackDependency<T>( DependencyFrameworkObservableCollection<T> masterPropertyOwnerCollection, Action callback )
		{
			HiddenRegistrationAPI.RegisterCallbackDependency( (INotifyCollectionChanged)masterPropertyOwnerCollection, callback );
		}

		void IBindableHiddenRegistrationAPI.RegisterCallbackDependency<T, T1>( DependencyFrameworkObservableCollection<T> masterPropertyOwnerCollection, Expression<Func<T, T1>> masterProperty,
								Action callback )
		{
			HiddenRegistrationAPI.RegisterCallbackDependency( (INotifyCollectionChanged)masterPropertyOwnerCollection, masterProperty, callback );
		}

		void IBindableHiddenRegistrationAPI.RegisterCallbackDependency<T, T1>( INotifyCollectionChanged masterPropertyOwnerCollection, Expression<Func<T, T1>> masterProperty, Action callback )
		{
			HiddenRegistrationAPI.RegisterCallbackDependency( masterPropertyOwnerCollection, masterProperty, new CallbackContainer( callback ) );
		}

		void IBindableHiddenRegistrationAPI.RegisterCallbackDependency( INotifyCollectionChanged masterPropertyOwnerCollection, Action callback )
		{
			HiddenRegistrationAPI.RegisterCallbackDependency( masterPropertyOwnerCollection, new CallbackContainer( callback ) );
		}

		void IBindableHiddenRegistrationAPI.RegisterCallbackDependency<T, T1>( INotifyCollectionChanged masterPropertyOwnerCollection, Expression<Func<T, T1>> masterProperty, CallbackContainer callbackContainer )
		{
			HiddenRegistrationAPI.RegisterCallbackDependency<T>( masterPropertyOwnerCollection, PropertyNameResolver.GetPropertyName( masterProperty ), callbackContainer );
		}

		private readonly HashSet<Guid> _registrationIdHashSet = new HashSet<Guid>();
		private readonly Guid _registrationId = Guid.NewGuid();
		internal Guid RegistrationId
		{
			[DebuggerStepThrough]
			get { return _registrationId; }
		}
		void IBindableHiddenRegistrationAPI.RegisterPropertyDependency( INotifyPropertyChanged masterPropertyOwner, string masterPropertyName, string dependantPropertyName )
		{
			if ( dependantPropertyName == null )
				throw new ArgumentNullException( "dependantPropertyName" );

			EnsurePropertyDependencyDictionaryAcceptsDependantProperties( masterPropertyOwner, masterPropertyName );

			if ( _propertyDependencies[masterPropertyOwner].PropertyDependencies[masterPropertyName].DependentProperties.Contains( dependantPropertyName ) )
				return;

#if DEBUG
			if ( ArePropertyDependencySanityChecksEnabled )
			{
				EnsureObjectExposesProperty( this, dependantPropertyName );
			}
#endif

			_propertyDependencies[masterPropertyOwner].PropertyDependencies[masterPropertyName].DependentProperties.Add( dependantPropertyName );

			SubscribeToMasterPropertyOwner( masterPropertyOwner );
		}


		void IBindableHiddenRegistrationAPI.RegisterPropertyDependency( INotifyCollectionChanged masterPropertyOwnerCollection, string masterPropertyName, string dependantPropertyName )
		{
			if ( dependantPropertyName == null )
				throw new ArgumentNullException( dependantPropertyName );

			foreach ( INotifyPropertyChanged child in ((IEnumerable)masterPropertyOwnerCollection) )
			{
				HiddenRegistrationAPI.RegisterPropertyDependency( child, masterPropertyName, dependantPropertyName );
			}

			masterPropertyOwnerCollection.CollectionChanged -= OnCollectionChanged;
			masterPropertyOwnerCollection.CollectionChanged += OnCollectionChanged;

			Action<object> registerNewItemDelegate = newItem => HiddenRegistrationAPI.RegisterPropertyDependency( (INotifyPropertyChanged)newItem, masterPropertyName, dependantPropertyName );
			if ( !_collectionDependencies.ContainsKey( masterPropertyOwnerCollection ) )
				_collectionDependencies.Add( masterPropertyOwnerCollection, new CollectionPropertyDependencyRegistration() );

			CollectionPropertyDependencyRegistration propertyDependencyRegistration = _collectionDependencies[masterPropertyOwnerCollection];
			propertyDependencyRegistration.ItemPropertyDependencyRegistrationDelegates.Add( new CollectionPropertyDependencyRegistration.PropertyRegistrationIdentifier( masterPropertyName, dependantPropertyName ), registerNewItemDelegate );

			if ( !propertyDependencyRegistration.DependentProperties.Contains( dependantPropertyName ) )
				propertyDependencyRegistration.DependentProperties.Add( dependantPropertyName );
		}

		void IBindableHiddenRegistrationAPI.RegisterCallbackDependency<T>( T masterPropertyOwner, CallbackContainer callbackContainer )
		{
			EnsurePropertyDependencyDictionaryContainsMasterObject( masterPropertyOwner );
			_propertyDependencies[masterPropertyOwner].Callbacks.Add( callbackContainer );

			SubscribeToMasterPropertyOwner( masterPropertyOwner );
		}

		void IBindableHiddenRegistrationAPI.RegisterCallbackDependency<T>( T masterPropertyOwner, string masterPropertyName, CallbackContainer callbackContainer )
		{
			EnsurePropertyDependencyDictionaryAcceptsDependantProperties( masterPropertyOwner, masterPropertyName );
			_propertyDependencies[masterPropertyOwner].PropertyDependencies[masterPropertyName].Callbacks.Add( callbackContainer );

			SubscribeToMasterPropertyOwner( masterPropertyOwner );
		}

		void IBindableHiddenRegistrationAPI.RegisterCallbackDependency<T>( INotifyCollectionChanged masterPropertyOwnerCollection, string masterPropertyName, CallbackContainer callbackContainer )
		{
			foreach ( T child in ((IEnumerable)masterPropertyOwnerCollection) )
			{
				HiddenRegistrationAPI.RegisterCallbackDependency( child, masterPropertyName, callbackContainer );
			}

			masterPropertyOwnerCollection.CollectionChanged -= OnCollectionChanged;
			masterPropertyOwnerCollection.CollectionChanged += OnCollectionChanged;

			Action<object> registerNewItemDelegate = newItem => HiddenRegistrationAPI.RegisterCallbackDependency( (T)newItem, masterPropertyName, callbackContainer );
			if ( !_collectionDependencies.ContainsKey( masterPropertyOwnerCollection ) )
				_collectionDependencies.Add( masterPropertyOwnerCollection, new CollectionPropertyDependencyRegistration() );

			CollectionPropertyDependencyRegistration propertyDependencyRegistration = _collectionDependencies[masterPropertyOwnerCollection];
			propertyDependencyRegistration.ItemPropertyDependencyRegistrationDelegates.Add( new CollectionPropertyDependencyRegistration.CallbackContainerRegistrationIdentifier( masterPropertyName, callbackContainer ), registerNewItemDelegate );
			propertyDependencyRegistration.Callbacks.Add( callbackContainer );
		}

		void IBindableHiddenRegistrationAPI.RegisterCallbackDependency( INotifyCollectionChanged masterPropertyOwnerCollection, CallbackContainer callbackContainer )
		{
			masterPropertyOwnerCollection.CollectionChanged -= OnCollectionChanged;
			masterPropertyOwnerCollection.CollectionChanged += OnCollectionChanged;

			if ( !_collectionDependencies.ContainsKey( masterPropertyOwnerCollection ) )
				_collectionDependencies.Add( masterPropertyOwnerCollection, new CollectionPropertyDependencyRegistration() );

			CollectionPropertyDependencyRegistration propertyDependencyRegistration = _collectionDependencies[masterPropertyOwnerCollection];
			propertyDependencyRegistration.Callbacks.Add( callbackContainer );
		}

		void IBindableHiddenRegistrationAPI.UnregisterAllPropertyDependencies()
		{
			foreach ( INotifyPropertyChanged propertyChangedMaster in _propertyDependencies.Keys.ToArray() )
			{
				_propertyDependencies.Remove( propertyChangedMaster );

				propertyChangedMaster.PropertyChanged -= OnPropertyOfDependencyChanged;

				if ( propertyChangedMaster is IDependencyFrameworkNotifyPropertyChangedInTransaction )
				{
					((IDependencyFrameworkNotifyPropertyChangedInTransaction)propertyChangedMaster).PropertyChangedInTransaction -= OnPropertyOfDependencyChangedInTransaction;
				}
				_registrationIdHashSet.Clear();
			}

			foreach ( INotifyCollectionChanged collection in _collectionDependencies.Keys.ToArray() )
			{
				collection.CollectionChanged -= OnCollectionChanged;
			}

			_propertyDependencies.Clear();
			_collectionDependencies.Clear();

			if ( this is IBindableExtensionHook )
				((IBindableExtensionHook)this).AfterUnregisterAllPropertyDependencies();
		}

		void IBindableHiddenRegistrationAPI.UnRegisterPropertyDependency( INotifyCollectionChanged masterPropertyOwnerCollection, string masterPropertyName, string dependantPropertyName )
		{
			foreach ( INotifyPropertyChanged child in ((IEnumerable)masterPropertyOwnerCollection) )
			{
				HiddenRegistrationAPI.UnRegisterPropertyDependency( child, masterPropertyName, dependantPropertyName );
			}

			if ( _collectionDependencies.ContainsKey( masterPropertyOwnerCollection ) )
			{
				CollectionPropertyDependencyRegistration propertyDependencyRegistration = _collectionDependencies[masterPropertyOwnerCollection];

				if ( propertyDependencyRegistration.DependentProperties.Contains( dependantPropertyName ) )
					propertyDependencyRegistration.DependentProperties.Remove( dependantPropertyName );

				var key = propertyDependencyRegistration.ItemPropertyDependencyRegistrationDelegates.Keys.OfType<CollectionPropertyDependencyRegistration.PropertyRegistrationIdentifier>().FirstOrDefault( k => k.CollectionMasterPropertyName == masterPropertyName && k.DependentPropertyName == dependantPropertyName );
				if ( key != null )
					propertyDependencyRegistration.ItemPropertyDependencyRegistrationDelegates.Remove( key );

				if ( propertyDependencyRegistration.DependentProperties.Count != 0 )
					return;

				if ( propertyDependencyRegistration.Callbacks.Count != 0 )
					return;

				if ( propertyDependencyRegistration.ItemPropertyDependencyRegistrationDelegates.Count != 0 )
					return;

				_collectionDependencies.Remove( masterPropertyOwnerCollection );
				masterPropertyOwnerCollection.CollectionChanged -= OnCollectionChanged;
			}
		}

		void IBindableHiddenRegistrationAPI.UnRegisterPropertyDependency( INotifyPropertyChanged masterPropertyOwner, string masterPropertyName, string dependantPropertyName )
		{
			EnsurePropertyDependencyDictionaryAcceptsDependantProperties( masterPropertyOwner, masterPropertyName );

			if ( _propertyDependencies[masterPropertyOwner].PropertyDependencies[masterPropertyName].DependentProperties.Contains( dependantPropertyName ) )
			{
				_propertyDependencies[masterPropertyOwner].PropertyDependencies[masterPropertyName].DependentProperties.Remove( dependantPropertyName );

				if ( _propertyDependencies[masterPropertyOwner].PropertyDependencies[masterPropertyName].DependentProperties.Count == 0 &&
					_propertyDependencies[masterPropertyOwner].PropertyDependencies[masterPropertyName].Callbacks.Count == 0 )
				{
					_propertyDependencies[masterPropertyOwner].PropertyDependencies.Remove( masterPropertyName );

					UnsubscribeFromMasterPropertyOwner(masterPropertyOwner);
				}
			}
		}

		private void SubscribeToMasterPropertyOwner( INotifyPropertyChanged masterPropertyOwner )
		{
			BindableBase bindableMasterPropertyOwner = masterPropertyOwner as BindableBase;
			if ( bindableMasterPropertyOwner == null )
			{
				masterPropertyOwner.PropertyChanged -= OnPropertyOfDependencyChanged;
				masterPropertyOwner.PropertyChanged += OnPropertyOfDependencyChanged;

				if ( masterPropertyOwner is IDependencyFrameworkNotifyPropertyChangedInTransaction )
				{
					((IDependencyFrameworkNotifyPropertyChangedInTransaction)masterPropertyOwner)
						.PropertyChangedInTransaction -= OnPropertyOfDependencyChangedInTransaction;
					((IDependencyFrameworkNotifyPropertyChangedInTransaction)masterPropertyOwner)
						.PropertyChangedInTransaction += OnPropertyOfDependencyChangedInTransaction;
				}
			}
			else
			{
				if ( !_registrationIdHashSet.Contains( bindableMasterPropertyOwner.RegistrationId ) )
				{
					bindableMasterPropertyOwner.PropertyChanged += OnPropertyOfDependencyChanged;
					bindableMasterPropertyOwner.PropertyChangedInTransaction += OnPropertyOfDependencyChangedInTransaction;

					_registrationIdHashSet.Add( bindableMasterPropertyOwner.RegistrationId );
				}
			}
		}

		private void UnsubscribeFromMasterPropertyOwner(INotifyPropertyChanged masterPropertyOwner)
		{
			if (_propertyDependencies[masterPropertyOwner].PropertyDependencies.Count == 0 &&
			    _propertyDependencies[masterPropertyOwner].Callbacks.Count == 0)
			{
				_propertyDependencies.Remove(masterPropertyOwner);
				masterPropertyOwner.PropertyChanged -= OnPropertyOfDependencyChanged;
				if (masterPropertyOwner is IDependencyFrameworkNotifyPropertyChangedInTransaction)
				{
					((IDependencyFrameworkNotifyPropertyChangedInTransaction) masterPropertyOwner).PropertyChangedInTransaction -=
						OnPropertyOfDependencyChangedInTransaction;
				}
				if (masterPropertyOwner is BindableBase)
				{
					_registrationIdHashSet.Remove(((BindableBase) masterPropertyOwner).RegistrationId);
				}
			}
		}

		#endregion

		#region Helpers

		protected internal void EnsurePropertyDependencyDictionaryAcceptsDependantProperties( INotifyPropertyChanged masterPropertyOwner, string masterPropertyName )
		{
			EnsurePropertyDependencyDictionaryContainsMasterObject( masterPropertyOwner );

			if ( !_propertyDependencies[masterPropertyOwner].PropertyDependencies.ContainsKey( masterPropertyName ) )
			{
#if DEBUG
				if ( ArePropertyDependencySanityChecksEnabled )
				{
					EnsureObjectExposesProperty( masterPropertyOwner, masterPropertyName );
				}
#endif
				_propertyDependencies[masterPropertyOwner].PropertyDependencies.Add( masterPropertyName, new PropertyDependencies() );
			}
		}

		protected internal void EnsurePropertyDependencyDictionaryContainsMasterObject( INotifyPropertyChanged masterPropertyOwner )
		{
			if ( !_propertyDependencies.ContainsKey( masterPropertyOwner ) )
			{
				_propertyDependencies.Add( masterPropertyOwner, new ObjectPropertyDependencyRegistration() );
			}
		}

		#endregion

		#endregion

		#region Property Change Framework Implementation

		protected void NotifyPropertyChanged<T>( Expression<Func<T>> propertyExpression )
		{
			NotifyPropertyChanged( PropertyNameResolver.GetPropertyName( propertyExpression ) );
		}

		protected void NotifyPropertyChanged( string propertyName )
		{
			if ( UseSmartPropertyChangeNotificationByDefault )
			{
				if ( DependencyFrameworkNotifyPropertyChangedScope.AreSourcePropertyChangesQueuedForDeferredExecution )
				{
					DependencyFrameworkNotifyPropertyChangedScope.Current.DeferSourcePropertyChangeForDeferredExecution( this, propertyName );
				}
				else
				{
					using ( new DependencyFrameworkNotifyPropertyChangedScope() )
					{
						OnPropertyChanged( propertyName );
					}
				}
			}
			else
			{
				OnPropertyChanged( propertyName );
			}
		}

		private void OnPropertyChanged( string propertyName )
		{
			lock ( _cachedPropertyValues )
			{
				if ( _cachedPropertyValues.ContainsKey( propertyName ) )
					_cachedPropertyValues.Remove( propertyName );
			}


			if ( DependencyFrameworkNotifyPropertyChangedScope.ArePropertyChangesCollected )
			{
				if ( !DependencyFrameworkNotifyPropertyChangedScope.Current.IsPropertyChangedQueued( this, propertyName ) )
				{
					DependencyFrameworkNotifyPropertyChangedScope.Current.QueuePropertyChange( this, propertyName );

					if ( PropertyChangedInTransaction != null )
						PropertyChangedInTransaction( this, new PropertyChangedEventArgs( propertyName ) );
				}
			}
			else
			{
				PropertyChangedEventHandler handler = PropertyChanged;
				if ( handler != null )
				{
					handler( this, new PropertyChangedEventArgs( propertyName ) );
				}
			}

			if ( this is IBindableExtensionHook )
				((IBindableExtensionHook)this).AfterOnPropertyChanged( propertyName );
		}

		void OnCollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
		{
			INotifyCollectionChanged collection = (INotifyCollectionChanged)sender;
			var propertyDependencyRegistration = _collectionDependencies[collection];

			bool raisePropertyChanges = false;

			switch ( e.Action )
			{
				case NotifyCollectionChangedAction.Add:
					foreach ( object newItem in e.NewItems )
					{
						foreach ( Action<object> registrationDelegate in propertyDependencyRegistration.ItemPropertyDependencyRegistrationDelegates.Values )
							registrationDelegate( newItem );
					}
					raisePropertyChanges = true;
					break;
				case NotifyCollectionChangedAction.Remove:
					foreach ( INotifyPropertyChanged oldItem in e.OldItems )
					{
						oldItem.PropertyChanged -= OnPropertyOfDependencyChanged;

						if ( oldItem is IDependencyFrameworkNotifyPropertyChangedInTransaction )
						{
							( (IDependencyFrameworkNotifyPropertyChangedInTransaction)oldItem ).PropertyChangedInTransaction -= OnPropertyOfDependencyChangedInTransaction;
						}

						_propertyDependencies.Remove( oldItem );
						if ( oldItem is BindableBase )
						{
							_registrationIdHashSet.Remove( ((BindableBase)oldItem).RegistrationId );
						}
					}
					raisePropertyChanges = true;
					break;
				case NotifyCollectionChangedAction.Reset:
					raisePropertyChanges = true;
					break;
			}

			if ( raisePropertyChanges )
			{
				foreach ( string property in propertyDependencyRegistration.DependentProperties )
				{
					OnPropertyChanged( property );
				}

				if ( DependencyFrameworkNotifyPropertyChangedScope.ArePropertyChangesCollected )
				{
					foreach ( CallbackContainer callback in propertyDependencyRegistration.Callbacks )
						DependencyFrameworkNotifyPropertyChangedScope.Current.QueueCollectionCallback( callback );
				}
				else
				{
					foreach ( CallbackContainer callback in propertyDependencyRegistration.Callbacks )
						callback.Call();
				}
			}
		}

		protected internal void OnPropertyOfDependencyChanged( object propertyOwner, PropertyChangedEventArgs args )
		{
			var masterPropertyOwner = (INotifyPropertyChanged)propertyOwner;
			if ( !_propertyDependencies.ContainsKey( masterPropertyOwner ) )
				return;

			foreach ( CallbackContainer callback in _propertyDependencies[masterPropertyOwner].Callbacks )
				callback.Call();

			if ( !_propertyDependencies[masterPropertyOwner].PropertyDependencies.ContainsKey( args.PropertyName ) )
				return;

			if ( DependencyFrameworkNotifyPropertyChangedScope.IsPropertyChangeConcatenationEnabled )
			{
				var dependentProperties = _propertyDependencies[masterPropertyOwner].PropertyDependencies[args.PropertyName].DependentProperties.ToArray();
				foreach ( string dependantProperty in dependentProperties )
					OnPropertyChanged( dependantProperty );
			}

			foreach ( CallbackContainer callback in _propertyDependencies[masterPropertyOwner].PropertyDependencies[args.PropertyName].Callbacks )
				callback.Call();
		}

		protected internal void OnPropertyOfDependencyChangedInTransaction( object propertyOwner, PropertyChangedEventArgs args )
		{
			var masterPropertyOwner = (INotifyPropertyChanged)propertyOwner;
			if ( !_propertyDependencies.ContainsKey( masterPropertyOwner ) )
				return;

			if ( !_propertyDependencies[masterPropertyOwner].PropertyDependencies.ContainsKey( args.PropertyName ) )
				return;

			var dependentProperties = _propertyDependencies[masterPropertyOwner].PropertyDependencies[args.PropertyName].DependentProperties;
			foreach ( string dependantProperty in dependentProperties )
				OnPropertyChanged( dependantProperty );
		}

		#endregion

		#region Cache Implementation

		Dictionary<string, object> _cachedPropertyValues = new Dictionary<string, object>();
		protected T CachedValue<T>( Expression<Func<T>> ofProperty, Func<T> propertyEvaluation )
		{
			string propertyName = PropertyNameResolver.GetPropertyName( ofProperty );

			if ( this is IBindableExtensionHook )
				((IBindableExtensionHook)this).BeforeCachedValue( propertyName );

			lock ( _cachedPropertyValues )
			{
				if ( !_cachedPropertyValues.ContainsKey( propertyName ) )
				{
					_cachedPropertyValues.Add( propertyName, propertyEvaluation() );
				}

				return (T)_cachedPropertyValues[propertyName];
			}
		}

		#endregion

		#region Unit Test Helpers
		protected Delegate[] GetPropertyChangedInTransactionInvocationList()
		{
			return PropertyChangedInTransaction == null ? new Delegate[] { } : PropertyChangedInTransaction.GetInvocationList();
		}

		protected Delegate[] GetPropertyChangedInvocationList()
		{
			return PropertyChanged == null ? new Delegate[] { } : PropertyChanged.GetInvocationList();
		}
		#endregion

		#region Sanity Checks
		private void EnsureObjectExposesProperty( object obj, string propertyName )
		{
			if ( !obj.GetType().GetProperties( BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic ).Any( k => k.Name == propertyName ) )
				throw new InvalidOperationException( "Object of Type " + obj.GetType() + " does not expose property " + propertyName );
		}
		#endregion

		#region Hidden Raise Property Change API
		void IBindableHiddenBaseAPI.OnPropertyChanged( string propertyName )
		{
			OnPropertyChanged( propertyName );
		}
		#endregion

		#region IBindableBaseAccessToProtectedFunctionality

		Delegate[] IBindableBaseAccessToProtectedFunctionality.TunnelledGetPropertyChangedInTransactionInvocationList()
		{
			return GetPropertyChangedInTransactionInvocationList();
		}

		Delegate[] IBindableBaseAccessToProtectedFunctionality.TunnelledGetPropertyChangedInvocationList()
		{
			return GetPropertyChangedInvocationList();
		}

		void IBindableBaseAccessToProtectedFunctionality.TunnelledNotifyPropertyChanged( string propertyName )
		{
			NotifyPropertyChanged( propertyName );
		}

		T IBindableBaseAccessToProtectedFunctionality.TunnelledCachedValue<T>( Expression<Func<T>> ofProperty,
																			  Func<T> propertyEvaluation )
		{
			return CachedValue( ofProperty, propertyEvaluation );
		}

		void IBindableBaseAccessToProtectedFunctionality.TunnelledRegisterPropertyDependency<T, T1, T2>( T masterPropertyOwner, Expression<Func<T, T1>> masterProperty,
																									Expression<Func<T2>> dependentProperty )
		{
			RegisterPropertyDependency( masterPropertyOwner, masterProperty, dependentProperty );
		}

		void IBindableBaseAccessToProtectedFunctionality.TunnelledRegisterPropertyDependency<T, T1, T2>(
			DependencyFrameworkObservableCollection<T> masterPropertyOwnerCollection, Expression<Func<T, T1>> masterProperty,
			Expression<Func<T2>> dependentProperty )
		{
			RegisterPropertyDependency( masterPropertyOwnerCollection,
									   masterProperty,
									   dependentProperty );
		}

		void IBindableBaseAccessToProtectedFunctionality.TunnelledRegisterCallbackDependency<T>( T masterPropertyOwner, Action callback )
		{
			RegisterCallbackDependency( masterPropertyOwner, callback );
		}

		void IBindableBaseAccessToProtectedFunctionality.TunnelledRegisterCallbackDependency<T, T1>( T masterPropertyOwner, Expression<Func<T, T1>> masterProperty, Action callback )
		{
			RegisterCallbackDependency( masterPropertyOwner, masterProperty, callback );
		}

		void IBindableBaseAccessToProtectedFunctionality.TunnelledRegisterCallbackDependency<T, T1>( T[] masterPropertyOwners, Expression<Func<T, T1>> masterProperty, Action callback )
		{
			RegisterCallbackDependency( masterPropertyOwners, masterProperty, callback );
		}

		void IBindableBaseAccessToProtectedFunctionality.TunnelledRegisterCallbackDependency<T>( DependencyFrameworkObservableCollection<T> masterPropertyOwnerCollection,
																							Action callback )
		{
			RegisterCallbackDependency( masterPropertyOwnerCollection, callback );
		}

		void IBindableBaseAccessToProtectedFunctionality.TunnelledRegisterCallbackDependency<T, T1>( DependencyFrameworkObservableCollection<T> masterPropertyOwnerCollection,
																								Expression<Func<T, T1>> masterProperty, Action callback )
		{
			RegisterCallbackDependency( masterPropertyOwnerCollection, masterProperty, callback );
		}

		void IBindableBaseAccessToProtectedFunctionality.TunnelledUnregisterAllPropertyDependencies()
		{
			UnregisterAllPropertyDependencies();
		}

		void IBindableBaseAccessToProtectedFunctionality.TunnelledUnRegisterPropertyDependency( INotifyCollectionChanged masterPropertyOwnerCollection,
																						   string masterPropertyName, string dependantPropertyName )
		{
			UnRegisterPropertyDependency( masterPropertyOwnerCollection, masterPropertyName, dependantPropertyName );
		}

		void IBindableBaseAccessToProtectedFunctionality.TunnelledUnRegisterPropertyDependency( INotifyPropertyChanged masterPropertyOwner, string masterPropertyName,
																						   string dependantPropertyName )
		{
			UnRegisterPropertyDependency( masterPropertyOwner, masterPropertyName, dependantPropertyName );
		}

		#endregion
	}
}
