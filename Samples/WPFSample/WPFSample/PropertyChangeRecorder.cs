using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyDependencyFramework;

namespace WPFSample
{
	public class PropertyChangeRecorder : BindableExt
	{
		private INotifyPropertyChanged _recorderTarget;

		public PropertyChangeRecorder(INotifyPropertyChanged recorderTarget)
		{
			_recorderTarget = recorderTarget;
			
			_recorderTarget.PropertyChanged += _recorderTarget_PropertyChanged;
		}

		void _recorderTarget_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if(!_propertyToNumberOfChanges.ContainsKey(e.PropertyName))
				_propertyToNumberOfChanges.Add(e.PropertyName, 0);

			_propertyToNumberOfChanges[e.PropertyName]++;

			NotifyPropertyChanged(() => NumberOfPropertyChangesOfProperty);
		}

		private SafeDictionary<string, int> _propertyToNumberOfChanges = new SafeDictionary<string, int>();
		public SafeDictionary<string, int> NumberOfPropertyChangesOfProperty
		{
			get { return _propertyToNumberOfChanges; }
		}

		public void Reset()
		{
			NumberOfPropertyChangesOfProperty.Clear();
			NotifyPropertyChanged(() => NumberOfPropertyChangesOfProperty);
		}
	}
}
