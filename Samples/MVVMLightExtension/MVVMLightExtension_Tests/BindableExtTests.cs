using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PropertyDependencyFramework;

namespace MVVMLightExtension_Tests
{
	[TestClass]
	public class BindableExtTests
	{
		private const string DefaultNewValue = "ABC";

		#region DynamicDependency_DependencyIsSwappedOut_PropertyChangeNotificationsStillFire
		class DynamicDependency_DependencyIsSwappedOut_PropertyChangeNotificationsStillFire_TestClass : BindableForUnitTests
		{
			public class ExternalDependency : BindableForUnitTests
			{
				public ExternalDependency(bool useSmartPropertyChangeNotificationByDefault)
					: base(useSmartPropertyChangeNotificationByDefault)
				{
				}

				private string _source;
				public string Source
				{
					[DebuggerStepThrough]
					get { return _source; }
					set
					{
						if (value == _source)
							return;

						_source = value;
						NotifyPropertyChanged(() => Source);
					}
				}
			}

			private ExternalDependency _externalDependency;
			public ExternalDependency Dependency
			{
				[DebuggerStepThrough]
				get { return _externalDependency; }
				set
				{
					if (value == _externalDependency)
						return;

					_externalDependency = value;
					NotifyPropertyChanged(() => Dependency);
				}
			}

			public string Target
			{
				get
				{
					Property(() => Target)
						.Depends(p => p.On(() => Dependency, k => k.Source));

					return Dependency == null ? string.Empty : Dependency.Source;
				}
			}

			public string Target2
			{
				get
				{
					Property(() => Target2)
						.Depends(p => p.On(() => Dependency, k => k.Source));

					return Dependency == null ? string.Empty : Dependency.Source.ReverseString();
				}
			}
		}

		[TestMethod]
		public void DynamicDependency_DependencyIsSwappedOut_PropertyChangeNotificationsStillFire()
		{
			DynamicDependency_DependencyIsSwappedOut_PropertyChangeNotificationsStillFire_Impl(useSmartNotification: true);
			DynamicDependency_DependencyIsSwappedOut_PropertyChangeNotificationsStillFire_Impl(useSmartNotification: false);
		}

		private static void DynamicDependency_DependencyIsSwappedOut_PropertyChangeNotificationsStillFire_Impl(bool useSmartNotification)
		{
			var tu = new DynamicDependency_DependencyIsSwappedOut_PropertyChangeNotificationsStillFire_TestClass();

			var recorderTarget1 = CreatePropertyChangeRecorder(tu, k => k.Target);
			var recorderTarget2 = CreatePropertyChangeRecorder(tu, k => k.Target2);
			tu.Dependency = new DynamicDependency_DependencyIsSwappedOut_PropertyChangeNotificationsStillFire_TestClass.ExternalDependency(useSmartNotification)
			{
				Source = DefaultNewValue
			};
			Assert.IsTrue(recorderTarget1.HasChanged);
			Assert.AreEqual(1, recorderTarget1.NumberOfChanges);
			Assert.AreEqual(DefaultNewValue, recorderTarget1.NewValues[0]);
			Assert.IsTrue(recorderTarget2.HasChanged);
			Assert.AreEqual(1, recorderTarget2.NumberOfChanges);
			Assert.AreEqual(DefaultNewValue.ReverseString(), recorderTarget2.NewValues[0]);

			//Swap out dependency
			tu.Dependency = new DynamicDependency_DependencyIsSwappedOut_PropertyChangeNotificationsStillFire_TestClass.ExternalDependency(useSmartNotification)
			{
				Source = DefaultNewValue + DefaultNewValue
			};
			Assert.AreEqual(2, recorderTarget1.NumberOfChanges);
			Assert.AreEqual(DefaultNewValue + DefaultNewValue, recorderTarget1.NewValues[1]);
			Assert.AreEqual(2, recorderTarget2.NumberOfChanges);
			Assert.AreEqual((DefaultNewValue + DefaultNewValue).ReverseString(), recorderTarget2.NewValues[1]);
		}
		#endregion

