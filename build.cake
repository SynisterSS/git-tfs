#tool "nuget:?package=GitVersion.CommandLine"
#tool "nuget:?package=xunit.runner.console"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var applicationName = "GitTfs";
var applicationPath = "./" + applicationName;
var pathToSln = applicationPath + ".sln";
string assemblyVersion = null;
var buildDirectory = applicationPath + "/bin";


//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var buildDir = Directory(buildDirectory) + Directory(configuration);

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
 //   CleanDirectory(buildDir);
});

//Task("Restore-NuGet-Packages")
//    .IsDependentOn("Clean")
//    .Does(() =>
//{
//    NuGetRestore(pathToSln);
//});
//
//Task("UpdateAssemblyInfo")
//    .IsDependentOn("Clean")
//    .Does(() =>
//{
//    var version = GitVersion();
//	var semanticVersionShort = version.Major + "." + version.Minor + "." + version.CommitsSinceVersionSource;
//	var semanticVersionLong = semanticVersionShort + "+" + version.Sha + "." + version.BranchName;
//	Information("Semantic version (short):" + semanticVersionShort);
//	Information("Semantic version (long ):" + semanticVersionLong);
//	
//	//To make nuget/octopack happy!
//	assemblyVersion = semanticVersionShort;
//	
//	var assemblyInfoPath = applicationPath + "/Properties/AssemblyInfo.cs";
//	var infos = ParseAssemblyInfo(assemblyInfoPath);
//	CreateAssemblyInfo(assemblyInfoPath, new AssemblyInfoSettings {
//    Product = infos.Product,
//	Title=infos.Title,
//	Description=infos.Description,
//	Configuration=infos.Configuration,
//	Company=infos.Company,
//	Trademark=infos.Trademark,
//	//Culture=infos.Culture,
//	ComVisible=infos.ComVisible,
//	Guid=infos.Guid,
//    Version = semanticVersionShort,
//    FileVersion = semanticVersionShort,
//    InformationalVersion = semanticVersionLong,
//    Copyright = string.Format("Copyright © Valtech {0}", DateTime.Now.Year)
//	});
//});

Task("Build")
    //.IsDependentOn("UpdateAssemblyInfo")
    //.IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
      // Use MSBuild
      MSBuild(pathToSln, settings =>
        settings.SetConfiguration(configuration)
				.WithTarget("GitTfs_Vs2015")
				.WithTarget("GitTfsTest"));
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
	Environment.SetEnvironmentVariable("GIT_AUTHOR_NAME", "git-tfs test");
	Environment.SetEnvironmentVariable("GIT_AUTHOR_EMAIL", "test@test.com");
	Environment.SetEnvironmentVariable("GIT_COMMITTER_NAME", "git-tfs test");
	Environment.SetEnvironmentVariable("GIT_COMMITTER_EMAIL", "test@test.com");
	Console.WriteLine("GIT_AUTHOR_EMAIL:" + EnvironmentVariable("GIT_AUTHOR_EMAIL"));
	
    XUnit2("./**/bin/" + configuration + "/GitTfsTest.dll", new XUnit2Settings()
		{
			XmlReport = true,
			OutputDirectory = "."
		}
		);
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Run-Unit-Tests");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
