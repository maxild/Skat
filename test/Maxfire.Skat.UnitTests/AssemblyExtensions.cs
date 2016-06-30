using System;
using System.IO;
using System.Reflection;

namespace Maxfire.Skat.UnitTests
{
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
