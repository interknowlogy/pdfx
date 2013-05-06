using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PropertyDependencyFramework
{
	internal class CollectionPropertyDependencyRegistration
	{
		public CollectionPropertyDependencyRegistration()
		{
			ItemPropertyDependencyRegistrationDelegates = new Dictionary<RegistrationIdentifier, Action<object>>();
			DependentProperties = new List<string>();
			Callbacks = new List<CallbackContainer>();
		}

		public class CallbackContainerRegistrationIdentifier : RegistrationIdentifier
		{
			public CallbackContainerRegistrationIdentifier(string collectionMasterPropertyName, CallbackContainer callback)
				: base(collectionMasterPropertyName)
			{
				Callback = callback;
			}

			public CallbackContainer Callback { get; private set; }
		}

		public class CallbackRegistrationIdentifier : RegistrationIdentifier
		{
			public CallbackRegistrationIdentifier(string collectionMasterPropertyName, Action callback)
				: base(collectionMasterPropertyName)
			{
				Callback = callback;
			}

			public Action Callback { get; private set; }
		}
		public class PropertyRegistrationIdentifier : RegistrationIdentifier
		{
			public PropertyRegistrationIdentifier(string collectionMasterPropertyName, string dependentPropertyName)
				: base(collectionMasterPropertyName)
			{
				DependentPropertyName = dependentPropertyName;
			}

			public string DependentPropertyName { get; private set; }
		}

		public abstract class RegistrationIdentifier
		{
			protected RegistrationIdentifier(string collectionMasterPropertyName)
			{
				CollectionMasterPropertyName = collectionMasterPropertyName;
			}

			public string CollectionMasterPropertyName { get; private set; }
		}

		public Dictionary<RegistrationIdentifier, Action<object>> ItemPropertyDependencyRegistrationDelegates { get; private set; }
		public List<string> DependentProperties { get; private set; }
		public List<CallbackContainer> Callbacks { get; private set; }
	}
}
