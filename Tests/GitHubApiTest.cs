using AutoUpdateViaGitHubRelease;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace UnitTestProject
{
	[TestClass]
	public class GitHubApiTest
	{
		private const string user = "danielScherzer";
		private const string repo = "AutoUpdateViaGitHubRelease";

		[TestMethod]
		public void ParseVersion()
		{
			var gitHub = new GitHubApi();
			var task = Task.Run(() => gitHub.GetLatestReleaseJSONAsync(user, repo));
			var json = task.Result;
			var version = GitHubApi.ParseVersion(json);
			Assert.AreNotEqual(0, version.Build);
		}

		[TestMethod]
		public void ParseDownloadUrl()
		{
			var gitHub = new GitHubApi();
			var task = Task.Run(() => gitHub.GetLatestReleaseJSONAsync(user, repo));
			var json = task.Result;
			var url = GitHubApi.ParseDownloadUrl(json);
			Assert.AreNotEqual(0, url.Length);
		}

		[TestMethod]
		public void DownloadInstaller()
		{
			var gitHub = new GitHubApi();
			string tempDir = TempDir();
			var task = Task.Run(() => gitHub.ExtractInstallerTo(tempDir));
			var installerZip = task.Result;
			var ext = Path.GetExtension(installerZip).ToLowerInvariant();
			Assert.IsTrue(ext == ".dll" || ext == ".exe");
		}

		[TestMethod]
		public void DownloadNewVersion()
		{
			var update = HelperUpdateCheck();
			update.DownloadTask.Wait();
			Assert.IsTrue(update.Available);
		}

		[TestMethod]
		public void Install()
		{
			var update = HelperUpdateCheck();
			update.DownloadTask.Wait();
			update.Install();
		}

		private static string TempDir() => Path.Combine(Path.GetTempPath(), repo);

		private static Update HelperUpdateCheck()
		{
			var tempDir = TempDir();
			var update = new Update(user, repo, new System.Version(0, 0), tempDir, "destination");
			return update;
		}
	}
}
