using AutoUpdateViaGitHubRelease;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;

namespace UnitTestProject
{
	[TestClass]
	public class GitHubApiTest
	{
		[TestMethod]
		public void DownloadUpdateInstallerTo()
		{
			var gitHub = new GitHubApi();
			string tempDir = TempDir();
			Directory.CreateDirectory(tempDir);
			Task.Run(() => gitHub.ExtractInstallerTo(tempDir)).Wait();
			Assert.IsTrue(File.Exists(Path.Combine(tempDir, "update.dll")));
			Assert.IsTrue(File.Exists(Path.Combine(tempDir, "Update.runtimeconfig.json")));
		}

		private static string TempDir()
		{
			return Path.Combine(Path.GetTempPath(), "AutoUpdateViaGitHubRelease");
		}

		[TestMethod]
		public void DownloadNewVersion()
		{
			var gitHub = new GitHubApi();
			var tempDir = TempDir();
			Directory.CreateDirectory(tempDir);
			async Task<bool> NewVersion()
			{
				return await gitHub.DownloadNewVersion("danielScherzer", "AutoUpdateViaGitHubRelease", new System.Version(0, 0), tempDir);
			}
			var task = Task.Run(NewVersion);
			task.Wait();
			Assert.IsTrue(task.Result);
		}

		[TestMethod]
		public void Install()
		{
			DownloadNewVersion();
			var tempDir = TempDir();
			UpdateTools.InstallUpdate(tempDir, Path.Combine(tempDir, "destination"));
		}
	}
}
