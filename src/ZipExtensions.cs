using System.IO;
using System.IO.Compression;

namespace AutoUpdateViaGitHubRelease
{
	internal static class ZipExtensions
	{
		internal static string ExtractOverwriteInstallerToDirectory(string zipFileName, string destinationDir)
		{
			using (var file = File.OpenRead(zipFileName))
			{
				using (var zip = new ZipArchive(file, ZipArchiveMode.Read))
				{
					var result = "";
					foreach (var entry in zip.Entries)
					{
						var destinationFileName = Path.Combine(destinationDir, entry.FullName).Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
						Directory.CreateDirectory(Path.GetDirectoryName(destinationFileName));
						entry.ExtractToFile(destinationFileName, true);
						if (entry.FullName.Contains(".runtimeconfig.json"))
						{
							result = destinationFileName.Replace(".runtimeconfig.json", ".dll");
						}
						else if (entry.FullName.ExtensionIs(".exe"))
						{
							result = destinationFileName;
						}
					}
					return result;
				}
			}
		}

		internal static bool ExtensionIs(this string fileName, string extensionLowerCase)
			=> Path.GetExtension(fileName).ToLowerInvariant() == extensionLowerCase;
	}
}
