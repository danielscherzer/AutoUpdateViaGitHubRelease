using System.IO;
using System.IO.Compression;
using System.Threading;

namespace Updater.core
{
	class Program
	{
		public const string updateArchive = "update.zip";

		static void Main(string[] args)
		{
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
			Cleanup();
		}

		private static void Cleanup()
		{
			//cleanup
			try
			{
				File.Delete(updateArchive);
			}
			catch { }
		}
	}
}
