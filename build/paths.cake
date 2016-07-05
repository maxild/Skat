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
