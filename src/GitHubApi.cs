using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AutoUpdateViaGitHubRelease
{
	/// <summary>
	/// Helper class to access the github rest API
	/// </summary>
	public class GitHubApi
	{
		/// <summary>
		/// Setting up an internal http client to send requests to github rest API
		/// </summary>
		public GitHubApi()
		{
			client.DefaultRequestHeaders.Accept.Clear();
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
			client.DefaultRequestHeaders.Add("User-Agent", "update from github release");
			//ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
		}

		/// <summary>
		/// Downloads the given url into the destination file name.
		/// </summary>
		/// <param name="url">The url to donload.</param>
		/// <param name="destinationFileName">The destination file name.</param>
		/// <returns><see cref="Task"/></returns>
		public async Task DownloadFileAsync(string url, string destinationFileName)
		{
			using (var stream = await client.GetStreamAsync(url))
			{
				using (var file = new FileStream(destinationFileName, FileMode.Create))
				{
					stream.CopyTo(file);
				}
			}
		}

		/// <summary>
		/// Returns the download url of the primary asset from the rest response json file
		/// </summary>
		/// <param name="json">JSON of the rest response.</param>
		/// <returns>A download URL.</returns>
		public static string ParseDownloadUrl(JObject json) => json["assets"][0]["browser_download_url"].ToObject<string>();

		/// <summary>
		/// Returns the version of the artifact described in the rest response. 
		/// </summary>
		/// <param name="json"></param>
		/// <returns><see cref="Version"/></returns>
		public static Version ParseVersion(JObject json) => new Version(json["name"].ToObject<string>());

		/// <summary>
		/// Returns the JSON rest respones for the given github directory
		/// </summary>
		/// <param name="gitHubDirectory">the github directory to query</param>
		/// <returns></returns>
		public async Task<JObject> GetJSONAsync(string gitHubDirectory) => JObject.Parse(await client.GetStringAsync($"https://api.github.com/{gitHubDirectory}"));

		/// <summary>
		/// Returns the JSON of the latest release.
		/// </summary>
		/// <param name="user">Th github user.</param>
		/// <param name="repository">The github repository.</param>
		/// <returns></returns>
		public async Task<JObject> GetLatestReleaseJSONAsync(string user, string repository) => await GetJSONAsync($"repos/{user}/{repository}/releases/latest");


		private readonly HttpClient client = new HttpClient();
	}
}
