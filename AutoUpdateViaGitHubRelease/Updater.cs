using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AutoUpdateViaGitHubRelease
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

		public void StartUpdate(string updateTempDir, string destinationDir)
		{
			{
				var updateDataArchive = UpdateArchive(updateTempDir);
				var updateTool = UpdateTool(updateTempDir);

				//string Quote(string input) => $"\"{input}\"";
				string Quote(string input) => input;
				Run($"{Quote(updateTool)}", $"{Quote(updateDataArchive)} {Quote(destinationDir)}");
			}
		}

		public async Task<bool> DownloadNewVersion(string user, string repository, Version currentVersion, string updateTempDir)
		{
			var latestVersionJson = await GetLatestVersionJSONAsync(user, repository);
			var version = new Version(latestVersionJson["name"].ToObject<string>());
			if (version > currentVersion)
			{
				//new version download
				var updateDataArchive = UpdateArchive(updateTempDir);
				await DownloadFile(DownloadUrl(latestVersionJson), updateDataArchive);
				//Get update application that will extract the update archive to the application directory
				var urlUpdateWindow = DownloadUrl(await GetLatestVersionJSONAsync("danielScherzer", "AutoUpdateViaGitHubRelease"));
				var updateTool = UpdateTool(updateTempDir);
				await DownloadFile(urlUpdateWindow, updateTool);
				return true;
			}
			return false;
		}

		private HttpClient client = new HttpClient();

		private async Task DownloadFile(string url, string fileName)
		{
			using (var stream = await client.GetStreamAsync(url))
			{
				using (var file = new FileStream(fileName, FileMode.Create))
				{
					stream.CopyTo(file);
				}
			}
		}

		private static string UpdateTool(string updateTempDir) => Path.Combine(updateTempDir, "updater.exe");

		private static string UpdateArchive(string updateTempDir) => Path.Combine(updateTempDir, "update.zip");

		private void Run(string executablePath, string parameters)
		{
			var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = executablePath,
					Arguments = parameters,
					WorkingDirectory = Path.GetDirectoryName(executablePath),
					RedirectStandardOutput = false,
					RedirectStandardError = false,
				}
			};
			process.Start();
		}

		private async Task<JObject> GetJSONAsync(string prefix) => JObject.Parse(await client.GetStringAsync($"https://api.github.com/{prefix}"));

		private async Task<JObject> GetLatestVersionJSONAsync(string user, string repository) => await GetJSONAsync($"repos/{user}/{repository}/releases/latest");

		private static string DownloadUrl(JObject json) => json["assets"][0]["browser_download_url"].ToObject<string>();
	}
}
