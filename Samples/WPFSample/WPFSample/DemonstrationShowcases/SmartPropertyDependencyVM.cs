using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyDependencyFramework;

namespace WPFSample
{
	public class SmartPropertyDependencyVM : BindableExt
	{
		private int _d1;

		public SmartPropertyDependencyVM()
		{
			PropertyChangeRecorder = new PropertyChangeRecorder(this);
		}

		public bool UseSmartNotification
		{
			get { return base.UseSmartPropertyChangeNotificationByDefault; }
			set { base.UseSmartPropertyChangeNotificationByDefault = value; PropertyChangeRecorder.Reset(); NotifyPropertyChanged(() => D1); }
		}

		public int A1
		{
			get
			{
				Property(() => A1)
					.Depends(p => p.On(() => B1)
								   .AndOn(() => B2)
								   .AndOn(() => B3)
								   .AndOn(() => B4)
								   .AndOn(() => B5)
								   .AndOn(() => B6));
				
				return B1 + B2 + B3 + B4 + B5 + B6;
			}
		}


		public int B1
		{
			get
			{
				Property(() => B1)
					.Depends(p => p.On(() => C1));

				return C1;
			}
		}

		public int B2
		{
			get
			{
				Property(() => B2)
					.Depends(p => p.On(() => C1).AndOn(() => C2));
				return C1 + C2;
			}
		}

		public int B3
		{
			get
			{
				Property(() => B3)
					.Depends(p => p.On(() => C2).AndOn(() => C3));
				return C2 + C3;
			}
		}

		public int B4
		{
			get
			{
				Property(() => B4)
					.Depends(p => p.On(() => C3).AndOn(() => C4));
				return C3 + C4;
			}
		}

		public int B5
		{
			get
			{
				Property(() => B5)
					.Depends(p => p.On(() => C4).AndOn(() => C5));

				return C4 + C5;
			}
		}

		public int B6
		{
			get
			{
				Property(() => B6)
					.Depends(p => p.On(() => C5));

				return C5;
			}
		}



		public int C1
		{
			get
			{
				Property(() => C1)
					.Depends(p => p.On(() => D1));
				return D1;
			}
		}

		public int C2
		{
			get
			{
				Property(() => C2)
					.Depends(p => p.On(() => D1));
				return D1;
			}
		}

		public int C3
		{
			get
			{
				Property(() => C3)
					.Depends(p => p.On(() => D1));
				return D1;
			}
		}

		public int C4
		{
			get
			{
				Property(() => C4)
					.Depends(p => p.On(() => D1));
				
				return 3*D1;
			}
		}

		public int C5
		{
			get
			{
				Property(() => C5)
					.Depends(p => p.On(() => D1));
				return 4*D1;
			}
		}

		public int D1
		{
			get { return _d1; }
			set { _d1 = value;
				PropertyChangeRecorder.Reset(); NotifyPropertyChanged(() => D1); }
		}

		public PropertyChangeRecorder PropertyChangeRecorder { get; private set; }

		public override string ToString()
		{
			return "Smart Property Dependency";
		}
	}
}
