using System.IO;
using System.Threading.Tasks;

namespace AutoUpdateViaGitHubRelease
{
	public static class UpdateTools
	{
		public static async Task<string> ExtractInstallerTo(this GitHubApi gitHub, string updateTempDir)
		{
			Directory.CreateDirectory(updateTempDir);
			var latestReleaseJson = await gitHub.GetLatestReleaseJSONAsync("danielScherzer", "AutoUpdateViaGitHubRelease");
			var urlUpdateInstaller = GitHubApi.ExtractDownloadUrl(latestReleaseJson);
			var installerFileName = Path.Combine(updateTempDir, Path.GetFileName(urlUpdateInstaller));
			await gitHub.DownloadFile(urlUpdateInstaller, installerFileName);
			if (installerFileName.ExtensionIs(".zip"))
			{
				var installer = ZipExtensions.ExtractOverwriteInstallerToDirectory(installerFileName, updateTempDir);
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
