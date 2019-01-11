namespace Example
{
	class Program
	{
		static void Main(string[] args)
		{
			var updater = new GitHubLatestRelease.Updater();
			updater.Update("danielScherzer", "GitHubReleaseUpdater").Wait();
		}
	}
}
