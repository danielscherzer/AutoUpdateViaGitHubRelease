# Application update via GitHub latest release

[![Build status](https://ci.appveyor.com/api/projects/status/kgatpn14q33smmwl?svg=true)](https://ci.appveyor.com/project/danielscherzer/autoupdateviagithubrelease)


---------------------------------------

This is a .net standard class library that allows application to update themselves. The main goal of this project is to create a library that supports creating self-updating applications. This library will make this process straight-forward.

## Assumptions
- The latest version of your application is deployed as a GitHub release.

## Usage
You can use the methods from the `UpdateTools` class or use the `Update` class if you need `INotifyPropertyChanged`.

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
		update.Install(destinationDir);
		Application.Close();
	}
}
```


See the [change log](CHANGELOG.md) for changes and road map.

## Features

- Check if a newer version is available
- Run update (you will need to close your application to allow overwriting your application files)

## Errors and questions
Please us the GitHub [Issue function](https://github.com/danielscherzer/AutoUpdateViaGitHubRelease/issues/new) to report errors or ask questions.

## Contribute
Check out the [contribution guidelines](CONTRIBUTING.md)
if you want to contribute to this project.


## License
[Apache 2.0](http://www.apache.org/licenses/LICENSE-2.0)