		#region DynamicDependency_DependencyIsSwappedOut_NoReferencesToTheOldObjectAreKept

		class DynamicDependency_DependencyIsSwappedOut_NoReferencesToTheOldObjectAreKept_TestClass : BindableForUnitTests
		{
			public class ExternalDependency : BindableForUnitTests
			{
				public ExternalDependency(bool useSmartPropertyChangeNotificationByDefault)
					: base(useSmartPropertyChangeNotificationByDefault)
				{
				}

				private string _source;
				public string Source
				{
					[DebuggerStepThrough]
					get { return _source; }
					set
					{
						if (value == _source)
							return;

						_source = value;
						NotifyPropertyChanged(() => Source);
					}
				}
			}

			private ExternalDependency _externalDependency;
			public ExternalDependency Dependency
			{
				[DebuggerStepThrough]
				get { return _externalDependency; }
				set
				{
					if (value == _externalDependency)
						return;

					_externalDependency = value;
					NotifyPropertyChanged(() => Dependency);
				}
			}

			public string Target
			{
				get
				{
					Property(() => Target)
						.Depends(p => p.On(() => Dependency, k => k.Source));

					return Dependency == null ? string.Empty : Dependency.Source;
				}
			}
		}

		[TestMethod]
		public void DynamicDependency_DependencyIsSwappedOut_NoReferencesToTheOldObjectAreKept()
		{
			DynamicDependency_DependencyIsSwappedOut_NoReferencesToTheOldObjectAreKept_Impl(true);
			DynamicDependency_DependencyIsSwappedOut_NoReferencesToTheOldObjectAreKept_Impl(false);
		}

		private static void DynamicDependency_DependencyIsSwappedOut_NoReferencesToTheOldObjectAreKept_Impl(bool useSmartNotification)
		{
			var tu = new DynamicDependency_DependencyIsSwappedOut_NoReferencesToTheOldObjectAreKept_TestClass();

			var recorder = CreatePropertyChangeRecorder(tu, k => k.Target);
			var originalDependency = new DynamicDependency_DependencyIsSwappedOut_NoReferencesToTheOldObjectAreKept_TestClass.ExternalDependency(useSmartNotification)
			{
				Source = DefaultNewValue
			};
			tu.Dependency = originalDependency;
			Assert.IsTrue(recorder.HasChanged);
			Assert.AreEqual(1, recorder.NumberOfChanges);
			Assert.IsTrue(originalDependency.AreEventHandlersRegisteredOnPropertyChanged);
			Assert.IsTrue(originalDependency.AreEventHandlersRegisteredOnPropertyChangedInTransaction);


			//Swap out dependency
			tu.Dependency = new DynamicDependency_DependencyIsSwappedOut_NoReferencesToTheOldObjectAreKept_TestClass.ExternalDependency(useSmartNotification)
			{
				Source = DefaultNewValue + DefaultNewValue
			};
			Assert.AreEqual(2, recorder.NumberOfChanges);
			Assert.IsFalse(originalDependency.AreEventHandlersRegisteredOnPropertyChanged);
			Assert.IsFalse(originalDependency.AreEventHandlersRegisteredOnPropertyChangedInTransaction);
		}

		#endregion

		#region DynamicCollectionDependency_DependencyIsSwappedOut_PropertyChangeNotificationsStillFire
		class DynamicCollectionDependency_DependencyIsSwappedOut_PropertyChangeNotificationsStillFire_TestClass : BindableForUnitTests
		{
			internal class ChildDependency : BindableForUnitTests
			{
				public ChildDependency(bool useSmartPropertyChangeNotificationByDefault)
					: base(useSmartPropertyChangeNotificationByDefault)
				{
				}

				private decimal _source;
				public decimal Source
				{
					[DebuggerStepThrough]
					get { return _source; }
					set
					{
						if (value == _source)
							return;

						_source = value;
						NotifyPropertyChanged(() => Source);
					}
				}
			}

