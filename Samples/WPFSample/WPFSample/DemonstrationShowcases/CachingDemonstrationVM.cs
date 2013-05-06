using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using PropertyDependencyFramework;

namespace WPFSample.DemonstrationShowcases
{
	class CachingDemonstrationVM : BindableExtTunnel
	{
		public CachingDemonstrationVM()
		{
			EnsureThatAllPropertiesGetReevaluatedWhenCacheIsToggled();
		}

		private void EnsureThatAllPropertiesGetReevaluatedWhenCacheIsToggled()
		{
			RegisterPropertyDependency(this, k => k.UseCaching, () => E1);
			RegisterPropertyDependency(this, k => k.UseCaching, () => E2);
			RegisterPropertyDependency(this, k => k.UseCaching, () => E3);
			RegisterPropertyDependency(this, k => k.UseCaching, () => E4);
			RegisterPropertyDependency(this, k => k.UseCaching, () => E5);
			RegisterPropertyDependency(this, k => k.UseCaching, () => E6);
		}

		private int _e1;
		public int E1
		{
			get { return _e1; }
			set
			{
				_e1 = value;
				ResetNumberOfPropertyEvaluations();
				NotifyPropertyChanged(() => E1);
			}
		}

		private int _e2;
		public int E2
		{
			get { return _e2; }
			set
			{
				_e2 = value;
				ResetNumberOfPropertyEvaluations();
				NotifyPropertyChanged(() => E2);
			}
		}

		private int _e3;
		public int E3
		{
			get { return _e3; }
			set
			{
				_e3 = value;
				ResetNumberOfPropertyEvaluations();
				NotifyPropertyChanged(() => E3);
			}
		}

		private int _e4;
		public int E4
		{
			get { return _e4; }
			set
			{
				_e4 = value;
				ResetNumberOfPropertyEvaluations();
				NotifyPropertyChanged(() => E4);
			}
		}

		private int _e5;
		public int E5
		{
			get { return _e5; }
			set
			{
				_e5 = value;
				ResetNumberOfPropertyEvaluations();
				NotifyPropertyChanged(() => E5);
			}
		}

		private int _e6;
		public int E6
		{
			get { return _e6; }
			set
			{
				_e6 = value;
				ResetNumberOfPropertyEvaluations();
				NotifyPropertyChanged(() => E6);
			}
		}

		public int D1
		{
			get
			{
				Property(() => D1).Depends(p => p.On(() => E1));

				return CachedValue(() => D1, () => E1);
			}
		}

		public int D2
		{
			get
			{
				Property(() => D2).Depends(p => p.On(() => E2));

				return CachedValue(() => D2, () => E2);
			}
		}

		public int D3
		{
			get
			{
				Property(() => D3).Depends(p => p.On(() => E3));

				return CachedValue(() => D3, () => E3);
			}
		}

		public int D4
		{
			get
			{
				Property(() => D4).Depends(p => p.On(() => E4));

				return CachedValue(() => D4, () => E4);
			}
		}

		public int D5
		{
			get
			{
				Property(() => D5).Depends(p => p.On(() => E5));

				return CachedValue(() => D5, () => E5);
			}
		}

		public int D6
		{
			get
			{
				Property(() => D6).Depends(p => p.On(() => E6));

				return CachedValue(() => D6, () => E6);
			}
		}

		public int C1
		{
			get
			{
				Property(() => C1)
					.Depends(p => p.On(() => D1)
					               .AndOn(() => D2));

				return CachedValue(() => C1, () => D1 + D2);
			}
		}

		public int C2
		{
			get
			{
				Property(() => C2)
					.Depends(p => p.On(() => D3)
					               .AndOn(() => D4));

				return CachedValue(() => C2, () => 3*D3 + 3*D4);
			}
		}

		public int C3
		{
			get
			{
				Property(() => C3)
					.Depends(p => p.On(() => D5)
					               .AndOn(() => D6));
				
				return CachedValue(() => C3, () => D5 + D6);
			}
		}

		public int B1
		{
			get
			{
				Property(() => B1)
					.Depends(p => p.On(() => C1)
					               .AndOn(() => C2));
				
				return CachedValue(() => B1, () => 2*C1 - C2);
			}
		}

		public int B2
		{
			get
			{
				Property(() => B2)
					.Depends(p => p.On(() => C2)
					               .AndOn(() => C3));
				
				return CachedValue(() => B2, () => -C2 + C3);
			}
		}

		public int A1
		{
			get
			{
				Property(() => A1)
					.Depends(p => p.On(() => B2)
					               .AndOn(() => B1));
				
				return CachedValue(() => A1, () => B1 + B2);
			}
		}

		public override string ToString()
		{
			return "Caching Demonstration";
		}
	}

	class BindableExtTunnel : BindableExt
	{
		private SafeDictionary<string, int> _numberOfEvaluations = new SafeDictionary<string, int>();
		public SafeDictionary<string, int> PropertyEvaluations
		{
			get { return _numberOfEvaluations; }
		}

		private bool _useCaching = true;
		public bool UseCaching
		{
			get { return _useCaching; }
			set
			{
				_useCaching = value; 
				ClearUnderlyingCache();
				NotifyPropertyChanged(() => UseCaching);
			}
		}

		public new T CachedValue<T>(Expression<Func<T>> ofProperty, Func<T> propertyEvaluation)
		{
			string propertyName = PropertyNameResolver.GetPropertyName(ofProperty);

			if (UseCaching == false)
			{
				PropertyEvaluations[propertyName] = PropertyEvaluations[propertyName] + 1;
				NotifyPropertyEvaluationsWereModified();

				return propertyEvaluation();
			}

			return base.CachedValue(ofProperty, () =>
				                                    {
					                                    PropertyEvaluations[propertyName] = PropertyEvaluations[propertyName] + 1;
														NotifyPropertyEvaluationsWereModified();

					                                    return propertyEvaluation();
				                                    });
		}

		protected void ResetNumberOfPropertyEvaluations()
		{
			_numberOfEvaluations.Clear();
			NotifyPropertyEvaluationsWereModified();
		}

		protected void ClearUnderlyingCache()
		{
			dynamic _this = this;
			_this._cachedPropertyValues.Clear();
		}

		private void NotifyPropertyEvaluationsWereModified()
		{
			NotifyPropertyChanged(() => PropertyEvaluations);
		}
	}
}
