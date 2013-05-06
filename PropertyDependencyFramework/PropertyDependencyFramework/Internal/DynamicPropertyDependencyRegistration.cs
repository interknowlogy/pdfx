using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PropertyDependencyFramework
{
	internal class DynamicPropertyDependencyRegistration
	{
		public DynamicPropertyDependencyRegistration()
		{
			RegistrationOfPropertyDependencyActions = new List<Action<object>>();
			UnRegistrationOfPropertyDependencyActions = new List<Action<object>>();
		}

		public string MasterPropertyOwnerGetterPropertyName { get; set; }
		public Func<object> MasterPropertyOwnerPropertyGetter { get; set; }
		public object OldMasterPropertyOwner { get; set; }

		public List<Action<object>> RegistrationOfPropertyDependencyActions { get; private set; }
		public List<Action<object>> UnRegistrationOfPropertyDependencyActions { get; private set; }
	}
}
