using GitHubLatestRelease;

namespace Example
{
	class Program
	{
		static void Main(string[] args)
		{
			var updater = new Updater();
			//updater.Run(@"D:\Daten\git\GitHubReleaseUpdater\UpdateWindow\bin\Debug\UpdateWindow.exe", @"C:\Users\Scherzer\AppData\Local\Temp\update.zip D:\Daten\git\GitHubReleaseUpdater\Example\bin\Debug\netcoreapp2.0");
			updater.Update("danielScherzer", "GitHubReleaseUpdater").Wait();
		}
	}
}
