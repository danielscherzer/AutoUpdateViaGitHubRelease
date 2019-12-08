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
				//Get update installer that will extract the update archive to the application directory
				await gitHub.ExtractInstallerTo(updateTempDir);
				await gitHub.DownloadFile(GitHubApi.ExtractDownloadUrl(latestReleaseJson), updateArchiveFileName);
				return true;
			}
			return false;
		}

		public static async Task ExtractInstallerTo(this GitHubApi gitHub, string updateTempDir)
		{
			Directory.CreateDirectory(updateTempDir);
			var installerFileName = Path.Combine(updateTempDir, "installer.zip");
			var latestReleaseJson = await gitHub.GetLatestReleaseJSONAsync("danielScherzer", "AutoUpdateViaGitHubRelease");
			var urlUpdateInstaller = GitHubApi.ExtractDownloadUrl(latestReleaseJson);
			await gitHub.DownloadFile(urlUpdateInstaller, installerFileName);
			ZipExtensions.ExtractOverwriteToDirectory(installerFileName, updateTempDir);
			try { File.Delete(installerFileName); } catch { }
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
			var updateDataArchive = GetUpdateArchiveFileName(updateTempDir);
			var updateTool = "Update.dll";

			//string Quote(string input) => $"\"{input}\"";
			string Quote(string input) => input;
			//RunProcess($"dotnet {Quote(updateTool)}", $"{Quote(updateDataArchive)} {Quote(destinationDir)}");
			var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = "dotnet",
					Arguments = $"{updateTool}",// {Quote(updateDataArchive)} {Quote(destinationDir)}",
					WorkingDirectory = updateTempDir,
					RedirectStandardOutput = false,
					RedirectStandardError = false,
				}
			};
			process.Start();
		}

		public static string GetUpdateArchiveFileName(string updateTempDir) => Path.Combine(updateTempDir, "update.zip");
	}
}
