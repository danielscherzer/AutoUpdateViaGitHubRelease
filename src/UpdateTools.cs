using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace AutoUpdateViaGitHubRelease
{
	public static class UpdateTools
	{
		public const string User = "danielScherzer";
		public const string Repo = "AutoUpdateViaGitHubRelease";

		public static async Task<string> ExtractInstallerToAsync(this GitHubApi gitHub, string destination)
		{
			Directory.CreateDirectory(destination);
			var latestReleaseJson = await gitHub.GetLatestReleaseJSONAsync(User, Repo);
			var urlInstaller = GitHubApi.ParseDownloadUrl(latestReleaseJson);
			var installerFileName = Path.Combine(destination, Path.GetFileName(urlInstaller));
			await gitHub.DownloadFile(urlInstaller, installerFileName);
			if (installerFileName.ExtensionIs(".zip"))
			{
				var installer = ZipExtensions.ExtractOverwriteInstallerToDirectory(installerFileName, destination);
				try { File.Delete(installerFileName); } catch { }
				return installer;
			}
			else
			{
				return installerFileName;
			}
		}

		public static async Task<string> DownloadInstallerAsync(this GitHubApi gitHub, string destination)
		{
			var latestReleaseJson = await gitHub.GetLatestReleaseJSONAsync(User, Repo);
			var urlInstaller = GitHubApi.ParseDownloadUrl(latestReleaseJson);
			var installerFileName = Path.Combine(destination, Path.GetFileName(urlInstaller));
			await gitHub.DownloadFile(urlInstaller, installerFileName);
			return installerFileName;
		}

		public static string ExtractInstaller(this string installerFileName, string destination)
		{
			if (installerFileName.ExtensionIs(".zip"))
			{
				var installer = ZipExtensions.ExtractOverwriteInstallerToDirectory(installerFileName, destination);
				try { File.Delete(installerFileName); } catch { }
				return installer;
			}
			else
			{
				return installerFileName;
			}

		}

		/// <summary>
		/// Check if a new version is available on github. If it is download it.
		/// </summary>
		/// <param name="user">github user name</param>
		/// <param name="repository">github repository name</param>
		/// <param name="currentVersion">Version to compare against</param>
		/// <param name="destinationFile">destination name of the update file</param>
		/// <returns><c>true</c> if a new version is available, otherwise <c>false</c></returns>
		public static async Task<bool> CheckDownloadNewVersionAsync(string user, string repository, Version currentVersion, string destinationFile)
		{
			var gitHub = new GitHubApi();
			try
			{
				var latestReleaseJson = await gitHub.GetLatestReleaseJSONAsync(user, repository);
				var version = GitHubApi.ParseVersion(latestReleaseJson);
				Debug.WriteLine($"Latest version on github {version}");
				if (version > currentVersion)
				{
					var updateUrl = GitHubApi.ParseDownloadUrl(latestReleaseJson);
					//new version download
					await gitHub.DownloadFile(updateUrl, destinationFile);
					return true;
				}
				return false;
			}
			catch(Exception e)
			{
				Debug.WriteLine(e.Message);
				return false;
			}
		}

		public static async Task<string> DownloadExtractInstallerToAsync(string destinationDir)
		{
			var gitHub = new GitHubApi();
			try
			{
				var installerZip = await gitHub.DownloadInstallerAsync(destinationDir);
				var installerName = installerZip.ExtractInstaller(destinationDir);
				return installerName;
			}
			catch
			{
				return string.Empty;
			}
		}

		public static async Task<bool> InstallAsync(string installer, string updateArchiveFileName, string destinationDir)
		{
//			string Quote(string input) => $"\"{input}\"";

			string Quote(string input) => input;
			var isExe = installer.ExtensionIs(".exe");
			var arg0 = isExe ? string.Empty : installer;
			var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = isExe ? installer : "dotnet",
					Arguments = $"{arg0} {Quote(updateArchiveFileName)} {Quote(destinationDir)}",
					WorkingDirectory = Path.GetDirectoryName(installer),
					RedirectStandardOutput = false,
					RedirectStandardError = false,
				}
			};
			await Task.Run(() =>
				{
					process.Start();
					process.WaitForExit();
				});
			return 0 == process.ExitCode;
		}
	}
}
