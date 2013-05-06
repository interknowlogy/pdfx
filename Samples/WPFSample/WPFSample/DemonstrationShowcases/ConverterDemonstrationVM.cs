using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PropertyDependencyFramework;

namespace WPFSample.DemonstrationShowcases
{
	class ConverterDemonstrationVM : BindableExt
	{
		ConverterDemonstrationModel _model = new ConverterDemonstrationModel();

		private const double EURInUSD = 1.3;

		//Two Way Converter
		public double MoneyInUSD
		{
			get
			{
				Property(() => MoneyInUSD)
					.Depends(p => p.On(_model, k => k.MoneyInEuros));

				return _model.MoneyInEuros*EURInUSD;
			}
			set { _model.MoneyInEuros = value/EURInUSD; }
		}

		//Simple Delegation
		public double MoneyInEUR
		{
			get
			{
				Property(() => MoneyInEUR)
					.Depends(p => p.On(_model, k => k.MoneyInEuros));
				
				return _model.MoneyInEuros;
			}
			set { _model.MoneyInEuros = value; }
		}

		//One Way Converter
		public string MoneyQuantification
		{
			get
			{
				Property(() => MoneyQuantification)
					.Depends(p => p.On(() => MoneyInUSD));

				if (MoneyInUSD < 0)
				{
					return "Poor";
				}

				if (MoneyInUSD < 1000)
				{
					return "Lower class";
				}

				if (MoneyInUSD < 10000)
				{
					return "Middle class";
				}

				return "Upper class";
			}
		}

		//One Way Wrapper Converter
		public PersonModelVM Person
		{
			get
			{
				Property(() => Person)
					.Depends(p => p.On(_model, k => k.PersonModel));

				return CachedValue(() => Person, () => new PersonModelVM(_model.PersonModel));
			}
		}

		public override string ToString()
		{
			return "Converter Demonstration";
		}
	}

	class PersonModelVM : BindableExt
	{
		private PersonModel _model;

		public PersonModelVM(PersonModel model)
		{
			_model = model;
		}

		public string FullName
		{
			get { return _model.FirstName + " " + _model.LastName; }
		}

		private double _caloriesBurnedToday;
		public double CaloriesBurnedToday
		{
			get { return _caloriesBurnedToday; }
			set { _caloriesBurnedToday = value; NotifyPropertyChanged(() => CaloriesBurnedToday); }
		}

		public DelegateCommand GoForARunCommand
		{
			get
			{
				return new DelegateCommand(() => { CaloriesBurnedToday += 1300; });
			}
		}
	}

	class ConverterDemonstrationModel : BindableExt
	{
		private double _moneyInEuros = 100;
		public double MoneyInEuros
		{
			get { return _moneyInEuros; }
			set
			{
				_moneyInEuros = value;
				NotifyPropertyChanged(() => MoneyInEuros);
			}
		}

		private PersonModel _personModel = new PersonModel() { FirstName = "John", LastName = "Doe"};
		public PersonModel PersonModel
		{
			get { return _personModel; }
			set
			{
				_personModel = value;
				NotifyPropertyChanged(() => PersonModel);
			}
		}
	}

	class PersonModel : BindableExt
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
	}
}
