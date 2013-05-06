using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyDependencyFramework;

namespace WPFSample
{
	class ExternalDependencyVM : BindableExt
	{
		public ExternalDependencyVM()
		{
			Destination = new DestinationVM();
		}

		public DestinationVM Destination { get; set; }

		public override string ToString()
		{
			return "External Dependency";
		}

		public class DestinationVM : BindableExt
		{
			public DestinationVM()
			{
				ExternalSource1 = new ExternalSource1();
				ExternalSource2 = new ExternalSource2();
			}

			public ExternalSource1 ExternalSource1 { get; private set; }
			public ExternalSource2 ExternalSource2 { get; private set; }

			public int A1
			{
				get
				{
					Property(() => A1)
						.Depends(p => p.On(() => B1)
									   .AndOn(ExternalSource1, k => k.A1)
									   .AndOn(ExternalSource2, k => k.A1));

					return B1 + ExternalSource1.A1 - ExternalSource2.A1;
				}
			}

			private int _b1;
			public int B1
			{
				get { return _b1; }
				set { _b1 = value; NotifyPropertyChanged(() => B1); }
			}
		}

		public class ExternalSource1 : BindableExt
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

		public class ExternalSource2 : BindableExt
		{
			private int _a1;
			public int A1
			{
				get { return _a1; }
				set { _a1 = value; NotifyPropertyChanged(() => A1); }
			}
		}
	}
}
