using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GitHubReleaseUpdater
{
	class GitHubRest
	{
		private HttpClient client = new HttpClient();

		public GitHubRest()
		{
			client.DefaultRequestHeaders.Accept.Clear();
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
			client.DefaultRequestHeaders.Add("User-Agent", "update from github release");
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
		}

		public async Task<Stream> Download(string url)
		{
			return await client.GetStreamAsync(url);
		}

		public async Task<(Version Version, string DownloadUrl)> GetLatestReleaseInfo(string user, string repository)
		{
			var response = await GetStringAsync($"repos/{user}/{repository}/releases/latest");
			var json = JObject.Parse(response);
			var version = new Version(json["name"].ToObject<string>());
			var downloadUrl = json["assets"][0]["browser_download_url"].ToObject<string>();
			return (version, downloadUrl);
		}

		private async Task<string> GetStringAsync(string prefix)
		{
			return await client.GetStringAsync($"https://api.github.com/{prefix}");
		}
	}
}
