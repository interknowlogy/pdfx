using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PropertyDependencyFramework;

namespace PropertyDependencyFramework_Tests
{
    [TestClass]
    public class BindableBaseTypeRegistrationTests
    {
        //Name: MemberName[_paramvalue[s]]_ExpectedResult

        [TestMethod]
        public void GetPropertyRegistrations_ExecuteActionsOnce()
        {
            //Arrange
            var target = new SimpleMockBindableBase(); //Target = What you are testing

            //Act
            //Act is performed during Instantiation
            var secondTypeOfTarget = new SimpleMockBindableBase();

            //Assert
            //Expected = What you expect
            Assert.IsTrue(target.ActionCount == 1);
        }

        [TestMethod]
        public void Constructor_RegistersDerivedType()
        {
            var target = new MockBindableBase();

            //Act is performed during Instantiation of target

            Type expected = typeof(MockBindableBase);

            Type[] registeredTypes = BindableBase._typeRegistrationProperties.Keys.ToArray();

            CollectionAssert.Contains(registeredTypes, expected);
        }

        [TestMethod]
        public void
            TypeRegistrationProperties_SingleMockBindableBaseInstantiated_DependentPropertyTypeRegistrationImplementationCreatedForTypeProperties
            ()
        {
            Dictionary<Type, Dictionary<string, DependentPropertyTypeRegistrationImplementation>> target =
                BindableBase._typeRegistrationProperties;

            //TODO: Figure out how to enforce this with all the static stuff going on...
            //if (target.Keys.Count > 0)
            //{
            //    Assert.Inconclusive("BindableBase has pre-existing Type registrations.");
            //}


            var bindable = new MockBindableBase();

            //TODO: Figure out how to enforce this with all the static stuff going on...
            //Assert.IsTrue(target.Keys.Count == 1);

            Type expectedType = typeof(MockBindableBase);
            Dictionary<string, DependentPropertyTypeRegistrationImplementation> actual = target[expectedType];

            Assert.IsTrue(actual.Keys.Count == 1);

            string expectedDependentPropertyName = "DependentProp";
            Assert.IsTrue(actual.ContainsKey(expectedDependentPropertyName));

            DependentPropertyTypeRegistrationImplementation actualDependentPropertyTypeRegistrationImplementation =
                actual[expectedDependentPropertyName];
            Assert.IsNotNull(actualDependentPropertyTypeRegistrationImplementation);
        }

        [TestMethod]
        public void DependenciesByType_SingleMockBindableBaseInstantiated_CorrectRegistrationCreatedForType()
        {
            Dictionary<Type, TypeDependencies> target =
                ((TypeRegistrationAPI)BindableBase._typeRegistrationApi).DependenciesByType;

            var bindable = new MockBindableBase();

            Type expectedType = typeof(MockBindableBase);
            Assert.IsTrue(target.ContainsKey(expectedType));

            TypeDependencies actualTypeDependencies = target[expectedType];
            Assert.IsNotNull(actualTypeDependencies);

            Assert.IsTrue(actualTypeDependencies.SourceProviders.Count == 1);

            SourceProvider actualSourceProvider = actualTypeDependencies.SourceProviders[expectedType];
            Assert.IsNotNull(actualSourceProvider);

            Assert.IsTrue(actualSourceProvider.SourceProperties.Count == 1);

            MockBindableBase expectedSource = bindable;
            INotifyPropertyChanged actualSource = actualSourceProvider.SourceRetrievalFunc(bindable);
            Assert.AreSame(expectedSource, actualSource);

            string expctedSourceProperty = "InputProp";
            SourceProperty actualSourceProperty = actualSourceProvider.SourceProperties[expctedSourceProperty];
            Assert.IsNotNull(actualSourceProperty);
            Assert.AreEqual(expctedSourceProperty, actualSourceProperty.Name);

            Assert.IsTrue(actualSourceProperty.DependentPropertyNames.Count == 1);

            string expectedDependentPropertyName = "DependentProp";
            Assert.AreEqual(expectedDependentPropertyName, actualSourceProperty.DependentPropertyNames[0]);
        }

        [TestMethod]
        public void DependenciesByType_TwoMockBindableBaseInstantiated_CorrectRegistrationCreatedForTypeAndNoDuplicates()
        {
            Dictionary<Type, TypeDependencies> target =
                ((TypeRegistrationAPI)BindableBase._typeRegistrationApi).DependenciesByType;

            var bindable1 = new MockBindableBase();
            var bindable2 = new MockBindableBase();

            Type expectedType = typeof(MockBindableBase);
            Assert.IsTrue(target.ContainsKey(expectedType));

            TypeDependencies actualTypeDependencies = target[expectedType];
            Assert.IsNotNull(actualTypeDependencies);

            Assert.IsTrue(actualTypeDependencies.SourceProviders.Count == 1);

            SourceProvider actualSourceProvider = actualTypeDependencies.SourceProviders[expectedType];
            Assert.IsNotNull(actualSourceProvider);

            Assert.IsTrue(actualSourceProvider.SourceProperties.Count == 1);

            MockBindableBase expectedSource1 = bindable1;
            INotifyPropertyChanged actualSource1 = actualSourceProvider.SourceRetrievalFunc(bindable1);
            Assert.AreSame(expectedSource1, actualSource1);

            MockBindableBase expectedSource2 = bindable2;
            INotifyPropertyChanged actualSource2 = actualSourceProvider.SourceRetrievalFunc(bindable2);
            Assert.AreSame(expectedSource2, actualSource2);

            string expctedSourceProperty = "InputProp";
            SourceProperty actualSourceProperty = actualSourceProvider.SourceProperties[expctedSourceProperty];
            Assert.IsNotNull(actualSourceProperty);
            Assert.AreEqual(expctedSourceProperty, actualSourceProperty.Name);

            Assert.IsTrue(actualSourceProperty.DependentPropertyNames.Count == 1);

            string expectedDependentPropertyName = "DependentProp";
            Assert.AreEqual(expectedDependentPropertyName, actualSourceProperty.DependentPropertyNames[0]);
        }

