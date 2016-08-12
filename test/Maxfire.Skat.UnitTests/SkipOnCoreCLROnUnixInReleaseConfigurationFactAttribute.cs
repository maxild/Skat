using System.Diagnostics;
using Xunit;

namespace Maxfire.Skat.UnitTests
{
    public class SkipOnCoreCLROnUnixInReleaseConfigurationFactAttribute : FactAttribute
    {
//#if !DEBUG
        public SkipOnCoreCLROnUnixInReleaseConfigurationFactAttribute()
        {
            if (RuntimeInformationHelper.IsCoreCLR() && RuntimeInformationHelper.IsRunningOnUnix())
            {
                Skip = "Skipped on CoreCLR running on Linux or OSX in Release configuration";
            }
        }
//#endif
    }

    // Explicit Tests with xunit v2.
    //
    // dotnet-test-xunit.exe and TraitAttribute("manual", "true")
    //   -trait "name=value"    : only run tests with matching name/value traits
    //                            (if specified more than once, acts as an OR operation).
    //
    //   -notrait "name=value"  : do not run tests with matching name/value traits
    //                            (if specified more than once, acts as an AND operation)
    //

    /// <summary>
    /// Explicit/manual tests: Those are tests that are run only if I explicitly select them to run in
    //  the UI using ReSharper, TestDriven.NET etc.
    /// </summary>
    public class ExplicitTestAttribute : FactAttribute
    {
        public ExplicitTestAttribute()
        {
            if (!Debugger.IsAttached)
            {
                Skip = "Only running in interactive mode.";
            }
        }
    }
}
