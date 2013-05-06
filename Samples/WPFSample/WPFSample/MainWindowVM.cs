using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyDependencyFramework;
using WPFSample.DemonstrationShowcases;

namespace WPFSample
{
	public class MainWindowVM : BindableExt
	{
		public MainWindowVM()
		{
			Showcases = new object[]
				            {
					            new SimplePropertyDependencyOldSqlVM(), new SimplePropertyDependencyVM(), new ExternalDependencyVM(),
					            new DynamicExternalDependencyVM(), new SimpleCollectionDependencyDemonstrationVM(),
								new DynamicCollectionDependencyVM(), new ConverterDemonstrationVM(),
								new SmartPropertyDependencyVM(), new CachingDemonstrationVM(),
								new CallbacksDemonstrationVM()
				            };
			SelectedShowcase = Showcases.First();

		}

		public object[] Showcases { get; set; }

		private object _selectedShowcase;
		public object SelectedShowcase
		{
			get { return _selectedShowcase; }
			set { _selectedShowcase = value; NotifyPropertyChanged(() => SelectedShowcase); }
		}
	}
}
