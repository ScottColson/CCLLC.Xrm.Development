using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoCContainerTests
{
    public interface ITestService2
    {
        string Name { get; }
        ITestService1 NestedService { get; }
    }

    public class TestService2A : ITestService2
    {
        public string Name => "2A";

        public ITestService1 NestedService { get; private set; }

        public TestService2A(ITestService1 nested)
        {
            this.NestedService = nested;
        }
    }

    public class TestService2B : ITestService2
    {
        public string Name => "2B";

        public ITestService1 NestedService { get; private set; }

        public TestService2B(ITestService1 nested)
        {
            this.NestedService = nested;
        }
    }
}
