///////////////////////////////////////////////////////////////////////////////
// TOOLS
///////////////////////////////////////////////////////////////////////////////
#tool "nuget:?package=gitreleasemanager&version=0.6.0"
#tool "nuget:?package=xunit.runner.console&version=2.1.0"

///////////////////////////////////////////////////////////////////////////////
// SCRIPTS
///////////////////////////////////////////////////////////////////////////////
#load "./tools/Maxfire.CakeScripts/content/all.cake"

///////////////////////////////////////////////////////////////////////////////
// GLOBAL VARIABLES
///////////////////////////////////////////////////////////////////////////////

var parameters = CakeScripts.GetParameters(
    Context,            // ICakeContext
    BuildSystem,        // BuildSystem alias
    new BuildSettings   // My personal overrides
    {
        MainRepositoryOwner = "maxild",
        RepositoryName = "Skat",
        DeployToCIFeedUrl = "https://www.myget.org/F/maxfire-ci/api/v2/package", // MyGet feed url
        DeployToProdFeedUrl = "https://www.nuget.org/api/v2/package"             // NuGet.org feed url
    });
bool publishingError = false;

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(context =>
{
    if (parameters.Git.IsMasterBranch && context.Log.Verbosity != Verbosity.Diagnostic) {
        Information("Increasing verbosity to diagnostic.");
        context.Log.Verbosity = Verbosity.Diagnostic;
    }

    Information("Building version {0} of {1} ({2}, {3}) using version {4} of Cake. (IsTagPush: {5})",
        parameters.VersionInfo.SemVer,
        parameters.ProjectName,
        parameters.Configuration,
        parameters.Target,
        parameters.VersionInfo.CakeVersion,
        parameters.IsTagPush);
});

///////////////////////////////////////////////////////////////////////////////
// PRIMARY TASKS (direct targets)
///////////////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Package");

Task("Travis")
    .IsDependentOn("Show-Info")
    .IsDependentOn("Test");

Task("AppVeyor")
    .IsDependentOn("Show-Info")
    .IsDependentOn("Print-AppVeyor-Environment-Variables")
    .IsDependentOn("Package")
    .IsDependentOn("Upload-AppVeyor-Artifacts")
    .IsDependentOn("Publish-CIFeed-MyGet")
    .IsDependentOn("Publish-ProdFeed-NuGet")
    .IsDependentOn("Publish-GitHub-Release")
    .Finally(() =>
{
    if (publishingError)
    {
        throw new Exception("An error occurred during the publishing of " + parameters.ProjectName + ".  All publishing tasks have been attempted.");
    }
});

Task("ReleaseNotes")
    .IsDependentOn("Create-Release-Notes");

Task("Clean")
    .IsDependentOn("Clear-Artifacts");

Task("CakeScripts")
    .Does(() =>
{
    var dirToDelete = parameters.Paths.Directories.BuildTools.Combine("Maxfire.CakeScripts");
    if (DirectoryExists(dirToDelete))
    {
        DeleteDirectory(dirToDelete, true);
    }
});

Task("Restore")
    .Does(() =>
{
    DotNetCoreRestore("./", new DotNetCoreRestoreSettings
    {
        Verbose = false,
        Verbosity = DotNetCoreRestoreVerbosity.Minimal
    });
});

Task("Build")
    .IsDependentOn("Patch-Project-Json")
    .IsDependentOn("Restore")
    .Does(() =>
{
    foreach (var project in GetFiles("./**/project.json"))
    {
        DotNetCoreBuild(project.GetDirectory().FullPath, new DotNetCoreBuildSettings {
            VersionSuffix = parameters.VersionInfo.VersionSuffix,
            Configuration = parameters.Configuration
        });
    }
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    Func<IFileSystemInfo, bool> exclude_test_driver =
        fileSystemInfo => fileSystemInfo.Path.FullPath.IndexOf("Maxfire.Skat.TestDriver", StringComparison.OrdinalIgnoreCase) < 0;

    foreach (var testProject in GetFiles(string.Format("{0}/**/project.json", parameters.Paths.Directories.Test), exclude_test_driver))
    {
        if (IsRunningOnWindows())
        {
            DotNetCoreTest(testProject.GetDirectory().FullPath, new DotNetCoreTestSettings {
                Configuration = parameters.Configuration,
                NoBuild = true,
                Verbose = false
            });
        }
        else
        {
            // Ideally we would use the 'dotnet test' command to test both netcoreapp1.0 (CoreCLR)
            // and net452 (Mono), but this currently doesn't work due to
            //    https://github.com/dotnet/cli/issues/3073

            //
            // .NET Core (on Linux and OS X)
            //

            DotNetCoreTest(testProject.GetDirectory().FullPath, new DotNetCoreTestSettings {
                Configuration = parameters.Configuration,
                Framework = "netcoreapp1.0",
                NoBuild = true,
                Verbose = false
            });

            //
            // Mono (on Linux and OS X)
            //

            var testProjectPath = testProject.GetDirectory().FullPath;
            var testProjectName = testProject.GetDirectory().GetDirectoryName();

            var xunitRunner = GetFiles(testProjectPath + "/bin/" + parameters.Configuration + "/net452/*/dotnet-test-xunit.exe").First().FullPath;
            var testAssembly = GetFiles(testProjectPath + "/bin/" + parameters.Configuration + "/net452/*/" + testProjectName + ".dll").First().FullPath;

            int exitCode = Run("mono", xunitRunner + " " + testAssembly);
            if (exitCode != 0)
            {
                throw new Exception("Tests in '" + testProjectName + "' failed on Mono!");
            }
        }
    }
});

