using CommandLine;
using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Threading;

namespace ExtractUpdate
{
	class ExtractUpdate
	{
		private static string logFileName;

		static void Main(string[] args)
		{
			logFileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "logfile.txt");
			try
			{
				Parser.Default.ParseArguments<Options>(args).WithParsed(options => Update(options));
			}
			catch (Exception e)
			{
				Log(e.ToString());
			}
		}

		private static void Log(string message)
		{
			//Console.WriteLine(message);
			var time = DateTime.Now.ToLongTimeString();
			File.AppendAllText(logFileName, $"{time}: {message}{Environment.NewLine}");
		}

		private static void Update(Options options)
		{
			if (!File.Exists(options.UpdateDataArchive)) throw new FileNotFoundException(options.UpdateDataArchive);
			if (!Directory.Exists(options.ApplicationDir)) throw new DirectoryNotFoundException(options.ApplicationDir);
			using (var file = File.OpenRead(options.UpdateDataArchive))
			{
				using (var zip = new ZipArchive(file, ZipArchiveMode.Read))
				{
					foreach (var entry in zip.Entries)
					{
						var destinationFile = Path.Combine(options.ApplicationDir, entry.FullName);
						for (var i = 0; i < 10; ++i)
						{
							try
							{
								Log($"Try {i} delete {destinationFile}");
								// try to delete
								File.Delete(destinationFile);
								// successful, so we can write new version
								break;
							}
							catch
							{
								// unsuccessful -> wait before next try
								Thread.Sleep(1000);
							}
						}
						try
						{
							Log($"Extracting new {destinationFile}");
							entry.ExtractToFile(destinationFile);
						}
						catch
						{
							//file still in use, no permission -> stop
							Log($"Error extracting new {destinationFile}");
							return;
						}
					}
				}
			}
			//cleanup
			try
			{
				Log($"Cleanup...");
				//File.Delete(options.UpdateDataArchive);
			}
			catch { Log($"Error cleanup"); }
			Log($"Update Finished");
		}
	}
}
