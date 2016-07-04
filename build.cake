//#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0

#load "build/runhelpers.cake"

using System.Net;

// Basic arguments
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

public class BuildSettings
{
    public string ArtifactsFolder { get; set; }
    public string SrcFolder { get; set; }
    public string TestFolder { get; set; }
    public string BuildToolsFolder { get; set; }
    public string BuildScriptsFolder { get; set; }
    public bool UseSystemDotNetPath { get; set; }
    public string DotNetCliFolder { get; set; }
    public string DotNetCliInstallScriptUrl { get; set; }
    public string DotNetCliBranch { get; set; }
    public string DotNetCliChannel { get; set; }
    public string DotNetCliVersion { get; set; }
}

public class BuildPaths {
    private readonly BuildSettings _settings;
    public BuildPaths(BuildSettings settings) {
        _settings = settings;
    }
    public string Root { get { return System.IO.Directory.GetCurrentDirectory(); } }
    public string Artifacts { get { return System.IO.Path.Combine(Root, _settings.ArtifactsFolder); } }
    public string Src { get { return System.IO.Path.Combine(Root, _settings.SrcFolder); } }
    public string Test { get { return System.IO.Path.Combine(Root, _settings.TestFolder); } }
    public string BuildTools { get { return System.IO.Path.Combine(Root, _settings.BuildToolsFolder); } }
    public string BuildScripts { get { return System.IO.Path.Combine(Root, _settings.BuildScriptsFolder); } }
    public string DotNetCli { get { return System.IO.Path.Combine(Root, _settings.DotNetCliFolder); } }
    public string DotNetExe { get { return System.IO.Path.Combine(DotNetCli, "dotnet.exe"); } }
}

// public class BuildTools {
//     private readonly BuildSettings _settings;
//     private readonly BuildPaths _paths;
//     public BuildTools(BuildSettings settings, BuildPaths paths) {
//         _settings = settings;
//         _paths = paths;
//     }
//     public string dotnet => _settings.UseSystemDotNetPath 
//             ? "dotnet" 
//             : System.IO.Path.Combine(_paths.DotNet, "dotnet");
// }

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

string dotnet = settings.UseSystemDotNetPath 
            ? "dotnet" 
            : System.IO.Path.Combine(paths.DotNetCli, "dotnet");

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
    Information("Downloading .NET Core SDK Binaries");

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

    if (!IsRunningOnWindows())
    {
        Shell("chmod +x {dotnetInstallScript}");
    }
    
    // Run the dotnet-install.{ps1|sh} script. 
    // Note: The script will bypass if the version of the SDK has already been downloaded
    Shell(string.Format("{0} -Channel {1} -Version {2} -InstallDir {3} -NoPath", dotnetInstallScript, settings.DotNetCliChannel, settings.DotNetCliVersion, paths.DotNetCli));
    
    if (!FileExists(paths.DotNetExe)) {
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
});

Task("Default")
    .IsDependentOn("InstallDotNet");

RunTarget(target);
