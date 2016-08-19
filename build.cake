// Install addins.
#addin "nuget:https://www.nuget.org/api/v2?package=Newtonsoft.Json&version=9.0.1"

// Install tools.
//#tool "nuget:https://www.nuget.org/api/v2?package=gitreleasemanager&version=0.5.0"
#tool "nuget:https://www.nuget.org/api/v2?package=GitVersion.CommandLine&version=3.6.2"

// Load other scripts
#load "build/parameters.cake"
#load "build/paths.cake"
#load "build/version.cake"
#load "build/runhelpers.cake"
#load "build/failurehelpers.cake"

using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Linq;

///////////////////////////////////////////////////////////////
// Parameters (target, configuration etc.)
BuildParameters parameters = BuildParameters.GetParameters(Context);

///////////////////////////////////////////////////////////////
// Versioning
BuildVersion versionInfo = BuildVersion.Calculate(Context, parameters);

///////////////////////////////////////////////////////////////
// Configuration (Note: branch of dotnet cli is '1.0.0-preview2')
var settings = new BuildSettings {
    ArtifactsFolder = "artifacts",
    SrcFolder = "src",
    TestFolder = "test",
    BuildToolsFolder = ".tools",
    BuildScriptsFolder = "build",
    UseSystemDotNetPath = false,
    DotNetCliFolder = ".dotnet",
    DotNetCliInstallScriptUrl = "https://raw.githubusercontent.com/dotnet/cli/rel/1.0.0-preview2/scripts/obtain",
    DotNetCliChannel = "preview",
    DotNetCliVersion = "1.0.0-preview2-003121"
};
var paths = BuildPaths.GetPaths(Context, settings);

// Tools (like aliases)
// TODO: Use Cake Tools framework (ToolsLocator etc..)
string dotnet = settings.UseSystemDotNetPath
            ? "dotnet"
            : System.IO.Path.Combine(paths.DotNetCli, "dotnet");
string nuget = System.IO.Path.Combine(paths.BuildTools, "nuget");

///////////////////////////////////////////////////////////////
// Tasks

/// <summary>
///  Clear artifacts folder.
/// </summary>
Task("Clear-Artifacts")
    .Does(() =>
{
    //CleanDirectory(paths.Artifacts); // this will not delete the artifacts folder

    // this will also delete the artifacts folder
    if (DirectoryExists(paths.Artifacts))
    {
        DeleteDirectory(paths.Artifacts, true);
    }
});

Task("Show-Version")
    .Does(() =>
{
    Information("Version: {0}", versionInfo.Version);
    Information("PkgVersion: {0}", versionInfo.PkgVersion);
    Information("SemVersion: {0}", versionInfo.SemVersion);
    Information("InformationalVersion: {0}", versionInfo.InformationalVersion);
    Information("DotNetVersionSuffix: {0}", versionInfo.DotNetVersionSuffix);
    Information("CakeVersion: {0}", versionInfo.CakeVersion);
});

Task("Patch-Project-Json")
    .Does(() =>
{
    // Only production code is patched
    var projects = GetFiles("./src/**/project.json");
    foreach (var project in projects)
    {
        // We subsitute the entire version value, so no need to use --version-suffix below
        if(!BuildVersion.PatchProjectJson(project, versionInfo.PkgVersion))
        {
            Warning("No version specified in {0}.", project.FullPath);
        }
    }
});

