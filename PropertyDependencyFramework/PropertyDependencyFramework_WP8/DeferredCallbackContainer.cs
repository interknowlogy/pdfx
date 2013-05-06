using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace PropertyDependencyFramework
{
	internal class DeferredCallbackContainer : CallbackContainer
	{
		int DeferredExecutionDelayInMilliseconds { get; set; }

		private readonly DispatcherTimer _timer = new DispatcherTimer();

		public DeferredCallbackContainer(Action callback, int deferredExecutionDelayInMilliseconds)
			: base(callback)
		{
			DeferredExecutionDelayInMilliseconds = deferredExecutionDelayInMilliseconds;

			_timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(deferredExecutionDelayInMilliseconds) };
			_timer.Tick += (sender, args) =>
			{
				_timer.Stop();
				base.ExecuteCallback();
			};
		}

		public virtual void Call()
		{
			_timer.Stop();
			_timer.Start();
		}
	}
}
