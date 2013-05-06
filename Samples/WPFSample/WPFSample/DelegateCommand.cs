using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Input;
using System.Windows.Threading;
using PropertyDependencyFramework;

namespace WPFSample
{
	/// <summary>
	/// An <see cref="ICommand"/> whose delegates can be attached for <see cref="Execute"/> and <see cref="CanExecute"/>.
	/// </summary>
	/// <typeparam name="T">Parameter type.</typeparam>
	public class DelegateCommand<T> : ICommand
	{
		private readonly Action<T> _executeMethod;
		private readonly Func<T, bool> _canExecuteMethod;
		private readonly Dispatcher _dispatcher;
		private readonly List<string> _triggerProperties;

		/// <summary>
		/// Constructor. Initializes delegate command with Execute delegate.
		/// </summary>
		/// <param name="executeMethod">Delegate to execute when Execute is called on the command.  This can be null to just hook up a CanExecute delegate.</param>
		/// <remarks><seealso cref="CanExecute"/> will always return true.</remarks>
		public DelegateCommand(Action<T> executeMethod)
			: this(executeMethod, null)
		{
		}


		/// <summary>
		/// Constructor. Initializes delegate command with Execute delegate and CanExecute delegate
		/// </summary>
		/// <param name="executeMethod">Delegate to execute when Execute is called on the command.  This can be null to just hook up a CanExecute delegate.</param>
		/// <param name="canExecuteMethod">Delegate to execute when CanExecute is called on the command.  This can be null.</param>
		public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod)
			: this(executeMethod, canExecuteMethod, new string[]{})
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DelegateCommand&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="executeMethod">Delegate to execute when Execute is called on the command.  This can be null to just hook up a CanExecute delegate.</param>
		/// <param name="canExecuteMethod">Delegate to execute when CanExecute is called on the command.  This can be null.</param>
		/// <param name="updateTriggerProperties">The set of property names that should be used to trigger calls to CanExecute.</param>
		public DelegateCommand(Action<T> executeMethod, Func<T, bool> canExecuteMethod, IEnumerable<string> updateTriggerProperties)
		{
			if (executeMethod == null && canExecuteMethod == null)
				throw new ArgumentNullException("executeMethod", "Both the executeMethod and the canExecuteMethod delegates cannot be null.");
			Contract.EndContractBlock();

			_executeMethod = executeMethod;
			_canExecuteMethod = canExecuteMethod;
			if (System.Windows.Application.Current != null)
			{
				_dispatcher = System.Windows.Application.Current.Dispatcher;
			}

			_triggerProperties = updateTriggerProperties != null ? new List<string>(updateTriggerProperties) : new List<string>();

			if (_canExecuteMethod != null)
			{
				INotifyPropertyChanged target = _canExecuteMethod.Target as INotifyPropertyChanged;
				if (target != null)
				{
					target.PropertyChanged += TriggerPropertyChanged;
				}
			}
		}

		public DelegateCommand( Action<T> executeMethod, Func<T, bool> canExecuteMethod, params Expression<Func<object>>[] updateTriggerProperties )
		{
			if ( executeMethod == null && canExecuteMethod == null )
				throw new ArgumentNullException( "executeMethod", "Both the executeMethod and the canExecuteMethod delegates cannot be null." );
			Contract.EndContractBlock();

			_executeMethod = executeMethod;
			_canExecuteMethod = canExecuteMethod;
			if ( System.Windows.Application.Current != null )
			{
				_dispatcher = System.Windows.Application.Current.Dispatcher;
			}

			_triggerProperties = updateTriggerProperties != null ? new List<string>( updateTriggerProperties.Select( PropertyNameResolver.GetPropertyName ) ) : new List<string>();

			if ( _canExecuteMethod != null )
			{
				INotifyPropertyChanged target = _canExecuteMethod.Target as INotifyPropertyChanged;
				if ( target != null )
				{
					target.PropertyChanged += TriggerPropertyChanged;
				}
			}
		}

