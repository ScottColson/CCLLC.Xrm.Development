using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoCContainerTests
{
    public interface ITestService1
    {
        string Name { get; }
    }

    public class TestService1A : ITestService1
    {
        public string Name => "1A";
    }

    public class TestService1B : ITestService1
    {
        public string Name => "1B";
    }
}
