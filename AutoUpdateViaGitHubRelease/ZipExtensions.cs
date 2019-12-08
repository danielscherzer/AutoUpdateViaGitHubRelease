using System.IO;
using System.IO.Compression;

namespace AutoUpdateViaGitHubRelease
{
	internal class ZipExtensions
	{
		internal static void ExtractOverwriteToDirectory(string zipFileName, string destinationDir)
		{
			using (var file = File.OpenRead(zipFileName))
			{
				using (var zip = new ZipArchive(file, ZipArchiveMode.Read))
				{
					foreach(var entry in zip.Entries)
					{
						var destinationFileName = Path.Combine(destinationDir, entry.FullName);
						entry.ExtractToFile(destinationFileName, true);
					}
				}
			}
		}
	}
}
