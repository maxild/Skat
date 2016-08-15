using System;

namespace Maxfire.Skat.TestDriver
{
    static class Program {

        public static void Main()
        {
            Console.WriteLine("Hello from test driver");

            // Load failing test
            var eksempler = new Maxfire.Skat.UnitTests.Eksempler();
            eksempler.Eksempel_22_ModregningFuldtUdPartnersSkat();

            Console.WriteLine("Test have succeeded");
        }
    }
}
