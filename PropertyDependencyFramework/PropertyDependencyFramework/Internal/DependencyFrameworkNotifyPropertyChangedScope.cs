using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PropertyDependencyFramework
{
	internal class DependencyFrameworkNotifyPropertyChangedScope : IDisposable
	{
		public static DependencyFrameworkNotifyPropertyChangedScope Current { get; set; }
		public static bool ArePropertyChangesCollected { get; private set; }
		public static bool IsPropertyChangeConcatenationEnabled { get; private set; }
		public static bool AreSourcePropertyChangesQueuedForDeferredExecution { get; private set; }


		static DependencyFrameworkNotifyPropertyChangedScope()
		{
			ArePropertyChangesCollected = false;
			IsPropertyChangeConcatenationEnabled = true;
		}

		private Dictionary<IDependencyFrameworkNotifyPropertyChangedInTransaction, List<string>> _queuedPropertyChanges = new Dictionary<IDependencyFrameworkNotifyPropertyChangedInTransaction, List<string>>();
        private List<CallbackContainer> _queuedCollectionCallbacks = new List<CallbackContainer>();
        private Dictionary<IDependencyFrameworkNotifyPropertyChangedInTransaction, List<string>> _deferredSourcePropertyChanges = new Dictionary<IDependencyFrameworkNotifyPropertyChangedInTransaction, List<string>>();
		private List<Action> _deferredSourceCollectionChanges = new List<Action>();

		public bool IsPropertyChangedQueued(IDependencyFrameworkNotifyPropertyChangedInTransaction propertyOwner, string propertyName)
		{
			if (!_queuedPropertyChanges.ContainsKey(propertyOwner))
				return false;

			if (!_queuedPropertyChanges[propertyOwner].Contains(propertyName))
				return false;

			return true;
		}

		public void QueuePropertyChange(IDependencyFrameworkNotifyPropertyChangedInTransaction propertyOwner, string propertyName)
		{
			if (!_queuedPropertyChanges.ContainsKey(propertyOwner))
				_queuedPropertyChanges.Add(propertyOwner, new List<string>());

			if (!_queuedPropertyChanges[propertyOwner].Contains(propertyName))
				_queuedPropertyChanges[propertyOwner].Add(propertyName);
		}

	    public void QueueCollectionCallback(CallbackContainer container)
	    {
	        if (!_queuedCollectionCallbacks.Contains(container))
	        {
	            _queuedCollectionCallbacks.Add(container);
	        }
	    }

	    public void DeferSourcePropertyChangeForDeferredExecution(IDependencyFrameworkNotifyPropertyChangedInTransaction propertyOwner, string propertyName)
		{
			if (!_deferredSourcePropertyChanges.ContainsKey(propertyOwner))
				_deferredSourcePropertyChanges.Add(propertyOwner, new List<string>());

			if (!_deferredSourcePropertyChanges[propertyOwner].Contains(propertyName))
				_deferredSourcePropertyChanges[propertyOwner].Add(propertyName);
		}

		public void DeferSourceCollectionChangeForDeferredExecution(Action internalCollectionChangeEvent)
		{
			_deferredSourceCollectionChanges.Add(internalCollectionChangeEvent);
		}

		public DependencyFrameworkNotifyPropertyChangedScope()
		{
			if (Current != null)
				throw new InvalidOperationException("NotifyPropertyChangedScope is already open");

			Current = this;
			ArePropertyChangesCollected = true;
			IsPropertyChangeConcatenationEnabled = true;
		}

		public void Dispose()
		{
			ArePropertyChangesCollected = false;
			IsPropertyChangeConcatenationEnabled = false;
			AreSourcePropertyChangesQueuedForDeferredExecution = true;
			FireQueuedPropertyChangedArgs();
            FireQueuedCollectionCallbacks();
			AreSourcePropertyChangesQueuedForDeferredExecution = false;
			IsPropertyChangeConcatenationEnabled = true;

			Current = null;

			FireDeferredSourcePropertyChangedArgs();
		}

	    private void FireQueuedCollectionCallbacks()
	    {
            foreach (CallbackContainer callback in _queuedCollectionCallbacks)
                callback.Call();
        }

	    private void FireQueuedPropertyChangedArgs()
		{
			foreach (IDependencyFrameworkNotifyPropertyChangedInTransaction propertyOwner in _queuedPropertyChanges.Keys)
			{
				foreach (string property in _queuedPropertyChanges[propertyOwner])
					propertyOwner.FirePropertyChanged(property);
			}
		}

		private void FireDeferredSourcePropertyChangedArgs()
		{
			if (_deferredSourcePropertyChanges.Count == 0 && _deferredSourceCollectionChanges.Count == 0)
				return;

			using (new DependencyFrameworkNotifyPropertyChangedScope())
			{
				foreach (IDependencyFrameworkNotifyPropertyChangedInTransaction propertyOwner in _deferredSourcePropertyChanges.Keys)
				{
					foreach (string property in _deferredSourcePropertyChanges[propertyOwner])
						propertyOwner.FirePropertyChanged(property);
				}

				foreach (var queuedSourceCollectionChange in _deferredSourceCollectionChanges)
					queuedSourceCollectionChange();
			}
		}
	}
}
