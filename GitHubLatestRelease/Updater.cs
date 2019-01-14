using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
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

		public async Task Update(string user, string repository)
		{
			var assembly = Assembly.GetEntryAssembly();
			var currentVersion = assembly.GetName().Version;
			var latestVersionJson = await GetLatestVersionJSONAsync(user, repository);
			var newVersion = new Version(latestVersionJson["name"].ToObject<string>());
			if (newVersion > currentVersion)
			{
				var updateDataArchive = Path.Combine(Path.GetTempPath(), "update.zip");
				using (var stream = await client.GetStreamAsync(DownloadUrl(latestVersionJson)))
				{
					using (var file = new FileStream(updateDataArchive, FileMode.Create))
					{
						stream.CopyTo(file);
					}
				}
				var updateToolDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
				Directory.CreateDirectory(updateToolDir);
				var urlUpdateExtract = DownloadUrl(await GetLatestVersionJSONAsync("danielScherzer", "GitHubReleaseUpdater"));
				await DownloadExtract(urlUpdateExtract, updateToolDir);
				var updateTool = Path.Combine(updateToolDir, "ExtractUpdate.dll");
				var destinationDir = Path.GetDirectoryName(assembly.Location);
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
				var updateDataDirectory = Path.Combine(updateTempDir, "data");
				Directory.CreateDirectory(updateDataDirectory);
				await DownloadExtract(DownloadUrl(latestVersionJson), updateDataDirectory);
				//Get UpdateExtract assembly
				var urlUpdateExtract = DownloadUrl(await GetLatestVersionJSONAsync("danielScherzer", "GitHubReleaseUpdater"));
				await DownloadExtract(urlUpdateExtract, updateTempDir);
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

		public void Run(string dotNetAssemblyPath, string parameters)
		{
			var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = "dotnet",
					Arguments = $"{dotNetAssemblyPath} {parameters}",
					WorkingDirectory = Path.GetDirectoryName(dotNetAssemblyPath),
					UseShellExecute = true,
					RedirectStandardOutput = false,
					RedirectStandardError = false,
					CreateNoWindow = true
				}
			};
			process.Start();
		}

		private HttpClient client = new HttpClient();

		private async Task<JObject> GetJSONAsync(string prefix) => JObject.Parse(await client.GetStringAsync($"https://api.github.com/{prefix}"));

		private async Task<JObject> GetLatestVersionJSONAsync(string user, string repository) => await GetJSONAsync($"repos/{user}/{repository}/releases/latest");

		private static string DownloadUrl(JObject json) => json["assets"][0]["browser_download_url"].ToObject<string>();
	}
}
