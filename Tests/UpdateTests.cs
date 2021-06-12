using Microsoft.VisualStudio.TestTools.UnitTesting;
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
			Assert.IsFalse(update.Available);
			var assembly = Assembly.GetExecutingAssembly();
			var version = assembly.GetName().Version;
			var tempDir = Path.Combine(Path.GetTempPath(), nameof(AutoUpdateViaGitHubRelease));
			var result = update.CheckDownloadNewVersionAsync(GitHubApiTest.User
				, GitHubApiTest.Repo, version, tempDir).Result;
			Assert.IsTrue(update.Available);
			Assert.AreEqual(result, update.Available);
			var destinationDir = Path.Combine(tempDir, "install");
			Assert.IsTrue(update.Install(destinationDir).Result);
		}

		private void Update_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			Assert.AreEqual(e.PropertyName, nameof(Update.Available));
			Assert.IsInstanceOfType(sender, typeof(Update));
			Assert.IsTrue((sender as Update)?.Available);
		}
	}
}