		///<summary>
		///Defines the method that determines whether the command can execute in its current state.
		///</summary>
		///<param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
		///<returns>
		///true if this command can be executed; otherwise, false.
		///</returns>
		public bool CanExecute(T parameter)
		{
			if (_canExecuteMethod == null) return true;
			return _canExecuteMethod(parameter);
		}

		///<summary>
		///Defines the method to be called when the command is invoked.
		///</summary>
		///<param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
		public void Execute(T parameter)
		{
			if (_executeMethod == null) return;
			_executeMethod(parameter);
		}

		private static T ConvertParameter(object parameter)
		{
			if (parameter != null)
			{
				if (parameter is T)
					return (T)parameter;

				TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
				if (converter.IsValid(parameter))
					return (T)converter.ConvertFrom(parameter);
			}

			return default(T);
		}

		///<summary>
		///Defines the method that determines whether the command can execute in its current state.
		///</summary>
		///<param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
		///<returns>
		///true if this command can be executed; otherwise, false.
		///</returns>
		bool ICommand.CanExecute(object parameter)
		{
			return CanExecute(ConvertParameter(parameter));
		}

		public event EventHandler CanExecuteChanged
		{
			add
			{
				if (this._canExecuteMethod != null)
				{
					CommandManager.RequerySuggested += value;
				}
			}
			remove
			{
				if (this._canExecuteMethod != null)
				{
					CommandManager.RequerySuggested -= value;
				}
			}
		}



		///<summary>
		///Defines the method to be called when the command is invoked.
		///</summary>
		///<param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
		void ICommand.Execute(object parameter)
		{
			Execute(ConvertParameter(parameter));
		}

		/// <summary>
		/// Raises <see cref="CanExecuteChanged"/> so every command invoker can requery to check if the command can execute.
		/// <remarks>This will trigger the execution of <see cref="CanExecute"/> once for each invoker.</remarks>
		/// </summary>
		public void RaiseCanExecuteChanged()
		{
			CommandManager.InvalidateRequerySuggested();
		}

