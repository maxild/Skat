using System;
using Xunit.Abstractions;

namespace Maxfire.Skat.TestDriver
{
    static class Program {

        public static void Main()
        {
            Console.WriteLine("Hello from test driver");

            // Load failing test
            var eksempler = new Maxfire.Skat.UnitTests.Eksempler(new ConsoleTestOutputHelper());
            eksempler.Eksempel_22_ModregningFuldtUdPartnersSkat();

            Console.WriteLine("Test have succeeded");
        }
    }

    class ConsoleTestOutputHelper : ITestOutputHelper
    {
        public void WriteLine(string message)
        {
            System.Console.WriteLine(message);

        }

        public void WriteLine(string format, params object[] args)
        {
            System.Console.WriteLine(format, args);
        }
    }
}