			private DependencyFrameworkObservableCollection<ChildDependency> _children;
			public DependencyFrameworkObservableCollection<ChildDependency> Children
			{
				[DebuggerStepThrough]
				get { return _children; }
				set
				{
					if (value == _children)
						return;

					_children = value;
					NotifyPropertyChanged(() => Children);
				}
			}

			public decimal Target
			{
				get
				{
					Property(() => Target)
						.Depends(p => p.OnCollectionChildProperty(() => Children, k => k.Source));

					if (Children == null)
						return 0;

					return Children.Select(k => k.Source).SumOrZero();
				}
			}
		}

		[TestMethod]
		public void DynamicCollectionDependency_DependencyIsSwappedOut_PropertyChangeNotificationsStillFire()
		{
			DynamicCollectionDependency_DependencyIsSwappedOut_PropertyChangeNotificationsStillFire_Impl(useSmartNotification: true);
			DynamicCollectionDependency_DependencyIsSwappedOut_PropertyChangeNotificationsStillFire_Impl(useSmartNotification: false);
		}

		private static void DynamicCollectionDependency_DependencyIsSwappedOut_PropertyChangeNotificationsStillFire_Impl(bool useSmartNotification)
		{
			var collection1 =
				new DependencyFrameworkObservableCollection
					<DynamicCollectionDependency_DependencyIsSwappedOut_PropertyChangeNotificationsStillFire_TestClass.ChildDependency>();

			collection1.Add(new DynamicCollectionDependency_DependencyIsSwappedOut_PropertyChangeNotificationsStillFire_TestClass.ChildDependency(useSmartNotification)
			{
				Source = 1
			});
			collection1.Add(new DynamicCollectionDependency_DependencyIsSwappedOut_PropertyChangeNotificationsStillFire_TestClass.ChildDependency(useSmartNotification)
			{
				Source = 10
			});

			var collection2 =
				new DependencyFrameworkObservableCollection
					<DynamicCollectionDependency_DependencyIsSwappedOut_PropertyChangeNotificationsStillFire_TestClass.ChildDependency>();

			collection2.Add(new DynamicCollectionDependency_DependencyIsSwappedOut_PropertyChangeNotificationsStillFire_TestClass.ChildDependency(useSmartNotification)
			{
				Source = 10
			});
			collection2.Add(new DynamicCollectionDependency_DependencyIsSwappedOut_PropertyChangeNotificationsStillFire_TestClass.ChildDependency(useSmartNotification)
			{
				Source = 100
			});

			var tu = new DynamicCollectionDependency_DependencyIsSwappedOut_PropertyChangeNotificationsStillFire_TestClass();
			var recorder = CreatePropertyChangeRecorder(tu, k => k.Target);

			tu.Children = collection1;

			Assert.IsTrue(recorder.HasChanged);
			Assert.AreEqual(1, recorder.NumberOfChanges);
			Assert.AreEqual(11m, recorder.NewValues[0]);

			collection1.Add(new DynamicCollectionDependency_DependencyIsSwappedOut_PropertyChangeNotificationsStillFire_TestClass.ChildDependency(useSmartNotification)
			{
				Source = 10
			});
			Assert.AreEqual(2, recorder.NumberOfChanges);
			Assert.AreEqual(21m, recorder.NewValues[1]);

			collection1[0].Source = 5;
			collection1[2].Source = 5;
			Assert.AreEqual(4, recorder.NumberOfChanges);
			Assert.AreEqual(20m, recorder.NewValues[3]);

			collection1.Remove(collection1[2]);
			Assert.AreEqual(5, recorder.NumberOfChanges);
			Assert.AreEqual(15m, recorder.NewValues[4]);

			tu.Children = collection2;
			Assert.AreEqual(6, recorder.NumberOfChanges);
			Assert.AreEqual(110m, recorder.NewValues[5]);

			collection2.Add(new DynamicCollectionDependency_DependencyIsSwappedOut_PropertyChangeNotificationsStillFire_TestClass.ChildDependency(useSmartNotification)
			{
				Source = 100
			});
			Assert.AreEqual(7, recorder.NumberOfChanges);
			Assert.AreEqual(210m, recorder.NewValues[6]);

			collection2[0].Source = 5;
			collection2[2].Source = 200;
			Assert.AreEqual(9, recorder.NumberOfChanges);
			Assert.AreEqual(305m, recorder.NewValues[8]);

			collection2.Remove(collection2[0]);
			Assert.AreEqual(10, recorder.NumberOfChanges);
			Assert.AreEqual(300m, recorder.NewValues[9]);
		}
		#endregion

