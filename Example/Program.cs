namespace Example
{
	class Program
	{
		static void Main(string[] args)
		{
			var updater = new GitHubLatestRelease.Updater();
			//updater.Run(@"D:\Daten\git\GitHubReleaseUpdater\ExtractUpdate\bin\Debug\ExtractUpdate.dll", @"C:\Users\Scherzer\Desktop\ControlClassLibrary.zip D:\Daten\git\ShaderForm\bin\Debug");
			//updater.Run(@"D:\Daten\git\GitHubReleaseUpdater\ExtractUpdate\bin\Debug\ExtractUpdate.dll", @"C:\Users\Scherzer\AppData\Local\Temp\update.zip D:\Daten\git\GitHubReleaseUpdater\Example\bin\Debug\netcoreapp2.1");
			updater.Run(@"C:\Users\Scherzer\AppData\Local\Temp\rafurtnx.xjv\ExtractUpdate.dll", @"C:\Users\Scherzer\AppData\Local\Temp\update.zip D:\Daten\git\GitHubReleaseUpdater\Example\bin\Debug\netcoreapp2.1");
			//updater.Update("danielScherzer", "GitHubReleaseUpdater").Wait();
		}
	}
}
