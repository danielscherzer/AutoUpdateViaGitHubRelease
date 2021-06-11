using AutoUpdateViaGitHubRelease;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
			var task = Task.Run(() => gitHub.GetLatestReleaseJSONAsync(UpdateTools.User, UpdateTools.Repo));
			var json = task.Result;
			var version = GitHubApi.ParseVersion(json);
			Assert.AreNotEqual(0, version.Build);
		}

		[TestMethod]
		public void ParseDownloadUrl()
		{
			var gitHub = new GitHubApi();
			var task = Task.Run(() => gitHub.GetLatestReleaseJSONAsync(UpdateTools.User, UpdateTools.Repo));
			var json = task.Result;
			var url = GitHubApi.ParseDownloadUrl(json);
			Assert.AreNotEqual(0, url.Length);
		}
	}
}