		#region SourcePropertyChangeIsAddOrRemoveOfACollection_ChildIsAddedAndRemoved_NotificationScopeIsOpenedProperlyAndSmartChangeNotificationIsInEffect
		class SourcePropertyChangeIsAddOrRemoveOfACollection_ChildIsAddedAndRemoved_NotificationScopeIsOpenedProperlyAndSmartChangeNotificationIsInEffect_TestClass : BindableForUnitTests
		{
			public SourcePropertyChangeIsAddOrRemoveOfACollection_ChildIsAddedAndRemoved_NotificationScopeIsOpenedProperlyAndSmartChangeNotificationIsInEffect_TestClass()
			{
				Children = new DependencyFrameworkObservableCollection<Dependency>();
			}

			public DependencyFrameworkObservableCollection<Dependency> Children { get; private set; }

			public decimal Target
			{
				get
				{
					Property(() => Target)
						.Depends(p => p.On(() => MidLayer1)
										.AndOn(() => MidLayer2));

					return MidLayer1 + MidLayer2;
				}
			}

			public decimal MidLayer1
			{
				get
				{
					Property(() => MidLayer1)
						.Depends(p => p.On(() => Sum));

					return Sum;
				}
			}

			public decimal MidLayer2
			{
				get
				{
					Property(() => MidLayer2)
						.Depends(p => p.On(() => Sum));

					return Sum;
				}
			}

			public decimal Sum
			{
				get
				{
					Property(() => Sum)
						.Depends(p => p.OnCollectionChildProperty(Children, k => k.Source));

					return Children.Select(k => k.Source).SumOrZero();
				}
			}

			public class Dependency : BindableForUnitTests
			{
				private decimal _source;
				public decimal Source
				{
					[DebuggerStepThrough]
					get { return _source; }
					set
					{
						if (value == _source)
							return;

						_source = value;
						NotifyPropertyChanged(() => Source);
					}
				}
			}
		}

		[TestMethod]
		public void SourcePropertyChangeIsAddOrRemoveOfACollection_ChildIsAddedAndRemoved_NotificationScopeIsOpenedProperlyAndSmartChangeNotificationIsInEffect()
		{
			var tu =
				new SourcePropertyChangeIsAddOrRemoveOfACollection_ChildIsAddedAndRemoved_NotificationScopeIsOpenedProperlyAndSmartChangeNotificationIsInEffect_TestClass();

			var recorder = CreatePropertyChangeRecorder(tu, k => k.Target);

			tu.Children.Add(new SourcePropertyChangeIsAddOrRemoveOfACollection_ChildIsAddedAndRemoved_NotificationScopeIsOpenedProperlyAndSmartChangeNotificationIsInEffect_TestClass
								 .Dependency()
			{
				Source = 5
			});

			Assert.IsTrue(recorder.HasChanged);
			Assert.AreEqual(1, recorder.NumberOfChanges);
			Assert.AreEqual(10m, recorder.NewValues.Last());
		}
		#endregion

