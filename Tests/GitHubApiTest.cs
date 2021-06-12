using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace AutoUpdateViaGitHubRelease.Tests
{
	[TestClass]
	public class GitHubApiTest
	{
		public const string User = "danielScherzer";
		public const string Repo = "AutoUpdateViaGitHubRelease";

		[TestMethod]
		public void ParseVersion()
		{
			var gitHub = new GitHubApi();
			var task = Task.Run(() => gitHub.GetLatestReleaseJSONAsync(User, Repo));
			var json = task.Result;
			var version = GitHubApi.ParseVersion(json);
			Assert.AreNotEqual(0, version.Build);
		}

		[TestMethod]
		public void ParseDownloadUrl()
		{
			var gitHub = new GitHubApi();
			var task = Task.Run(() => gitHub.GetLatestReleaseJSONAsync(User, Repo));
			var json = task.Result;
			var url = GitHubApi.ParseDownloadUrl(json);
			Assert.AreNotEqual(0, url.Length);
		}
	}
}
