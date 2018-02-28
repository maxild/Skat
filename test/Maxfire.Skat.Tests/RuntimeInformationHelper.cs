namespace Maxfire.Skat.UnitTests
{
    static class RuntimeInformationHelper
    {
#if DEBUG
        public static bool IsDebugBuild()
        {
            return true;
        }
        public static bool IsReleaseBuild()
        {
            return false;
        }
#else
        public static bool IsDebugBuild()
        {
            return false;
        }
        public static bool IsReleaseBuild()
        {
            return true;
        }
#endif

#if NETCORE
        private const string FrameworkName = ".NET Core";
        public static bool IsCoreCLR()
        {
            return true;
        }
        public static bool IsDesktopCLR()
        {
            return false;
        }
#else
        private const string FrameworkName = ".NET Framework";
        public static bool IsCoreCLR()
        {
            return false;
        }
        public static bool IsDesktopCLR()
        {
            return true;
        }
#endif

        public static bool IsMonoCLR()
        {
            return System.Type.GetType("Mono.Runtime") != null;
        }

        public static bool IsMicrosoftCLR()
        {
            return System.Type.GetType("Mono.Runtime") == null;
        }

        public static bool IsRunningOnWindows()
        {
            return !IsRunningOnUnix();
        }

        public static bool IsRunningOnUnix()
        {
            // See https://github.com/dotnet/corefx/issues/1017
#if NETCORE
            // System.Runtime.InteropServices.RuntimeInformation only works on CoreCLR, therefore we determine upfront if running on mono
            bool result = IsMonoCLR() ||
                          System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.Linux) ||
                          System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(System.Runtime.InteropServices.OSPlatform.OSX);
            return result;
#else
            var platform = (int)System.Environment.OSVersion.Platform;
            if (platform == (int)System.PlatformID.MacOSX)
            {
                // OSX
                return true;
            }
            if (platform == 4 || platform == 6 || platform == 128)
            {
                // Linux
                return true;
            }
            if (platform <= 3 || platform == 5)
            {
                // Windows
                return false;
            }
            // Unknown
            return false;
#endif
        }
    }
}
