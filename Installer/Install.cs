using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace Installer
{
	class Install
	{
		private Logger logger;

		public Install(Logger logger)
		{
			this.logger = logger;
		}

		private void Log(string message) => logger.Log(message);

		internal void Execute(string applicationDir, string updateDataArchive)
		{
			Directory.CreateDirectory(applicationDir);
			logger.LogFileName = Path.Combine(applicationDir, Path.GetFileName(logger.LogFileName));
			if (!File.Exists(updateDataArchive)) throw new FileNotFoundException(updateDataArchive);

			using (var file = File.OpenRead(updateDataArchive))
			{
				try
				{
					using (var zip = new ZipArchive(file, ZipArchiveMode.Read))
					{
						foreach (var entry in zip.Entries)
						{
							var destinationFile = Path.Combine(applicationDir, entry.FullName);
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
								return;
							}
						}
					}
				}
				catch
				{
					var destinationFile = Path.Combine(applicationDir, Path.GetFileName(updateDataArchive));
					TryDeleteWait(destinationFile);
					Log($"Creating new {destinationFile}");
					try
					{
						using (var destination = File.Create(destinationFile))
						{
							file.CopyTo(destination);
						}
					}
					catch
					{
						//file still in use, no permission -> stop
						Log($"Error creating new {destinationFile}");
						return;
					}
				}
			}
			Log($"Update Finished");
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

		private static void Run(string executablePath, string parameters)
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
	}
}
