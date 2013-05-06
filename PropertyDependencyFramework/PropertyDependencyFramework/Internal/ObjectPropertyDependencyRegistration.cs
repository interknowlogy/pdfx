using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PropertyDependencyFramework
{
	internal class ObjectPropertyDependencyRegistration
	{
		public ObjectPropertyDependencyRegistration()
		{
			PropertyDependencies = new Dictionary<string, PropertyDependencies>();
			Callbacks = new List<CallbackContainer>();
		}

		public Dictionary<string, PropertyDependencies> PropertyDependencies { get; private set; }
		public List<CallbackContainer> Callbacks { get; private set; }
	}
}
