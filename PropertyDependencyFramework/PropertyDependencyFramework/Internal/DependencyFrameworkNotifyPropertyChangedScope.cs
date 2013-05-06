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
		private Dictionary<IDependencyFrameworkNotifyPropertyChangedInTransaction, List<string>> _queuedSourcePropertyChanges = new Dictionary<IDependencyFrameworkNotifyPropertyChangedInTransaction, List<string>>();
		private List<Action> _queuedSourceCollectionChanges = new List<Action>();

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

		public void QueueSourcePropertyChangeForDeferredExecution(IDependencyFrameworkNotifyPropertyChangedInTransaction propertyOwner, string propertyName)
		{
			if (!_queuedSourcePropertyChanges.ContainsKey(propertyOwner))
				_queuedSourcePropertyChanges.Add(propertyOwner, new List<string>());

			if (!_queuedSourcePropertyChanges[propertyOwner].Contains(propertyName))
				_queuedSourcePropertyChanges[propertyOwner].Add(propertyName);
		}

		public void QueueSourceCollectionChangeForDeferredExecution(Action internalCollectionChangeEvent)
		{
			_queuedSourceCollectionChanges.Add(internalCollectionChangeEvent);
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
			FirePropertyChangedArgs();
			AreSourcePropertyChangesQueuedForDeferredExecution = false;
			IsPropertyChangeConcatenationEnabled = true;

			Current = null;

			FireQueuedSourcePropertyChangedArgs();
		}

		private void FirePropertyChangedArgs()
		{
			foreach (IDependencyFrameworkNotifyPropertyChangedInTransaction propertyOwner in _queuedPropertyChanges.Keys)
			{
				foreach (string property in _queuedPropertyChanges[propertyOwner])
					propertyOwner.FirePropertyChanged(property);
			}
		}

		private void FireQueuedSourcePropertyChangedArgs()
		{
			if (_queuedSourcePropertyChanges.Count == 0 && _queuedSourceCollectionChanges.Count == 0)
				return;

			using (new DependencyFrameworkNotifyPropertyChangedScope())
			{
				foreach (IDependencyFrameworkNotifyPropertyChangedInTransaction propertyOwner in _queuedSourcePropertyChanges.Keys)
				{
					foreach (string property in _queuedSourcePropertyChanges[propertyOwner])
						propertyOwner.FirePropertyChanged(property);
				}

				foreach (var queuedSourceCollectionChange in _queuedSourceCollectionChanges)
					queuedSourceCollectionChange();
			}
		}
	}
}