        [TestMethod]
        public void
            DependenciesByType_SingleMockBindableBaseInstantiatedAndSingleMockBindableBaseDependentOnMockBindableBaseInstantiated_CorrectRegistrationCreatedForType
            ()
        {
            Dictionary<Type, TypeDependencies> target =
                ((TypeRegistrationAPI)BindableBase._typeRegistrationApi).DependenciesByType;

            var bindable1 = new MockBindableBase();
            var bindable2 = new MockBindableBaseDependentOnMockBindableBase(bindable1);

            Type expectedType = typeof(MockBindableBase);
            Assert.IsTrue(target.ContainsKey(expectedType));

            TypeDependencies actualTypeDependencies = target[expectedType];
            Assert.IsNotNull(actualTypeDependencies);

            Assert.IsTrue(actualTypeDependencies.SourceProviders.Count == 1);

            SourceProvider actualSourceProvider = actualTypeDependencies.SourceProviders[expectedType];
            Assert.IsNotNull(actualSourceProvider);

            Assert.IsTrue(actualSourceProvider.SourceProperties.Count == 1);

            MockBindableBase expectedSource = bindable1;
            INotifyPropertyChanged actualSource = actualSourceProvider.SourceRetrievalFunc(bindable1);
            Assert.AreSame(expectedSource, actualSource);

            string expctedSourceProperty = "InputProp";
            SourceProperty actualSourceProperty = actualSourceProvider.SourceProperties[expctedSourceProperty];
            Assert.IsNotNull(actualSourceProperty);
            Assert.AreEqual(expctedSourceProperty, actualSourceProperty.Name);

            Assert.IsTrue(actualSourceProperty.DependentPropertyNames.Count == 1);

            string expectedDependentPropertyName = "DependentProp";
            Assert.AreEqual(expectedDependentPropertyName, actualSourceProperty.DependentPropertyNames[0]);

            //***********************//

            Type expectedType2 = typeof(MockBindableBaseDependentOnMockBindableBase);
            Assert.IsTrue(target.ContainsKey(expectedType2));

            TypeDependencies actualTypeDependencies2 = target[expectedType2];
            Assert.IsNotNull(actualTypeDependencies2);

            Assert.IsTrue(actualTypeDependencies2.SourceProviders.Count == 1);

            SourceProvider actualSourceProvider2 = actualTypeDependencies2.SourceProviders[expectedType];
            //Expecting the first BindableBase type
            Assert.IsNotNull(actualSourceProvider2);

            Assert.IsTrue(actualSourceProvider2.SourceProperties.Count == 1);

            INotifyPropertyChanged actualSource2 = actualSourceProvider2.SourceRetrievalFunc(bindable2);
            Assert.AreSame(expectedSource, actualSource2); //Expecting the first BindableBase type instance

            string expctedSourceProperty2 = "DependentProp";
            SourceProperty actualSourceProperty2 = actualSourceProvider2.SourceProperties[expctedSourceProperty2];
            Assert.IsNotNull(actualSourceProperty2);
            Assert.AreEqual(expctedSourceProperty2, actualSourceProperty2.Name);

            Assert.IsTrue(actualSourceProperty2.DependentPropertyNames.Count == 1);

            string expectedDependentPropertyName2 = "DependentOnBindableBaseProp";
            Assert.AreEqual(expectedDependentPropertyName2, actualSourceProperty2.DependentPropertyNames[0]);
        }

        [TestMethod]
        public void
            RegisterPropertyDependenciesForTypeInstance_SingleMockBindableBaseInstantiated_CorrectInstanceDependeciesRegistered
            ()
        {
            var bindable = new MockBindableBase();

            Dictionary<INotifyPropertyChanged, ObjectPropertyDependencyRegistration> target =
                bindable._propertyDependencies;
            Assert.IsTrue(target.Count == 1);

            MockBindableBase expectedSourceInstance = bindable;
            INotifyPropertyChanged actualSourceInstance = target.Keys.First();
            Assert.AreSame(expectedSourceInstance, actualSourceInstance);

            ObjectPropertyDependencyRegistration actualObjectPropertyDependencyRegistration =
                target[actualSourceInstance];
            Assert.IsNotNull(actualObjectPropertyDependencyRegistration);
            Assert.IsTrue(actualObjectPropertyDependencyRegistration.Callbacks.Count == 0);
            Assert.IsTrue(actualObjectPropertyDependencyRegistration.PropertyDependencies.Count == 1);

            string expectedSourcePropertyName = "InputProp";
            string actualSourcePropertyName =
                actualObjectPropertyDependencyRegistration.PropertyDependencies.Keys.First();
            Assert.AreEqual(expectedSourcePropertyName, actualSourcePropertyName);

            PropertyDependencies actualSourcePropertyDependencies =
                actualObjectPropertyDependencyRegistration.PropertyDependencies[actualSourcePropertyName];
            Assert.IsNotNull(actualSourcePropertyDependencies);
            Assert.IsTrue(actualSourcePropertyDependencies.Callbacks.Count == 0);
            Assert.IsTrue(actualSourcePropertyDependencies.DependentProperties.Count == 1);

            string expectedDependentPropertyName = "DependentProp";
            string actualDependentPropertyName = actualSourcePropertyDependencies.DependentProperties.First();
            Assert.AreEqual(expectedDependentPropertyName, actualDependentPropertyName);
        }

        [TestMethod]
        public void
            RegisterPropertyDependenciesForTypeInstance_SingleMockBindableBaseInstantiated_NotifyPropertyChangedSubscribedOnlyOnce
            ()
        {
            var bindable = new MockBindableBase();

            var target = PropertyChangeRecorder.CreatePropertyChangeRecorder(bindable, b => b.DependentProp);

            bindable.InputProp = 1;

            Assert.IsTrue(target.NumberOfChanges == 1);
        }

