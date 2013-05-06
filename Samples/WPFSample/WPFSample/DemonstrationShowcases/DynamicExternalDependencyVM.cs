using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyDependencyFramework;

namespace WPFSample.DemonstrationShowcases
{
	public class DynamicExternalDependencyVM : BindableExt
	{
		public DynamicExternalDependencyVM()
		{
			ExternalSource1 = new ExternalSource();
			ExternalSource2 = new ExternalSource();
			Destination = new DestinationVM() { ExternalSource = ExternalSource1 };
		}

		public DestinationVM Destination { get; set; }
		public ExternalSource ExternalSource1 { get; set; }
		public ExternalSource ExternalSource2 { get; set; }
		public bool IsUsingExternalSource1
		{
			get
			{
				Property(() => IsUsingExternalSource1)
					.Depends(p => p.On(Destination, k => k.ExternalSource));
				
				return Destination.ExternalSource == ExternalSource1;
			}
		}

		private DelegateCommand _useExternalSource1Command;
		public DelegateCommand UseExternalSource1Command
		{
			get
			{
				return
					_useExternalSource1Command =
					_useExternalSource1Command ?? new DelegateCommand(() => Destination.ExternalSource = ExternalSource1);
			}
		}

		private DelegateCommand _useExternalSource2Command;
		public DelegateCommand UseExternalSource2Command
		{
			get
			{
				return
					_useExternalSource2Command =
					_useExternalSource2Command ?? new DelegateCommand(() => Destination.ExternalSource = ExternalSource2);
			}
		}

		public class DestinationVM : BindableExt
		{
			public int A1
			{
				get
				{
					Property(() => A1)
						.Depends(p => p.On(() => B1)
									   .AndOn(() => ExternalSource, k => k.A1));

					return B1 + ExternalSource.A1;
				}
			}

			private ExternalSource _externalSource;
			public ExternalSource ExternalSource
			{
				get { return _externalSource; }
				set { _externalSource = value; NotifyPropertyChanged(() => ExternalSource);}
			}

			private int _b1;
			public int B1
			{
				get { return _b1; }
				set { _b1 = value; NotifyPropertyChanged(() => B1); }
			}
		}

		public class ExternalSource : BindableExt
		{
			public int A1
			{
				get
				{
					Property(() => A1)
						.Depends(p => p.On(() => B1));

					return 3 * B1;
				}
			}

			private int _b1;
			public int B1
			{
				get { return _b1; }
				set { _b1 = value; NotifyPropertyChanged(() => B1); }
			}
		}

		public override string ToString()
		{
			return "Dynamic External Dependency";
		}
	}
}
