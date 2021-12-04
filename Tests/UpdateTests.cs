using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace AutoUpdateViaGitHubRelease.Tests
{
	[TestClass()]
	public class UpdateTests
	{
		[TestMethod()]
		public void AllSteps()
		{
			var update = new Update();
			update.PropertyChanged += Update_PropertyChanged;
			var assembly = Assembly.GetExecutingAssembly();
			var version = assembly.GetName().Version;
			var tempDir = Path.Combine(Path.GetTempPath(), nameof(AutoUpdateViaGitHubRelease));
			Stopwatch time = Stopwatch.StartNew();
			var newVersion = update.CheckDownloadNewVersionAsync(GitHubApiTest.User
				, GitHubApiTest.Repo, version, tempDir);
			Assert.IsFalse(update.Available);

			var gitHub = new GitHubApi();
			var latestReleaseJson = gitHub.GetLatestReleaseJSONAsync(GitHubApiTest.User
				, GitHubApiTest.Repo).Result;
			var latestVersion = GitHubApi.ParseVersion(latestReleaseJson);

			Assert.AreEqual(newVersion.Result, update.Available);
			Assert.AreEqual(update.Available, version < latestVersion);
			if (update.Available)
			{
				var destinationDir = Path.Combine(tempDir, "install");
				var installProcess = update.StartInstall(destinationDir);
				installProcess.WaitForExit();
				Assert.AreEqual(0, installProcess.ExitCode);
			}
		}

		private void Update_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			Assert.AreEqual(e.PropertyName, nameof(Update.Available));
			Assert.IsInstanceOfType(sender, typeof(Update));
		}
	}
}