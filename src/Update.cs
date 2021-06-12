using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

namespace AutoUpdateViaGitHubRelease
{
	/// <summary>
	/// class that handles update and implements <see cref="INotifyPropertyChanged"/>
	/// </summary>
	public class Update : INotifyPropertyChanged
	{
		/// <summary>
		/// Check if an update is available and download it and the installer if this is the case
		/// </summary>
		/// <param name="user">github user name</param>
		/// <param name="repository">github repository name</param>
		/// <param name="currentVersion">Current version of application.</param>
		/// <param name="tempDir">The directory were temporary files should be stored</param>
		/// <returns><see langword="true"/> if a new version is available, otherwise <see langword="false"/></returns>
		public async Task<bool> CheckDownloadNewVersionAsync(string user, string repository
			, Version currentVersion, string tempDir)
		{
			Directory.CreateDirectory(tempDir);
			updateArchiveFileName = Path.Combine(tempDir, "update.zip");
			Available = await UpdateTools.CheckDownloadNewVersionAsync(
				user, repository, currentVersion, updateArchiveFileName);

			var taskInstall = UpdateTools.DownloadExtractInstallerToAsync(tempDir);
			installerName = await taskInstall;
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Available)));
			return Available;
		}

		/// <summary>
		/// Is <see langword="true"/> if a new update is available.
		/// </summary>
		public bool Available { get; private set; } = false;

		/// <summary>
		/// Event handler for <see cref="PropertyChangedEventHandler"/> events.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Install the update. <see cref="CheckDownloadNewVersionAsync(string, string, Version, string)"/> has to be called otherwise no update can be available.
		/// If you update the currently executing program. Do not wait for the install to finish,
		/// but close the program.
		/// </summary>
		/// <returns><see langword="true"/> if the update was successfull.</returns>
		public async Task<bool> Install(string destinationDir)
		{
			try
			{
				return await UpdateTools.InstallAsync(installerName, updateArchiveFileName, destinationDir);
			}
			catch
			{
				return false;
			}
		}

		private string installerName = string.Empty;
		private string updateArchiveFileName = string.Empty;
	}
}