		private void TriggerPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (_triggerProperties.Contains(e.PropertyName))
				RaiseCanExecuteChanged();
		}
	}



	/// <summary>
	/// An <see cref="ICommand"/> whose delegates can be attached for <see cref="Execute"/> and <see cref="CanExecute"/>.
	/// </summary>
	public class DelegateCommand : ICommand
	{
		private readonly Action _executeMethod;
		private readonly Func<bool> _canExecuteMethod;
		private readonly Dispatcher _dispatcher;
		private readonly List<string> _triggerProperties;

		/// <summary>
		/// Constructor. Initializes delegate command with Execute delegate.
		/// </summary>
		/// <param name="executeMethod">Delegate to execute when Execute is called on the command.  This can be null to just hook up a CanExecute delegate.</param>
		/// <remarks><seealso cref="CanExecute"/> will always return true.</remarks>
		public DelegateCommand( Action executeMethod )
			: this( executeMethod, null )
		{
		}


		/// <summary>
		/// Constructor. Initializes delegate command with Execute delegate and CanExecute delegate
		/// </summary>
		/// <param name="executeMethod">Delegate to execute when Execute is called on the command.  This can be null to just hook up a CanExecute delegate.</param>
		/// <param name="canExecuteMethod">Delegate to execute when CanExecute is called on the command.  This can be null.</param>
		public DelegateCommand( Action executeMethod, Func<bool> canExecuteMethod )
			: this( executeMethod, canExecuteMethod, new string[]{} )
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DelegateCommand&lt;T&gt;"/> class.
		/// </summary>
		/// <param name="executeMethod">Delegate to execute when Execute is called on the command.  This can be null to just hook up a CanExecute delegate.</param>
		/// <param name="canExecuteMethod">Delegate to execute when CanExecute is called on the command.  This can be null.</param>
		/// <param name="updateTriggerProperties">The set of property names that should be used to trigger calls to CanExecute.</param>
		public DelegateCommand( Action executeMethod, Func<bool> canExecuteMethod, IEnumerable<string> updateTriggerProperties )
		{
			if ( executeMethod == null && canExecuteMethod == null )
				throw new ArgumentNullException( "executeMethod", "Both the executeMethod and the canExecuteMethod delegates cannot be null." );
			Contract.EndContractBlock();

			_executeMethod = executeMethod;
			_canExecuteMethod = canExecuteMethod;
			if ( System.Windows.Application.Current != null )
			{
				_dispatcher = System.Windows.Application.Current.Dispatcher;
			}

			_triggerProperties = updateTriggerProperties != null ? new List<string>( updateTriggerProperties ) : new List<string>();

			if ( _canExecuteMethod != null )
			{
				INotifyPropertyChanged target = _canExecuteMethod.Target as INotifyPropertyChanged;
				if ( target != null )
				{
					target.PropertyChanged += TriggerPropertyChanged;
				}
			}
		}

		public DelegateCommand( Action executeMethod, Func<bool> canExecuteMethod, params Expression<Func<object>>[] updateTriggerProperties )
		{
			if ( executeMethod == null && canExecuteMethod == null )
				throw new ArgumentNullException( "executeMethod", "Both the executeMethod and the canExecuteMethod delegates cannot be null." );
			Contract.EndContractBlock();

			_executeMethod = executeMethod;
			_canExecuteMethod = canExecuteMethod;
			if ( System.Windows.Application.Current != null )
			{
				_dispatcher = System.Windows.Application.Current.Dispatcher;
			}
			
			_triggerProperties = updateTriggerProperties != null ? new List<string>( updateTriggerProperties.Select( PropertyNameResolver.GetPropertyName) ) : new List<string>();

			if ( _canExecuteMethod != null )
			{
				INotifyPropertyChanged target = _canExecuteMethod.Target as INotifyPropertyChanged;
				if ( target != null )
				{
					target.PropertyChanged += TriggerPropertyChanged;
				}
			}
		}

		///<summary>
		///Defines the method that determines whether the command can execute in its current state.
		///</summary>
		///<returns>
		///true if this command can be executed; otherwise, false.
		///</returns>
		public bool CanExecute()
		{
			if ( _canExecuteMethod == null ) return true;
			return _canExecuteMethod();
		}

		///<summary>
		///Defines the method to be called when the command is invoked.
		///</summary>
		public void Execute()
		{
			if ( _executeMethod == null ) return;
			_executeMethod();
		}

		///<summary>
		///Defines the method that determines whether the command can execute in its current state.
		///</summary>
		///<param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
		///<returns>
		///true if this command can be executed; otherwise, false.
		///</returns>
		bool ICommand.CanExecute( object parameter )
		{
			return CanExecute();
		}

		///<summary>
		///Occurs when changes occur that affect whether or not the command should execute.
		///</summary>
		public event EventHandler CanExecuteChanged = delegate { };

		///<summary>
		///Defines the method to be called when the command is invoked.
		///</summary>
		///<param name="parameter">Data used by the command.  If the command does not require data to be passed, this object can be set to null.</param>
		void ICommand.Execute( object parameter )
		{
			Execute();
		}

		/// <summary>
		/// Raises <seealso cref="CanExecuteChanged"/> on the UI thread.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnCanExecuteChanged( object sender, EventArgs e )
		{
			CanExecuteChanged( sender, e );
		}

		/// <summary>
		/// Raises <see cref="CanExecuteChanged"/> so every command invoker can requery to check if the command can execute.
		/// <remarks>This will trigger the execution of <see cref="CanExecute"/> once for each invoker.</remarks>
		/// </summary>
		public void RaiseCanExecuteChanged()
		{
			OnCanExecuteChanged( this, EventArgs.Empty );
		}

		private void TriggerPropertyChanged( object sender, PropertyChangedEventArgs e )
		{
			if ( _triggerProperties.Contains( e.PropertyName ) )
				RaiseCanExecuteChanged();
		}
	}

}