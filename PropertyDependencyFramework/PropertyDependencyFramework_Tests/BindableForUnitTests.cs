using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyDependencyFramework;

namespace PropertyDependencyFramework_Tests
{
	public class BindableForUnitTests : Bindable
	{
		public BindableForUnitTests()
		{
		}

		public BindableForUnitTests(bool useSmartPropertyChangeNotificationByDefault) : base(useSmartPropertyChangeNotificationByDefault)
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
	}
}
