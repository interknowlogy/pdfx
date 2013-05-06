using System;
using System.ComponentModel;

namespace PropertyDependencyFramework
{
	public interface IDependencyFrameworkNotifyPropertyChangedInTransaction
	{
		void FirePropertyChanged(string propertyName);
		event EventHandler<PropertyChangedEventArgs> PropertyChangedInTransaction; 
	}
}