Task("Package")
    .IsDependentOn("Clear-Artifacts")
    .IsDependentOn("Test")
    .Does(() =>
{
    foreach (var project in GetFiles(string.Format("{0}/**/project.json", parameters.Paths.Directories.Src)))
    {
        DotNetCorePack(project.GetDirectory().FullPath, new DotNetCorePackSettings {
            VersionSuffix = parameters.VersionInfo.VersionSuffix,
            Configuration = parameters.Configuration,
            OutputDirectory = parameters.Paths.Directories.Artifacts,
            NoBuild = true,
            Verbose = false
        });
    }
});

Task("Publish-CIFeed-MyGet")
    .IsDependentOn("Package")
    .WithCriteria(() => parameters.ShouldDeployToCIFeed)
    .Does(() =>
{
    foreach (var package in GetFiles(parameters.Paths.Directories.Artifacts + "/*.nupkg"))
    {
        NuGetPush(package.FullPath, new NuGetPushSettings {
            Source = parameters.CIFeed.SourceUrl,
            ApiKey = parameters.CIFeed.ApiKey,
            ArgumentCustomization = args => args.Append("-NoSymbols")
        });
    }
})
.OnError(exception =>
{
    Information("Publish-MyGet Task failed, but continuing with next Task...");
    publishingError = true;
});

Task("Publish-ProdFeed-NuGet")
    .IsDependentOn("Package")
    .WithCriteria(() => parameters.ShouldDeployToProdFeed)
    .Does(() =>
{
    foreach (var package in GetFiles(parameters.Paths.Directories.Artifacts + "/*.nupkg"))
    {
        NuGetPush(package.FullPath, new NuGetPushSettings {
            Source = parameters.ProdFeed.SourceUrl,
            ApiKey = parameters.ProdFeed.ApiKey,
            ArgumentCustomization = args => args.Append("-NoSymbols")
        });
    }
})
.OnError(exception =>
{
    Information("Publish-NuGet Task failed, but continuing with next Task...");
    publishingError = true;
});

///////////////////////////////////////////////////////////////////////////////
// SECONDARY TASKS (indirect targets)
///////////////////////////////////////////////////////////////////////////////

Task("Create-Release-Notes")
    .Does(() =>

{
    // This is both the title and tagName of the release (title can be edited on github.com)
    string milestone = Environment.GetEnvironmentVariable("GitHubMilestone") ??
                       parameters.VersionInfo.Milestone;
    Information("Creating draft release of version '{0}' on GitHub", milestone);
    GitReleaseManagerCreate(parameters.GitHub.UserName, parameters.GitHub.Password,
                            parameters.GitHub.RepositoryOwner, parameters.GitHub.RepositoryName,
        new GitReleaseManagerCreateSettings
        {
            Milestone         = milestone,
            Prerelease        = false,
            TargetCommitish   = "master"
        });
});

// Invoked on AppVeyor after draft release have been published on github.com
Task("Publish-GitHub-Release")
    .IsDependentOn("Package")
    .WithCriteria(() => parameters.ShouldDeployToProdFeed)
    .WithCriteria(() => parameters.ConfigurationIsRelease())
    .Does(() =>
{
    // TODO: Both NAME.nupkg and NAME.symbols.nupkg?
    foreach (var package in GetFiles(parameters.Paths.Directories.Artifacts + "/*.nupkg"))
    {
        GitReleaseManagerAddAssets(parameters.GitHub.UserName, parameters.GitHub.Password,
                                   parameters.GitHub.RepositoryOwner, parameters.GitHub.RepositoryName,
                                   parameters.VersionInfo.Milestone, package.FullPath);

    }

    // Close the milestone
    GitReleaseManagerClose(parameters.GitHub.UserName, parameters.GitHub.Password,
                           parameters.GitHub.RepositoryOwner, parameters.GitHub.RepositoryName,
                           parameters.VersionInfo.Milestone);
})
.OnError(exception =>
{
    Information("Publish-GitHub-Release Task failed, but continuing with next Task...");
    publishingError = true;
});

