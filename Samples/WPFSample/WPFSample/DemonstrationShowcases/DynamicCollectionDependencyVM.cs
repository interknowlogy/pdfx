using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyDependencyFramework;

namespace WPFSample.DemonstrationShowcases
{
	class DynamicCollectionDependencyVM : Bindable
	{
		public DynamicCollectionDependencyVM()
		{
			Destination = new DestinationVM();
		}

		public DestinationVM Destination { get; set; }

		internal class DestinationVM : BindableExt
		{
			public DestinationVM()
			{
				Children1 = new DependencyFrameworkObservableCollection<ChildVM>();
				Children2 = new DependencyFrameworkObservableCollection<ChildVM>();
				Children = Children1;

				Children1.Add(new ChildVM(Children1));
				Children1.Add(new ChildVM(Children1));
				Children1.Add(new ChildVM(Children1));

				Children2.Add(new ChildVM(Children2) { B1 = 2 });
				Children2.Add(new ChildVM(Children2) { B1 = 2 });
				Children2.Add(new ChildVM(Children2) { B1 = 2 });
			}

			private DependencyFrameworkObservableCollection<ChildVM> _children = new DependencyFrameworkObservableCollection<ChildVM>();
			public DependencyFrameworkObservableCollection<ChildVM> Children
			{
				get { return _children; }
				set
				{
					_children = value;
					NotifyPropertyChanged(() => Children);
				}
			}
			
			
			public DependencyFrameworkObservableCollection<ChildVM> Children1 { get; set; }
			public DependencyFrameworkObservableCollection<ChildVM> Children2 { get; set; }

			public DelegateCommand AddToChildren1Command
			{
				get
				{
					return new DelegateCommand(() => Children1.Add(new ChildVM(Children1)));
				}
			}

			public DelegateCommand AddToChildren2Command
			{
				get
				{
					return new DelegateCommand(() => Children2.Add(new ChildVM(Children2)));
				}
			}

			public DelegateCommand UseLeftCommand
			{
				get
				{
					return new DelegateCommand(() => Children = Children1);
				}
			}

			public DelegateCommand UseRightCommand
			{
				get
				{
					return new DelegateCommand(() => Children = Children2);
				}
			}

			public bool IsUsingChildren1
			{
				get
				{
					Property(() => IsUsingChildren1)
						.Depends(p => p.On(() => Children));
					
					return Children == Children1;
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
						.Depends(p => p.OnCollectionChildProperty(() => Children, k => k.A1));

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
			return "Dynamic Collection Dependency Demonstration";
		}
	}
}
