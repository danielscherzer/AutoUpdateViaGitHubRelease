namespace Example
{
	class Program
	{
		static void Main(string[] args)
		{
			var updater = new GitHubLatestRelease.Updater();
			updater.Run(@"D:\Daten\git\GitHubReleaseUpdater\ExtractUpdate\bin\Debug\ExtractUpdate.dll C:\Users\Scherzer\Desktop\ControlClassLibrary.zip D:\Daten\git\ShaderForm\bin\Debug");
			//updater.Update("danielScherzer", "GitHubReleaseUpdater").Wait();
		}
	}
}
