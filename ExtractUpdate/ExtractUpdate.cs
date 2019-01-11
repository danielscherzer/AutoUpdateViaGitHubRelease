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
			if(0 == args.Length)
			{
				Console.WriteLine("Argument archive file name missing.");
				return;
			}
			var updateArchive = args[1];
			if (!File.Exists(updateArchive)) return;
			using (var file = File.OpenRead(updateArchive))
			{
				using (var zip = new ZipArchive(file, ZipArchiveMode.Read))
				{
					foreach (var entry in zip.Entries)
					{
						for (var i = 0; i < 10; ++i)
						{
							try
							{
								// try to delete
								File.Delete(entry.FullName);
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
							entry.ExtractToFile(entry.FullName);
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
				File.Delete(updateArchive);
			}
			catch { }
		}
	}
}
