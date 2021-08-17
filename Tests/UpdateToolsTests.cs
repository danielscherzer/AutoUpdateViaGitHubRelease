using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;

namespace AutoUpdateViaGitHubRelease.Tests
{
	[TestClass()]
	public class UpdateToolsTests
	{
		private static string TempDir() => Path.Combine(Path.GetTempPath(), nameof(AutoUpdateViaGitHubRelease), Path.GetRandomFileName());

		[TestMethod()]
		public void DownloadExtractInstallerTo()
		{
			string tempDir = TempDir();
			Directory.CreateDirectory(tempDir);
			string installerName = UpdateTools.DownloadExtractInstallerToAsync(tempDir).Result;
			var ext = Path.GetExtension(installerName).ToLowerInvariant();
			Assert.IsTrue(ext == ".dll" || ext == ".exe");
			Assert.IsTrue(File.Exists(installerName));
			Directory.Delete(tempDir, true);
		}

		[TestMethod()]
		public void CheckDownloadNewVersion()
		{
			string tempDir = TempDir();
			Directory.CreateDirectory(tempDir);
			var destFile = Path.Combine(tempDir, "Update.zip");
			var downloadOk = UpdateTools.CheckDownloadNewVersionAsync(GitHubApiTest.User, GitHubApiTest.Repo, new System.Version(999, 0), destFile).Result;
			Assert.IsFalse(downloadOk);
			Assert.IsFalse(File.Exists(destFile));
			Directory.Delete(tempDir, true);
		}

		[TestMethod()]
		public void CheckDownloadNewVersionForVersionedBackup()
		{
			string tempDir = TempDir();
			Directory.CreateDirectory(tempDir);
			var destFile = Path.Combine(tempDir, "Update.zip");
			var downloadOk = UpdateTools.CheckDownloadNewVersionAsync(GitHubApiTest.User
				, "VersionedBackup", new System.Version(0, 1), destFile).Result;
			Assert.IsTrue(downloadOk);
			Assert.IsTrue(File.Exists(destFile));
			Directory.Delete(tempDir, true);
		}

		[TestMethod()]
		public void CheckDownloadNewVersionAssembly()
		{
			string tempDir = TempDir();
			Directory.CreateDirectory(tempDir);
			var destFile = Path.Combine(tempDir, "Update.zip");
			var assembly = Assembly.GetExecutingAssembly();
			var downloadOk = UpdateTools.CheckDownloadNewVersionAsync(GitHubApiTest.User
				, GitHubApiTest.Repo, assembly, destFile).Result;

			var version = assembly.GetName().Version;
			var gitHub = new GitHubApi();
			var latestReleaseJson = gitHub.GetLatestReleaseJSONAsync(GitHubApiTest.User
				, GitHubApiTest.Repo).Result;
			var latestVersion = GitHubApi.ParseVersion(latestReleaseJson);

			Assert.AreEqual(downloadOk, version < latestVersion);
			if (downloadOk) Assert.IsTrue(File.Exists(destFile));
			Directory.Delete(tempDir, true);
		}

		[TestMethod()]
		public void AllSteps()
		{
			string tempDir = TempDir();
			Directory.CreateDirectory(tempDir);
			string installerName = UpdateTools.DownloadExtractInstallerToAsync(tempDir).Result;
			var updateArchiveFileName = Path.Combine(tempDir, "Update.zip");
			var downloadOk = UpdateTools.CheckDownloadNewVersionAsync(GitHubApiTest.User, GitHubApiTest.Repo, new System.Version(0, 0), updateArchiveFileName).Result;
			Assert.IsTrue(downloadOk);
			var installDir = Path.Combine(tempDir, "install");
			var installProcess = UpdateTools.StartInstall(installerName, updateArchiveFileName, installDir);
			installProcess.WaitForExit();
			Assert.AreEqual(0, installProcess.ExitCode);
			Assert.IsTrue(Directory.Exists(installDir));

			Directory.Delete(tempDir, true);
		}
	}
}