        [TestMethod]
        public void
            RegisterPropertyDependenciesForTypeInstance_SingleMockBindableBaseInstantiatedAndSingleMockBindableBaseDependentOnMockBindableBaseInstantiated_CorrectInstanceDependeciesRegistered
            ()
        {
            var bindable1 = new MockBindableBase();
            var bindable2 = new MockBindableBaseDependentOnMockBindableBase(bindable1);

            Dictionary<INotifyPropertyChanged, ObjectPropertyDependencyRegistration> target1 =
                bindable1._propertyDependencies;
            Assert.IsTrue(target1.Count == 1);

            MockBindableBase expectedSourceInstance1 = bindable1;
            INotifyPropertyChanged actualSourceInstance1 = target1.Keys.First();
            Assert.AreSame(expectedSourceInstance1, actualSourceInstance1);

            ObjectPropertyDependencyRegistration actualObjectPropertyDependencyRegistration1 =
                target1[actualSourceInstance1];
            Assert.IsNotNull(actualObjectPropertyDependencyRegistration1);
            Assert.IsTrue(actualObjectPropertyDependencyRegistration1.Callbacks.Count == 0);
            Assert.IsTrue(actualObjectPropertyDependencyRegistration1.PropertyDependencies.Count == 1);

            string expectedSourcePropertyName1 = "InputProp";
            string actualSourcePropertyName1 =
                actualObjectPropertyDependencyRegistration1.PropertyDependencies.Keys.First();
            Assert.AreEqual(expectedSourcePropertyName1, actualSourcePropertyName1);

            PropertyDependencies actualSourcePropertyDependencies1 =
                actualObjectPropertyDependencyRegistration1.PropertyDependencies[actualSourcePropertyName1];
            Assert.IsNotNull(actualSourcePropertyDependencies1);
            Assert.IsTrue(actualSourcePropertyDependencies1.Callbacks.Count == 0);
            Assert.IsTrue(actualSourcePropertyDependencies1.DependentProperties.Count == 1);

            string expectedDependentPropertyName1 = "DependentProp";
            string actualDependentPropertyName1 = actualSourcePropertyDependencies1.DependentProperties.First();
            Assert.AreEqual(expectedDependentPropertyName1, actualDependentPropertyName1);



            Dictionary<INotifyPropertyChanged, ObjectPropertyDependencyRegistration> target2 =
                bindable2._propertyDependencies;
            Assert.IsTrue(target2.Count == 1);


            INotifyPropertyChanged actualSourceInstance2 = target2.Keys.First();
            Assert.AreSame(expectedSourceInstance1, actualSourceInstance2);

            ObjectPropertyDependencyRegistration actualObjectPropertyDependencyRegistration2 =
                target2[actualSourceInstance2];
            Assert.IsNotNull(actualObjectPropertyDependencyRegistration2);
            Assert.IsTrue(actualObjectPropertyDependencyRegistration2.Callbacks.Count == 0);
            Assert.IsTrue(actualObjectPropertyDependencyRegistration2.PropertyDependencies.Count == 1);

            string expectedSourcePropertyName2 = "DependentProp";
            string actualSourcePropertyName2 =
                actualObjectPropertyDependencyRegistration2.PropertyDependencies.Keys.First();
            Assert.AreEqual(expectedSourcePropertyName2, actualSourcePropertyName2);

            PropertyDependencies actualSourcePropertyDependencies2 =
                actualObjectPropertyDependencyRegistration2.PropertyDependencies[actualSourcePropertyName2];
            Assert.IsNotNull(actualSourcePropertyDependencies2);
            Assert.IsTrue(actualSourcePropertyDependencies2.Callbacks.Count == 0);
            Assert.IsTrue(actualSourcePropertyDependencies2.DependentProperties.Count == 1);

            string expectedDependentPropertyName2 = "DependentOnBindableBaseProp";
            string actualDependentPropertyName2 = actualSourcePropertyDependencies2.DependentProperties.First();
            Assert.AreEqual(expectedDependentPropertyName2, actualDependentPropertyName2);
        }

        [TestMethod]
        public void
            RegisterPropertyDependenciesForTypeInstance_SingleMockBindableBaseInstantiatedAndSingleMockBindableBaseDependentOnMockBindableBaseInstantiated_NotifyPropertyChangedSubscribedOnlyOncePerInstance
            ()
        {
            var bindable1 = new MockBindableBase();
            var bindable2 = new MockBindableBaseDependentOnMockBindableBase(bindable1);

            var target1 = PropertyChangeRecorder.CreatePropertyChangeRecorder(bindable1, b => b.DependentProp);
            var target2 = PropertyChangeRecorder.CreatePropertyChangeRecorder(bindable2,
                b => b.DependentOnBindableBaseProp);

            bindable1.InputProp = 1;

            Assert.IsTrue(target1.NumberOfChanges == 1);
            Assert.IsTrue(target2.NumberOfChanges == 1);
        }

        [TestMethod]
        public void
            RegisterPropertyDependenciesForTypeInstance_SingleMockMultipleDependenciesOnSelfBindableBaseInstantiated_CorrectInstanceDependeciesRegistered
            ()
        {
            var bindable = new MockMultipleDependenciesOnSelfBindableBase();

            Dictionary<INotifyPropertyChanged, ObjectPropertyDependencyRegistration> target =
                bindable._propertyDependencies;
            Assert.IsTrue(target.Count == 1);

            MockMultipleDependenciesOnSelfBindableBase expectedSourceInstance = bindable;
            INotifyPropertyChanged actualSourceInstance = target.Keys.First();
            Assert.AreSame(expectedSourceInstance, actualSourceInstance);

            ObjectPropertyDependencyRegistration actualObjectPropertyDependencyRegistration =
                target[actualSourceInstance];
            Assert.IsNotNull(actualObjectPropertyDependencyRegistration);
            Assert.IsTrue(actualObjectPropertyDependencyRegistration.Callbacks.Count == 0);
            Assert.IsTrue(actualObjectPropertyDependencyRegistration.PropertyDependencies.Count == 1);

            string expectedSourcePropertyName = "InputProp";
            string actualSourcePropertyName =
                actualObjectPropertyDependencyRegistration.PropertyDependencies.Keys.First();
            Assert.AreEqual(expectedSourcePropertyName, actualSourcePropertyName);

            PropertyDependencies actualSourcePropertyDependencies =
                actualObjectPropertyDependencyRegistration.PropertyDependencies[actualSourcePropertyName];
            Assert.IsNotNull(actualSourcePropertyDependencies);
            Assert.IsTrue(actualSourcePropertyDependencies.Callbacks.Count == 0);
            Assert.IsTrue(actualSourcePropertyDependencies.DependentProperties.Count == 2);

            string expectedDependentPropertyNameA = "DependentProp";
            string expectedDependentPropertyNameB = "DependentProp2";
            CollectionAssert.Contains(actualSourcePropertyDependencies.DependentProperties,
                expectedDependentPropertyNameA);
            CollectionAssert.Contains(actualSourcePropertyDependencies.DependentProperties,
                expectedDependentPropertyNameB);
        }

        [TestMethod]
        public void
            RegisterPropertyDependenciesForTypeInstance_SingleMockMultipleDependenciesOnSelfBindableBaseInstantiated_NotifyPropertyChangedSubscribedOnlyOnce
            ()
        {
            var bindable = new MockMultipleDependenciesOnSelfBindableBase();

            var targetA = PropertyChangeRecorder.CreatePropertyChangeRecorder(bindable, b => b.DependentProp);
            var targetB = PropertyChangeRecorder.CreatePropertyChangeRecorder(bindable, b => b.DependentProp2);

            bindable.InputProp = 1;

            Assert.IsTrue(targetA.NumberOfChanges == 1);
            Assert.IsTrue(targetB.NumberOfChanges == 1);
        }