Task("Upload-AppVeyor-Artifacts")
    .IsDependentOn("Package")
    .WithCriteria(() => parameters.IsRunningOnAppVeyor)
    .Does(() =>
{
    // TODO: Both NAME.nupkg and NAME.symbols.nupkg?
    foreach (var package in GetFiles(parameters.Paths.Directories.Artifacts + "/*.nupkg"))
    {
        // appveyor PushArtifact <path> [options] (See https://www.appveyor.com/docs/build-worker-api/#push-artifact)
        AppVeyor.UploadArtifact(package);
    }
});

Task("Clear-Artifacts")
    .Does(() =>
{
    if (DirectoryExists(parameters.Paths.Directories.Artifacts))
    {
        DeleteDirectory(parameters.Paths.Directories.Artifacts, true);
    }
});

Task("Show-Info")
    .Does(() =>
{
    parameters.PrintToLog();
});

Task("Print-AppVeyor-Environment-Variables")
    .WithCriteria(AppVeyor.IsRunningOnAppVeyor)
    .Does(() =>
{
    parameters.PrintAppVeyorEnvironmentVariables();
});

Task("Patch-Project-Json")
    .Does(() =>
{
    // Only production code is patched
    var projects = GetFiles("./src/**/project.json");

    foreach (var project in projects)
    {
        Information("Patching project.json in '{0}' to have version equal to {1}",
            project.GetDirectory().GetDirectoryName(),
            parameters.VersionInfo.NuGetVersion);

        // Reads the current version without the '-*' suffix
        string currVersion = ProjectJsonUtil.ReadProjectJsonVersion(project.FullPath);

        Information("The version in the project.json is {0}", currVersion);

        // Only patch project.json files if the major.minor.patch versions do not match
        if (parameters.VersionInfo.MajorMinorPatch != currVersion) {

            Information("Patching version to {0}", parameters.VersionInfo.PatchedVersion);

            if (!ProjectJsonUtil.PatchProjectJsonVersion(project, parameters.VersionInfo.PatchedVersion))
            {
                Warning("No version specified in {0}.", project.FullPath);
            }
        }
    }
});

Task("Generate-CommonAssemblyInfo")
    .Does(() =>
{
    // No heredocs in c#, so using verbatim string (cannot use $"", because of Cake version)
    string template = @"using System.Reflection;

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4927
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

[assembly: AssemblyCompany(""Maxfire"")]
[assembly: AssemblyProduct(""Maxfire.Skat"")]
[assembly: AssemblyCopyright(""Copyright (c) Morten Maxild."")]

[assembly: AssemblyVersion(""{0}"")]
[assembly: AssemblyFileVersion(""{1}"")]
[assembly: AssemblyInformationalVersion(""{2}"")]

#if DEBUG
[assembly: AssemblyConfiguration(""Debug"")]
#else
[assembly: AssemblyConfiguration(""Release"")]
#endif";

    string content = string.Format(template,
        parameters.VersionInfo.AssemblyVersion,
        parameters.VersionInfo.AssemblyFileVersion,
        parameters.VersionInfo.AssemblyInformationalVersion);

    // Only production code is assembly version patched
    var projects = GetFiles("./src/**/project.json");
    foreach (var project in projects)
    {
        System.IO.File.WriteAllText(parameters.Paths.Files.CommonAssemblyInfo.FullPath, content, Encoding.UTF8);
        //System.IO.File.WriteAllText(System.IO.Path.Combine(parameters.Paths.Directories.Src, "Maxfire.Skat", "Properties" , "AssemblyVersionInfo.cs"), content, Encoding.UTF8);
        //System.IO.File.WriteAllText(System.IO.Path.Combine(project.GetDirectory().FullPath, "Properties" , "AssemblyVersionInfo.cs"), content, Encoding.UTF8);
    }
});

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

    var nuget = parameters.Paths.Tools.NuGet.FullPath;

    foreach (var cache in nugetCaches.Where(kvp => kvp.Value).Select(kvp => kvp.Key))
    {
        Information("Clearing nuget resources in {0}.", cache);
        int exitCode = Run(nuget, string.Format("locals {0} -clear -verbosity detailed", cache));
        FailureHelper.ExceptionOnError(exitCode, string.Format("Failed to clear nuget {0}.", cache));
    }

    Information("NuGet package cache clearing was succesful!");
});

///////////////////////////////////////////////////////////////////////////////
// EXECUTION
///////////////////////////////////////////////////////////////////////////////

RunTarget(parameters.Target);
