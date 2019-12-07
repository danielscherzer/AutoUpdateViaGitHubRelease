using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;

namespace AutoUpdateViaGitHubRelease
{
	public static class UpdateTools
	{
		public static async Task<bool> DownloadNewVersion(this GitHubApi gitHub, string user, string repository, Version currentVersion, string updateTempDir)
		{
			var latestReleaseJson = await gitHub.GetLatestReleaseJSONAsync(user, repository);
			var version = GitHubApi.ExtractVersion(latestReleaseJson);
			if (version > currentVersion)
			{
				//new version download
				var updateArchiveFileName = GetUpdateArchiveFileName(updateTempDir);
				//Get update installer that will extract the update archive to the application directory
				await gitHub.ExtractUpdateInstallerTo(updateTempDir);
				await gitHub.DownloadFile(GitHubApi.ExtractDownloadUrl(latestReleaseJson), updateArchiveFileName);
				return true;
			}
			return false;
		}

		public static async Task ExtractUpdateInstallerTo(this GitHubApi gitHub, string updateTempDir)
		{
			var installerFileName = Path.Combine(updateTempDir, "updater.zip");
			var latestReleaseJson = await gitHub.GetLatestReleaseJSONAsync("danielScherzer", "AutoUpdateViaGitHubRelease");
			var urlUpdateInstaller = GitHubApi.ExtractDownloadUrl(latestReleaseJson);
			await gitHub.DownloadFile(urlUpdateInstaller, installerFileName);
			ZipFile.ExtractToDirectory(installerFileName, updateTempDir);
			File.Delete(installerFileName);
		}

		public static void RunProcess(string executablePath, string parameters)
		{
			var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = executablePath,
					Arguments = parameters,
					WorkingDirectory = Path.GetDirectoryName(executablePath),
					RedirectStandardOutput = false,
					RedirectStandardError = false,
				}
			};
			process.Start();
		}

		public static void InstallUpdate(string updateTempDir, string destinationDir)
		{
			{
				var updateDataArchive = GetUpdateArchiveFileName(updateTempDir);
				var updateTool = Path.Combine(updateTempDir, "update.dll"); ;

				//string Quote(string input) => $"\"{input}\"";
				string Quote(string input) => input;
				RunProcess($"dotnet", $"{Quote(updateTool)} {Quote(updateDataArchive)} {Quote(destinationDir)}");
			}
		}

		public static string GetUpdateArchiveFileName(string updateTempDir) => Path.Combine(updateTempDir, "update.zip");
	}
}
