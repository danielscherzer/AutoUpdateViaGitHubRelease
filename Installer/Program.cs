using AutoUpdateViaGitHubRelease;
using Installer;
using System;
using System.IO;
using System.Reflection;
using System.Threading;

var logger = new Logger(Path.ChangeExtension(Assembly.GetExecutingAssembly().Location, ".log"));
var install = new Install(logger);
try
{
	if (2 != args.Length)
	{
		logger.Log($"Usage: Installer <UpdateDataArchive> <ApplicationDir>");
		Environment.ExitCode = 1;
		return;
	}
	var updateDataArchive = args[0];
	var applicationDir = args[1];
	logger.Log($"Parameter are {nameof(updateDataArchive)}='{updateDataArchive}' and {nameof(applicationDir)}='{applicationDir}'");
	if(!install.Execute(applicationDir, updateDataArchive)) Environment.ExitCode = 2;
}
catch (Exception ex)
{
	logger.Log(ex.ToString());
	Environment.ExitCode = 3;
}
for (int i = 5; i > 0; --i)
{
	Console.WriteLine($"Closing in {i}seconds");
	Thread.Sleep(1000);
}