/// <summary>
///  Install the .NET Core SDK Binaries (preview2 bits).
/// </summary>
Task("InstallDotNet")
    .Does(() =>
{
    Information("Installing .NET Core SDK Binaries...");

    var ext = IsRunningOnWindows() ? "ps1" : "sh";
    var installScript = string.Format("dotnet-install.{0}", ext);
    var installScriptDownloadUrl = string.Format("{0}/{1}", settings.DotNetCliInstallScriptUrl, installScript);
    var dotnetInstallScript = System.IO.Path.Combine(paths.DotNetCli, installScript);

    CreateDirectory(paths.DotNetCli);

    // TODO: wget(installScriptDownloadUrl, dotnetInstallScript)
    using (WebClient client = new WebClient())
    {
        client.DownloadFile(installScriptDownloadUrl, dotnetInstallScript);
    }

    if (IsRunningOnUnix())
    {
        Shell(string.Format("chmod +x {0}", dotnetInstallScript));
    }

    // Run the dotnet-install.{ps1|sh} script.
    // Note: The script will bypass if the version of the SDK has already been downloaded
    Shell(string.Format("{0} -Channel {1} -Version {2} -InstallDir {3} -NoPath", dotnetInstallScript, settings.DotNetCliChannel, settings.DotNetCliVersion, paths.DotNetCli));

    var dotNetExe = IsRunningOnWindows() ? "dotnet.exe" : "dotnet";
    if (!FileExists(System.IO.Path.Combine(paths.DotNetCli, dotNetExe)))
    {
        throw new Exception(string.Format("Unable to find {0}. The dotnet CLI install may have failed.", dotNetExe));
    }

    try
    {
        Run(dotnet, "--info");
    }
    catch
    {
        throw new Exception("dotnet --info have failed to execute. The dotnet CLI install may have failed.");
    }

    Information(".NET Core SDK install was succesful!");
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("InstallDotNet")
    .Does(() =>
{
    Information("Restoring packages...");

    // TODO: --verbosity minimal
    // TODO: Can run restore once in project root
    // TODO: Use DotNetCoreRestore helper
    // int exitCode1 = Run(dotnet, "restore", paths.Src);
    // FailureHelper.ExceptionOnError(exitCode1, "Failed to restore packages under src folder.");
    // int exitCode2 = Run(dotnet, "restore", paths.Test);
    // FailureHelper.ExceptionOnError(exitCode2, "Failed to restore packages under test folder.");

    DotNetCoreRestore("./", new DotNetCoreRestoreSettings
    {
        ToolPath = paths.DotNetToolPath,
        Verbose = false,
        Verbosity = DotNetCoreRestoreVerbosity.Minimal
    });

    Information("Package restore was successful!");
});

/// <summary>
///  Clears local nuget resources such as the packages cache
///  and the machine-wide global packages folder.
/// </summary>
Task("Clear-PackageCache")
    .Does(() =>
{
    Information("Clearing NuGet package caches...");

    // NuGet restore with single source (nuget.org v3 feed) reports
    //    Feeds used:
    //        %LOCALAPPDATA%\NuGet\Cache          (packages-cache)
    //        C:\Users\Maxfire\.nuget\packages\   (global-packages)
    //        https://api.nuget.org/v3/index.json (only configured feed)

    var nugetCaches = new Dictionary<string, bool>
    {
        {"http-cache", false},      // %LOCALAPPDATA%\NuGet\v3-cache
        {"packages-cache", true},   // %LOCALAPPDATA%\NuGet\Cache
        {"global-packages", true},  // ~\.nuget\packages\
        {"temp", false},            // %LOCALAPPDATA%\Temp\NuGetScratch
    };

    foreach (var cache in nugetCaches.Where(kvp => kvp.Value).Select(kvp => kvp.Key))
    {
        Information("Clearing nuget resources in {0}.", cache);
        int exitCode = Run(nuget, string.Format("locals {0} -clear -verbosity detailed", cache));
        FailureHelper.ExceptionOnError(exitCode, string.Format("Failed to clear nuget {0}.", cache));
    }

    Information("NuGet package cache clearing was succesful!");
});


Task("Build")
    .IsDependentOn("Patch-Project-Json")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    foreach (var project in GetFiles("./**/project.json"))
    {
        DotNetCoreBuild(project.GetDirectory().FullPath, new DotNetCoreBuildSettings {
            ToolPath = paths.DotNetToolPath,
            //VersionSuffix = versionInfo.DotNetVersionSuffix,
            Configuration = parameters.Configuration
        });
    }
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    var results = new List<TestResult>();

    // TODO: For Release builds the following test methods fail:
    //     1) Maxfire.Skat.UnitTests.Eksempler.Eksempel_22_ModregningFuldtUdPartnersSkat
    //     2) Maxfire.Skat.UnitTests.Eksempler.Eksempel_23_DenEnePartnerHarUnderskudFraTidligereAar
    //     3) Maxfire.Skat.UnitTests.Eksempler.Eksempel_24_DenEnePartnerHarNegativSkattepligtigIndkomstDenAndenHarEtFremfoertUnderskud
    //     4) Maxfire.Skat.UnitTests.Eksempler.Eksempel_25_BeggeHarUnderskudFraTidligereAar

    Func<IFileSystemInfo, bool> exclude_test_driver =
        fileSystemInfo => fileSystemInfo.Path.FullPath.IndexOf("Maxfire.Skat.TestDriver", StringComparison.OrdinalIgnoreCase) < 0;

    foreach (var testPrj in GetFiles(string.Format("{0}/**/project.json", paths.Test), exclude_test_driver))
    {
        Information("Run tests in {0}", testPrj);

        var testPrjDir = testPrj.GetDirectory();
        var testPrjName = testPrjDir.GetDirectoryName();

        if (IsRunningOnWindows())
        {
            int exitCode = Run(dotnet, string.Format("test {0} --configuration {1}", testPrj, parameters.Configuration));
            FailureHelper.ExceptionOnError(exitCode, string.Format("Failed to run tests on Core CLR in {0}.", testPrjDir));
        }
        else
        {
            // Ideally we would use the 'dotnet test' command to test both netcoreapp1.0 (CoreCLR)
            // and net46 (Mono), but this currently doesn't work due to
            //    https://github.com/dotnet/cli/issues/3073
            int exitCode1 = Run(dotnet, string.Format("test {0} --configuration {1} --framework netcoreapp1.0", testPrj, parameters.Configuration));
            //FailureHelper.ExceptionOnError(exitCode1, string.Format("Failed to run tests on Core CLR in {0}.", testPrjDir));
            results.Add(new TestResult(string.Format("CoreCLR: {0}", testPrjName), exitCode1));

            // Instead we run xUnit.net .NET CLI test runner directly with mono for the net46 target framework

            // Build using .NET CLI
            int exitCode2 = Run(dotnet, string.Format("build {0} --configuration {1} --framework net46", testPrj, parameters.Configuration));
            FailureHelper.ExceptionOnError(exitCode2, string.Format("Failed to build tests on Desktop CLR in {0}.", testPrjDir));

            // Shell() helper does not support running mono, so we glob here
            var dotnetTestXunit = GetFiles(string.Format("{0}/bin/{1}/net46/*/dotnet-test-xunit.exe", testPrjDir, parameters.Configuration)).First();
            var dotnetTestAssembly = GetFiles(string.Format("{0}/bin/{1}/net46/*/{2}.dll", testPrjDir, parameters.Configuration, testPrjName)).First();

            // Run using Mono
            int exitCode3 = Run("mono", string.Format("{0} {1}", dotnetTestXunit, dotnetTestAssembly));
            //FailureHelper.ExceptionOnError(exitCode3, string.Format("Failed to run tests on Desktop CLR in {0}.", testPrjDir));
            results.Add(new TestResult(string.Format("DesktopCLR: {0}", testPrjName), exitCode3));

        }

        if (results.Any(r => r.Failed))
        {
            throw new Exception(
                results.Aggregate(new StringBuilder(), (sb, result) =>
                    sb.AppendFormat("{0}{1}", result.ErrorMessage, Environment.NewLine)).ToString().TrimEnd()
                );
        }

        Information("Tests in {0} was succesful!", testPrj);
    }
});

