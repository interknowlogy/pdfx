using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PropertyDependencyFramework
{
	internal class PropertyDependencies
	{
		public PropertyDependencies()
		{
			DependentProperties = new List<string>();
			Callbacks = new List<CallbackContainer>();
		}

		public List<string> DependentProperties { get; private set; }
		public List<CallbackContainer> Callbacks { get; private set; }
	}
}