        [TestMethod]
        public void
            RegisterPropertyDependenciesForTypeInstance_SingleMockMultipleDependenciesOnSelfBindableBaseInstantiatedAndSingleMockBindableBaseMultipleDependentciesOnMockMultipleDependenciesOnSelfBindableBaseInstantiated_CorrectInstanceDependeciesRegistered
            ()
        {
            var bindable1 = new MockMultipleDependenciesOnSelfBindableBase();
            var bindable2 =
                new MockBindableBaseMultipleDependentciesOnMockMultipleDependenciesOnSelfBindableBase(bindable1);

            Dictionary<INotifyPropertyChanged, ObjectPropertyDependencyRegistration> target1 =
                bindable1._propertyDependencies;
            Assert.IsTrue(target1.Count == 1);

            MockMultipleDependenciesOnSelfBindableBase expectedSourceInstance1 = bindable1;
            INotifyPropertyChanged actualSourceInstance1 = target1.Keys.First();
            Assert.AreSame(expectedSourceInstance1, actualSourceInstance1);

            ObjectPropertyDependencyRegistration actualObjectPropertyDependencyRegistration1 =
                target1[actualSourceInstance1];
            Assert.IsNotNull(actualObjectPropertyDependencyRegistration1);
            Assert.IsTrue(actualObjectPropertyDependencyRegistration1.Callbacks.Count == 0);
            Assert.IsTrue(actualObjectPropertyDependencyRegistration1.PropertyDependencies.Count == 1);

            string expectedSourcePropertyName1 = "InputProp";
            string actualSourcePropertyName1 =
                actualObjectPropertyDependencyRegistration1.PropertyDependencies.Keys.First();
            Assert.AreEqual(expectedSourcePropertyName1, actualSourcePropertyName1);

            PropertyDependencies actualSourcePropertyDependencies1 =
                actualObjectPropertyDependencyRegistration1.PropertyDependencies[actualSourcePropertyName1];
            Assert.IsNotNull(actualSourcePropertyDependencies1);
            Assert.IsTrue(actualSourcePropertyDependencies1.Callbacks.Count == 0);
            Assert.IsTrue(actualSourcePropertyDependencies1.DependentProperties.Count == 2);

            string expectedDependentPropertyName1A = "DependentProp";
            string expectedDependentPropertyName1B = "DependentProp2";
            CollectionAssert.Contains(actualSourcePropertyDependencies1.DependentProperties,
                expectedDependentPropertyName1A);
            CollectionAssert.Contains(actualSourcePropertyDependencies1.DependentProperties,
                expectedDependentPropertyName1B);



            Dictionary<INotifyPropertyChanged, ObjectPropertyDependencyRegistration> target2 =
                bindable2._propertyDependencies;
            Assert.IsTrue(target2.Count == 1);


            INotifyPropertyChanged actualSourceInstance2 = target2.Keys.First();
            Assert.AreSame(expectedSourceInstance1, actualSourceInstance2);

            ObjectPropertyDependencyRegistration actualObjectPropertyDependencyRegistration2 =
                target2[actualSourceInstance2];
            Assert.IsNotNull(actualObjectPropertyDependencyRegistration2);
            Assert.IsTrue(actualObjectPropertyDependencyRegistration2.Callbacks.Count == 0);
            Assert.IsTrue(actualObjectPropertyDependencyRegistration2.PropertyDependencies.Count == 1);

            string expectedSourcePropertyName2 = "DependentProp";
            string actualSourcePropertyName2 =
                actualObjectPropertyDependencyRegistration2.PropertyDependencies.Keys.First();
            Assert.AreEqual(expectedSourcePropertyName2, actualSourcePropertyName2);

            PropertyDependencies actualSourcePropertyDependencies2 =
                actualObjectPropertyDependencyRegistration2.PropertyDependencies[actualSourcePropertyName2];
            Assert.IsNotNull(actualSourcePropertyDependencies2);
            Assert.IsTrue(actualSourcePropertyDependencies2.Callbacks.Count == 0);
            Assert.IsTrue(actualSourcePropertyDependencies2.DependentProperties.Count == 2);

            string expectedDependentPropertyName2A = "DependentOnBindableBaseProp";
            string expectedDependentPropertyName2B = "DependentOnBindableBaseProp2";
            CollectionAssert.Contains(actualSourcePropertyDependencies2.DependentProperties,
                expectedDependentPropertyName2A);
            CollectionAssert.Contains(actualSourcePropertyDependencies2.DependentProperties,
                expectedDependentPropertyName2B);
        }

        [TestMethod]
        public void
            RegisterPropertyDependenciesForTypeInstance_SingleMockMultipleDependenciesOnSelfBindableBaseInstantiatedAndSingleMockBindableBaseMultipleDependentciesOnMockMultipleDependenciesOnSelfBindableBaseInstantiated_NotifyPropertyChangedSubscribedOnlyOncePerInstance
            ()
        {
            var bindable1 = new MockMultipleDependenciesOnSelfBindableBase();
            var bindable2 =
                new MockBindableBaseMultipleDependentciesOnMockMultipleDependenciesOnSelfBindableBase(bindable1);

            var target1A = PropertyChangeRecorder.CreatePropertyChangeRecorder(bindable1, b => b.DependentProp);
            var target1B = PropertyChangeRecorder.CreatePropertyChangeRecorder(bindable1, b => b.DependentProp2);
            var target2A = PropertyChangeRecorder.CreatePropertyChangeRecorder(bindable2,
                b => b.DependentOnBindableBaseProp);
            var target2B = PropertyChangeRecorder.CreatePropertyChangeRecorder(bindable2,
                b => b.DependentOnBindableBaseProp2);

            bindable1.InputProp = 1;

            Assert.IsTrue(target1A.NumberOfChanges == 1);
            Assert.IsTrue(target1B.NumberOfChanges == 1);
            Assert.IsTrue(target2A.NumberOfChanges == 1);
            Assert.IsTrue(target2B.NumberOfChanges == 1);
        }

        [TestMethod]
        public void
            RegisterPropertyDependenciesForTypeInstance_SingleMockMultipleDependenciesAndMultipleInputsOnSelfBindableBaseInstantiated_CorrectInstanceDependeciesRegistered
            ()
        {
            var bindable = new MockMultipleDependenciesAndMultipleInputsOnSelfBindableBase();

            Dictionary<INotifyPropertyChanged, ObjectPropertyDependencyRegistration> target =
                bindable._propertyDependencies;
            Assert.IsTrue(target.Count == 1);

            MockMultipleDependenciesAndMultipleInputsOnSelfBindableBase expectedSourceInstance = bindable;
            INotifyPropertyChanged actualSourceInstance = target.Keys.First();
            Assert.AreSame(expectedSourceInstance, actualSourceInstance);

            ObjectPropertyDependencyRegistration actualObjectPropertyDependencyRegistration =
                target[actualSourceInstance];
            Assert.IsNotNull(actualObjectPropertyDependencyRegistration);
            Assert.IsTrue(actualObjectPropertyDependencyRegistration.Callbacks.Count == 0);
            Assert.IsTrue(actualObjectPropertyDependencyRegistration.PropertyDependencies.Count == 2);

            string expectedSourcePropertyNameA = "InputProp";
            string expectedSourcePropertyNameB = "InputProp2";
            CollectionAssert.Contains(actualObjectPropertyDependencyRegistration.PropertyDependencies.Keys,
                expectedSourcePropertyNameA);
            CollectionAssert.Contains(actualObjectPropertyDependencyRegistration.PropertyDependencies.Keys,
                expectedSourcePropertyNameB);


            PropertyDependencies actualSourcePropertyDependenciesA =
                actualObjectPropertyDependencyRegistration.PropertyDependencies[expectedSourcePropertyNameA];
            Assert.IsNotNull(actualSourcePropertyDependenciesA);
            Assert.IsTrue(actualSourcePropertyDependenciesA.Callbacks.Count == 0);
            Assert.IsTrue(actualSourcePropertyDependenciesA.DependentProperties.Count == 1);

            string expectedDependentPropertyNameA = "DependentProp";
            string actualDependentPropertyNameA = actualSourcePropertyDependenciesA.DependentProperties.First();
            Assert.AreEqual(expectedDependentPropertyNameA, actualDependentPropertyNameA);


            PropertyDependencies actualSourcePropertyDependenciesB =
                actualObjectPropertyDependencyRegistration.PropertyDependencies[expectedSourcePropertyNameB];
            Assert.IsNotNull(actualSourcePropertyDependenciesB);
            Assert.IsTrue(actualSourcePropertyDependenciesB.Callbacks.Count == 0);
            Assert.IsTrue(actualSourcePropertyDependenciesB.DependentProperties.Count == 1);

            string expectedDependentPropertyNameB = "DependentProp2";
            string actualDependentPropertyNameB = actualSourcePropertyDependenciesB.DependentProperties.First();
            Assert.AreEqual(expectedDependentPropertyNameB, actualDependentPropertyNameB);


        }