		#region CallbackChangesProperty_CallbackIsInitiatedWithASmartPropertyNotification_SecondPropertyChangedIsExecutedWithSmartPropertyNotification
		class CallbackChangesProperty_CallbackIsInitiatedWithASmartPropertyNotification_SecondPropertyChangedIsExecutedWithSmartPropertyNotification_TestClass : BindableForUnitTests
		{
			public CallbackChangesProperty_CallbackIsInitiatedWithASmartPropertyNotification_SecondPropertyChangedIsExecutedWithSmartPropertyNotification_TestClass(bool useSmartPropertyChangeNotificationByDefault)
				: base(useSmartPropertyChangeNotificationByDefault)
			{
				RegisterCallbackDependency(this, k => k.DelegatedSource, OnDelegatedSourceChanged);
				RegisterCallbackDependency(this, k => k.DelegatedSourceInRoundTwo, OnDelegatedSourceInRoundTwoChanged);
			}

			private void OnDelegatedSourceChanged()
			{
				SourceInRoundTwo = OriginalSource.ReverseString();
			}

			private void OnDelegatedSourceInRoundTwoChanged()
			{
				SourceInFinalRound = SourceInRoundTwo;
			}

			public string DelegatedSource
			{
				get
				{
					Property(() => DelegatedSource)
						.Depends(p => p.On(() => OriginalSource));

					return OriginalSource;
				}
			}

			private string _originalSource;
			public string OriginalSource
			{
				[DebuggerStepThrough]
				get { return _originalSource; }
				set
				{
					if (value == _originalSource)
						return;

					_originalSource = value;
					NotifyPropertyChanged(() => OriginalSource);
				}
			}

			public string DelegatedSourceInRoundTwo
			{
				get
				{
					Property(() => DelegatedSourceInRoundTwo)
						.Depends(p => p.On(() => SourceInRoundTwo));

					return SourceInRoundTwo;
				}
			}

			private string _sourceInRoundTwo;
			public string SourceInRoundTwo
			{
				[DebuggerStepThrough]
				get { return _sourceInRoundTwo; }
				set
				{
					if (value == _sourceInRoundTwo)
						return;

					_sourceInRoundTwo = value;
					NotifyPropertyChanged(() => SourceInRoundTwo);
				}
			}

			private string _sourceInFinalRound;
			public string SourceInFinalRound
			{
				[DebuggerStepThrough]
				get { return _sourceInFinalRound; }
				set
				{
					if (value == _sourceInFinalRound)
						return;

					_sourceInFinalRound = value;
					NotifyPropertyChanged(() => SourceInFinalRound);
				}
			}

			public string MidLevel1
			{
				get
				{
					Property(() => MidLevel1)
						.Depends(p => p.On(() => SourceInFinalRound));

					return SourceInFinalRound;
				}
			}

			public string MidLevel2
			{
				get
				{
					Property(() => MidLevel2)
						.Depends(p => p.On(() => SourceInFinalRound));

					return SourceInFinalRound;
				}
			}

			public string TopLevel
			{
				get
				{
					Property(() => TopLevel)
						.Depends(p => p.On(() => MidLevel1)
										.AndOn(() => MidLevel2));

					return MidLevel1 + MidLevel2;
				}
			}
		}

		[TestMethod]
		public void CallbackChangesProperty_CallbackIsInitiatedWithASmartPropertyNotification_SecondPropertyChangedIsExecutedWithSmartPropertyNotification()
		{
			var tu =
				new CallbackChangesProperty_CallbackIsInitiatedWithASmartPropertyNotification_SecondPropertyChangedIsExecutedWithSmartPropertyNotification_TestClass(true);

			var recorderDelegatedSource = CreatePropertyChangeRecorder(tu, k => k.DelegatedSource);
			var recorderDelegatedSourceInRoundTwo = CreatePropertyChangeRecorder(tu, k => k.DelegatedSourceInRoundTwo);
			var recorderTopLevel = CreatePropertyChangeRecorder(tu, k => k.TopLevel);

			tu.OriginalSource = DefaultNewValue;

			Assert.IsTrue(recorderDelegatedSource.HasChanged);
			Assert.IsTrue(recorderDelegatedSourceInRoundTwo.HasChanged);
			Assert.IsTrue(recorderTopLevel.HasChanged);
			Assert.AreEqual(DefaultNewValue.ReverseString() + DefaultNewValue.ReverseString(), recorderTopLevel.NewValues.Last());
			Assert.AreEqual(1, recorderTopLevel.NumberOfChanges);
		}
		#endregion

