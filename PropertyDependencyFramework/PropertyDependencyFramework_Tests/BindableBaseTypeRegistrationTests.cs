using System;
using System.Collections.Generic;
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
            const int expected = 1;

            Assert.AreEqual(expected, target.ActionCount);
        }

        [TestMethod]
        public void Constructor_RegistersDerivedType()
        {
            var target = new SimpleMockBindableBase();

            //Act is performed during Instantiation of target

            Type expected = typeof(SimpleMockBindableBase);

            CollectionAssert.Contains(BindableBase.RegisteredTypes.ToArray(), expected);
        }

        [TestMethod]
        public void TypeRegistration_DependentPropertyTypeRegistrationImplementation_CreatedForTypeProperties()
        {
            Dictionary<Type, Dictionary<string, DependentPropertyTypeRegistrationImplementation>> target = BindableBase._typeRegistrationProperties;

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

            int expectedCount = 1;
            Assert.IsTrue(actual.Keys.Count == expectedCount);

            string expectedDependentPropertyName = "DependentProp";
            Assert.IsTrue(actual.ContainsKey(expectedDependentPropertyName));

            DependentPropertyTypeRegistrationImplementation actualDependentPropertyTypeRegistrationImplementation = actual[expectedDependentPropertyName];
            Assert.IsNotNull(actualDependentPropertyTypeRegistrationImplementation);
        }

        [TestMethod]
        public void TypeRegistration_PropertyDependencyGraph_CreatedForTypes()
        {
            Dictionary<Type, TypeDependencies> target = ((TypeRegistrationAPI)BindableBase._typeRegistrationApi)._dependenciesByType;

            var bindable = new MockBindableBase();

            Type expectedType = typeof(MockBindableBase);
            Assert.IsTrue(target.ContainsKey(expectedType));

            TypeDependencies actualTypeDependencies = target[expectedType];
            Assert.IsNotNull(actualTypeDependencies);

            int expectedSourceProvidersCount = 1;
            Assert.IsTrue(actualTypeDependencies.SourceProviders.Count == expectedSourceProvidersCount);

            SourceProvider actualSourceProvider = actualTypeDependencies.SourceProviders[expectedType];
            Assert.IsNotNull(actualSourceProvider);

            int expectedSourcePropertiesCount = 1;
            Assert.IsTrue(actualSourceProvider.SourceProperties.Count == expectedSourcePropertiesCount);

            MockBindableBase expectedSource = bindable;
            INotifyPropertyChanged actualSource = actualSourceProvider.SourceRetrievalFunc(bindable);
            Assert.AreSame(expectedSource, actualSource);

            string expctedSourceProperty = "InputProp";
            SourceProperty actualSourceProperty = actualSourceProvider.SourceProperties[expctedSourceProperty];
            Assert.IsNotNull(actualSourceProperty);
            Assert.AreEqual(expctedSourceProperty, actualSourceProperty.Name);

            int expectedDependentPropertiesCount = 1;
            Assert.IsTrue(actualSourceProperty.DependentProperties.Count == expectedDependentPropertiesCount);

            string expectedDependentPropertyName = "DependentProp";
            Assert.AreEqual(expectedDependentPropertyName, actualSourceProperty.DependentProperties[0]);
        }

        [TestMethod]
        public void TypeRegistration_MultipleOfSameType_PropertyDependencyGraph_CreatedForTypes()
        {
            Dictionary<Type, TypeDependencies> target = ((TypeRegistrationAPI)BindableBase._typeRegistrationApi)._dependenciesByType;

            var bindable1 = new MockBindableBase();
            var bindable2 = new MockBindableBase();

            Type expectedType = typeof(MockBindableBase);
            Assert.IsTrue(target.ContainsKey(expectedType));

            TypeDependencies actualTypeDependencies = target[expectedType];
            Assert.IsNotNull(actualTypeDependencies);

            int expectedSourceProvidersCount = 1;
            Assert.IsTrue(actualTypeDependencies.SourceProviders.Count == expectedSourceProvidersCount);

            SourceProvider actualSourceProvider = actualTypeDependencies.SourceProviders[expectedType];
            Assert.IsNotNull(actualSourceProvider);

            int expectedSourcePropertiesCount = 1;
            Assert.IsTrue(actualSourceProvider.SourceProperties.Count == expectedSourcePropertiesCount);

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

            int expectedDependentPropertiesCount = 1;
            Assert.IsTrue(actualSourceProperty.DependentProperties.Count == expectedDependentPropertiesCount);

            string expectedDependentPropertyName = "DependentProp";
            Assert.AreEqual(expectedDependentPropertyName, actualSourceProperty.DependentProperties[0]);
        }

        [TestMethod]
        public void TypeRegistration_TwoTypes_OneDependentOnTheOther_PropertyDependencyGraph_CreatedForTypes()
        {
            Assert.Fail();
        }

    }

    public class SimpleMockBindableBase : BindableBase
    {
        public int ActionCount { get; set; }

        protected override Action[] GetPropertyRegistrations()
        {
            return new Action[] { () => ActionCount++ };
        }
    }

    public class MockBindableBase : BindableBase
    {
        //NOTE: Number of registrations in this class matters for some tests.
        private void RegisterDependentProp()
        {
            TypeRegistrationProperty(GetType(), () => DependentProp)
                .Depends(p => p.On<MockBindableBase, MockBindableBase, decimal>(bindableBase => bindableBase, () => InputProp));
        }
        public decimal DependentProp
        {
            get
            {
                //NOT TESTING CACHING
                return InputProp;
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

        protected override Action[] GetPropertyRegistrations()
        {
            return new Action[]
            {
                RegisterDependentProp
            };
        }
    }

}