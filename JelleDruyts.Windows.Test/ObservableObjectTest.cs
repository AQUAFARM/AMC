using System;
using System.Collections.Generic;
using System.Linq;
using JelleDruyts.Windows.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JelleDruyts.Windows.Test
{
    [TestClass]
    public class ObservableObjectTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ObservableObjectShouldThrowWhenGettingPropertyWithNullName()
        {
            var sut = new MyObservableObject();
            sut.GetPropertyWithNullProperty();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ObservableObjectShouldThrowWhenSettingPropertyWithNullName()
        {
            var sut = new MyObservableObject();
            sut.SetPropertyWithNullProperty();
        }

        [TestMethod]
        public void ObservableObjectShouldReturnPropertyValues()
        {
            var sut = new MyObservableObject();
            Assert.IsFalse(sut.MyFlag);
            Assert.IsNull(sut.MyMessage);
            Assert.AreEqual<bool>(MyObservableObject.MyFlagWithDefaultProperty.DefaultValue, sut.MyFlagWithDefault);
            Assert.AreEqual<string>(MyObservableObject.MyMessageWithDefaultProperty.DefaultValue, sut.MyMessageWithDefault);

            sut.MyFlag = true;
            Assert.IsTrue(sut.MyFlag);
            sut.MyMessage = "Test";
            sut.MyFlagWithDefault = false;
            sut.MyMessageWithDefault = "New message";

            Assert.IsTrue(sut.MyFlag);
            Assert.AreEqual<string>(sut.MyMessage, "Test");
            Assert.IsFalse(sut.MyFlagWithDefault);
            Assert.AreEqual<string>(sut.MyMessageWithDefault, "New message");
        }

        [TestMethod]
        public void ObservableObjectShouldRaisePropertyChanged()
        {
            var sut = new MyObservableObject();
            var listener = new PropertyChangedListener(sut);

            sut.MyFlag = false;
            Assert.AreEqual<int>(0, listener.RaisedPropertyChangedEventArgs.Count);
            Assert.AreEqual<int>(0, listener.RaisedObservablePropertyChangedEventArgs.Count);
            sut.MyFlag = true;
            Assert.AreEqual<int>(1, listener.RaisedPropertyChangedEventArgs.Count);
            Assert.AreEqual<int>(1, listener.RaisedObservablePropertyChangedEventArgs.Count);
            Assert.AreEqual<string>("MyFlag", listener.RaisedPropertyChangedEventArgs[0].PropertyName);
            Assert.AreEqual<ObservableProperty>(MyObservableObject.MyFlagProperty, listener.RaisedObservablePropertyChangedEventArgs[0].Property);
        }

        [TestMethod]
        public void ObservableObjectShouldNotRaisePropertyChangedWhenSettingToDefaultValue()
        {
            var sut = new MyObservableObject();
            var listener = new PropertyChangedListener(sut);

            sut.MyFlag = false;
            sut.MyFlagWithDefault = MyObservableObject.MyFlagWithDefaultProperty.DefaultValue;
            sut.MyMessage = null;
            sut.MyMessageWithDefault = MyObservableObject.MyMessageWithDefaultProperty.DefaultValue;
            Assert.AreEqual<int>(0, listener.RaisedPropertyChangedEventArgs.Count);
            Assert.AreEqual<int>(0, listener.RaisedObservablePropertyChangedEventArgs.Count);

            // Set the same values again to make sure no event is raised.
            sut.MyFlag = false;
            sut.MyFlagWithDefault = MyObservableObject.MyFlagWithDefaultProperty.DefaultValue;
            sut.MyMessage = null;
            sut.MyMessageWithDefault = MyObservableObject.MyMessageWithDefaultProperty.DefaultValue;
            Assert.AreEqual<int>(0, listener.RaisedPropertyChangedEventArgs.Count);
            Assert.AreEqual<int>(0, listener.RaisedObservablePropertyChangedEventArgs.Count);
        }

        [TestMethod]
        public void ObservableObjectShouldRaisePropertyChangedWhenSettingToNonDefaultValue()
        {
            var sut = new MyObservableObject();
            var listener = new PropertyChangedListener(sut);

            // Set non-default values.
            sut.MyFlag = true;
            sut.MyFlagWithDefault = false;
            sut.MyMessage = "Test";
            sut.MyMessageWithDefault = "New message";
            Assert.AreEqual<int>(4, listener.RaisedPropertyChangedEventArgs.Count);
            Assert.AreEqual<int>(4, listener.RaisedObservablePropertyChangedEventArgs.Count);
            Assert.AreEqual<string>("MyFlag", listener.RaisedPropertyChangedEventArgs[0].PropertyName);
            Assert.AreEqual<string>("MyFlagWithDefault", listener.RaisedPropertyChangedEventArgs[1].PropertyName);
            Assert.AreEqual<string>("MyMessage", listener.RaisedPropertyChangedEventArgs[2].PropertyName);
            Assert.AreEqual<string>("MyMessageWithDefault", listener.RaisedPropertyChangedEventArgs[3].PropertyName);
            Assert.AreEqual<ObservableProperty>(MyObservableObject.MyFlagProperty, listener.RaisedObservablePropertyChangedEventArgs[0].Property);
            Assert.AreEqual<ObservableProperty>(MyObservableObject.MyFlagWithDefaultProperty, listener.RaisedObservablePropertyChangedEventArgs[1].Property);
            Assert.AreEqual<ObservableProperty>(MyObservableObject.MyMessageProperty, listener.RaisedObservablePropertyChangedEventArgs[2].Property);
            Assert.AreEqual<ObservableProperty>(MyObservableObject.MyMessageWithDefaultProperty, listener.RaisedObservablePropertyChangedEventArgs[3].Property);

            // Set the same values again to make sure no event is raised.
            sut.MyFlag = true;
            sut.MyFlagWithDefault = false;
            sut.MyMessage = "Test";
            sut.MyMessageWithDefault = "New message";
            Assert.AreEqual<int>(4, listener.RaisedPropertyChangedEventArgs.Count);
            Assert.AreEqual<int>(4, listener.RaisedObservablePropertyChangedEventArgs.Count);

            // Set new values again to make sure event is raised.
            sut.MyFlag = false;
            sut.MyFlagWithDefault = true;
            sut.MyMessage = "Test 2";
            sut.MyMessageWithDefault = "New message 2";
            Assert.AreEqual<int>(8, listener.RaisedPropertyChangedEventArgs.Count);
            Assert.AreEqual<int>(8, listener.RaisedObservablePropertyChangedEventArgs.Count);
            Assert.AreEqual<string>("MyFlag", listener.RaisedPropertyChangedEventArgs[4].PropertyName);
            Assert.AreEqual<string>("MyFlagWithDefault", listener.RaisedPropertyChangedEventArgs[5].PropertyName);
            Assert.AreEqual<string>("MyMessage", listener.RaisedPropertyChangedEventArgs[6].PropertyName);
            Assert.AreEqual<string>("MyMessageWithDefault", listener.RaisedPropertyChangedEventArgs[7].PropertyName);
            Assert.AreEqual<ObservableProperty>(MyObservableObject.MyFlagProperty, listener.RaisedObservablePropertyChangedEventArgs[4].Property);
            Assert.AreEqual<ObservableProperty>(MyObservableObject.MyFlagWithDefaultProperty, listener.RaisedObservablePropertyChangedEventArgs[5].Property);
            Assert.AreEqual<ObservableProperty>(MyObservableObject.MyMessageProperty, listener.RaisedObservablePropertyChangedEventArgs[6].Property);
            Assert.AreEqual<ObservableProperty>(MyObservableObject.MyMessageWithDefaultProperty, listener.RaisedObservablePropertyChangedEventArgs[7].Property);
        }

        [TestMethod]
        public void ObservableObjectShouldCallObservablePropertyChangedCallback()
        {
            var sut = new MyObservableObject();
            Assert.AreEqual<int>(0, sut.RaisedObservablePropertyChangedEventArgs.Count);
            sut.MyMessage = "New message";
            Assert.AreEqual<int>(1, sut.RaisedObservablePropertyChangedEventArgs.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ObservablePropertyShouldThrowOnMethodLambda()
        {
            var prop = new ObservableProperty<string, MyObservableObject>(o => o.SomeMethod());
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ObservablePropertyShouldThrowOnFieldLambda()
        {
            var prop = new ObservableProperty<string, MyObservableObject>(o => o.SomeNonObservableField);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ObservablePropertyShouldThrowOnWrongTypeLambda()
        {
            var prop = new ObservableProperty<string, MyObservableObject>(o => o.GetType().Name);
        }

        [TestMethod]
        public void ObservableObjectCloneShouldBeShallowClone()
        {
            var someExceptionReference = new ApplicationException("Some Exception Reference");
            var original = new MyObservableObject()
            {
                MyMessage = "Should be cloned",
                SomeNonObservableProperty = "Should not be cloned",
                SomeNonObservableField = "Should not be cloned",
                MyExceptionReference = someExceptionReference
            };
            var clone = original.Clone<MyObservableObject>();

            Assert.IsNotNull(clone);
            Assert.AreNotSame(original, clone);
            Assert.AreSame(original.MyMessage, clone.MyMessage);
            Assert.AreSame(original.MyExceptionReference, clone.MyExceptionReference);
            Assert.AreNotEqual<string>(original.SomeNonObservableField, clone.SomeNonObservableField);
            Assert.AreNotEqual<string>(original.SomeNonObservableProperty, clone.SomeNonObservableProperty);
        }

        [TestMethod]
        public void ObservableObjectMergeShouldEqualClone()
        {
            var someExceptionReference = new ApplicationException("Some Exception Reference");
            var original = new MyObservableObject()
            {
                MyMessage = "Original",
                MyExceptionReference = null,
                MyMessageWithDefault = "Original",
                MyFlag = true,
                SomeNonObservableField = "Original",
                SomeNonObservableProperty = "Original",
            };
            original.RaisedObservablePropertyChangedEventArgs.Clear();
            var copy = new MyObservableObject()
            {
                MyMessage = "Copy",
                MyExceptionReference = someExceptionReference,
                SomeNonObservableField = "Copy",
                SomeNonObservableProperty = "Copy",
            };
            copy.RaisedObservablePropertyChangedEventArgs.Clear();

            // Make the copy.
            original.CopyFrom(copy);

            // The copy should not have changed.
            Assert.AreEqual<int>(0, copy.RaisedObservablePropertyChangedEventArgs.Count);

            // The original should have changed 4 times:
            // - 2 times to reset existing properties to their default values (MyMessageWithDefault and MyFlag)
            // - 2 times to copy not yet existing properties (MyMessage and MyExceptionReference)
            Assert.AreEqual<int>(4, original.RaisedObservablePropertyChangedEventArgs.Count);
            Assert.AreEqual<int>(1, original.RaisedObservablePropertyChangedEventArgs.Count(e => e.Property == MyObservableObject.MyMessageWithDefaultProperty));
            Assert.AreEqual<int>(1, original.RaisedObservablePropertyChangedEventArgs.Count(e => e.Property == MyObservableObject.MyFlagProperty));
            Assert.AreEqual<int>(1, original.RaisedObservablePropertyChangedEventArgs.Count(e => e.Property == MyObservableObject.MyMessageProperty));
            Assert.AreEqual<int>(1, original.RaisedObservablePropertyChangedEventArgs.Count(e => e.Property == MyObservableObject.MyExceptionReferenceProperty));

            // The original should be equal to the copy except for the non-observable properties.
            Assert.AreSame(copy.MyMessage, original.MyMessage);
            Assert.AreSame(copy.MyMessageWithDefault, original.MyMessageWithDefault);
            Assert.AreEqual<bool>(copy.MyFlag, original.MyFlag);
            Assert.AreEqual<bool>(copy.MyFlagWithDefault, original.MyFlagWithDefault);
            Assert.AreSame(copy.MyExceptionReference, original.MyExceptionReference);
            Assert.AreNotEqual<string>(copy.SomeNonObservableField, original.SomeNonObservableField);
            Assert.AreEqual<string>("Original", original.SomeNonObservableField);
            Assert.AreNotEqual<string>(copy.SomeNonObservableProperty, original.SomeNonObservableProperty);
            Assert.AreEqual<string>("Original", original.SomeNonObservableProperty);
        }

        private class MyObservableObject : ObservableObject
        {
            public IList<ObservablePropertyChangedEventArgs> RaisedObservablePropertyChangedEventArgs { get; private set; }

            public MyObservableObject()
            {
                this.RaisedObservablePropertyChangedEventArgs = new List<ObservablePropertyChangedEventArgs>();
                this.ObservablePropertyChanged += (sender, e) => { this.RaisedObservablePropertyChangedEventArgs.Add(e); };
            }

            public bool MyFlag
            {
                get { return this.GetValue(MyFlagProperty); }
                set { this.SetValue(MyFlagProperty, value); }
            }

            public static ObservableProperty<bool> MyFlagProperty = new ObservableProperty<bool, MyObservableObject>(o => o.MyFlag);

            public bool MyFlagWithDefault
            {
                get { return this.GetValue<bool>(MyFlagWithDefaultProperty); }
                set { this.SetValue<bool>(MyFlagWithDefaultProperty, value); }
            }

            public static ObservableProperty<bool> MyFlagWithDefaultProperty = new ObservableProperty<bool, MyObservableObject>(o => o.MyFlagWithDefault, true);

            public string MyMessage
            {
                get { return this.GetValue<string>(MyMessageProperty); }
                set { this.SetValue(MyMessageProperty, value); }
            }

            public static ObservableProperty<string> MyMessageProperty = new ObservableProperty<string, MyObservableObject>(o => o.MyMessage);

            public string MyMessageWithDefault
            {
                get { return this.GetValue<string>(MyMessageWithDefaultProperty); }
                set { this.SetValue(MyMessageWithDefaultProperty, value); }
            }

            public static ObservableProperty<string> MyMessageWithDefaultProperty = new ObservableProperty<string, MyObservableObject>(o => o.MyMessageWithDefault, "Default message");

            public Exception MyExceptionReference
            {
                get { return this.GetValue(MyExceptionReferenceProperty); }
                set { this.SetValue(MyExceptionReferenceProperty, value); }
            }

            public static ObservableProperty<Exception> MyExceptionReferenceProperty = new ObservableProperty<Exception, MyObservableObject>(o => o.MyExceptionReference);

            public void GetPropertyWithNullProperty()
            {
                this.GetValue<string>(null);
            }

            public void SetPropertyWithNullProperty()
            {
                this.SetValue<string>(null, "dummy");
            }

            public string SomeMethod()
            {
                return null;
            }

            public string SomeNonObservableField = "Some Value";
            public string SomeNonObservableProperty { get; set; }
        }
    }
}