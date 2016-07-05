//#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0

#load "build/paths.cake"
#load "build/runhelpers.cake"
#load "build/failurehelpers.cake"

using System.Net;

// Basic arguments
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

// Tools
string dotnet = settings.UseSystemDotNetPath 
            ? "dotnet" 
            : System.IO.Path.Combine(paths.DotNetCli, "dotnet");

///////////////////////////////////////////////////////////////
// Tasks

Task("EnsureFolders")
    .Does(() => 
{
    CreateDirectory(paths.Artifacts);
});

/// <summary>
///  Clean artifacts.
/// </summary>
Task("Clean")
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
    var installScript = "dotnet-install." + ext;
    var installScriptDownloadUrl = settings.DotNetCliInstallScriptUrl +"/" + installScript;
    var dotnetInstallScript = System.IO.Path.Combine(paths.DotNetCli, installScript);
    
    CreateDirectory(paths.DotNetCli);
    
    // TODO: wget(installScriptDownloadUrl, dotnetInstallScript)
    using (WebClient client = new WebClient())
    {
        client.DownloadFile(installScriptDownloadUrl, dotnetInstallScript);
    }

    if (IsRunningOnUnix())
    {
        Shell("chmod +x {dotnetInstallScript}");
    }
    
    // Run the dotnet-install.{ps1|sh} script. 
    // Note: The script will bypass if the version of the SDK has already been downloaded
    Shell(string.Format("{0} -Channel {1} -Version {2} -InstallDir {3} -NoPath", dotnetInstallScript, settings.DotNetCliChannel, settings.DotNetCliVersion, paths.DotNetCli));
    
    if (!FileExists(paths.DotNetExe)) 
    {
        throw new Exception("Unable to find dotnet.exe. The CLI install may have failed.");
    }

    try
    {
        Run(dotnet, "--info");
    }
    catch
    {
        throw new Exception(".NET CLI binary cannot be found.");
    }

    Information(".NET Core SDK install was succesful!");
});

Task("Restore")
    .IsDependentOn("InstallDotNet")
    .Does(() => 
{
    Information("Restoring packages...");
    int exitCode1 = Run(dotnet, "restore", paths.Src);
    FailureHelper.ExceptionOnError(exitCode1, "Failed to restore packages under src folder.");
    int exitCode2 = Run(dotnet, "restore", paths.Test);
    FailureHelper.ExceptionOnError(exitCode2, "Failed to restore packages under test folder.");
    Information("Package restore was successful!");
});

Task("Default")
    .IsDependentOn("Restore");

RunTarget(target);
