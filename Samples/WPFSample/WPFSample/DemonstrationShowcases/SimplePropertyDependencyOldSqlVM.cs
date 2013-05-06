using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyDependencyFramework;

namespace WPFSample.DemonstrationShowcases
{
	class SimplePropertyDependencyOldSqlVM : BindableExt
	{
		private int _d1;
		public int D1
		{
			get { return _d1; }
			set
			{
				_d1 = value;
				NotifyPropertyChanged(() => D1);
				RecalculateEverything();
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
				RecalculateEverything();
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
				RecalculateEverything();
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
				RecalculateEverything();
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
				RecalculateEverything();
			}
		}

		private int _c1;
		public int C1
		{
			get { return _c1; }
			set { _c1 = value; NotifyPropertyChanged(() => C1); }
		}

		private int _c2;
		public int C2
		{
			get { return _c2; }
			set { _c2 = value; NotifyPropertyChanged(() => C2); }
		}

		private int _c3;
		public int C3
		{
			get { return _c3; }
			set { _c3 = value; NotifyPropertyChanged(() => C3); }
		}

		private int _b1;
		public int B1
		{
			get { return _b1; }
			set { _b1 = value; NotifyPropertyChanged(() => B1); }
		}

		private int _b2;
		public int B2
		{
			get { return _b2; }
			set { _b2 = value; NotifyPropertyChanged(() => B2); }
		}

		private int _a1;
		public int A1
		{
			get { return _a1; }
			set { _a1 = value; NotifyPropertyChanged(() => A1); }
		}

		void RecalculateEverything()
		{
			C1 = D1 + D2;
			C2 = 3*D3;
			C3 = D4 + D5;
			B1 = 2*C1 - C2;
			B2 = C3 - C2;
			A1 = B1 + B2;
		}

		public override string ToString()
		{
			return "Simple Property Dependency OLD SQL Demonstration";
		}
	}
}
