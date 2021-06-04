using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace AutoUpdateViaGitHubRelease
{
	public class Update : INotifyPropertyChanged
	{
		public Update(string user, string repository, Version currentVersion, string tempDir, string destinationDir)
		{
			this.tempDir = tempDir;
			Directory.CreateDirectory(tempDir);
			this.destinationDir = destinationDir;
			Directory.CreateDirectory(destinationDir);
			gitHub = new GitHubApi();
			async Task<bool> DownloadNewVersion()
			{
				try
				{
					var latestReleaseJson = await gitHub.GetLatestReleaseJSONAsync(user, repository);
					var version = GitHubApi.ExtractVersion(latestReleaseJson);
					if (version > currentVersion)
					{
						//Get update installer that will extract the update archive to the application directory
						installer = await gitHub.ExtractInstallerTo(tempDir);
						var updateUrl = GitHubApi.ExtractDownloadUrl(latestReleaseJson);
						updateArchiveFileName = Path.Combine(tempDir, Path.GetFileName(updateUrl));
						//new version download
						await gitHub.DownloadFile(updateUrl, updateArchiveFileName);
						return true;
					}
					return false;
				}
				catch
				{
					return false;
				}
			}
			var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
			DownloadTask = Task.Run(DownloadNewVersion)
				.ContinueWith(task => Available = task.Result, scheduler);
		}

		public Update(string user, string repository, Assembly assembly, string tempDir)
			: this(user, repository, assembly.GetName().Version, tempDir, Path.GetDirectoryName(assembly.Location)) { }

			public Update(string user, string repository, Assembly assembly)
			: this(user, repository, assembly, Path.Combine(Path.GetTempPath(), nameof(repository))) { }

		public bool Available
		{
			get => _available;
			private set
			{
				_available = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Available)));
			}
		}

		public Task<bool> DownloadTask { get; }

		public event PropertyChangedEventHandler PropertyChanged;

		public bool Install()
		{
			try
			{
				if (Available)
				{
					//string Quote(string input) => $"\"{input}\"";
					
					string Quote(string input) => input;
					var isExe = installer.ExtensionIs(".exe");
					var arg0 = isExe ? string.Empty : installer;
					var process = new Process
					{
						StartInfo = new ProcessStartInfo
						{
							FileName = isExe ? installer : "dotnet",
							Arguments = $"{arg0} {Quote(updateArchiveFileName)} {Quote(destinationDir)}",
							WorkingDirectory = tempDir,
							RedirectStandardOutput = false,
							RedirectStandardError = false,
						}
					};
					process.Start();
				}
				return Available;
			}
			catch
			{
				return false;
			}
		}

		private readonly GitHubApi gitHub;
		private readonly string tempDir;
		private readonly string destinationDir;
		private bool _available = false;
		private string installer;
		private string updateArchiveFileName;
	}
}
