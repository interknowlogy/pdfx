using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace PropertyDependencyFramework
{
	public class DependencyFrameworkObservableCollection<T> : ObservableCollection<T>
	{
		private readonly bool _openNewScopeOnCollectionChanged;

		public DependencyFrameworkObservableCollection(bool openNewScopeOnCollectionChanged = true)
		{
			_openNewScopeOnCollectionChanged = openNewScopeOnCollectionChanged;
		}

		public void ReplaceAllWith(IEnumerable<T> newObjects)
		{
			Action replaceWork = () =>
			{
				var newObjectsArray = newObjects.ToArray();
				foreach (var itemToRemove in this.Where(k => newObjectsArray.Contains(k) == false).ToArray())
				{
					Remove(itemToRemove);
				}

				foreach (var newItem in newObjectsArray.Where(k => this.Contains(k) == false).ToArray())
				{
					Add(newItem);
				}
			};

			if (_openNewScopeOnCollectionChanged)
			{
				if (DependencyFrameworkNotifyPropertyChangedScope.ArePropertyChangesCollected == false &&
					 DependencyFrameworkNotifyPropertyChangedScope.AreSourcePropertyChangesQueuedForDeferredExecution == false)
				{
					using (new DependencyFrameworkNotifyPropertyChangedScope())
					{
						replaceWork();
						return;
					}
				}
			}

			replaceWork();
		}

		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			if (_openNewScopeOnCollectionChanged)
			{
				if (DependencyFrameworkNotifyPropertyChangedScope.ArePropertyChangesCollected)
				{
					base.OnCollectionChanged(e);
					return;
				}

				if (DependencyFrameworkNotifyPropertyChangedScope.AreSourcePropertyChangesQueuedForDeferredExecution)
				{
					DependencyFrameworkNotifyPropertyChangedScope.Current.DeferSourceCollectionChangeForDeferredExecution(() => base.OnCollectionChanged(e));
					return;
				}

				using (new DependencyFrameworkNotifyPropertyChangedScope())
				{
					base.OnCollectionChanged(e);
				}
			}
			else
			{
				base.OnCollectionChanged(e);
			}
		}
		protected override void ClearItems()
		{
			Action replaceWork = () =>
			{
				while ( Count > 0 )
				{
					RemoveAt( 0 );
				}
			};
			if ( _openNewScopeOnCollectionChanged )
			{
				if ( DependencyFrameworkNotifyPropertyChangedScope.ArePropertyChangesCollected == false &&
					 DependencyFrameworkNotifyPropertyChangedScope.AreSourcePropertyChangesQueuedForDeferredExecution == false )
				{
					using ( new DependencyFrameworkNotifyPropertyChangedScope() )
					{
						replaceWork();
						return;
					}
				}
			}
			replaceWork();
		}
	}
}
