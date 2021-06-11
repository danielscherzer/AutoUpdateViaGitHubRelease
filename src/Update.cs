using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace AutoUpdateViaGitHubRelease
{
	public class Update : INotifyPropertyChanged
	{
		public Update(string user, string repository, Version currentVersion, string tempDir, string destinationDir)
		{
			TempDir = tempDir;
			Directory.CreateDirectory(tempDir);
			DestinationDir = destinationDir;
			Directory.CreateDirectory(destinationDir);
			gitHub = new GitHubApi();
			async Task<bool> DownloadNewVersion()
			{
				try
				{
					var latestReleaseJson = await gitHub.GetLatestReleaseJSONAsync(user, repository);
					var version = GitHubApi.ParseVersion(latestReleaseJson);
					if (version > currentVersion)
					{
						//Get update installer that will extract the update archive to the application directory
						Installer = await gitHub.ExtractInstallerToAsync(tempDir);
						var updateUrl = GitHubApi.ParseDownloadUrl(latestReleaseJson);
						UpdateArchiveFileName = Path.Combine(tempDir, Path.GetFileName(updateUrl));
						//new version download
						await gitHub.DownloadFile(updateUrl, UpdateArchiveFileName);
						return true;
					}
					return false;
				}
				catch
				{
					return false;
				}
			}
			synchronizationContext = SynchronizationContext.Current;
			taskDownloadNewVersion = Task.Run(DownloadNewVersion)
				.ContinueWith(task => AvailableChanged(task.Result));
		}

		public Update(string user, string repository, Assembly assembly, string tempDir)
			: this(user, repository, assembly.GetName().Version, tempDir, Path.GetDirectoryName(assembly.Location)) { }

		public Update(string user, string repository, Assembly assembly)
			: this(user, repository, assembly, Path.Combine(Path.GetTempPath(), nameof(repository))) { }

		public bool Available => taskDownloadNewVersion.Result;

		public string DestinationDir { get; }

		public string TempDir { get; }

		public string UpdateArchiveFileName { get; set; }

		public string Installer { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

		public bool Install()
		{
			try
			{
				if (Available)
				{
					//string Quote(string input) => $"\"{input}\"";
					
					string Quote(string input) => input;
					var isExe = Installer.ExtensionIs(".exe");
					var arg0 = isExe ? string.Empty : Installer;
					var process = new Process
					{
						StartInfo = new ProcessStartInfo
						{
							FileName = isExe ? Installer : "dotnet",
							Arguments = $"{arg0} {Quote(UpdateArchiveFileName)} {Quote(DestinationDir)}",
							WorkingDirectory = TempDir,
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
		private readonly SynchronizationContext synchronizationContext;
		private readonly Task<bool> taskDownloadNewVersion;

		private bool AvailableChanged(bool result)
		{
			if (PropertyChanged is null) return result;
			TaskScheduler scheduler = TaskScheduler.Default;
			try
			{
				scheduler = TaskScheduler.FromCurrentSynchronizationContext();
			}
			catch { }
			//synchronizationContext.
//			Task.Run(() => PropertyChanged.Invoke(this, new PropertyChangedEventArgs(nameof(Available))), scheduler);
			return result;
		}
	}
}
