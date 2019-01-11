using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GitHubLatestRelease
{
	public class Updater
	{
		public Updater()
		{
			client.DefaultRequestHeaders.Accept.Clear();
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
			client.DefaultRequestHeaders.Add("User-Agent", "update from github release");
			//ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
		}

		public async Task<bool> DownloadNewVersion(string user, string repository, Version currentVersion, string updateArchiveFilePath)
		{
			var latestVersionJson = await GetLatestVersionJSONAsync(user, repository);
			var version = new Version(latestVersionJson["name"].ToObject<string>());
			if (version > currentVersion)
			{
				//new version download
				var updateDirectory = Path.GetDirectoryName(updateArchiveFilePath);
				var updateDataDirectory = Path.Combine(updateDirectory, "data");
				Directory.CreateDirectory(updateDataDirectory);
				//await DownloadToFile(DownloadUrl(latestVersionJson), updateArchiveFilePath);
				await DownloadExtract(DownloadUrl(latestVersionJson), updateDataDirectory);
				//Get UpdateExtract assembly
				var urlUpdateExtract = DownloadUrl(await GetLatestVersionJSONAsync("danielScherzer", "GitHubReleaseUpdater"));
				//var updateExtract = Path.Combine(updateDirectory, "updateExtract.zip");
				await DownloadExtract(urlUpdateExtract, updateDirectory);
				return true;
			}
			return false;
		}

		private async Task DownloadExtract(string downloadUrl, string directory)
		{
			using (var zip = new ZipArchive(await client.GetStreamAsync(downloadUrl), ZipArchiveMode.Read))
			{
				zip.ExtractToDirectory(directory);
			}
		}

		public async Task DownloadToFile(string downloadUrl, string filePath)
		{
			using (var file = new FileStream(filePath, FileMode.Create))
			{
				var stream = await Download(downloadUrl);
				//save download
				stream.CopyTo(file);
			}
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

		public async Task<Stream> Download(string url) => await client.GetStreamAsync(url);

		private HttpClient client = new HttpClient();

		private async Task<JObject> GetJSONAsync(string prefix) => JObject.Parse(await client.GetStringAsync($"https://api.github.com/{prefix}"));

		private async Task<JObject> GetLatestVersionJSONAsync(string user, string repository) => await GetJSONAsync($"repos/{user}/{repository}/releases/latest");

		private static string DownloadUrl(JObject json) => json["assets"][0]["browser_download_url"].ToObject<string>();
	}
}
