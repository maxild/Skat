using System;
using System.IO;
using System.Reflection;
using System.Security;
using System.Security.Permissions;


namespace Maxfire.Skat.UnitTests
{
	public static class PartialTrustContext
	{
		public static void RunTest<TRunner>(Action<TRunner> testDriver, Action<PermissionSet> permissionsSetup = null)
		{
			var setup = new AppDomainSetup
			{
				ApplicationBase = Assembly.GetExecutingAssembly().GetCodeBaseDirectory()
			};

			var permissions = new PermissionSet(null);
			permissions.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));
		    permissionsSetup?.Invoke(permissions);

		    AppDomain sandbox = null;
			try
			{
				sandbox = AppDomain.CreateDomain("sandbox", null, setup, permissions);
				var runner = (TRunner)sandbox.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName,
				                                                      typeof(TRunner).FullName);

				testDriver(runner);
			}
			finally
			{
				if (sandbox != null)
				{
					AppDomain.Unload(sandbox);
				}
			}
		}
	}

    public static class AssemblyExtensions
    {
        /// <summary>
        /// Get the directory where the assembly is found.
        /// </summary>
        /// <remarks>
        /// This is often useful when using a runner (NCover, NUnit etc.) that loads assemblies from a temporary <see cref="AppDomain"/>.
        /// </remarks>
        public static string GetCodeBaseDirectory(this Assembly assembly)
        {
            // The CodeBase is a URL to the place where the file was found, 
            // while the Location is the path where it was actually loaded.
            string codeBaseUriString = assembly.CodeBase;
            var uri = new UriBuilder(codeBaseUriString);
            string codeBasePath = Uri.UnescapeDataString(uri.Path);
            return Path.GetDirectoryName(codeBasePath);
        }
    }
}
