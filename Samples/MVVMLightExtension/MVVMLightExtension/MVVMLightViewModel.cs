using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using GalaSoft.MvvmLight;
using PropertyDependencyFramework;
using PropertyDependencyFramework.DeclarativeAPI;

namespace MVVMLightExtension
{
	public class MVVMLightViewModel : ViewModelBase, IDependencyFrameworkNotifyPropertyChangedInTransaction
	{
		public MVVMLightViewModel()
		{
			InstallPDFxTunnel();
		}

		public MVVMLightViewModel(bool useSmartPropertyChangeNotificationByDefault)
		{
			InstallPDFxTunnel(useSmartPropertyChangeNotificationByDefault);
		}

		private void InstallPDFxTunnel(bool useSmartPropertyChangeNotificationByDefault = true)
		{
			_pdfx = new BindableExt(useSmartPropertyChangeNotificationByDefault) {ArePropertyDependencySanityChecksEnabled = false};
			_pdfx.PropertyChanged += (sender, args) => OnPDFxPropertyChanged(args.PropertyName);
			_pdfx.PropertyChangedInTransaction += (sender, args) => OnPDFxPropertyChangedInTransaction(args.PropertyName);
		}


		#region MVVM Light Specific
		private void FirePropertyChanged(string propertyName)
		{
			//Base class specific implementation. In this scenario MVVM Light specific
			base.RaisePropertyChanged(propertyName);
		}

		protected override void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
		{
			PDFx.TunnelledNotifyPropertyChanged(PropertyNameResolver.GetPropertyName(propertyExpression));
		}

		protected override void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression, T oldValue, T newValue, bool broadcast)
		{
			RaisePropertyChanged(PropertyNameResolver.GetPropertyName(propertyExpression), oldValue, newValue, broadcast);
		}

		protected override void RaisePropertyChanged(string propertyName)
		{
			PDFx.TunnelledNotifyPropertyChanged(propertyName);
		}
		#endregion


		private void OnPDFxPropertyChanged(string propertyName)
		{
			FirePropertyChanged(propertyName);
		}

		private void OnPDFxPropertyChangedInTransaction(string propertyName)
		{
			FirePropertyChangedInTransaction(propertyName);
		}

		private void FirePropertyChangedInTransaction(string propertyName)
		{
			if (PropertyChangedInTransaction != null)
				PropertyChangedInTransaction(this, new PropertyChangedEventArgs(propertyName));
		}

		private BindableExt _pdfx;

		private IBindableExtAccessToProtectedFunctionality PDFx
		{
			get { return _pdfx; }
		}

		protected T CachedValue<T>(Expression<Func<T>> ofProperty, Func<T> propertyEvaluation)
		{
			return PDFx.TunnelledCachedValue(ofProperty, propertyEvaluation);
		}

		protected IDependentPropertyExt Property<T>(Expression<Func<T>> property)
		{
			return PDFx.TunnelledProperty(property);
		}

		protected void RegisterCallbackDependency<T>(T masterPropertyOwner, Action callback) where T : INotifyPropertyChanged
		{
			PDFx.TunnelledRegisterCallbackDependency(masterPropertyOwner, callback);
		}

		protected void RegisterCallbackDependency<T, T1>(T[] masterPropertyOwners, Expression<Func<T, T1>> masterProperty,
		                                                 Action callback) where T : INotifyPropertyChanged
		{
			PDFx.TunnelledRegisterCallbackDependency(masterPropertyOwners, masterProperty, callback);
		}

		protected void RegisterCallbackDependency<T>(DependencyFrameworkObservableCollection<T> masterPropertyOwnerCollection,
		                                             Action callback)
		{
			PDFx.TunnelledRegisterCallbackDependency(masterPropertyOwnerCollection, callback);
		}

		protected void RegisterCallbackDependency<T, T1>(
			DependencyFrameworkObservableCollection<T> masterPropertyOwnerCollection,
			Expression<Func<T, T1>> masterProperty, Action callback) where T : INotifyPropertyChanged
		{
			PDFx.TunnelledRegisterCallbackDependency(masterPropertyOwnerCollection, masterProperty, callback);
		}

		protected void RegisterCallbackDependency<T, T1>(T masterPropertyOwner, Expression<Func<T, T1>> masterProperty,
		                                                 Action callback) where T : INotifyPropertyChanged
		{
			PDFx.TunnelledRegisterCallbackDependency(masterPropertyOwner, masterProperty, callback);
		}

		protected void RegisterDeferredCallbackDependency<T, T1>(T masterPropertyOwner, Expression<Func<T, T1>> masterProperty,
		                                                         Action callback) where T : INotifyPropertyChanged
		{
			PDFx.TunnelledRegisterDeferredCallbackDependency(masterPropertyOwner, masterProperty, callback);
		}

		protected void RegisterDeferredCallbackDependency<T, T1>(
			DependencyFrameworkObservableCollection<T> masterPropertyOwnerCollection, Expression<Func<T, T1>> masterProperty,
			Action callback) where T : INotifyPropertyChanged
		{
			PDFx.TunnelledRegisterDeferredCallbackDependency(masterPropertyOwnerCollection, masterProperty, callback);
		}

		protected void RegisterDeferredCallbackDependency<T, T1>(T masterPropertyOwner, Expression<Func<T, T1>> masterProperty,
		                                                         Action callback, int delayInMilliseconds)
			where T : INotifyPropertyChanged
		{
			PDFx.TunnelledRegisterDeferredCallbackDependency(masterPropertyOwner, masterProperty, callback, delayInMilliseconds);
		}

