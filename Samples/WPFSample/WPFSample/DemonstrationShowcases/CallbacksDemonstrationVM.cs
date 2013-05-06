using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyDependencyFramework;

namespace WPFSample.DemonstrationShowcases
{
	class CallbacksDemonstrationVM : BindableExt
	{
		public CallbacksDemonstrationVM()
		{
			Articles = new DependencyFrameworkObservableCollection<ArticleVM>();
			Articles.Add(new ArticleVM(Articles) { NetPrice = 100});
			Articles.Add(new ArticleVM(Articles) { NetPrice = 200 });
			Articles.Add(new ArticleVM(Articles) { NetPrice = 300 });

			LogEntries = new ObservableCollection<string>();

			RegisterCallbacks();
		}

		private void RegisterCallbacks()
		{
			RegisterDeferredCallbackDependency(this, k => TotalGross, SubmitPriceToServer);
			RegisterCallbackDependency(Articles, k => k.NetPrice, AnyNetPriceHasChanged);
		}

		public DependencyFrameworkObservableCollection<ArticleVM> Articles { get; set; }
		public ObservableCollection<string> LogEntries { get; set; }

		public DelegateCommand AddNewArticleCommand
		{
			get
			{
				return new DelegateCommand(() => Articles.Add(new ArticleVM(Articles) { NetPrice=50}));
			}
		}

		private double _salesTax;
		public double SalesTax
		{
			get { return _salesTax; }
			set { _salesTax = value; NotifyPropertyChanged(() => SalesTax); }
		}

		public double TotalNet
		{
			get
			{
				Property(() => TotalNet)
					.Depends(p => p.OnCollectionChildProperty(Articles, k => k.NetPrice));

				if (Articles.Count == 0)
					return 0;

				return Articles.Sum(k => k.NetPrice);
			}
		}

		public double TotalGross
		{
			get
			{
				Property(() => TotalGross)
					.Depends(p => p.On(() => TotalNet)
					               .AndOn(() => SalesTax));

				return TotalNet*(1 + ((SalesTax)/100));
			}
		}

		public override string ToString()
		{
			return "Callbacks Demonstration";
		}

		void SubmitPriceToServer()
		{
			AddLogEntry("Submitting new Price to Server...");
		}

		void AnyNetPriceHasChanged()
		{
			AddLogEntry("Total Net has changed to: " + TotalNet);
		}

		void AddLogEntry(string message)
		{
			LogEntries.Insert(0, DateTime.Now.ToLongTimeString() + " - " + message);
		}
		 
		internal class ArticleVM : BindableExt
		{
			private IList<ArticleVM> _articles;

			public ArticleVM(IList<ArticleVM> articles)
			{
				_articles = articles;
			}

			private double _netPrice;
			public double NetPrice
			{
				get { return _netPrice; }
				set
				{
					_netPrice = value;
					NotifyPropertyChanged(() => NetPrice);
				}
			}

			public DelegateCommand RemoveCommand
			{
				get
				{
					return new DelegateCommand(() => { _articles.Remove(this); });
				}
			}
		}
	}
}
