# Application update via GitHub latest release

[![Build status](https://ci.appveyor.com/api/projects/status/kgatpn14q33smmwl?svg=true)](https://ci.appveyor.com/project/danielscherzer/autoupdateviagithubrelease)


---------------------------------------

This is a .net standard class library that allows application to update themselves. The main goal of this project is to create a library that supports creating self-updating applications. This library will make this process straight-forward.

## Assumptions
- The latest version of your application is deployed as a GitHub release.

## Usage
1. Create an instance of the `Updater` class
2. If you want to do an immediate update call `update` with your GitHub user name and the repository with the latest release you want to update to.

```
using AutoUpdateViaGitHubRelease;
...
var updater = new Updater();
var currentVersion = Assembly.GetEntryAssembly().GetName().Version;
var newVersionAvailable = await updater.DownloadNewVersion("your user name", "your repository", currentVersion, Path.GetTempPath());
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