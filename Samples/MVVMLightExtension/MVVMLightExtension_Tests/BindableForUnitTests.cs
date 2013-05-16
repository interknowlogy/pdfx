using System;
using System.Linq.Expressions;
using MVVMLightExtension;
using PropertyDependencyFramework;

namespace MVVMLightExtension_Tests
{
	public class BindableForUnitTests : MVVMLightViewModel
	{
		public BindableForUnitTests()
		{
		}

		public BindableForUnitTests(bool useSmartPropertyChangeNotificationByDefault)
			: base(useSmartPropertyChangeNotificationByDefault)
		{
		}

		public bool AreEventHandlersRegisteredOnPropertyChanged
		{
			get
			{
				return GetPropertyChangedInvocationList().Length > 0;
			}
		}

		public bool AreEventHandlersRegisteredOnPropertyChangedInTransaction
		{
			get { return GetPropertyChangedInTransactionInvocationList().Length > 0; }
		}

		protected void NotifyPropertyChanged<T>(Expression<Func<T>> propertyExpression)
		{
			NotifyPropertyChanged(PropertyNameResolver.GetPropertyName(propertyExpression));
		}

		protected void NotifyPropertyChanged(string propertyName)
		{
			RaisePropertyChanged(propertyName);
		}
	}
}
