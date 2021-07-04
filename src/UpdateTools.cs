using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace AutoUpdateViaGitHubRelease
{
	/// <summary>
	/// Helper methods for updating your application
	/// </summary>
	public static class UpdateTools
	{
		/// <summary>
		/// Check if a new version is available on github. If it is download it.
		/// </summary>
		/// <param name="user">github user name</param>
		/// <param name="repository">github repository name</param>
		/// <param name="assembly">The assembly that should be updated.</param>
		/// <param name="destinationFile">destination name of the update file</param>
		/// <returns><see langword="true"/> if a new version is available, otherwise <see langword="false"/></returns>
		public static async Task<bool> CheckDownloadNewVersionAsync(string user, string repository, Assembly assembly, string destinationFile)
		{
			return await CheckDownloadNewVersionAsync(user, repository, assembly.GetName().Version, destinationFile);
		}

		/// <summary>
		/// Check if a new version is available on github. If it is download it.
		/// </summary>
		/// <param name="user">github user name</param>
		/// <param name="repository">github repository name</param>
		/// <param name="currentVersion">Version to compare against</param>
		/// <param name="destinationFile">destination name of the update file</param>
		/// <param name="logger">An optional logger.</param>
		/// <returns><see langword="true"/> if a new version is available, otherwise <see langword="false"/></returns>
		public static async Task<bool> CheckDownloadNewVersionAsync(string user, string repository, Version currentVersion, string destinationFile, Logger logger = null)
		{
			if (logger is null) logger = new Logger(Path.ChangeExtension(destinationFile, ".log"));
			var gitHub = new GitHubApi();
			try
			{
				var latestReleaseJson = await gitHub.GetLatestReleaseJSONAsync(user, repository);
				var version = GitHubApi.ParseVersion(latestReleaseJson);
				logger.Log($"Comparing {currentVersion} with latest version on github {version}");
				if (version > currentVersion)
				{
					var updateUrl = GitHubApi.ParseDownloadUrl(latestReleaseJson);
					//new version download
					await gitHub.DownloadFile(updateUrl, destinationFile);
					return true;
				}
				return false;
			}
			catch (Exception e)
			{
				logger.Log(e.Message);
				return false;
			}
		}

		/// <summary>
		/// Downloads and extracts the installer into the given directory
		/// </summary>
		/// <param name="destinationDir">Directory to extract the installer to.</param>
		/// <returns>The name of the installer, if all was successfull.</returns>
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

		/// <summary>
		/// Install the update. <see cref="CheckDownloadNewVersionAsync(string, string, Assembly, string)"/> has to be called otherwise no update can be available.
		/// </summary>
		/// <param name="installer">The installer file name.</param>
		/// <param name="updateArchiveFileName">The update archive file name.</param>
		/// <param name="destinationDir">The destination to install to.</param>
		/// <exception cref="ArgumentException">If one of the argument files does not exist.</exception>
		/// <returns><see langword="true"/> If the update was successfull.</returns>
		public static Process StartInstall(string installer, string updateArchiveFileName, string destinationDir)
		{
			//			string Quote(string input) => $"\"{input}\"";
			if (!File.Exists(installer)) throw new ArgumentException($"Installer file '{installer}' does not exist");
			if (!File.Exists(updateArchiveFileName)) throw new ArgumentException($"Archive file '{updateArchiveFileName}' does not exist");
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
			process.Start();
			return process;
		}

		private const string user = "danielScherzer";
		private const string repo = "AutoUpdateViaGitHubRelease";

		private static async Task<string> DownloadInstallerAsync(this GitHubApi gitHub, string destination)
		{
			var latestReleaseJson = await gitHub.GetLatestReleaseJSONAsync(user, repo);
			var urlInstaller = GitHubApi.ParseDownloadUrl(latestReleaseJson);
			var installerFileName = Path.Combine(destination, Path.GetFileName(urlInstaller));
			await gitHub.DownloadFile(urlInstaller, installerFileName);
			return installerFileName;
		}

		private static string ExtractInstaller(this string installerFileName, string destination)
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
	}
}