		#region CallbackChangesCollection_CallbackIsInitiatedWithASmartPropertyNotification_CollectionIsChangedWithSmartPropertyNotification

		class CallbackChangesCollection_CallbackIsInitiatedWithASmartPropertyNotification_CollectionIsChangedWithSmartPropertyNotification_TestClass : BindableForUnitTests
		{
			public CallbackChangesCollection_CallbackIsInitiatedWithASmartPropertyNotification_CollectionIsChangedWithSmartPropertyNotification_TestClass(bool useSmartPropertyChangeNotificationByDefault)
				: base(useSmartPropertyChangeNotificationByDefault)
			{
				Children = new DependencyFrameworkObservableCollection<ChildDependency>();

				RegisterCallbackDependency(this, k => k.DelegatedSource, OnDelegatedSourceChanged);
				RegisterCallbackDependency(this, k => k.DelegatedSourceInRoundTwo, OnDelegatedSourceInRoundTwoChanged);
			}

			private void OnDelegatedSourceChanged()
			{
				SourceInRoundTwo = OriginalSource.ReverseString();
			}

			private void OnDelegatedSourceInRoundTwoChanged()
			{
				Children.Add(new ChildDependency() { Source = SourceInRoundTwo });
			}

			internal class ChildDependency : BindableForUnitTests
			{
				private string _source;
				public string Source
				{
					[DebuggerStepThrough]
					get { return _source; }
					set
					{
						if (value == _source)
							return;

						_source = value;
						NotifyPropertyChanged(() => Source);
					}
				}
			}

			public string DelegatedSource
			{
				get
				{
					Property(() => DelegatedSource)
						.Depends(p => p.On(() => OriginalSource));

					return OriginalSource;
				}
			}

			private string _originalSource;
			public string OriginalSource
			{
				[DebuggerStepThrough]
				get { return _originalSource; }
				set
				{
					if (value == _originalSource)
						return;

					_originalSource = value;
					NotifyPropertyChanged(() => OriginalSource);
				}
			}

			public string DelegatedSourceInRoundTwo
			{
				get
				{
					Property(() => DelegatedSourceInRoundTwo)
						.Depends(p => p.On(() => SourceInRoundTwo));

					return SourceInRoundTwo;
				}
			}

			private string _sourceInRoundTwo;
			public string SourceInRoundTwo
			{
				[DebuggerStepThrough]
				get { return _sourceInRoundTwo; }
				set
				{
					if (value == _sourceInRoundTwo)
						return;

					_sourceInRoundTwo = value;
					NotifyPropertyChanged(() => SourceInRoundTwo);
				}
			}

			public DependencyFrameworkObservableCollection<ChildDependency> Children { get; private set; }

			public string SourceInFinalRound
			{
				get
				{
					Property(() => SourceInFinalRound)
						.Depends(p => p.OnCollectionChildProperty(Children, k => k.Source));

					return string.Join("", Children.Select(k => k.Source).ToArray());
				}
			}

			public string MidLevel1
			{
				get
				{
					Property(() => MidLevel1)
						.Depends(p => p.On(() => SourceInFinalRound));

					return SourceInFinalRound;
				}
			}

			public string MidLevel2
			{
				get
				{
					Property(() => MidLevel2)
						.Depends(p => p.On(() => SourceInFinalRound));

					return SourceInFinalRound;
				}
			}

			public string TopLevel
			{
				get
				{
					Property(() => TopLevel)
						.Depends(p => p.On(() => MidLevel1)
										.AndOn(() => MidLevel2));

					return MidLevel1 + MidLevel2;
				}
			}
		}

