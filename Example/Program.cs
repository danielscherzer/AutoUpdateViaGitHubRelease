using GitHubLatestRelease;

namespace Example
{
	class Program
	{
		static void Main(string[] args)
		{
			var updater = new Updater();
			updater.Run(@"D:\Daten\git\GitHubReleaseUpdater\UpdateWindow\bin\Debug\UpdateWindow.exe", @"C:\Users\Scherzer\AppData\Local\Temp\update.zip D:\Daten\git\GitHubReleaseUpdater\Example\bin\Debug");
			//on update the example app will replace itself with the release on github
			//updater.Update("danielScherzer", "GitHubReleaseUpdater").Wait();
		}
	}
}
