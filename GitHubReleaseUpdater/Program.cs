using CommandLine;
using System;
using System.IO;
using System.Threading.Tasks;

namespace GitHubReleaseUpdater
{
	class Program
	{
		static void Main(string[] args)
		{
			Parser.Default.ParseArguments<Options>(args).WithParsed(options => Run(options));
		}

		static void Run(Options options)
		{
			try
			{
				var task = Update(options);
				task.Wait();
				Environment.ExitCode = task.Result ? 99 : 0;
			}
			catch(Exception e)
			{
				Console.WriteLine(e.ToString());
				Environment.ExitCode = 1;
			}
		}

		static async Task<bool> Update(Options options)
		{
			var updater = new GitHubLatestRelease.Updater();
			var updateArchiveFilePath = Path.Combine(options.UpdateDirectory, "update/update.zip");
			return await updater.DownloadNewVersion(options.User, options.Repository, options.Version, updateArchiveFilePath);
		}
	}
}