		[TestMethod]
		public void CallbackChangesCollection_CallbackIsInitiatedWithASmartPropertyNotification_CollectionIsChangedWithSmartPropertyNotification()
		{
			var tu =
				new CallbackChangesCollection_CallbackIsInitiatedWithASmartPropertyNotification_CollectionIsChangedWithSmartPropertyNotification_TestClass(true);

			var recorderDelegatedSource = CreatePropertyChangeRecorder(tu, k => k.DelegatedSource);
			var recorderDelegatedSourceInRoundTwo = CreatePropertyChangeRecorder(tu, k => k.DelegatedSourceInRoundTwo);
			var recorderTopLevel = CreatePropertyChangeRecorder(tu, k => k.TopLevel);

			tu.OriginalSource = DefaultNewValue;

			Assert.IsTrue(recorderDelegatedSource.HasChanged);
			Assert.IsTrue(recorderDelegatedSourceInRoundTwo.HasChanged);
			Assert.IsTrue(recorderTopLevel.HasChanged);
			Assert.AreEqual(DefaultNewValue.ReverseString() + DefaultNewValue.ReverseString(), recorderTopLevel.NewValues.Last());
			Assert.AreEqual(1, recorderTopLevel.NumberOfChanges);
		}

		#endregion

		#region CallbackChangesAProperty_CallbackIsFiredOfObservableCollectionChange_PropertyChangeIsDeferred
		public class CallbackChangesAProperty_CallbackIsFiredOfObservableCollectionChange_PropertyChangeIsDeferred_TestClass : BindableForUnitTests
		{
			public CallbackChangesAProperty_CallbackIsFiredOfObservableCollectionChange_PropertyChangeIsDeferred_TestClass(bool useSmartPropertyChangeNotificationByDefault)
				: base(useSmartPropertyChangeNotificationByDefault)
			{
				Children = new DependencyFrameworkObservableCollection<ChildClass>();

				RegisterCallbackDependency(Children, OnChildrenChanged);
			}

			private void OnChildrenChanged()
			{
				Target = "Changed!";
			}

			private string _target;
			public string Target
			{
				[DebuggerStepThrough]
				get { return _target; }
				set
				{
					if (value == _target)
						return;

					_target = value;
					NotifyPropertyChanged(() => Target);
				}
			}

			public class ChildClass : Bindable
			{

			}


			public DependencyFrameworkObservableCollection<ChildClass> Children { get; private set; }
		}

		[TestMethod]
		public void CallbackChangesAProperty_CallbackIsFiredOfObservableCollectionChange_PropertyChangeIsDeferred()
		{
			var tu = new CallbackChangesAProperty_CallbackIsFiredOfObservableCollectionChange_PropertyChangeIsDeferred_TestClass(true);
			var recorder = CreatePropertyChangeRecorder(tu, k => k.Target);

			tu.Children.Add(new CallbackChangesAProperty_CallbackIsFiredOfObservableCollectionChange_PropertyChangeIsDeferred_TestClass.ChildClass());

			Assert.IsTrue(recorder.HasChanged);
			Assert.AreEqual(1, recorder.NumberOfChanges);
			Assert.AreEqual("Changed!", recorder.NewValues[0]);
		}
		#endregion

		static PropertyChangeRecorder CreatePropertyChangeRecorder<TBindable, TProperty>(TBindable bindable, Expression<Func<TBindable, TProperty>> propertyExpression)
			where TBindable : INotifyPropertyChanged
		{
			var recorder = new PropertyChangeRecorder();

			var compiledExpression = propertyExpression.Compile();
			var initialValue = compiledExpression(bindable);

			var propertyName = PropertyNameResolver.GetPropertyName(propertyExpression);
			bindable.PropertyChanged += (s, e) =>
			{
				if (e.PropertyName == propertyName)
				{
					recorder.NumberOfChanges++;
					recorder.NewValues.Add(compiledExpression(bindable));
				}
			};

			return recorder;
		}
	}
}
