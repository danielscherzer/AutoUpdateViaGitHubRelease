using AutoUpdateViaGitHubRelease;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace UnitTestProject
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void DownloadUpdateInstallerTo()
		{
			var gitHub = new GitHubApi();
			var tempDir = Path.Combine(Path.GetTempPath(), "AutoUpdateViaGitHubRelease");
			Directory.CreateDirectory(tempDir);
			async Task ExtractInstaller()
			{
				var installerFileName = Path.Combine(tempDir, "updater.zip");
				await gitHub.ExtractUpdateInstallerTo(tempDir);
			}
			Task.Run(ExtractInstaller).Wait();
			Assert.IsTrue(File.Exists(Path.Combine(tempDir, "update.dll")));
		}

		[TestMethod]
		public void TestMethod1()
		{
			var update = new Update("danielScherzer", "BatchExecute", Assembly.GetExecutingAssembly(), Path.GetTempPath());
			//update.PropertyChanged += (s, a) => Available = update.Available;
		}
	}
}
