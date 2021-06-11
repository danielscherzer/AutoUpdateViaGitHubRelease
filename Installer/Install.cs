using System.IO;
using System.IO.Compression;
using System.Threading;

namespace Installer
{
	class Install
	{
		private readonly Logger logger;

		public Install(Logger logger)
		{
			this.logger = logger;
		}

		private void Log(string message) => logger.Log(message);

		internal bool Execute(string applicationDir, string updateDataArchive)
		{
			Directory.CreateDirectory(applicationDir);
			logger.LogFileName = Path.Combine(applicationDir, Path.GetFileName(logger.LogFileName));
			if (!File.Exists(updateDataArchive))
			{
				logger.Log($"{nameof(updateDataArchive)}='{updateDataArchive}' does not exist.");
				return false;
			}

			using (var file = File.OpenRead(updateDataArchive))
			{
				try
				{
					using var zip = new ZipArchive(file, ZipArchiveMode.Read);
					foreach (var entry in zip.Entries)
					{
						string destinationFile = FullFileName(applicationDir, entry.FullName);
						Directory.CreateDirectory(Path.GetDirectoryName(destinationFile));
						TryDeleteWait(destinationFile);
						Log($"Creating new {destinationFile}");
						try
						{
							entry.ExtractToFile(destinationFile);
						}
						catch
						{
							//file still in use, no permission -> stop
							Log($"Error creating new {destinationFile}");
							return false;
						}
					}
				}
				catch
				{
					string destinationFile = FullFileName(applicationDir, Path.GetFileName(updateDataArchive));
					TryDeleteWait(destinationFile);
					Log($"Creating new {destinationFile}");
					try
					{
						using var destination = File.Create(destinationFile);
						file.CopyTo(destination);
					}
					catch
					{
						//file still in use, no permission -> stop
						Log($"Error creating new {destinationFile}");
						return false;
					}
				}
			}
			Log($"Update Finished");
			return true;

			static string FullFileName(string applicationDir, string name)
			{
				return Path.Combine(applicationDir, name).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
			}
		}

		private bool TryDeleteWait(string destinationFile, int tries = 10, int waitTimeMsec = 1000)
		{
			for (var i = 0; i < tries; ++i)
			{
				Log($"Try {i} delete {destinationFile}");
				try
				{
					// try to delete
					File.Delete(destinationFile);
					// successful, so we can write new version
					return true;
				}
				catch
				{
					// unsuccessful -> wait before next try
					Thread.Sleep(waitTimeMsec);
				}
			}
			return false;
		}
	}
}
