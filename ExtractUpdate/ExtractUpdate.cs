using CommandLine;
using System;
using System.IO;
using System.IO.Compression;
using System.Threading;

namespace ExtractUpdate
{
	class ExtractUpdate
	{
		static void Main(string[] args)
		{
			try
			{
				Parser.Default.ParseArguments<Options>(args).WithParsed(options => Update(options));
			}
			catch (Exception e)
			{
				Console.WriteLine(e.ToString());
			}
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
							entry.ExtractToFile(destinationFile);
						}
						catch
						{
							//file still in use, no permission -> stop
							return;
						}
					}
				}
			}
			//cleanup
			try
			{
				File.Delete(options.UpdateDataArchive);
			}
			catch { }
		}
	}
}
