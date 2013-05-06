using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyDependencyFramework;

namespace WPFSample
{
	public class SimplePropertyDependencyVM : BindableExt
	{
		public bool UseSmartPropertyChangeNotification
		{
			get { return UseSmartPropertyChangeNotificationByDefault; }
			set { UseSmartPropertyChangeNotificationByDefault = value; }
		}

		private int _d1;
		public int D1
		{
			get { return _d1; }
			set
			{
				_d1 = value; 
				NotifyPropertyChanged(() => D1);
			}
		}

		private int _d2;
		public int D2
		{
			get { return _d2; }
			set
			{
				_d2 = value;
				NotifyPropertyChanged(() => D2);
			}
		}

		private int _d3;
		public int D3
		{
			get { return _d3; }
			set
			{
				_d3 = value;
				NotifyPropertyChanged(() => D3);
			}
		}

		private int _d4;
		public int D4
		{
			get { return _d4; }
			set
			{
				_d4 = value;
				NotifyPropertyChanged(() => D4);
			}
		}

		private int _d5;
		public int D5
		{
			get { return _d5; }
			set
			{
				_d5 = value;
				NotifyPropertyChanged(() => D5);
			}
		}

		public int C1
		{
			get
			{
				Property(() => C1)
					.Depends(p => p.On(() => D1)
					               .AndOn(() => D2));

				return D1 + D2;
			}
		}

		public int C2
		{
			get
			{
				Property(() => C2)
					.Depends(p => p.On(() => D3));

				return 3*D3;
			}
		}

		public int C3
		{
			get
			{
				Property(() => C3)
					.Depends(p => p.On(() => D4)
					               .AndOn(() => D5));
				
				return D4 + D5;
			}
		}

		public int B1
		{
			get
			{
				Property(() => B1)
					.Depends(p => p.On(() => C1)
					               .AndOn(() => C2));
				
				return 2*C1 - C2;
			}
		}

		public int B2
		{
			get
			{
				Property(() => B2)
					.Depends(p => p.On(() => C2)
					               .AndOn(() => C3));
				
				return -C2 + C3;
			}
		}

		public int A1
		{
			get
			{
				Property(() => A1)
					.Depends(p => p.On(() => B1)
					               .AndOn(() => B2));
				
				return B1 + B2;
			}
		}

		public override string ToString()
		{
			return "Simple Property Dependency Demonstration";
		}
	}
}
