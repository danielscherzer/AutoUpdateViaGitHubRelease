using CommandLine;

namespace ExtractUpdate
{
	class Options
	{
		[Value(0, Required = true, HelpText = "Update data archive name missing.")]
		public string UpdateDataArchive { get; set; }

		[Value(1, Required = true, HelpText = "Application directory missing")]
		public string ApplicationDir { get; set; }

		//[Value(2, Required = true, HelpText = "The assembly version")]
		//public Version Version { get; set; }

		//[Option('u', "update", Default = "", HelpText = "Folder of application to update.")]
		//public string UpdateDirectory { get; set; }
	}
}