        [TestMethod]
        public void
            RegisterPropertyDependenciesForTypeInstance_SingleMockMultipleDependenciesAndMultipleInputsOnSelfBindableBaseInstantiated_NotifyPropertyChangedSubscribedOnlyOnce
            ()
        {
            var bindable = new MockMultipleDependenciesAndMultipleInputsOnSelfBindableBase();

            var targetA = PropertyChangeRecorder.CreatePropertyChangeRecorder(bindable, b => b.DependentProp);
            var targetB = PropertyChangeRecorder.CreatePropertyChangeRecorder(bindable, b => b.DependentProp2);

            bindable.InputProp = 1;
            bindable.InputProp2 = 2;

            Assert.IsTrue(targetA.NumberOfChanges == 1);
            Assert.IsTrue(targetB.NumberOfChanges == 1);
        }

        [TestMethod]
        public void
            RegisterPropertyDependenciesForTypeInstance_SingleMockMultipleDependenciesAndMultipleInputsOnSelfBindableBaseInstantiatedAndSingleMockBindableBaseMultipleDependentciesOnMockMultipleDependenciesAndMultipleInputsOnSelfBindableBaseInstantiated_CorrectInstanceDependeciesRegistered
            ()
        {
            var bindable1 = new MockMultipleDependenciesAndMultipleInputsOnSelfBindableBase();
            var bindable2 =
                new MockBindableBaseMultipleDependentciesOnMockMultipleDependenciesAndMultipleInputsOnSelfBindableBase(
                    bindable1);

            Dictionary<INotifyPropertyChanged, ObjectPropertyDependencyRegistration> target1 =
                bindable1._propertyDependencies;
            Assert.IsTrue(target1.Count == 1);

            MockMultipleDependenciesAndMultipleInputsOnSelfBindableBase expectedSourceInstance = bindable1;
            INotifyPropertyChanged actualSourceInstance1 = target1.Keys.First();
            Assert.AreSame(expectedSourceInstance, actualSourceInstance1);

            ObjectPropertyDependencyRegistration actualObjectPropertyDependencyRegistration1 =
                target1[actualSourceInstance1];
            Assert.IsNotNull(actualObjectPropertyDependencyRegistration1);
            Assert.IsTrue(actualObjectPropertyDependencyRegistration1.Callbacks.Count == 0);
            Assert.IsTrue(actualObjectPropertyDependencyRegistration1.PropertyDependencies.Count == 2);

            string expectedSourcePropertyName1A = "InputProp";
            string expectedSourcePropertyName1B = "InputProp2";
            CollectionAssert.Contains(actualObjectPropertyDependencyRegistration1.PropertyDependencies.Keys,
                expectedSourcePropertyName1A);
            CollectionAssert.Contains(actualObjectPropertyDependencyRegistration1.PropertyDependencies.Keys,
                expectedSourcePropertyName1B);


            PropertyDependencies actualSourcePropertyDependencies1A =
                actualObjectPropertyDependencyRegistration1.PropertyDependencies[expectedSourcePropertyName1A];
            Assert.IsNotNull(actualSourcePropertyDependencies1A);
            Assert.IsTrue(actualSourcePropertyDependencies1A.Callbacks.Count == 0);
            Assert.IsTrue(actualSourcePropertyDependencies1A.DependentProperties.Count == 1);

            string expectedDependentPropertyName1A = "DependentProp";
            string actualDependentPropertyName1A = actualSourcePropertyDependencies1A.DependentProperties.First();
            Assert.AreEqual(expectedDependentPropertyName1A, actualDependentPropertyName1A);


            PropertyDependencies actualSourcePropertyDependencies1B =
                actualObjectPropertyDependencyRegistration1.PropertyDependencies[expectedSourcePropertyName1B];
            Assert.IsNotNull(actualSourcePropertyDependencies1B);
            Assert.IsTrue(actualSourcePropertyDependencies1B.Callbacks.Count == 0);
            Assert.IsTrue(actualSourcePropertyDependencies1B.DependentProperties.Count == 1);

            string expectedDependentPropertyName1B = "DependentProp2";
            string actualDependentPropertyName1B = actualSourcePropertyDependencies1B.DependentProperties.First();
            Assert.AreEqual(expectedDependentPropertyName1B, actualDependentPropertyName1B);



            Dictionary<INotifyPropertyChanged, ObjectPropertyDependencyRegistration> target2 =
                bindable2._propertyDependencies;
            Assert.IsTrue(target2.Count == 1);


            INotifyPropertyChanged actualSourceInstance2 = target2.Keys.First();
            Assert.AreSame(expectedSourceInstance, actualSourceInstance2);

            ObjectPropertyDependencyRegistration actualObjectPropertyDependencyRegistration2 =
                target2[actualSourceInstance2];
            Assert.IsNotNull(actualObjectPropertyDependencyRegistration2);
            Assert.IsTrue(actualObjectPropertyDependencyRegistration2.Callbacks.Count == 0);
            Assert.IsTrue(actualObjectPropertyDependencyRegistration2.PropertyDependencies.Count == 2);

            string expectedSourcePropertyName2A = "DependentProp";
            string expectedSourcePropertyName2B = "DependentProp2";
            CollectionAssert.Contains(actualObjectPropertyDependencyRegistration2.PropertyDependencies.Keys,
                expectedSourcePropertyName2A);
            CollectionAssert.Contains(actualObjectPropertyDependencyRegistration2.PropertyDependencies.Keys,
                expectedSourcePropertyName2B);


            PropertyDependencies actualSourcePropertyDependencies2A =
                actualObjectPropertyDependencyRegistration2.PropertyDependencies[expectedSourcePropertyName2A];
            Assert.IsNotNull(actualSourcePropertyDependencies2A);
            Assert.IsTrue(actualSourcePropertyDependencies2A.Callbacks.Count == 0);
            Assert.IsTrue(actualSourcePropertyDependencies2A.DependentProperties.Count == 1);

            string expectedDependentPropertyName2A = "DependentOnBindableBaseProp";
            string actualDependentPropertyName2A = actualSourcePropertyDependencies2A.DependentProperties.First();
            Assert.AreEqual(expectedDependentPropertyName2A, actualDependentPropertyName2A);


            PropertyDependencies actualSourcePropertyDependencies2B =
                actualObjectPropertyDependencyRegistration2.PropertyDependencies[expectedSourcePropertyName2B];
            Assert.IsNotNull(actualSourcePropertyDependencies2B);
            Assert.IsTrue(actualSourcePropertyDependencies2B.Callbacks.Count == 0);
            Assert.IsTrue(actualSourcePropertyDependencies2B.DependentProperties.Count == 1);

            string expectedDependentPropertyName2B = "DependentOnBindableBaseProp2";
            string actualDependentPropertyName2B = actualSourcePropertyDependencies2B.DependentProperties.First();
            Assert.AreEqual(expectedDependentPropertyName2B, actualDependentPropertyName2B);
        }

