using System;
using System.Reflection;
using System.Runtime.InteropServices;

[assembly: CLSCompliant(true)]
[assembly: ComVisible(false)]

// The build system is aware of the following preprocessor symbols
// based on the tfm's from project.json
#if NETSTANDARD1_0                                                         // netstandard1.0
[assembly: AssemblyTitle("Maxfire.Skat .NET Standard 1.0")]
#elif NETSTANDARD1_1
[assembly: AssemblyTitle("Maxfire.Skat .NET Standard 1.1")]
#elif NETSTANDARD1_2
[assembly: AssemblyTitle("Maxfire.Skat .NET Standard 1.2")]
#elif NETSTANDARD1_3
[assembly: AssemblyTitle("Maxfire.Skat .NET Standard 1.3")]
#elif NETSTANDARD1_4
[assembly: AssemblyTitle("Maxfire.Skat .NET Standard 1.4")]
#elif NETSTANDARD1_5
[assembly: AssemblyTitle("Maxfire.Skat .NET Standard 1.5")]
#elif NETSTANDARD1_6
[assembly: AssemblyTitle("Maxfire.Skat .NET Standard 1.6")]
#elif NET45
[assembly: AssemblyTitle("Maxfire.Skat .NET Framework 4.5")]
#elif NET451
[assembly: AssemblyTitle("Maxfire.Skat .NET Framework 4.5.1")]
#elif NET452
[assembly: AssemblyTitle("Maxfire.Skat .NET Framework 4.5.2")]
#elif NET46
[assembly: AssemblyTitle("Maxfire.Skat .NET Framework 4.6")]
#elif NET461
[assembly: AssemblyTitle("Maxfire.Skat .NET Framework 4.6.1")]
#elif NET462
[assembly: AssemblyTitle("Maxfire.Skat .NET Framework 4.6.2")]
#else
[assembly: AssemblyTitle("Maxfire.Skat")]
#endif
