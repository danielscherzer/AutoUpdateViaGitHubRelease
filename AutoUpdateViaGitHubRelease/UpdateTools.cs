using System;
using System.Diagnostics;
using System.IO;
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
				await gitHub.DownloadFile(GitHubApi.ExtractDownloadUrl(latestReleaseJson), updateArchiveFileName);
				//Get update installer that will extract the update archive to the application directory
				await gitHub.DownloadUpdateInstallerTo(updateTempDir);
				return true;
			}
			return false;
		}

		public static async Task DownloadUpdateInstallerTo(this GitHubApi gitHub, string tempDir)
		{
			var latestReleaseJson = await gitHub.GetLatestReleaseJSONAsync("danielScherzer", "AutoUpdateViaGitHubRelease");
			var urlUpdateInstaller = GitHubApi.ExtractDownloadUrl(latestReleaseJson);
			await gitHub.DownloadFile(urlUpdateInstaller, GetUpdateInstallerExeName(tempDir));
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
				var updateTool = GetUpdateInstallerExeName(updateTempDir);

				//string Quote(string input) => $"\"{input}\"";
				string Quote(string input) => input;
				RunProcess($"{Quote(updateTool)}", $"{Quote(updateDataArchive)} {Quote(destinationDir)}");
			}
		}

		public static string GetUpdateInstallerExeName(string updateTempDir) => Path.Combine(updateTempDir, "updater.exe");

		public static string GetUpdateArchiveFileName(string updateTempDir) => Path.Combine(updateTempDir, "update.zip");
	}
}
