# Application update via GitHub latest release

[![Build status](https://ci.appveyor.com/api/projects/status/kgatpn14q33smmwl?svg=true)](https://ci.appveyor.com/project/danielscherzer/autoupdateviagithubrelease)


---------------------------------------

This is a .net standard class library that allows application to update themselves. The main goal of this project is to create a library that supports creating self-updating applications. This library will make this process straight-forward.

## Assumptions
- The latest version of your application is deployed as a GitHub release.

## Usage
1. You can use the `Update` class if you need `INotifyPropertyChanged`:

```C#
using AutoUpdateViaGitHubRelease;
...
var update = new Update();
update.PropertyChanged += Update_PropertyChanged;
var assembly = Assembly.GetExecutingAssembly();
var version = assembly.GetName().Version;
update.CheckDownloadNewVersionAsync(GitHubUser, GitHubRepo, version, tempDir);
...
private void Update_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
{
	if(update.Available)
	{
		var destinationDir = Path.GetDirectory(assembly.Location);
		update.StartInstall(destinationDir);
		Application.Close();
	}
}
```

2. If you want more control, you can use the methods from the `UpdateTools` class: 
```C#
using AutoUpdateViaGitHubRelease;
...
var assembly = Assembly.GetExecutingAssembly();
var tempDir = Path.Combine(Path.GetTempPath(), nameof(ApplicationName));
Directory.CreateDirectory(tempDir);
var updateArchive = Path.Combine(tempDir, "update.zip");
var version = assembly.GetName().Version;
var updateTask = UpdateTools.CheckDownloadNewVersionAsync(GitHubUser, GitHubRepo, assembly.GetName().Version, updateArchive);
... // application code
var updateAvailable = updateTask.Result;
if (updateAvailable)
{
	Console.Write("Update? (Y/N)");
	if (ConsoleKey.Y == Console.ReadKey().Key)
	{
		var installer = Path.Combine(tempDir, UpdateTools.DownloadExtractInstallerToAsync(tempDir).Result);
		var destinationDir = Path.GetDirectoryName(assembly.Location);
		UpdateTools.StartInstall(installer, updateArchive, destinationDir); // don't wait for install process to finish in windows! The applications needs to be closed before it can be updated.
		// Your application should close right after starting install
		Environment.Exit(0);
	}
}


```

See the [change log](CHANGELOG.md) for changes and road map.

## Features

- Check if a newer version is available
- Run update (you will need to close your application to allow overwriting your application files)

## Projects using this
- Command line
  - [Versioned Copy](https://github.com/danielscherzer/VersionedCopy)
- WinForm
  - [Shader Form](https://github.com/danielscherzer/ShaderForm)
- WPF
  - [Shader Form 2](https://github.com/danielscherzer/ShaderForm2)
  - [Batch Renamer](https://github.com/danielscherzer/BatchRenamer)
  - [Batch Execute](https://github.com/danielscherzer/BatchExecute)

## Errors and questions
Please us the GitHub [Issue function](https://github.com/danielscherzer/AutoUpdateViaGitHubRelease/issues/new) to report errors or ask questions.

## Contribute
Check out the [contribution guidelines](CONTRIBUTING.md)
if you want to contribute to this project.


## License
[Apache 2.0](http://www.apache.org/licenses/LICENSE-2.0)