        [TestMethod]
        public void
            RegisterPropertyDependenciesForTypeInstance_SingleMockMultipleDependenciesAndMultipleInputsOnSelfBindableBaseInstantiatedAndSingleMockBindableBaseMultipleDependentciesOnMockMultipleDependenciesAndMultipleInputsOnSelfBindableBaseInstantiated_NotifyPropertyChangedSubscribedOnlyOncePerInstance
            ()
        {
            var bindable1 = new MockMultipleDependenciesAndMultipleInputsOnSelfBindableBase();
            var bindable2 =
                new MockBindableBaseMultipleDependentciesOnMockMultipleDependenciesAndMultipleInputsOnSelfBindableBase(
                    bindable1);

            var target1A = PropertyChangeRecorder.CreatePropertyChangeRecorder(bindable1, b => b.DependentProp);
            var target1B = PropertyChangeRecorder.CreatePropertyChangeRecorder(bindable1, b => b.DependentProp2);
            var target2A = PropertyChangeRecorder.CreatePropertyChangeRecorder(bindable2,
                b => b.DependentOnBindableBaseProp);
            var target2B = PropertyChangeRecorder.CreatePropertyChangeRecorder(bindable2,
                b => b.DependentOnBindableBaseProp2);

            bindable1.InputProp = 1;
            bindable1.InputProp2 = 2;

            Assert.IsTrue(target1A.NumberOfChanges == 1);
            Assert.IsTrue(target1B.NumberOfChanges == 1);
            Assert.IsTrue(target2A.NumberOfChanges == 1);
            Assert.IsTrue(target2B.NumberOfChanges == 1);
        }

        [TestMethod]
        public void RegisterPropertyDependenciesForTypeInstance_SingleMockBindableBaseSinglePropertyWithTwoDependencies_PropertyChangedReactedToForAndOn()
        {
            var bindable1 = new MockBindableBaseSinglePropertyWithTwoDependencies();

            var target = PropertyChangeRecorder.CreatePropertyChangeRecorder(bindable1, b => b.DependentProp);

            bindable1.Input1 = 1; //On

            if (target.NumberOfChanges != 1)
            {
                Assert.Inconclusive("The '.On' dependency didn't work. Cannot test '.AndOn' dependency.");
            }

            bindable1.Input2 = 2; //AndOn

            Assert.IsTrue(target.NumberOfChanges == 2);
        }

        [TestMethod]
        public void
            TypeRegistration_CollectionChildProperty_MockBindableBaseDfocDependency_CorrectInstanceDependeciesRegisteredPerChild()
        {
            //Dictionary<Type, TypeDependencies> target =
            //                ((TypeRegistrationAPI)BindableBase._typeRegistrationApi).DependenciesByType;

            //var bindable = new MockBindableBaseDfocDependency();

            //Type expectedType = bindable.GetType();
            //Assert.IsTrue(target.ContainsKey(expectedType));

            //TypeDependencies actualTypeDependencies = target[expectedType];
            //Assert.IsNotNull(actualTypeDependencies);

            //Assert.IsTrue(actualTypeDependencies.SourceProviders.Count == 1);

            //SourceProvider actualSourceProvider = actualTypeDependencies.SourceProviders[expectedType];
            //Assert.IsNotNull(actualSourceProvider);

            //Assert.IsTrue(actualSourceProvider.SourceProperties.Count == 1);

            //MockBindableBaseDfocDependency expectedSource = bindable;
            //INotifyPropertyChanged actualSource = actualSourceProvider.SourceRetrievalFunc(bindable);
            //Assert.AreSame(expectedSource, actualSource);

            //string expctedSourceProperty = "InputProp";
            //SourceProperty actualSourceProperty = actualSourceProvider.SourceProperties[expctedSourceProperty];
            //Assert.IsNotNull(actualSourceProperty);
            //Assert.AreEqual(expctedSourceProperty, actualSourceProperty.Name);

            //Assert.IsTrue(actualSourceProperty.DependentPropertyNames.Count == 1);

            //string expectedDependentPropertyName = "DependentProp";
            //Assert.AreEqual(expectedDependentPropertyName, actualSourceProperty.DependentPropertyNames[0]);

            Assert.Fail();
        }

        [TestMethod]
        public void
            TypeRegistration_CollectionChildProperty_MockBindableBaseDfocDependency_NotifyPropertyChangedSubscribedOnlyOncePerChild()
        {
            Assert.Fail();

        }

        [TestMethod]
        public void
            TypeRegistration_CollectionChildProperty_MockBindableBaseINotifyCollectionChangedDependency_CorrectInstanceDependeciesRegisteredPerChild()
        {
            Assert.Fail();

        }

        [TestMethod]
        public void
            TypeRegistration_CollectionChildProperty_MockBindableBaseINotifyCollectionChangedDependency_NotifyPropertyChangedSubscribedOnlyOncePerChild()
        {
            Assert.Fail();

        }

    }

    public class MockBindableBaseDfocDependency : BindableBase
    {
        private readonly int _numberOfMockInputObjectsToCreate = 3;

