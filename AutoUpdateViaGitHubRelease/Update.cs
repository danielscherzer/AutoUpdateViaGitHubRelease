using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace AutoUpdateViaGitHubRelease
{
	public class Update : INotifyPropertyChanged
	{
		public Update(string user, string project, Assembly assembly, string tempDir)
		{
			this.tempDir = tempDir;
			destinationDir = Path.GetDirectoryName(assembly.Location);
			gitHub = new GitHubApi();
			async Task<bool> DownloadNewVersion()
			{
				try
				{
					var currentVersion = assembly.GetName().Version;
					return await gitHub.DownloadNewVersion(user, project, currentVersion, tempDir);
				}
				catch
				{
					return false;
				}
			}
			Task.Run(DownloadNewVersion).ContinueWith(task => Available = task.Result);
		}

		public bool Available
		{
			get => _available;
			private set
			{
				_available = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Available)));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		public bool Install()
		{
			try
			{
				if (Available) { UpdateTools.InstallUpdate(tempDir, destinationDir); }
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
	}
}
