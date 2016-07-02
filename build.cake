//#tool nuget:?package=NUnit.ConsoleRunner&version=3.4.0

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

// Define directories.
var artifactsDir = Directory("./artifacts");

Task("Clean")
    .Does(() =>
{
    CleanDirectory(artifactsDir);
});

Task("SayHello").Does(() => 
{
    // string interpolation is not supported (without -experimental)
    // See https://github.com/cake-build/cake/issues/293
    // See https://github.com/cake-build/cake/issues/326
    Information("Say hello, configuration: {0}, target: {1}.", configuration, target);
    //Information($"Say hello, configuration: {configuration}, target: ${target}.");
});

Task("Default")
    .IsDependentOn("SayHello");

RunTarget(target);