        public MockBindableBaseDfocDependency()
        {
            InputCollection = new DependencyFrameworkObservableCollection<MockInputObject>();

            for (int i = 0; i < _numberOfMockInputObjectsToCreate; i++)
            {
                InputCollection.Add(new MockInputObject());
            }

            InitializePropertyDependencies(
                new Action[]
                {
                    RegisterDependentPropOnCollectionChildren,
                });
        }

        public void RegisterDependentPropOnCollectionChildren()
        {
            TypeRegistrationProperty(() => DependentPropOnCollectionChildren)
                .Depends(p => p.OnCollectionChildProperty(this, sourceOwner => sourceOwner.InputCollection, i => i.InputProp1));
        }

        public decimal DependentPropOnCollectionChildren
        {
            get
            {
                return InputCollection.Sum(i => i.InputProp1);
            }
        }


        private DependencyFrameworkObservableCollection<MockInputObject> _inputCollection;
        public DependencyFrameworkObservableCollection<MockInputObject> InputCollection
        {
            get { return _inputCollection; }
            set
            {
                if (value == _inputCollection)
                    return;

                _inputCollection = value;
                NotifyPropertyChanged(() => InputCollection);
            }
        }

        public MockInputObject AddToCollection()
        {
            var newMockInputObject = new MockInputObject();
            InputCollection.Add(newMockInputObject);
            return newMockInputObject;
        }

        public bool RemoveFromCollection(MockInputObject objectToRemove)
        {
            return InputCollection.Remove(objectToRemove);
        }
    }

    public class MockBindableBaseINotifyCollectionChangedDependency : BindableBase
    {
        private readonly int _numberOfMockInputObjectsToCreate = 3;

        public MockBindableBaseINotifyCollectionChangedDependency()
        {
            InputCollection = new ObservableCollection<MockInputObject>();

            for (int i = 0; i < _numberOfMockInputObjectsToCreate; i++)
            {
                InputCollection.Add(new MockInputObject());
            }

            InitializePropertyDependencies(
                new Action[]
                {
                    RegisterDependentPropOnCollectionChildren,
                });
        }

        public void RegisterDependentPropOnCollectionChildren()
        {
            TypeRegistrationProperty(() => DependentPropOnCollectionChildren)
                .Depends(p => p.OnCollectionChildProperty<MockBindableBaseINotifyCollectionChangedDependency, MockInputObject, decimal>(
                    this, sourceOwner => sourceOwner.InputCollection, i => i.InputProp1));
        }

        public decimal DependentPropOnCollectionChildren
        {
            get
            {
                return InputCollection.Sum(i => i.InputProp1);
            }
        }

        private ObservableCollection<MockInputObject> _inputCollection;
        public ObservableCollection<MockInputObject> InputCollection
        {
            get { return _inputCollection; }
            set
            {
                if (value == _inputCollection)
                    return;

                _inputCollection = value;
                NotifyPropertyChanged(() => InputCollection);
            }
        }

        public MockInputObject AddToCollection()
        {
            var newMockInputObject = new MockInputObject();
            InputCollection.Add(newMockInputObject);
            return newMockInputObject;
        }
        public bool RemoveFromCollection(MockInputObject objectToRemove)
        {
            return InputCollection.Remove(objectToRemove);
        }

    }

    public class MockInputObject : BindableBase
    {
        private decimal _inputProp1;
        public decimal InputProp1
        {
            get { return _inputProp1; }
            set
            {
                if (value == _inputProp1)
                    return;

                _inputProp1 = value;
                NotifyPropertyChanged(() => InputProp1);
            }
        }

        private decimal _inputProp2;
        public decimal InputProp2
        {
            get { return _inputProp2; }
            set
            {
                if (value == _inputProp2)
                    return;

                _inputProp2 = value;
                NotifyPropertyChanged(() => InputProp2);
            }
        }
    }

    #region MockBindableBase Classes

    public class SimpleMockBindableBase : BindableBase
    {
        public SimpleMockBindableBase()
        {
            InitializePropertyDependencies(
                new Action[] { () => ActionCount++ });
        }

        public int ActionCount { get; set; }
    }

    public class MockBindableBase : BindableBase
    {
        public MockBindableBase()
        {
            InitializePropertyDependencies(
                new Action[]
                {
                    RegisterDependentProp,
                });
        }

        //NOTE: Number of registrations in this class matters for some tests.
        private void RegisterDependentProp()
        {
            TypeRegistrationProperty(() => DependentProp)
                .Depends(p => p.On(this, k => k, () => InputProp));
        }

        public decimal DependentProp
        {
            get
            {
                //NOT TESTING CACHING
                return this.InputProp;
            }
        }


        private decimal _inputProp;

        public decimal InputProp
        {
            get { return _inputProp; }
            set
            {
                if (value == _inputProp)
                    return;

                _inputProp = value;
                NotifyPropertyChanged(() => InputProp);
            }
        }
    }

    public class MockBindableBaseDependentOnMockBindableBase : BindableBase
    {
        public MockBindableBaseDependentOnMockBindableBase(MockBindableBase inputBindableBase)
        {
            InputBindableBase = inputBindableBase;

            InitializePropertyDependencies(
                new Action[]
                {
                    RegisterDependentProp,
                });
        }

        //NOTE: Number of registrations in this class matters for some tests.
        private void RegisterDependentProp()
        {
            TypeRegistrationProperty(() => DependentOnBindableBaseProp)
                .Depends(p => p.On(this, k => k.InputBindableBase, () => InputBindableBase.DependentProp));
        }

        public decimal DependentOnBindableBaseProp
        {
            get
            {
                //NOT TESTING CACHING
                return InputBindableBase.DependentProp;
            }
        }

        private MockBindableBase _inputBindableBase;

        public MockBindableBase InputBindableBase
        {
            get { return _inputBindableBase; }
            private set
            {
                if (value == _inputBindableBase)
                    return;

                _inputBindableBase = value;
                NotifyPropertyChanged(() => InputBindableBase);
            }
        }
    }

    public class MockMultipleDependenciesOnSelfBindableBase : BindableBase
    {
        public MockMultipleDependenciesOnSelfBindableBase()
        {
            InitializePropertyDependencies(
                new Action[]
                {
                    RegisterDependentProp,
                    RegisterDependentProp2,
                });

        }

        //NOTE: Number of registrations in this class matters for some tests.
        private void RegisterDependentProp()
        {
            TypeRegistrationProperty(() => DependentProp)
                .Depends(p => p.On(this, bindableBase => bindableBase, () => InputProp));
        }

        public decimal DependentProp
        {
            get
            {
                //NOT TESTING CACHING
                return InputProp;
            }
        }

        private void RegisterDependentProp2()
        {
            TypeRegistrationProperty(() => DependentProp2)
                .Depends(p => p.On(this, bindableBase => bindableBase, () => InputProp));

        }

        public decimal DependentProp2
        {
            get
            {
                return InputProp + 1;
            }
        }


        private decimal _inputProp;

