using CommandLine;
using System;

namespace GitHubReleaseUpdater
{
	class Options
	{
		[Value(0, Required = true, HelpText = "The GitHub user name")]
		public string User { get; set; }

		[Value(1, Required = true, HelpText = "The GitHub repository name")]
		public string Repository { get; set; }

		[Value(2, Required = true, HelpText = "The assembly version")]
		public Version Version { get; set; }

		[Option('u', "update", Default = "", HelpText = "Folder of application to update.")]
		public string UpdateDirectory { get; set; }
	}
}
