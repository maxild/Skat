namespace Maxfire.Skat.UnitTests
{
    public class SkipOnCoreCLROnUnixInReleaseConfigurationFactAttribute : Xunit.FactAttribute
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
}