/// <summary>
///  Build packages.
/// </summary>
Task("Pack")
    .IsDependentOn("Clear-Artifacts")
    .IsDependentOn("Build")
    .Does(() =>
{
    foreach (var project in GetFiles(string.Format("{0}/**/project.json", paths.Src)))
    {
        Information("Build nupkg in {0}", project.GetDirectory());
        // TODO: version via git describe or gitversion
        // string buildLabel = "local";
        // int buildNumber = 1234;
        // string prerelaseTag = string.Format("{0}-{1:D5}", buildLabel, buildNumber);
        // // TODO: -v, if Verbose
        // int exitCode = Run(dotnet, string.Format("pack {0} --serviceable --configuration {1} --output {2} --version-suffix {3}", project, parameters.Configuration, paths.Artifacts, prerelaseTag));
        // FailureHelper.ExceptionOnError(exitCode, string.Format("Failed to pack '{0}'.", project.GetDirectory()));

        DotNetCorePack(project.GetDirectory().FullPath, new DotNetCorePackSettings {
            ToolPath = paths.DotNetToolPath,
            //VersionSuffix = versionInfo.DotNetVersionSuffix,
            Configuration = parameters.Configuration,
            OutputDirectory = paths.Artifacts,
            NoBuild = true,
            Verbose = false
        });
    }
});

Task("All")
    .IsDependentOn("Run-Unit-Tests")
    .IsDependentOn("Pack");

Task("Verify")
    .IsDependentOn("Run-Unit-Tests");

Task("Default")
    .IsDependentOn("All");

RunTarget(parameters.Target);
