using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

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
			Assert.IsTrue(File.Exists(Path.Combine(tempDir, installerName)));
			Directory.Delete(tempDir, true);
		}

		[TestMethod()]
		public void CheckDownloadNewVersion()
		{
			string tempDir = TempDir();
			Directory.CreateDirectory(tempDir);
			var destFile = Path.Combine(tempDir, "Update.zip");
			var downloadOk = UpdateTools.CheckDownloadNewVersionAsync(UpdateTools.User, UpdateTools.Repo, new System.Version(0, 0), destFile).Result;
			Assert.IsTrue(downloadOk);
			Assert.IsTrue(File.Exists(destFile));
			Directory.Delete(tempDir, true);
		}

		[TestMethod()]
		public void Install()
		{
			string tempDir = TempDir();
			Directory.CreateDirectory(tempDir);
			string installerName = Path.Combine(tempDir, UpdateTools.DownloadExtractInstallerToAsync(tempDir).Result);
			var updateArchiveFileName = Path.Combine(tempDir, "Update.zip");
			var downloadOk = UpdateTools.CheckDownloadNewVersionAsync(UpdateTools.User, UpdateTools.Repo, new System.Version(0, 0), updateArchiveFileName).Result;
			Assert.IsTrue(downloadOk);
			var installDir = Path.Combine(tempDir, "install");
			var result = UpdateTools.InstallAsync(installerName, updateArchiveFileName, installDir).Result;
			Assert.IsTrue(result);
			Directory.Delete(tempDir, true);
		}
	}
}