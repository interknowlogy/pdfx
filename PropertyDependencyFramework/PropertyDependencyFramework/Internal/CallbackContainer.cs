using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PropertyDependencyFramework
{
	internal class CallbackContainer
	{
		Action Callback { get; set; }

		public CallbackContainer(Action callback)
		{
			Callback = callback;
		}


		public virtual void Call()
		{
			ExecuteCallback();
		}

		protected void ExecuteCallback()
		{
			Callback();
		}
	}
}
