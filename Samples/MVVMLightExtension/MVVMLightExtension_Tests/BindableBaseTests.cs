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
	public class BindableBaseTests
	{
		private const string DefaultNewValue = "ABC";

		#region SimpleDirectDependency_SourcePropertyChanges_TargetPropertyShouldChange
		class SimpleDirectDependency_SourcePropertyChanges_TargetPropertyShouldChange_TestClass : BindableForUnitTests
		{
			public SimpleDirectDependency_SourcePropertyChanges_TargetPropertyShouldChange_TestClass(bool useSmartPropertyChangeNotificationByDefault)
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

			public string Target
			{
				get
				{
					Property(() => Target)
						.Depends(p => p.On(() => Source));

					return Source;
				}
			}
		}

		[TestMethod]
		public void SimpleDirectDependency_SourcePropertyChanges_TargetPropertyShouldChange()
		{
			SimpleDirectDependency_SourcePropertyChanges_TargetPropertyShouldChange_Impl(useSmartNotification: true);
			SimpleDirectDependency_SourcePropertyChanges_TargetPropertyShouldChange_Impl(useSmartNotification: false);
		}

		private static void SimpleDirectDependency_SourcePropertyChanges_TargetPropertyShouldChange_Impl(bool useSmartNotification)
		{
			var tu = new SimpleDirectDependency_SourcePropertyChanges_TargetPropertyShouldChange_TestClass(useSmartNotification);
			tu.Source = "Initial";
			string expectedValue = "NewValue";

			var recorder = CreatePropertyChangeRecorder(tu, k => k.Target);
			tu.Source = expectedValue;

			Assert.IsTrue(recorder.HasChanged);
			Assert.AreEqual(recorder.NewValues[0], expectedValue);
		}
		#endregion

		#region SimpleIndirectDependency_SourcePropertyChanges_TargetPropertyShouldChange
		class SimpleIndirectDependency_SourcePropertyChanges_TargetPropertyShouldChange_TestClass : BindableForUnitTests
		{
			public SimpleIndirectDependency_SourcePropertyChanges_TargetPropertyShouldChange_TestClass(bool useSmartPropertyChangeNotificationByDefault)
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

			public string IntermediateSource
			{
				get
				{
					Property(() => IntermediateSource)
						.Depends(p => p.On(() => Source));

					return Source;
				}
			}

			public string Target
			{
				get
				{
					Property(() => Target)
						.Depends(p => p.On(() => IntermediateSource));

					return IntermediateSource;
				}
			}
		}

		[TestMethod]
		public void SimpleIndirectDependency_SourcePropertyChanges_TargetPropertyShouldChange()
		{
			SimpleIndirectDependency_SourcePropertyChanges_TargetPropertyShouldChange_Impl(useSmartNotification: true);
			SimpleIndirectDependency_SourcePropertyChanges_TargetPropertyShouldChange_Impl(useSmartNotification: false);
		}

		private static void SimpleIndirectDependency_SourcePropertyChanges_TargetPropertyShouldChange_Impl(bool useSmartNotification)
		{
			var tu = new SimpleIndirectDependency_SourcePropertyChanges_TargetPropertyShouldChange_TestClass(useSmartNotification);
			tu.Source = "Initial";
			var expectedValue = "ABC";

			var recorder = CreatePropertyChangeRecorder(tu, k => k.Target);
			tu.Source = expectedValue;

			Assert.IsTrue(recorder.HasChanged);
			Assert.AreEqual(recorder.NewValues[0], expectedValue);
		}
		#endregion

		#region DirectDependencyOnExternalObject_ExternalObjectPropertyChanges_TargetPropertyShouldChange
		class DirectDependencyOnExternalObject_ExternalObjectPropertyChanges_TargetPropertyShouldChange_TestClass : BindableForUnitTests
		{
			public DirectDependencyOnExternalObject_ExternalObjectPropertyChanges_TargetPropertyShouldChange_TestClass(Dependency externalObject)
			{
				ExternalObject = externalObject;
			}

			public Dependency ExternalObject { get; private set; }

			public string Target
			{
				get
				{
					Property(() => Target)
						.Depends(p => p.On(ExternalObject, i => i.Source));

					return ExternalObject.Source;
				}
			}

			public class Dependency : BindableForUnitTests
			{
				public Dependency()
				{
				}

				private string _source;

				public Dependency(bool useSmartPropertyChangeNotificationByDefault)
					: base(useSmartPropertyChangeNotificationByDefault)
				{
				}

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
		}

		[TestMethod]
		public void DirectDependencyOnExternalObject_ExternalObjectPropertyChanges_TargetPropertyShouldChange()
		{
			DirectDependencyOnExternalObject_ExternalObjectPropertyChanges_TargetPropertyShouldChange_Impl(useSmartNotification: true);
			DirectDependencyOnExternalObject_ExternalObjectPropertyChanges_TargetPropertyShouldChange_Impl(useSmartNotification: false);
		}

		private static void DirectDependencyOnExternalObject_ExternalObjectPropertyChanges_TargetPropertyShouldChange_Impl(bool useSmartNotification)
		{
			DirectDependencyOnExternalObject_ExternalObjectPropertyChanges_TargetPropertyShouldChange_TestClass.Dependency externalObject = new DirectDependencyOnExternalObject_ExternalObjectPropertyChanges_TargetPropertyShouldChange_TestClass.Dependency(useSmartNotification);
			DirectDependencyOnExternalObject_ExternalObjectPropertyChanges_TargetPropertyShouldChange_TestClass dependentClass = new DirectDependencyOnExternalObject_ExternalObjectPropertyChanges_TargetPropertyShouldChange_TestClass(externalObject);

			var recorder = CreatePropertyChangeRecorder(dependentClass, k => k.Target);
			externalObject.Source = DefaultNewValue;

			Assert.IsTrue(recorder.HasChanged);
			Assert.AreEqual(recorder.NewValues[0], DefaultNewValue);
		}
		#endregion

		#region IndirectDependency_PropertyChanges_OnlyOnePropertyChangeNotificationIsFired

		class IndirectDependency_PropertyChanges_OnlyOnePropertyChangeNotificationIsFired_TestClass : BindableForUnitTests
		{
			public IndirectDependency_PropertyChanges_OnlyOnePropertyChangeNotificationIsFired_TestClass(bool useSmartPropertyChangeNotificationByDefault)
				: base(useSmartPropertyChangeNotificationByDefault)
			{
			}

			private string _bottomLayer;
			public string BottomLayer
			{
				[DebuggerStepThrough]
				get { return _bottomLayer; }
				set
				{
					if (value == _bottomLayer)
						return;

					_bottomLayer = value;
					NotifyPropertyChanged(() => BottomLayer);
				}
			}

			public string MidLayer1
			{
				get
				{
					Property(() => MidLayer1)
						.Depends(p => p.On(() => BottomLayer));

					return BottomLayer;
				}
			}

			public string MidLayer2
			{
				get
				{
					Property(() => MidLayer2)
						.Depends(p => p.On(() => BottomLayer));

					return BottomLayer;
				}
			}

			public string MidLayer3
			{
				get
				{
					Property(() => MidLayer3)
						.Depends(p => p.On(() => BottomLayer));

					return BottomLayer;
				}
			}

			public string TopLayer
			{
				get
				{
					Property(() => TopLayer)
						.Depends(p => p.On(() => MidLayer1)
										.AndOn(() => MidLayer2)
										.AndOn(() => MidLayer3));

					return MidLayer1 + MidLayer2 + MidLayer3;
				}
			}
		}

		[TestMethod]
		public void IndirectDependency_PropertyChanges_OnlyOnePropertyChangeNotificationIsFired()
		{
			var tu = new IndirectDependency_PropertyChanges_OnlyOnePropertyChangeNotificationIsFired_TestClass(useSmartPropertyChangeNotificationByDefault: true);

			var recorder = CreatePropertyChangeRecorder(tu, k => k.TopLayer);
			tu.BottomLayer = DefaultNewValue;

			Assert.IsTrue(recorder.HasChanged);
			Assert.AreEqual(1, recorder.NumberOfChanges);
			Assert.AreEqual(DefaultNewValue + DefaultNewValue + DefaultNewValue, recorder.NewValues[0]);
		}

		#endregion

		#region CachedValueOfDirectDependency_SourcePropertiesChange_CacheValueIsReevaluated

		class CachedValueOfDirectDependency_SourcePropertiesChange_CacheValueIsReevaluated_TestClass : BindableForUnitTests
		{
			public CachedValueOfDirectDependency_SourcePropertiesChange_CacheValueIsReevaluated_TestClass(bool useSmartPropertyChangeNotificationByDefault)
				: base(useSmartPropertyChangeNotificationByDefault)
			{
			}

			public int Source1Evaluations { get; set; }
			public int Source2Evaluations { get; set; }
			public int Source3Evaluations { get; set; }

			public string Target
			{
				get
				{
					Property(() => Target)
						.Depends(p => p.On(() => Source1)
										.AndOn(() => Source2)
										.AndOn(() => Source3));

					return CachedValue(() => Target, () => Source1 + Source2 + Source3);
				}
			}

			private string _source1;
			public string Source1
			{
				[DebuggerStepThrough]
				get
				{
					Source1Evaluations++;
					return _source1;

				}
				set
				{
					if (value == _source1)
						return;

					_source1 = value;
					NotifyPropertyChanged(() => Source1);
				}
			}

			private string _source2;
			public string Source2
			{
				[DebuggerStepThrough]
				get
				{
					Source2Evaluations++;
					return _source2;

				}
				set
				{
					if (value == _source2)
						return;

					_source2 = value;
					NotifyPropertyChanged(() => Source2);
				}
			}

			private string _source3;
			public string Source3
			{
				[DebuggerStepThrough]
				get
				{
					Source3Evaluations++;
					return _source3;
				}
				set
				{
					if (value == _source3)
						return;

					_source3 = value;
					NotifyPropertyChanged(() => Source3);
				}
			}
		}



		[TestMethod]
		public void CachedValueOfDirectDependency_SourcePropertiesChange_CacheValueIsReevaluated()
		{
			CachedValueOfDirectDependency_SourcePropertiesChange_CacheValueIsReevaluated_Impl(useSmartNotification: true);
			CachedValueOfDirectDependency_SourcePropertiesChange_CacheValueIsReevaluated_Impl(useSmartNotification: false);
		}

		private static void CachedValueOfDirectDependency_SourcePropertiesChange_CacheValueIsReevaluated_Impl(bool useSmartNotification)
		{
			var tu = new CachedValueOfDirectDependency_SourcePropertiesChange_CacheValueIsReevaluated_TestClass(useSmartNotification)
			{
				Source1 = "A",
				Source2 = "B",
				Source3 = "C"
			};

			var evaluation1 = tu.Target;
			var evaluation2 = tu.Target;
			var evaluation3 = tu.Target;

			Assert.AreEqual(1, tu.Source1Evaluations);
			Assert.AreEqual(1, tu.Source2Evaluations);
			Assert.AreEqual(1, tu.Source3Evaluations);
			Assert.AreEqual("ABC", tu.Target);

			//Invalidate cache
			tu.Source1 = "AA";

			evaluation1 = tu.Target;
			evaluation2 = tu.Target;
			evaluation3 = tu.Target;

			Assert.AreEqual(2, tu.Source1Evaluations);
			Assert.AreEqual(2, tu.Source2Evaluations);
			Assert.AreEqual(2, tu.Source3Evaluations);
			Assert.AreEqual("AABC", tu.Target);

			//Invalidate cache
			tu.Source2 = "BB";

			evaluation1 = tu.Target;
			evaluation2 = tu.Target;
			evaluation3 = tu.Target;

			Assert.AreEqual(3, tu.Source1Evaluations);
			Assert.AreEqual(3, tu.Source2Evaluations);
			Assert.AreEqual(3, tu.Source3Evaluations);
			Assert.AreEqual("AABBC", tu.Target);

			//Invalidate cache
			tu.Source3 = "CC";

			evaluation1 = tu.Target;
			evaluation2 = tu.Target;
			evaluation3 = tu.Target;

			Assert.AreEqual(4, tu.Source1Evaluations);
			Assert.AreEqual(4, tu.Source2Evaluations);
			Assert.AreEqual(4, tu.Source3Evaluations);
			Assert.AreEqual("AABBCC", tu.Target);
		}

		#endregion

		#region DirectDependencyOnCollectionsChildProperty_CollectionChildIsModifiedAddedAndRemoved_TargetPropertyIsUpdated
		class DirectDependencyOnCollectionsChildProperty_CollectionChildIsModifiedAddedAndRemoved_TargetPropertyIsUpdated_TestClass : BindableForUnitTests
		{
			public DirectDependencyOnCollectionsChildProperty_CollectionChildIsModifiedAddedAndRemoved_TargetPropertyIsUpdated_TestClass(bool useSmartPropertyChangeNotificationByDefault)
				: base(useSmartPropertyChangeNotificationByDefault)
			{
				Children = new DependencyFrameworkObservableCollection<DirectDependencyOnCollectionsChildProperty_CollectionChildIsModifiedAddedAndRemoved_TargetPropertyIsUpdated_CollectionChildClass>();
			}

			public decimal SumOfChildren
			{
				get
				{
					Property(() => SumOfChildren)
						.Depends(p => p.OnCollectionChildProperty(Children, k => k.Source));

					return Children.Select(k => k.Source).SumOrZero();
				}
			}

			public DependencyFrameworkObservableCollection<DirectDependencyOnCollectionsChildProperty_CollectionChildIsModifiedAddedAndRemoved_TargetPropertyIsUpdated_CollectionChildClass> Children { get; private set; }
		}

		class DirectDependencyOnCollectionsChildProperty_CollectionChildIsModifiedAddedAndRemoved_TargetPropertyIsUpdated_CollectionChildClass : BindableForUnitTests
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

		[TestMethod]
		public void DirectDependencyOnCollectionsChildProperty_CollectionChildIsModifiedAddedAndRemoved_TargetPropertyIsUpdated()
		{
			DirectDependencyOnCollectionsChildProperty_CollectionChildIsModifiedAddedAndRemoved_TargetPropertyIsUpdated_Impl(false);
			DirectDependencyOnCollectionsChildProperty_CollectionChildIsModifiedAddedAndRemoved_TargetPropertyIsUpdated_Impl(true);
		}

		private static void DirectDependencyOnCollectionsChildProperty_CollectionChildIsModifiedAddedAndRemoved_TargetPropertyIsUpdated_Impl(bool useSmartNotification)
		{
			var testClass = new DirectDependencyOnCollectionsChildProperty_CollectionChildIsModifiedAddedAndRemoved_TargetPropertyIsUpdated_TestClass(useSmartNotification);

			var recorder = CreatePropertyChangeRecorder(testClass, k => k.SumOfChildren);

			testClass.Children.Add(
								   new DirectDependencyOnCollectionsChildProperty_CollectionChildIsModifiedAddedAndRemoved_TargetPropertyIsUpdated_CollectionChildClass()
								   {
									   Source = 1
								   });

			Assert.IsTrue(recorder.HasChanged);
			Assert.AreEqual(1, recorder.NumberOfChanges);
			Assert.AreEqual(1m, recorder.NewValues.Last());

			testClass.Children.Add(
								   new DirectDependencyOnCollectionsChildProperty_CollectionChildIsModifiedAddedAndRemoved_TargetPropertyIsUpdated_CollectionChildClass()
								   {
									   Source = 10
								   });

			Assert.AreEqual(2, recorder.NumberOfChanges);
			Assert.AreEqual(11m, recorder.NewValues.Last());

			testClass.Children[0].Source = 5;
			testClass.Children[1].Source = 100;

			Assert.AreEqual(4, recorder.NumberOfChanges);
			Assert.AreEqual(105m, recorder.NewValues.Last());

			testClass.Children.Remove(testClass.Children.Last());

			Assert.AreEqual(5, recorder.NumberOfChanges);
			Assert.AreEqual(5m, recorder.NewValues.Last());
		}
		#endregion

		#region DirectDependencyOnCollectionsChildProperty_ChildIsRemoved_NoReferencesAreKeptOnChild

		class DirectDependencyOnCollectionsChildProperty_ChildIsRemoved_NoReferencesAreKeptOnChild_TestClass : BindableForUnitTests
		{
			public DirectDependencyOnCollectionsChildProperty_ChildIsRemoved_NoReferencesAreKeptOnChild_TestClass(bool useSmartPropertyChangeNotificationByDefault)
				: base(useSmartPropertyChangeNotificationByDefault)
			{
				Children = new DependencyFrameworkObservableCollection<Dependency>();
			}

			public DependencyFrameworkObservableCollection<Dependency> Children { get; private set; }

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

			public decimal Target
			{
				get
				{
					Property(() => Target)
						.Depends(p => p.OnCollectionChildProperty(Children, k => k.Source));

					return Children.Select(k => k.Source).SumOrZero();
				}
			}
		}

		[TestMethod]
		public void DirectDependencyOnCollectionsChildProperty_ChildIsRemoved_NoReferencesAreKeptOnChild()
		{
			DirectDependencyOnCollectionsChildProperty_ChildIsRemoved_NoReferencesAreKeptOnChild_Impl(true);
			DirectDependencyOnCollectionsChildProperty_ChildIsRemoved_NoReferencesAreKeptOnChild_Impl(false);
		}

		private static void DirectDependencyOnCollectionsChildProperty_ChildIsRemoved_NoReferencesAreKeptOnChild_Impl(bool useSmartNotification)
		{
			var tu = new DirectDependencyOnCollectionsChildProperty_ChildIsRemoved_NoReferencesAreKeptOnChild_TestClass(useSmartNotification);

			var recorder = CreatePropertyChangeRecorder(tu, k => k.Target);
			tu.Children.Add(new DirectDependencyOnCollectionsChildProperty_ChildIsRemoved_NoReferencesAreKeptOnChild_TestClass.Dependency()
			{
				Source = 1
			});
			tu.Children.Add(new DirectDependencyOnCollectionsChildProperty_ChildIsRemoved_NoReferencesAreKeptOnChild_TestClass.Dependency()
			{
				Source = 2
			});

			var child1 = tu.Children[0];
			var child2 = tu.Children[1];

			Assert.IsTrue(recorder.HasChanged);
			Assert.AreEqual(3m, recorder.NewValues.Last());
			Assert.IsTrue(child1.AreEventHandlersRegisteredOnPropertyChanged);
			Assert.IsTrue(child1.AreEventHandlersRegisteredOnPropertyChangedInTransaction);
			Assert.IsTrue(child2.AreEventHandlersRegisteredOnPropertyChanged);
			Assert.IsTrue(child2.AreEventHandlersRegisteredOnPropertyChangedInTransaction);

			tu.Children.Remove(child2);
			Assert.IsTrue(child1.AreEventHandlersRegisteredOnPropertyChanged);
			Assert.IsTrue(child1.AreEventHandlersRegisteredOnPropertyChangedInTransaction);
			Assert.IsFalse(child2.AreEventHandlersRegisteredOnPropertyChanged);
			Assert.IsFalse(child2.AreEventHandlersRegisteredOnPropertyChangedInTransaction);

			tu.Children.Remove(child1);
			Assert.IsFalse(child1.AreEventHandlersRegisteredOnPropertyChanged);
			Assert.IsFalse(child1.AreEventHandlersRegisteredOnPropertyChangedInTransaction);
			Assert.IsFalse(child2.AreEventHandlersRegisteredOnPropertyChanged);
			Assert.IsFalse(child2.AreEventHandlersRegisteredOnPropertyChangedInTransaction);
		}

		#endregion

		#region CallbackRegisteredOnDirectDependency_DependencyChanges_CallbackIsFiredOnce
		public class CallbackRegisteredOnDirectDependency_DependencyChanges_CallbackIsFiredOnce_TestClass : BindableForUnitTests
		{
			public CallbackRegisteredOnDirectDependency_DependencyChanges_CallbackIsFiredOnce_TestClass(bool useSmartPropertyChangeNotificationByDefault)
				: base(useSmartPropertyChangeNotificationByDefault)
			{
				Children = new DependencyFrameworkObservableCollection<CollectionChildClass>();

				RegisterCallbackDependency(this, k => k.Target, () => { NumberOfCallbackCallsAfterTargetPropertyChanged++; });
				RegisterCallbackDependency<CollectionChildClass, string>(Children, k => k.Source, () => { NumberOfCallbackCallsAfterChildPropertyChanged++; });
				RegisterCallbackDependency(Children, () => { NumberOfCallbackCallsAfterNumberOfChildrenChanged++; });
			}

			public DependencyFrameworkObservableCollection<CollectionChildClass> Children { get; private set; }

			public int NumberOfCallbackCallsAfterTargetPropertyChanged { get; private set; }
			public int NumberOfCallbackCallsAfterChildPropertyChanged { get; private set; }
			public int NumberOfCallbackCallsAfterNumberOfChildrenChanged { get; private set; }

			public class CollectionChildClass : Bindable
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

			public string Target
			{
				get
				{
					Property(() => Target)
						.Depends(p => p.On(() => Source));

					return Source;
				}
			}
		}

		[TestMethod]
		public void CallbackRegisteredOnDirectDependency_DependencyChanges_CallbackIsFiredOnce()
		{
			CallbackRegisteredOnDirectDependency_DependencyChanges_CallbackIsFiredOnce_Impl(useSmartNotification: true);
			CallbackRegisteredOnDirectDependency_DependencyChanges_CallbackIsFiredOnce_Impl(useSmartNotification: false);
		}

		private static void CallbackRegisteredOnDirectDependency_DependencyChanges_CallbackIsFiredOnce_Impl(bool useSmartNotification)
		{
			var tu = new CallbackRegisteredOnDirectDependency_DependencyChanges_CallbackIsFiredOnce_TestClass(useSmartNotification);

			var initilization = tu.Target;

			tu.Source = "Change1";
			Assert.AreEqual(1, tu.NumberOfCallbackCallsAfterTargetPropertyChanged);

			tu.Source = "Change2";
			Assert.AreEqual(2, tu.NumberOfCallbackCallsAfterTargetPropertyChanged);

			tu.Children.Add(new CallbackRegisteredOnDirectDependency_DependencyChanges_CallbackIsFiredOnce_TestClass.CollectionChildClass() { Source = "Initial" });
			Assert.AreEqual(1, tu.NumberOfCallbackCallsAfterNumberOfChildrenChanged);
			Assert.AreEqual(1, tu.NumberOfCallbackCallsAfterChildPropertyChanged);

			tu.Children[0].Source = "New";
			Assert.AreEqual(1, tu.NumberOfCallbackCallsAfterNumberOfChildrenChanged);
			Assert.AreEqual(2, tu.NumberOfCallbackCallsAfterChildPropertyChanged);

			tu.Children.RemoveAt(0);
			Assert.AreEqual(2, tu.NumberOfCallbackCallsAfterNumberOfChildrenChanged);
			Assert.AreEqual(3, tu.NumberOfCallbackCallsAfterChildPropertyChanged);
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