		protected void RegisterDeferredCallbackDependency<T>(T masterPropertyOwner, Action callback)
			where T : INotifyPropertyChanged
		{
			PDFx.TunnelledRegisterDeferredCallbackDependency(masterPropertyOwner, callback);
		}

		protected void RegisterDeferredCallbackDependency<T>(T[] masterPropertyOwners, Action callback)
			where T : INotifyPropertyChanged
		{
			PDFx.TunnelledRegisterDeferredCallbackDependency(masterPropertyOwners, callback);
		}

		protected void RegisterDeferredCallbackDependency<T>(T masterPropertyOwner, Action callback, int delayInMilliseconds)
			where T : INotifyPropertyChanged
		{
			PDFx.TunnelledRegisterDeferredCallbackDependency(masterPropertyOwner, callback, delayInMilliseconds);
		}

		protected void RegisterDeferredCallbackDependency<T>(
			DependencyFrameworkObservableCollection<T> masterPropertyOwnerCollection, Action callback)
		{
			PDFx.TunnelledRegisterDeferredCallbackDependency(masterPropertyOwnerCollection, callback);
		}

		protected void RegisterDeferredCallbackDependency<T, T1>(T[] masterPropertyOwners,
		                                                         Expression<Func<T, T1>> masterProperty,
		                                                         Action callback) where T : INotifyPropertyChanged
		{
			PDFx.TunnelledRegisterDeferredCallbackDependency(masterPropertyOwners, masterProperty, callback);
		}

		protected void RegisterPropertyDependency<TCollectionType, TCollectionItemPropertyType, TProperty>(
			Expression<Func<DependencyFrameworkObservableCollection<TCollectionType>>> collectionPropertyGetter,
			Expression<Func<TCollectionType, TCollectionItemPropertyType>> collectionChildProperty,
			Expression<Func<TProperty>> dependentProperty) where TCollectionType : INotifyPropertyChanged
		{
			PDFx.TunnelledRegisterPropertyDependency(collectionPropertyGetter, collectionChildProperty, dependentProperty);
		}

		protected void RegisterPropertyDependency<T, T1, T2>(T masterPropertyOwner, Expression<Func<T, T1>> masterProperty,
		                                                     Expression<Func<T2>> dependentProperty)
			where T : INotifyPropertyChanged
		{
			PDFx.TunnelledRegisterPropertyDependency(masterPropertyOwner, masterProperty, dependentProperty);
		}

		protected void RegisterPropertyDependency<T, T1, T2>(
			DependencyFrameworkObservableCollection<T> masterPropertyOwnerCollection, Expression<Func<T, T1>> masterProperty,
			Expression<Func<T2>> dependentProperty) where T : INotifyPropertyChanged
		{
			PDFx.TunnelledRegisterPropertyDependency(masterPropertyOwnerCollection, masterProperty, dependentProperty);
		}

		protected void RegisterPropertyDependency<TOwner, TProperty, TDependantPropertyType>(
			Expression<Func<TOwner>> ownerProvider,
			Expression<Func<TOwner, TProperty>> property,
			Expression<Func<TDependantPropertyType>> dependantProperty) where TOwner : INotifyPropertyChanged
		{
			PDFx.TunnelledRegisterPropertyDependency(ownerProvider, property, dependantProperty);
		}

		protected void UnregisterAllPropertyDependencies()
		{
			PDFx.TunnelledUnregisterAllPropertyDependencies();
		}

		protected void UnRegisterPropertyDependency(INotifyCollectionChanged masterPropertyOwnerCollection,
		                                            string masterPropertyName, string dependantPropertyName)
		{
			PDFx.TunnelledUnRegisterPropertyDependency(masterPropertyOwnerCollection, masterPropertyName, dependantPropertyName);
		}

		protected void UnRegisterPropertyDependency(INotifyPropertyChanged masterPropertyOwner, string masterPropertyName,
		                                            string dependantPropertyName)
		{
			PDFx.TunnelledUnRegisterPropertyDependency(masterPropertyOwner, masterPropertyName, dependantPropertyName);
		}

		protected Delegate[] GetPropertyChangedInTransactionInvocationList()
		{
			if (PropertyChangedInTransaction==null)
				return new Delegate[]{};

			return PropertyChangedInTransaction.GetInvocationList();
		}

		protected Delegate[] GetPropertyChangedInvocationList()
		{
			var fieldInfo = typeof(ObservableObject).GetField(
				"PropertyChanged", BindingFlags.Instance | BindingFlags.NonPublic);
			var field = fieldInfo.GetValue(this);
			MulticastDelegate eventDelegate = (MulticastDelegate)field;
			if (eventDelegate != null) // will be null if no subscribed event consumers
			{
				var delegates = eventDelegate.GetInvocationList().ToList();
				return delegates.ToArray();
			}
			return new Delegate[]{};
		}

		void IDependencyFrameworkNotifyPropertyChangedInTransaction.FirePropertyChanged(string propertyName)
		{
			((IDependencyFrameworkNotifyPropertyChangedInTransaction)PDFx).FirePropertyChanged(propertyName);
		}

		public event EventHandler<PropertyChangedEventArgs> PropertyChangedInTransaction;
	}
}