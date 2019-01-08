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
			var gitHub = new GitHubRest();
			var (newVersion, downloadUrl) = await gitHub.GetLatestReleaseInfo(options.User, options.Repository);
			if (newVersion > options.Version)
			{
				if (string.IsNullOrEmpty(options.UpdateDirectory)) return true;
				if (!Directory.Exists(options.UpdateDirectory)) throw new DirectoryNotFoundException(options.UpdateDirectory);
				//new version download
				var fileName = Path.Combine(options.UpdateDirectory, "update.zip");
				using (var file = new FileStream(fileName, FileMode.Create))
				{
					var stream = await gitHub.Download(downloadUrl);
					//save download
					stream.CopyTo(file);
					//file.Seek(0, SeekOrigin.Begin);
				}
				return true;
			}
			else return false;
		}
	}
}
