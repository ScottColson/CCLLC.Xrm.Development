using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CCLLC.Core;

namespace IoCContainerTests
{
    [TestClass]
    public class Container_Implementation_Resolution_Tests
    {
        [TestMethod]
        public void Resolve_implementations()
        {
            IIocContainer container = new IocContainer();
            container.Register<ITestService1, TestService1A>();
            container.Register<ITestService2, TestService2A>();

            var s1 = container.Resolve<ITestService1>();
            Assert.IsInstanceOfType(s1, typeof(TestService1A));
            Assert.AreEqual("1A", s1.Name);

            var s2 = container.Resolve<ITestService1>();
            Assert.IsInstanceOfType(s2, typeof(TestService1A));
            Assert.AreEqual("1A", s2.Name);

            Assert.AreNotSame(s1, s2);

            //These services have a constructor that is depending on ITestService1 and that dependency
            //should automatically get resolved.
            var s3 = container.Resolve<ITestService2>();
            Assert.IsInstanceOfType(s3, typeof(TestService2A));
            Assert.IsNotNull(s3.NestedService);
            Assert.IsInstanceOfType(s3.NestedService, typeof(TestService1A));
            Assert.AreEqual("2A", s3.Name);

            var s4 = container.Resolve<ITestService2>();
            Assert.IsInstanceOfType(s4, typeof(TestService2A));
            Assert.IsNotNull(s4.NestedService);
            Assert.IsInstanceOfType(s4.NestedService, typeof(TestService1A));
            Assert.AreEqual("2A", s4.Name);

            Assert.AreNotSame(s3, s4);
            Assert.AreNotSame(s3.NestedService, s4.NestedService);
        }

        [TestMethod]
        public void Resolve_fluent_implementations()
        {
            IIocContainer container = new IocContainer();
            container.Implement<ITestService1>().Using<TestService1A>();
            container.Implement<ITestService1>().Using<TestService1B>();  //this registration should be ignored because it was not an overwrite.
            container.Implement<ITestService2>().Using<TestService2A>();            

            var s1 = container.Resolve<ITestService1>();
            Assert.IsInstanceOfType(s1, typeof(TestService1A));
            Assert.AreEqual("1A", s1.Name);

            var s2 = container.Resolve<ITestService1>();
            Assert.IsInstanceOfType(s2, typeof(TestService1A));
            Assert.AreEqual("1A", s2.Name);

            Assert.AreNotSame(s1, s2);

            //These services have a constructor that is depending on ITestService1 and that dependency
            //should automatically get resolved.
            var s3 = container.Resolve<ITestService2>();
            Assert.IsInstanceOfType(s3, typeof(TestService2A));
            Assert.IsNotNull(s3.NestedService);
            Assert.IsInstanceOfType(s3.NestedService, typeof(TestService1A));
            Assert.AreEqual("2A", s3.Name);

            var s4 = container.Resolve<ITestService2>();
            Assert.IsInstanceOfType(s4, typeof(TestService2A));
            Assert.IsNotNull(s4.NestedService);
            Assert.IsInstanceOfType(s4.NestedService, typeof(TestService1A));
            Assert.AreEqual("2A", s4.Name);

            Assert.AreNotSame(s3, s4);
            Assert.AreNotSame(s3.NestedService, s4.NestedService);
        }

        [TestMethod]
        public void Resolve_singleinstance_implementations()
        {
            IIocContainer container = new IocContainer();
            container.RegisterAsSingleInstance<ITestService1, TestService1A>();
            container.Register<ITestService2, TestService2A>();

            var s1 = container.Resolve<ITestService1>();
            Assert.IsInstanceOfType(s1, typeof(TestService1A));
            Assert.AreEqual("1A", s1.Name);

            var s2 = container.Resolve<ITestService1>();
            Assert.IsInstanceOfType(s2, typeof(TestService1A));
            Assert.AreEqual("1A", s2.Name);

            Assert.AreSame(s1, s2);

            //These services have a constructor that is depending on ITestService1 and that dependency
            //should automatically get resolved and both s3 and s4 should reference the same nested service.
            var s3 = container.Resolve<ITestService2>();
            Assert.IsInstanceOfType(s3, typeof(TestService2A));
            Assert.IsNotNull(s3.NestedService);
            Assert.IsInstanceOfType(s3.NestedService, typeof(TestService1A));
            Assert.AreEqual("2A", s3.Name);

            var s4 = container.Resolve<ITestService2>();
            Assert.IsInstanceOfType(s4, typeof(TestService2A));
            Assert.IsNotNull(s4.NestedService);
            Assert.IsInstanceOfType(s4.NestedService, typeof(TestService1A));
            Assert.AreEqual("2A", s4.Name);

            Assert.AreNotSame(s3, s4);
            Assert.AreSame(s3.NestedService, s4.NestedService);
        }

