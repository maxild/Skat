//#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0

#load "build/paths.cake"
#load "build/runhelpers.cake"
#load "build/failurehelpers.cake"

using System.Net;
using System.Linq;

// argument defaults
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

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
var paths = new BuildPaths(settings);  
//var tools = new BuildTools(settings, paths);

// Tools (like aliases)
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
    if (DirectoryExists(paths.Artifacts)) {
        DeleteDirectory(paths.Artifacts, true);
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

Task("Restore")
    .IsDependentOn("InstallDotNet")
    .Does(() => 
{
    Information("Restoring packages...");
    // TODO: --verbosity minimal
    int exitCode1 = Run(dotnet, "restore", paths.Src);
    FailureHelper.ExceptionOnError(exitCode1, "Failed to restore packages under src folder.");
    int exitCode2 = Run(dotnet, "restore", paths.Test);
    FailureHelper.ExceptionOnError(exitCode2, "Failed to restore packages under test folder.");
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

Task("Run-Tests")
    .IsDependentOn("Restore")
    .Does(() => 
{
    foreach (var testPrj in GetFiles(string.Format("{0}/**/project.json", paths.Test)))
    {
        Information("Run tests in {0}", testPrj);
        // TODO: --framework
        int exitCode = Run(dotnet, string.Format("test {0} --configuration {1}", testPrj, configuration));
        FailureHelper.ExceptionOnError(exitCode, string.Format("Failed to run tests in {0}.", testPrj.GetDirectory()));    
    }
});

/// <summary>
///  Build packages.
/// </summary>
Task("Pack")
    .IsDependentOn("Clear-Artifacts")
    .IsDependentOn("Restore")
    .Does(() => 
{
    foreach (var srcPrj in GetFiles(string.Format("{0}/**/project.json", paths.Src)))
    {
        Information("Build nupkg in {0}", srcPrj.GetDirectory());
        // TODO: version via git describe or gitversion
        string buildLabel = "local";
        int buildNumber = 1234;
        string prerelaseTag = string.Format("{0}-{1:D5}", buildLabel, buildNumber);
        // TODO: -v, if Verbose
        int exitCode = Run(dotnet, string.Format("pack {0} --serviceable --configuration {1} --output {2} --version-suffix {3}", srcPrj, configuration, paths.Artifacts, prerelaseTag));
        FailureHelper.ExceptionOnError(exitCode, string.Format("Failed to pack '{0}'.", srcPrj.GetDirectory()));    
    }
});

Task("All")
    .IsDependentOn("Run-Tests")
    .IsDependentOn("Pack");

Task("Default")
    .IsDependentOn("All");

RunTarget(target);
