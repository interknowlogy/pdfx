using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyDependencyFramework;

namespace WPFSample.DemonstrationShowcases
{
	class SimpleCollectionDependencyDemonstrationVM : BindableExt
	{
		public SimpleCollectionDependencyDemonstrationVM()
		{
			Destination = new DestinationVM();
		}

		public DestinationVM Destination { get; set; }

		internal class DestinationVM : BindableExt
		{
			public DestinationVM()
			{
				Children.Add(new ChildVM(_children));
				Children.Add(new ChildVM(_children));
				Children.Add(new ChildVM(_children));
			}

			private DependencyFrameworkObservableCollection<ChildVM> _children = new DependencyFrameworkObservableCollection<ChildVM>();
			public DependencyFrameworkObservableCollection<ChildVM> Children
			{
				get { return _children; }
			}

			public DelegateCommand AddCommand
			{
				get
				{
					return new DelegateCommand(() => Children.Add(new ChildVM(Children)));
				}
			}

			public int A1
			{
				get
				{
					Property(() => A1)
						.Depends(p => p.On(() => B1).AndOn(() => B2));
					
					return B1 + B2;
				}
			}

			public int B1
			{
				get
				{
					Property(() => B1)
						.Depends(p => p.OnCollectionChildProperty(Children, k => k.A1));

					if (Children.Count == 0)
						return 0;

					return Children.Select(k => k.A1).Sum();
				}
			}

			private int _b2 = 0;
			public int B2
			{
				get { return _b2; }
				set { _b2 = value;
					NotifyPropertyChanged(() => B2);
				}
			}
		}

		internal class ChildVM : BindableExt
		{
			private DependencyFrameworkObservableCollection<ChildVM> _children;

			public ChildVM(DependencyFrameworkObservableCollection<ChildVM> children)
			{
				_children = children;
			}

			private int _b1 = 1;
			public int B1
			{
				get { return _b1; }
				set { _b1 = value; NotifyPropertyChanged(() => B1); }
			}

			public int A1
			{
				get
				{
					Property(() => A1)
						.Depends(p => p.On(() => B1));
					
					return B1 * 3;
				}
			}

			public DelegateCommand RemoveCommand
			{
				get
				{
					return new DelegateCommand(() => _children.Remove(this));
				}
			}
		}

		public override string ToString()
		{
			return "Simple Collection Dependency Demonstration";
		}
	}
}