        public decimal InputProp
        {
            get { return _inputProp; }
            set
            {
                if (value == _inputProp)
                    return;

                _inputProp = value;
                NotifyPropertyChanged(() => InputProp);
            }
        }
    }

    public class MockBindableBaseMultipleDependentciesOnMockMultipleDependenciesOnSelfBindableBase : BindableBase
    {
        public MockBindableBaseMultipleDependentciesOnMockMultipleDependenciesOnSelfBindableBase(
            MockMultipleDependenciesOnSelfBindableBase inputBindableBase)
        {
            InputBindableBase = inputBindableBase;

            InitializePropertyDependencies(
                new Action[]
                {
                    RegisterDependentOnBindableBaseProp,
                    RegisterDependentOnBindableBaseProp2
                });

        }

        //NOTE: Number of registrations in this class matters for some tests.
        private void RegisterDependentOnBindableBaseProp()
        {
            TypeRegistrationProperty(() => DependentOnBindableBaseProp)
                .Depends(
                    p =>
                        p.On(this, bindableBase => bindableBase.InputBindableBase, () => InputBindableBase.DependentProp));
        }

        public decimal DependentOnBindableBaseProp
        {
            get
            {
                //NOT TESTING CACHING
                return InputBindableBase.DependentProp;
            }
        }

        private void RegisterDependentOnBindableBaseProp2()
        {
            TypeRegistrationProperty(() => DependentOnBindableBaseProp2)
                .Depends(
                    p =>
                        p.On(this, bindableBase => bindableBase.InputBindableBase, () => InputBindableBase.DependentProp));
        }

        public decimal DependentOnBindableBaseProp2
        {
            get
            {
                //NOT TESTING CACHING
                return InputBindableBase.DependentProp + 1;
            }
        }


        private MockMultipleDependenciesOnSelfBindableBase _inputBindableBase;

        public MockMultipleDependenciesOnSelfBindableBase InputBindableBase
        {
            get { return _inputBindableBase; }
            private set
            {
                if (value == _inputBindableBase)
                    return;

                _inputBindableBase = value;
                NotifyPropertyChanged(() => InputBindableBase);
            }
        }
    }

    public class MockMultipleDependenciesAndMultipleInputsOnSelfBindableBase : BindableBase
    {
        public MockMultipleDependenciesAndMultipleInputsOnSelfBindableBase()
        {
            InitializePropertyDependencies(
                new Action[]
                {
                    RegisterDependentProp,
                    RegisterDependentProp2
                });

        }

        //NOTE: Number of registrations in this class matters for some tests.
        private void RegisterDependentProp()
        {
            TypeRegistrationProperty(() => DependentProp)
                .Depends(p => p.On(this, bindableBase => bindableBase, () => InputProp));
        }

        public decimal DependentProp
        {
            get
            {
                //NOT TESTING CACHING
                return InputProp;
            }
        }

        private void RegisterDependentProp2()
        {
            TypeRegistrationProperty(() => DependentProp2)
                .Depends(p => p.On(this, bindableBase => bindableBase, () => InputProp2));

        }

        public decimal DependentProp2
        {
            get
            {
                return InputProp2 + 1;
            }
        }


        private decimal _inputProp;

        public decimal InputProp
        {
            get { return _inputProp; }
            set
            {
                if (value == _inputProp)
                    return;

                _inputProp = value;
                NotifyPropertyChanged(() => InputProp);
            }
        }

        private decimal _inputProp2;

        public decimal InputProp2
        {
            get { return _inputProp2; }
            set
            {
                if (value == _inputProp2)
                    return;

                _inputProp2 = value;
                NotifyPropertyChanged(() => InputProp2);
            }
        }
    }

    public class MockBindableBaseMultipleDependentciesOnMockMultipleDependenciesAndMultipleInputsOnSelfBindableBase :
        BindableBase
    {
        public MockBindableBaseMultipleDependentciesOnMockMultipleDependenciesAndMultipleInputsOnSelfBindableBase(
            MockMultipleDependenciesAndMultipleInputsOnSelfBindableBase inputBindableBase)
        {
            InputBindableBase = inputBindableBase;

            InitializePropertyDependencies(
                new Action[]
                {
                    RegisterDependentOnBindableBaseProp,
                    RegisterDependentOnBindableBaseProp2
                });

        }

        //NOTE: Number of registrations in this class matters for some tests.
        private void RegisterDependentOnBindableBaseProp()
        {
            TypeRegistrationProperty(() => DependentOnBindableBaseProp)
                .Depends(
                    p =>
                        p.On(this, bindableBase => bindableBase.InputBindableBase, () => InputBindableBase.DependentProp));
        }

        public decimal DependentOnBindableBaseProp
        {
            get
            {
                //NOT TESTING CACHING
                return InputBindableBase.DependentProp;
            }
        }

        private void RegisterDependentOnBindableBaseProp2()
        {
            TypeRegistrationProperty(() => DependentOnBindableBaseProp2)
                .Depends(
                    p =>
                        p.On(this, bindableBase => bindableBase.InputBindableBase,
                            () => InputBindableBase.DependentProp2));
        }

        public decimal DependentOnBindableBaseProp2
        {
            get
            {
                //NOT TESTING CACHING
                return InputBindableBase.DependentProp2 + 1;
            }
        }


        private MockMultipleDependenciesAndMultipleInputsOnSelfBindableBase _inputBindableBase;

        public MockMultipleDependenciesAndMultipleInputsOnSelfBindableBase InputBindableBase
        {
            get { return _inputBindableBase; }
            private set
            {
                if (value == _inputBindableBase)
                    return;

                _inputBindableBase = value;
                NotifyPropertyChanged(() => InputBindableBase);
            }
        }
    }

    public class MockBindableBaseSinglePropertyWithTwoDependencies : BindableBase
    {
        public MockBindableBaseSinglePropertyWithTwoDependencies()
        {
            InitializePropertyDependencies(
                new Action[]
                {
                    RegisterDependentProp,
                });
        }

        public void RegisterDependentProp()
        {
            TypeRegistrationProperty(() => DependentProp)
                .Depends(p => p.On(this, sourceOwner => sourceOwner, () => Input1)
                               .AndOn(this, sourceOwner => sourceOwner, () => Input2));
        }

        public decimal DependentProp
        {
            get
            {
                return Input1 + Input2;
            }
        }

        private decimal _input1;
        public decimal Input1
        {
            get { return _input1; }
            set
            {
                if (value == _input1)
                    return;

                _input1 = value;
                NotifyPropertyChanged(() => Input1);
            }
        }

        private decimal _input2;
        public decimal Input2
        {
            get { return _input2; }
            set
            {
                if (value == _input2)
                    return;

                _input2 = value;
                NotifyPropertyChanged(() => Input2);
            }
        }
    }

    #endregion
}