        [TestMethod]
        public void Resolve_fluent_singleinstance_implementations()
        {
            IIocContainer container = new IocContainer();
            container.Implement<ITestService1>().Using<TestService1A>().AsSingleInstance();
            container.Implement<ITestService2>().Using<TestService2A>();

            var s1 = container.Resolve<ITestService1>();
            Assert.IsInstanceOfType(s1, typeof(TestService1A));
            Assert.AreEqual("1A", s1.Name);

            var s2 = container.Resolve<ITestService1>();
            Assert.IsInstanceOfType(s2, typeof(TestService1A));
            Assert.AreEqual("1A", s2.Name);

            Assert.AreSame(s1, s2);

            //These services have a constructor that is depending on ITestService1 and that dependency
            //should automatically get resolved and both s3 and s4 should reference the same nested service.
            var s3 = container.Resolve<ITestService2>();
            Assert.IsInstanceOfType(s3, typeof(TestService2A));
            Assert.IsNotNull(s3.NestedService);
            Assert.IsInstanceOfType(s3.NestedService, typeof(TestService1A));
            Assert.AreEqual("2A", s3.Name);

            var s4 = container.Resolve<ITestService2>();
            Assert.IsInstanceOfType(s4, typeof(TestService2A));
            Assert.IsNotNull(s4.NestedService);
            Assert.IsInstanceOfType(s4.NestedService, typeof(TestService1A));
            Assert.AreEqual("2A", s4.Name);

            Assert.AreNotSame(s3, s4);
            Assert.AreSame(s3.NestedService, s4.NestedService);
        }

        [TestMethod]
        public void Resolve_after_changing_registration()
        {
            IIocContainer container = new IocContainer();
            container.Register<ITestService1, TestService1A>();
            container.Register<ITestService2, TestService2A>();

            var s1 = container.Resolve<ITestService1>();
            Assert.IsInstanceOfType(s1, typeof(TestService1A));
            Assert.AreEqual("1A", s1.Name);

            var s2 = container.Resolve<ITestService1>();
            Assert.IsInstanceOfType(s2, typeof(TestService1A));
            Assert.AreEqual("1A", s2.Name);

            Assert.AreNotSame(s1, s2);

            //change the registration for test service 1
            container.Register<ITestService1, TestService1B>();
            
            var s3 = container.Resolve<ITestService1>();
            Assert.IsInstanceOfType(s3, typeof(TestService1B));            
            Assert.AreEqual("1B", s3.Name);

            var s4 = container.Resolve<ITestService1>();
            Assert.IsInstanceOfType(s4, typeof(TestService1B));           
            Assert.AreEqual("1B", s4.Name);

            Assert.AreNotSame(s3, s4);
           
        }

        [TestMethod]
        public void Resolve_fluent_after_changing_registration()
        {
            IIocContainer container = new IocContainer();
            container.Implement<ITestService1>().Using<TestService1A>();
            container.Implement<ITestService2>().Using<TestService2A>();

            var s1 = container.Resolve<ITestService1>();
            Assert.IsInstanceOfType(s1, typeof(TestService1A));
            Assert.AreEqual("1A", s1.Name);

            var s2 = container.Resolve<ITestService1>();
            Assert.IsInstanceOfType(s2, typeof(TestService1A));
            Assert.AreEqual("1A", s2.Name);

            Assert.AreNotSame(s1, s2);

            //change the registration for test service 1
            container.Implement<ITestService1>().Using<TestService1B>().WithOverwrite();

            var s3 = container.Resolve<ITestService1>();
            Assert.IsInstanceOfType(s3, typeof(TestService1B));
            Assert.AreEqual("1B", s3.Name);

            var s4 = container.Resolve<ITestService1>();
            Assert.IsInstanceOfType(s4, typeof(TestService1B));
            Assert.AreEqual("1B", s4.Name);

            Assert.AreNotSame(s3, s4);

        }
    }
}
