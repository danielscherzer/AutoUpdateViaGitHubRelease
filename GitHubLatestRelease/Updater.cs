using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GitHubLatestRelease
{
	public class Updater
	{
		public string DownloadUrl { get; private set; }
		public Version Version { get; private set; }

		public async Task Connect(string user, string repository)
		{
			client.DefaultRequestHeaders.Accept.Clear();
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
			client.DefaultRequestHeaders.Add("User-Agent", "update from github release");
			//ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
			var response = await GetStringAsync($"repos/{user}/{repository}/releases/latest");
			var json = JObject.Parse(response);
			Version = new Version(json["name"].ToObject<string>());
			var asset = json["assets"][0];
			DownloadUrl = asset["browser_download_url"].ToObject<string>();
		}

		public async Task<bool> DownloadNewVersion(string user, string repository, Version currentVersion, string updateArchiveFilePath)
		{
			await Connect(user, repository);
			if (Version > currentVersion)
			{
				//new version download
				var directory = Path.GetDirectoryName(updateArchiveFilePath);
				Directory.CreateDirectory(directory);
				using (var file = new FileStream(updateArchiveFilePath, FileMode.Create))
				{
					var stream = await Download(DownloadUrl);
					//save download
					stream.CopyTo(file);
				}
				return true;
			}
			return false;
		}

		public void Update()
		{
			var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = "dotnet",
					Arguments = "updater.dll",
					UseShellExecute = true,
					RedirectStandardOutput = false,
					RedirectStandardError = false,
					CreateNoWindow = true
				}
			};
			process.Start();
		}

		public async Task<Stream> Download(string url)
		{
			return await client.GetStreamAsync(url);
		}

		private HttpClient client = new HttpClient();

		private async Task<string> GetStringAsync(string prefix)
		{
			return await client.GetStringAsync($"https://api.github.com/{prefix}");
		}
	}
}
