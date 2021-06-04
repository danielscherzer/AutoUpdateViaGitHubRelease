using AutoUpdateViaGitHubRelease;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace UnitTestProject
{
	[TestClass]
	public class GitHubApiTest
	{
		[TestMethod]
		public void ParseVersion()
		{
			var gitHub = new GitHubApi();
			var task = Task.Run(() => gitHub.GetLatestReleaseJSONAsync("danielScherzer", "AutoUpdateViaGitHubRelease"));
			task.Wait();
			var json = task.Result;
			var version = GitHubApi.ExtractVersion(json);
			Assert.AreNotEqual(0, version.Build);
		}

		//[TestMethod]
		//public void ParseDownloadUrl()
		//{
		//	var gitHub = new GitHubApi();
		//	var task = Task.Run(() => gitHub.GetLatestReleaseJSONAsync("danielScherzer", "AutoUpdateViaGitHubRelease"));
		//	task.Wait();
		//	var json = task.Result;
		//	var url = GitHubApi.ExtractDownloadUrl(json);
		//	Assert.AreNotEqual(0, url.Length);
		//}
		//[TestMethod]
		//public void DownloadInstaller()
		//{
		//	var result = HelperDownloadInstaller();
		//	Assert.IsTrue(result.Length > 0);
		//}

		//[TestMethod]
		//public void DownloadNewVersion()
		//{
		//	var update = HelperUpdateCheck();
		//	update.DownloadTask.Wait();
		//	Assert.IsTrue(update.Available);
		//}

		[TestMethod]
		public void Install()
		{
			var update = HelperUpdateCheck();
			update.DownloadTask.Wait();
			update.Install();
		}

		private static string TempDir() => Path.Combine(Path.GetTempPath(), "AutoUpdateViaGitHubRelease");

		private static string HelperDownloadInstaller()
		{
			var gitHub = new GitHubApi();
			string tempDir = TempDir();
			Directory.CreateDirectory(tempDir);
			var task = Task.Run(() => gitHub.ExtractInstallerTo(tempDir));
			task.Wait();
			return task.Result;
		}

		private static Update HelperUpdateCheck()
		{
			SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
			var tempDir = TempDir();
			var update = new Update("danielScherzer", "AutoUpdateViaGitHubRelease", new System.Version(0, 0), tempDir, "destination");
			return update;
		}
	}
}
